using System.Security.Cryptography;
using Fisd.Application.Email;
using Fisd.Application.Email.Models;
using Fisd.Application.Models.receive.AdminUser;
using Fisd.Application.Models.result.AdminUser;
using Fisd.Application.Security;
using Fisd.Application.Services.Interfaces;
using Fisd.Application.Site;
using Fisd.Persistence;
using Fisd.Persistence.Entities.Security;
using Fisd.Persistence.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Fisd.Application.Services
{
    public class AdminUsersService : IAdminUsersService
    {
        private static readonly TimeSpan InvitationLifetime = TimeSpan.FromHours(48);

        private readonly FisdDbContext _dbContext;
        private readonly IGlobalEmailService _globalEmailService;
        private readonly SiteOptions _siteOptions;

        public AdminUsersService(FisdDbContext dbContext, IGlobalEmailService globalEmailService, IOptions<SiteOptions> siteOptions)
        {
            _dbContext = dbContext;
            _globalEmailService = globalEmailService;
            _siteOptions = siteOptions.Value;
        }

        public async Task<List<AdminUserResultModel>> GetAllAsync()
        {
            var entities = await _dbContext.AdminUsers.OrderBy(u => u.Email).ToListAsync();
            return entities.Select(ToResultModel).ToList();
        }

        public async Task InviteAsync(InviteAdminUserModel model, Guid actingAdminId, AdminRoleEnum actingAdminRole)
        {
            var normalizedEmail = model.Email.Trim().ToLowerInvariant();
            if (await _dbContext.AdminUsers.AnyAsync(u => u.Email == normalizedEmail))
            {
                throw new AdminUserOperationException("Un compte existe déjà avec ce courriel.");
            }

            // Re-inviting the same address (e.g. the first email got lost) replaces any invite
            // still pending rather than blocking - the old link is invalidated by removing it,
            // so only the newest one can ever be accepted.
            var pendingInvitations = await _dbContext.AdminUserInvitations
                .Where(i => i.Email == normalizedEmail && i.AcceptedAt == null)
                .ToListAsync();
            if (pendingInvitations.Count > 0)
            {
                _dbContext.AdminUserInvitations.RemoveRange(pendingInvitations);
            }

            var role = ParseRole(model.Role);
            EnsureCanAssignRole(role, actingAdminRole);

            var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                .Replace('+', '-').Replace('/', '_').TrimEnd('=');

            _dbContext.AdminUserInvitations.Add(new AdminUserInvitationEntity
            {
                Id = Guid.NewGuid(),
                Email = normalizedEmail,
                Role = role,
                TokenHash = HashToken(rawToken),
                ExpiresAt = DateTimeOffset.UtcNow.Add(InvitationLifetime),
                CreatedAt = DateTimeOffset.UtcNow,
                InvitedByAdminUserId = actingAdminId
            });
            await _dbContext.SaveChangesAsync();

            await SendInvitationEmailAsync(normalizedEmail, rawToken);
        }

        public async Task<List<PendingAdminInvitationResultModel>> GetPendingInvitationsAsync()
        {
            var invitations = await _dbContext.AdminUserInvitations
                .Where(i => i.AcceptedAt == null && i.ExpiresAt > DateTimeOffset.UtcNow)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return invitations.Select(i => new PendingAdminInvitationResultModel
            {
                Id = i.Id,
                Email = i.Email,
                Role = i.Role.ToString(),
                ExpiresAt = i.ExpiresAt,
                CreatedAt = i.CreatedAt
            }).ToList();
        }

        public async Task<bool> CancelInvitationAsync(Guid id)
        {
            var invitation = await _dbContext.AdminUserInvitations.FirstOrDefaultAsync(i => i.Id == id && i.AcceptedAt == null);
            if (invitation == null)
            {
                return false;
            }

            _dbContext.AdminUserInvitations.Remove(invitation);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private async Task SendInvitationEmailAsync(string email, string rawToken)
        {
            var invitationLink = $"{_siteOptions.BaseUrl.TrimEnd('/')}/admin/accepter-invitation?token={rawToken}";
            var subject = "Invitation à administrer le site FISD";
            var body = $"""
                <div style="background:#F7FAFF;padding:32px 16px;font-family:Segoe UI,Arial,sans-serif;">
                  <div style="max-width:520px;margin:0 auto;background:#FFFFFF;border-radius:16px;overflow:hidden;box-shadow:0 25px 65px -15px rgba(0,0,34,0.18);">
                    <div style="background:#000022;padding:24px 32px;">
                      <span style="color:#FFFFFF;font-weight:700;font-size:18px;letter-spacing:0.02em;">FISD</span>
                    </div>
                    <div style="padding:32px;">
                      <h1 style="margin:0 0 12px;font-size:20px;color:#000022;">Vous êtes invité(e) à administrer le site FISD</h1>
                      <p style="margin:0 0 20px;font-size:14px;line-height:1.6;color:#0F172A;">
                        Un accès administrateur a été créé pour vous ({email}). Cliquez sur le bouton ci-dessous pour
                        choisir votre mot de passe et activer votre compte.
                      </p>
                      <div style="text-align:center;margin:28px 0;">
                        <a href="{invitationLink}" style="display:inline-block;background:#0000FF;color:#FFFFFF;font-weight:700;font-size:14px;text-decoration:none;padding:12px 28px;border-radius:8px;">
                          Activer mon compte
                        </a>
                      </div>
                      <p style="margin:0 0 8px;font-size:12px;color:#64748B;">
                        Ce lien expire dans 48 heures. Si vous ne vous attendiez pas à ce courriel, vous pouvez l'ignorer.
                      </p>
                      <p style="margin:0;font-size:12px;color:#64748B;">
                        Vous ne voyez pas ce courriel ? Vérifiez votre dossier de courriers indésirables (spam).
                      </p>
                    </div>
                    <div style="background:#000022;padding:16px 32px;">
                      <span style="color:#8585FF;font-size:11px;">Festival International des Solidarités et du Développement</span>
                    </div>
                  </div>
                </div>
                """;

            var sender = _globalEmailService.GetEmailOptions().Senders["Generic"];
            var messages = new List<Message>([new Message
            {
                To = new Recipient { Name = email, Email = email },
                Subject = subject,
                Body = body
            }]);

            try
            {
                await _globalEmailService.SendEmailAsync(sender, messages);
            }
            catch (InvalidOperationException)
            {
                throw new AdminUserOperationException("L'invitation a été enregistrée, mais le courriel n'a pas pu être envoyé : la configuration SMTP du serveur est manquante. Contactez un administrateur système.");
            }
        }

        private static string HashToken(string token) => Convert.ToBase64String(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token)));

        public async Task<AdminUserResultModel?> UpdateRoleAsync(Guid id, string role, Guid actingAdminId, AdminRoleEnum actingAdminRole)
        {
            if (id == actingAdminId)
            {
                throw new AdminUserOperationException("Vous ne pouvez pas modifier votre propre rôle.");
            }

            var entity = await _dbContext.AdminUsers.FirstOrDefaultAsync(u => u.Id == id);
            if (entity == null)
            {
                return null;
            }

            var newRole = ParseRole(role);
            EnsureCanActOn(entity.Role, actingAdminRole);
            EnsureCanAssignRole(newRole, actingAdminRole);

            if (entity.Role == AdminRoleEnum.SuperAdmin && newRole != AdminRoleEnum.SuperAdmin && await IsLastAsync(AdminRoleEnum.SuperAdmin, entity.Id))
            {
                throw new AdminUserOperationException("Impossible de rétrograder le dernier super-administrateur.");
            }
            if (entity.Role == AdminRoleEnum.PlatformAdmin && newRole != AdminRoleEnum.PlatformAdmin && await IsLastAsync(AdminRoleEnum.PlatformAdmin, entity.Id))
            {
                throw new AdminUserOperationException("Impossible de rétrograder le dernier administrateur plateforme.");
            }

            entity.Role = newRole;
            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<bool> DeleteAsync(Guid id, Guid actingAdminId, AdminRoleEnum actingAdminRole)
        {
            if (id == actingAdminId)
            {
                throw new AdminUserOperationException("Vous ne pouvez pas supprimer votre propre compte.");
            }

            var entity = await _dbContext.AdminUsers.FirstOrDefaultAsync(u => u.Id == id);
            if (entity == null)
            {
                return false;
            }

            EnsureCanActOn(entity.Role, actingAdminRole);

            if (entity.Role == AdminRoleEnum.SuperAdmin && await IsLastAsync(AdminRoleEnum.SuperAdmin, entity.Id))
            {
                throw new AdminUserOperationException("Impossible de supprimer le dernier super-administrateur.");
            }
            if (entity.Role == AdminRoleEnum.PlatformAdmin && await IsLastAsync(AdminRoleEnum.PlatformAdmin, entity.Id))
            {
                throw new AdminUserOperationException("Impossible de supprimer le dernier administrateur plateforme.");
            }

            _dbContext.AdminUsers.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<AdminUserResultModel?> SetActiveAsync(Guid id, bool isActive, Guid actingAdminId, AdminRoleEnum actingAdminRole)
        {
            if (actingAdminRole != AdminRoleEnum.PlatformAdmin)
            {
                throw new AdminUserOperationException("Seul un administrateur plateforme peut activer ou désactiver un compte.");
            }
            if (id == actingAdminId)
            {
                throw new AdminUserOperationException("Vous ne pouvez pas désactiver votre propre compte.");
            }

            var entity = await _dbContext.AdminUsers.FirstOrDefaultAsync(u => u.Id == id);
            if (entity == null)
            {
                return null;
            }

            if (!isActive && entity.Role == AdminRoleEnum.PlatformAdmin && await IsLastAsync(AdminRoleEnum.PlatformAdmin, entity.Id))
            {
                throw new AdminUserOperationException("Impossible de désactiver le dernier administrateur plateforme.");
            }

            entity.IsActive = isActive;
            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<AdminUserResultModel?> ResetTwoFactorAsync(Guid id, AdminRoleEnum actingAdminRole)
        {
            if (actingAdminRole != AdminRoleEnum.PlatformAdmin)
            {
                throw new AdminUserOperationException("Seul un administrateur plateforme peut réinitialiser la double authentification.");
            }

            var entity = await _dbContext.AdminUsers.FirstOrDefaultAsync(u => u.Id == id);
            if (entity == null)
            {
                return null;
            }

            entity.TwoFactorEnabled = false;
            entity.TwoFactorSecret = null;
            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        private static void EnsureCanActOn(AdminRoleEnum targetRole, AdminRoleEnum actingAdminRole)
        {
            if (targetRole == AdminRoleEnum.PlatformAdmin && actingAdminRole != AdminRoleEnum.PlatformAdmin)
            {
                throw new AdminUserOperationException("Seul un administrateur plateforme peut agir sur ce compte.");
            }
        }

        private static void EnsureCanAssignRole(AdminRoleEnum role, AdminRoleEnum actingAdminRole)
        {
            if (role == AdminRoleEnum.PlatformAdmin && actingAdminRole != AdminRoleEnum.PlatformAdmin)
            {
                throw new AdminUserOperationException("Seul un administrateur plateforme peut attribuer ce rôle.");
            }
        }

        private async Task<bool> IsLastAsync(AdminRoleEnum role, Guid excludingId) =>
            !await _dbContext.AdminUsers.AnyAsync(u => u.Id != excludingId && u.Role == role);

        private static AdminRoleEnum ParseRole(string role) =>
            Enum.TryParse<AdminRoleEnum>(role, ignoreCase: true, out var parsed)
                ? parsed
                : throw new AdminUserOperationException("Rôle invalide.");

        private static AdminUserResultModel ToResultModel(AdminUserEntity entity) => new()
        {
            Id = entity.Id,
            Email = entity.Email,
            Role = entity.Role.ToString(),
            CreatedAt = entity.CreatedAt,
            IsActive = entity.IsActive,
            TwoFactorEnabled = entity.TwoFactorEnabled
        };
    }
}
