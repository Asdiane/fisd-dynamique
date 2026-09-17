using System.Security.Cryptography;
using System.Text;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Fisd.Application.Email;
using Fisd.Application.Email.Models;
using Fisd.Application.Models.receive.Auth;
using Fisd.Application.Models.result.Auth;
using Fisd.Application.Security;
using Fisd.Application.Services.Interfaces;
using Fisd.Application.Site;
using Fisd.Persistence;
using Fisd.Persistence.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OtpNet;

namespace Fisd.Application.Services
{
    public class AuthService : IAuthService
    {
        private const string TotpIssuer = "FISD";
        private const int PasskeyChallengeExpiryMinutes = 5;
        private static readonly TimeSpan ResetTokenLifetime = TimeSpan.FromHours(1);

        private readonly FisdDbContext _dbContext;
        private readonly JwtTokenGenerator _tokenGenerator;
        private readonly IGlobalEmailService _globalEmailService;
        private readonly SiteOptions _siteOptions;
        private readonly IFido2 _fido2;
        private readonly IMemoryCache _memoryCache;

        public AuthService(
            FisdDbContext dbContext,
            JwtTokenGenerator tokenGenerator,
            IGlobalEmailService globalEmailService,
            IOptions<SiteOptions> siteOptions,
            IFido2 fido2,
            IMemoryCache memoryCache)
        {
            _dbContext = dbContext;
            _tokenGenerator = tokenGenerator;
            _globalEmailService = globalEmailService;
            _siteOptions = siteOptions.Value;
            _fido2 = fido2;
            _memoryCache = memoryCache;
        }

        public async Task<LoginResultModel> LoginAsync(string email, string password)
        {
            var adminUser = await FindByCredentialsAsync(email, password);
            if (adminUser == null)
            {
                return new LoginResultModel { Success = false, Error = "Identifiants invalides." };
            }

            if (!adminUser.IsActive)
            {
                return new LoginResultModel { Success = false, Error = "Ce compte a été désactivé." };
            }

            if (adminUser.TwoFactorEnabled)
            {
                return new LoginResultModel { Success = true, RequiresTwoFactor = true, Email = adminUser.Email };
            }

            // Keep the pending QR code valid across retries and additional tabs.
            if (string.IsNullOrEmpty(adminUser.TwoFactorSecret))
            {
                adminUser.TwoFactorSecret = GenerateTwoFactorSecret();
                await _dbContext.SaveChangesAsync();
            }
            var secret = adminUser.TwoFactorSecret;

            return new LoginResultModel
            {
                Success = true,
                RequiresTwoFactorSetup = true,
                Email = adminUser.Email,
                TwoFactorSecret = secret,
                TwoFactorQrUri = BuildTwoFactorQrUri(adminUser.Email, secret)
            };
        }

        public async Task<LoginResultModel> EnableTwoFactorAsync(string email, string password, string totpCode)
        {
            var adminUser = await FindByCredentialsAsync(email, password);
            if (adminUser == null)
            {
                return new LoginResultModel { Success = false, Error = "Identifiants invalides." };
            }

            if (string.IsNullOrEmpty(adminUser.TwoFactorSecret))
            {
                return new LoginResultModel { Success = false, Error = "Aucune configuration 2FA en attente. Reconnectez-vous." };
            }

            if (!VerifyTotpCode(adminUser.TwoFactorSecret, totpCode))
            {
                return new LoginResultModel { Success = false, Error = "Code de vérification invalide." };
            }

            adminUser.TwoFactorEnabled = true;
            await _dbContext.SaveChangesAsync();

            return BuildAuthenticatedResult(adminUser);
        }

        public async Task<LoginResultModel> VerifyTwoFactorAsync(string email, string password, string totpCode)
        {
            var adminUser = await FindByCredentialsAsync(email, password);
            if (adminUser == null)
            {
                return new LoginResultModel { Success = false, Error = "Identifiants invalides." };
            }

            if (!adminUser.TwoFactorEnabled || string.IsNullOrEmpty(adminUser.TwoFactorSecret))
            {
                return new LoginResultModel { Success = false, Error = "La double authentification n'est pas configurée pour ce compte." };
            }

            if (!VerifyTotpCode(adminUser.TwoFactorSecret, totpCode))
            {
                return new LoginResultModel { Success = false, Error = "Code de vérification invalide." };
            }

            return BuildAuthenticatedResult(adminUser);
        }

        private async Task<AdminUserEntity?> FindByCredentialsAsync(string email, string password)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            var adminUser = await _dbContext.AdminUsers.FirstOrDefaultAsync(u => u.Email == normalizedEmail);
            return adminUser != null && PasswordHasher.Verify(password, adminUser.PasswordHash) ? adminUser : null;
        }

        private LoginResultModel BuildAuthenticatedResult(AdminUserEntity adminUser)
        {
            var role = adminUser.Role.ToString();
            var (token, expiresAt) = _tokenGenerator.GenerateAdminToken(adminUser.Id, adminUser.Email, role);
            return new LoginResultModel { Success = true, Token = token, ExpiresAt = expiresAt, Email = adminUser.Email, Role = role };
        }

        private static string GenerateTwoFactorSecret() => Base32Encoding.ToString(KeyGeneration.GenerateRandomKey(20));

        private static string BuildTwoFactorQrUri(string email, string secretBase32) =>
            $"otpauth://totp/{TotpIssuer}:{Uri.EscapeDataString(email)}?secret={secretBase32}&issuer={TotpIssuer}&algorithm=SHA1&digits=6&period=30";

        private static bool VerifyTotpCode(string secretBase32, string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            var totp = new Totp(Base32Encoding.ToBytes(secretBase32));
            return totp.VerifyTotp(string.Concat(code.Where(c => !char.IsWhiteSpace(c))), out _, new VerificationWindow(1, 1));
        }

        public async Task RequestPasswordResetAsync(string email)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            var adminUser = await _dbContext.AdminUsers.FirstOrDefaultAsync(u => u.Email == normalizedEmail);
            if (adminUser == null)
            {
                // Same response either way - a real vs unknown email must look identical to the caller.
                return;
            }

            var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                .Replace('+', '-').Replace('/', '_').TrimEnd('=');

            _dbContext.PasswordResetTokens.Add(new PasswordResetTokenEntity
            {
                Id = Guid.NewGuid(),
                AdminUserId = adminUser.Id,
                TokenHash = HashToken(rawToken),
                ExpiresAt = DateTimeOffset.UtcNow.Add(ResetTokenLifetime),
                CreatedAt = DateTimeOffset.UtcNow
            });
            await _dbContext.SaveChangesAsync();

            var resetLink = $"{_siteOptions.BaseUrl.TrimEnd('/')}/admin/reset-password?token={rawToken}";
            var subject = "Réinitialisation de votre mot de passe FISD";
            var body = $"""
                <p>Bonjour,</p>
                <p>Une demande de réinitialisation de mot de passe a été effectuée pour votre compte FISD ({adminUser.Email}).</p>
                <p><a href="{resetLink}">Réinitialiser mon mot de passe</a></p>
                <p>Ce lien expire dans une heure. Si vous n'êtes pas à l'origine de cette demande, ignorez ce courriel.</p>
                <p>Vous ne voyez pas ce courriel ? Vérifiez votre dossier de courriers indésirables (spam).</p>
                """;

            var sender = _globalEmailService.GetEmailOptions().Senders["Generic"];
            var messages = new List<Message>([new Message
            {
                To = new Recipient { Name = adminUser.Email, Email = adminUser.Email },
                Subject = subject,
                Body = body
            }]);

            await _globalEmailService.SendEmailAsync(sender, messages);
        }

        public async Task<AdminInvitationInfoResultModel?> GetInvitationInfoAsync(string token)
        {
            var tokenHash = HashToken(token);
            var invitation = await _dbContext.AdminUserInvitations.FirstOrDefaultAsync(i => i.TokenHash == tokenHash);
            if (invitation == null || invitation.AcceptedAt != null || invitation.ExpiresAt < DateTimeOffset.UtcNow)
            {
                return null;
            }

            return new AdminInvitationInfoResultModel { Email = invitation.Email, Role = invitation.Role.ToString(), ExpiresAt = invitation.ExpiresAt };
        }

        public async Task<LoginResultModel> AcceptInvitationAsync(string token, string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                return new LoginResultModel { Success = false, Error = "Le mot de passe doit contenir au moins 8 caractères." };
            }

            var tokenHash = HashToken(token);
            var invitation = await _dbContext.AdminUserInvitations.FirstOrDefaultAsync(i => i.TokenHash == tokenHash);
            if (invitation == null || invitation.AcceptedAt != null || invitation.ExpiresAt < DateTimeOffset.UtcNow)
            {
                return new LoginResultModel { Success = false, Error = "Cette invitation est invalide ou expirée." };
            }

            if (await _dbContext.AdminUsers.AnyAsync(u => u.Email == invitation.Email))
            {
                return new LoginResultModel { Success = false, Error = "Un compte existe déjà avec ce courriel." };
            }

            var adminUser = new AdminUserEntity
            {
                Id = Guid.NewGuid(),
                Email = invitation.Email,
                PasswordHash = PasswordHasher.Hash(password),
                Role = invitation.Role,
                CreatedAt = DateTimeOffset.UtcNow
            };
            _dbContext.AdminUsers.Add(adminUser);
            invitation.AcceptedAt = DateTimeOffset.UtcNow;
            await _dbContext.SaveChangesAsync();

            // Same as a brand-new admin's first login - go straight into mandatory 2FA setup
            // instead of a second round trip through the login form.
            // Keep the pending QR code valid across retries and additional tabs.
            if (string.IsNullOrEmpty(adminUser.TwoFactorSecret))
            {
                adminUser.TwoFactorSecret = GenerateTwoFactorSecret();
                await _dbContext.SaveChangesAsync();
            }
            var secret = adminUser.TwoFactorSecret;

            return new LoginResultModel
            {
                Success = true,
                RequiresTwoFactorSetup = true,
                Email = adminUser.Email,
                TwoFactorSecret = secret,
                TwoFactorQrUri = BuildTwoFactorQrUri(adminUser.Email, secret)
            };
        }

        public async Task<string?> ResetPasswordAsync(string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
            {
                return "Le mot de passe doit contenir au moins 8 caractères.";
            }

            var tokenHash = HashToken(token);
            var resetToken = await _dbContext.PasswordResetTokens.FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
            if (resetToken == null || resetToken.UsedAt != null || resetToken.ExpiresAt < DateTimeOffset.UtcNow)
            {
                return "Ce lien de réinitialisation est invalide ou expiré.";
            }

            var adminUser = await _dbContext.AdminUsers.FirstOrDefaultAsync(u => u.Id == resetToken.AdminUserId);
            if (adminUser == null)
            {
                return "Ce lien de réinitialisation est invalide ou expiré.";
            }

            adminUser.PasswordHash = PasswordHasher.Hash(newPassword);
            resetToken.UsedAt = DateTimeOffset.UtcNow;
            await _dbContext.SaveChangesAsync();
            return null;
        }

        private static string HashToken(string token) => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

        public async Task<PasskeyOptionsResultModel?> GetPasskeyRegistrationOptionsAsync(Guid adminUserId)
        {
            var adminUser = await _dbContext.AdminUsers
                .Include(u => u.Passkeys)
                .FirstOrDefaultAsync(u => u.Id == adminUserId);
            if (adminUser == null)
            {
                return null;
            }

            var excludeCredentials = adminUser.Passkeys
                .Select(p => new PublicKeyCredentialDescriptor(Convert.FromBase64String(p.CredentialId)))
                .ToList();

            var options = _fido2.RequestNewCredential(new RequestNewCredentialParams
            {
                User = new Fido2User { Id = adminUser.Id.ToByteArray(), Name = adminUser.Email, DisplayName = adminUser.Email },
                ExcludeCredentials = excludeCredentials,
                AuthenticatorSelection = new AuthenticatorSelection { ResidentKey = ResidentKeyRequirement.Preferred, UserVerification = UserVerificationRequirement.Preferred },
                AttestationPreference = AttestationConveyancePreference.None
            });

            var challengeId = Guid.NewGuid().ToString();
            _memoryCache.Set(PasskeyRegistrationCacheKey(challengeId), new PasskeyRegistrationChallenge { AdminUserId = adminUser.Id, Options = options },
                TimeSpan.FromMinutes(PasskeyChallengeExpiryMinutes));

            return new PasskeyOptionsResultModel { ChallengeId = challengeId, Options = options };
        }

        public async Task<PasskeyActionResultModel> VerifyPasskeyRegistrationAsync(Guid adminUserId, PasskeyRegisterVerifyModel model)
        {
            if (!_memoryCache.TryGetValue(PasskeyRegistrationCacheKey(model.ChallengeId), out PasskeyRegistrationChallenge? challenge) || challenge == null || challenge.AdminUserId != adminUserId)
            {
                return new PasskeyActionResultModel { Success = false, Error = "Cette demande a expiré, réessayez." };
            }
            _memoryCache.Remove(PasskeyRegistrationCacheKey(model.ChallengeId));

            var credential = await _fido2.MakeNewCredentialAsync(new MakeNewCredentialParams
            {
                AttestationResponse = model.AttestationResponse,
                OriginalOptions = challenge.Options,
                IsCredentialIdUniqueToUserCallback = async (p, ct) =>
                    !await _dbContext.AdminPasskeys.AnyAsync(x => x.CredentialId == Convert.ToBase64String(p.CredentialId), ct)
            });

            var passkeyEntity = new AdminPasskeyEntity
            {
                Id = Guid.NewGuid(),
                AdminUserId = adminUserId,
                CredentialId = Convert.ToBase64String(credential.Id),
                PublicKey = Convert.ToBase64String(credential.PublicKey),
                SignCount = credential.SignCount,
                DeviceLabel = model.DeviceLabel,
                AaGuid = credential.AaGuid,
                Transports = credential.Transports?.Length > 0 ? string.Join(",", credential.Transports) : null,
                CreatedOn = DateTimeOffset.UtcNow
            };
            _dbContext.AdminPasskeys.Add(passkeyEntity);
            await _dbContext.SaveChangesAsync();

            return new PasskeyActionResultModel
            {
                Success = true,
                Passkey = new PasskeyResultModel { Id = passkeyEntity.Id, DeviceLabel = passkeyEntity.DeviceLabel, CreatedOn = passkeyEntity.CreatedOn }
            };
        }

        public async Task<List<PasskeyResultModel>> ListPasskeysAsync(Guid adminUserId) =>
            await _dbContext.AdminPasskeys
                .Where(p => p.AdminUserId == adminUserId)
                .OrderByDescending(p => p.CreatedOn)
                .Select(p => new PasskeyResultModel { Id = p.Id, DeviceLabel = p.DeviceLabel, CreatedOn = p.CreatedOn, LastUsedOn = p.LastUsedOn })
                .ToListAsync();

        public async Task<bool> DeletePasskeyAsync(Guid adminUserId, Guid passkeyId)
        {
            var passkey = await _dbContext.AdminPasskeys.FirstOrDefaultAsync(p => p.Id == passkeyId && p.AdminUserId == adminUserId);
            if (passkey == null)
            {
                return false;
            }

            _dbContext.AdminPasskeys.Remove(passkey);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public PasskeyOptionsResultModel GetPasskeyLoginOptions()
        {
            var options = _fido2.GetAssertionOptions(new GetAssertionOptionsParams
            {
                AllowedCredentials = new List<PublicKeyCredentialDescriptor>(),
                UserVerification = UserVerificationRequirement.Preferred
            });

            var challengeId = Guid.NewGuid().ToString();
            _memoryCache.Set(PasskeyLoginCacheKey(challengeId), new PasskeyLoginChallenge { Options = options }, TimeSpan.FromMinutes(PasskeyChallengeExpiryMinutes));

            return new PasskeyOptionsResultModel { ChallengeId = challengeId, Options = options };
        }

        public async Task<LoginResultModel> VerifyPasskeyLoginAsync(PasskeyLoginVerifyModel model)
        {
            if (!_memoryCache.TryGetValue(PasskeyLoginCacheKey(model.ChallengeId), out PasskeyLoginChallenge? challenge) || challenge == null)
            {
                return new LoginResultModel { Success = false, Error = "Cette demande a expiré, réessayez." };
            }
            _memoryCache.Remove(PasskeyLoginCacheKey(model.ChallengeId));

            var credentialId = Convert.ToBase64String(model.AssertionResponse.RawId);
            var passkeyEntity = await _dbContext.AdminPasskeys
                .Include(p => p.AdminUser)
                .FirstOrDefaultAsync(p => p.CredentialId == credentialId);
            if (passkeyEntity == null)
            {
                return new LoginResultModel { Success = false, Error = "Cette clé d'accès est inconnue." };
            }

            var assertionResult = await _fido2.MakeAssertionAsync(new MakeAssertionParams
            {
                AssertionResponse = model.AssertionResponse,
                OriginalOptions = challenge.Options,
                StoredPublicKey = Convert.FromBase64String(passkeyEntity.PublicKey),
                StoredSignatureCounter = (uint)passkeyEntity.SignCount,
                IsUserHandleOwnerOfCredentialIdCallback = (p, _) => Task.FromResult(p.UserHandle.SequenceEqual(passkeyEntity.AdminUserId.ToByteArray()))
            });

            passkeyEntity.SignCount = assertionResult.SignCount;
            passkeyEntity.LastUsedOn = DateTimeOffset.UtcNow;
            await _dbContext.SaveChangesAsync();

            return BuildAuthenticatedResult(passkeyEntity.AdminUser);
        }

        private static string PasskeyRegistrationCacheKey(string challengeId) => $"passkey_reg_{challengeId}";
        private static string PasskeyLoginCacheKey(string challengeId) => $"passkey_login_{challengeId}";

        private sealed class PasskeyRegistrationChallenge
        {
            public Guid AdminUserId { get; set; }
            public CredentialCreateOptions Options { get; set; }
        }

        private sealed class PasskeyLoginChallenge
        {
            public AssertionOptions Options { get; set; }
        }
    }
}
