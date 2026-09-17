using Fisd.Application.Models.receive.Auth;
using Fisd.Application.Models.result.Auth;

namespace Fisd.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResultModel> LoginAsync(string email, string password);
        Task<LoginResultModel> EnableTwoFactorAsync(string email, string password, string totpCode);
        Task<LoginResultModel> VerifyTwoFactorAsync(string email, string password, string totpCode);
        Task RequestPasswordResetAsync(string email);
        Task<string?> ResetPasswordAsync(string token, string newPassword);

        Task<AdminInvitationInfoResultModel?> GetInvitationInfoAsync(string token);
        Task<LoginResultModel> AcceptInvitationAsync(string token, string password);

        Task<PasskeyOptionsResultModel?> GetPasskeyRegistrationOptionsAsync(Guid adminUserId);
        Task<PasskeyActionResultModel> VerifyPasskeyRegistrationAsync(Guid adminUserId, PasskeyRegisterVerifyModel model);
        Task<List<PasskeyResultModel>> ListPasskeysAsync(Guid adminUserId);
        Task<bool> DeletePasskeyAsync(Guid adminUserId, Guid passkeyId);
        PasskeyOptionsResultModel GetPasskeyLoginOptions();
        Task<LoginResultModel> VerifyPasskeyLoginAsync(PasskeyLoginVerifyModel model);
    }
}
