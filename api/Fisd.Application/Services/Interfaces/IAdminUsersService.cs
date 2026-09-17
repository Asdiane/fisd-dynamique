using Fisd.Application.Models.receive.AdminUser;
using Fisd.Application.Models.result.AdminUser;
using Fisd.Persistence.Enums;

namespace Fisd.Application.Services.Interfaces
{
    public class AdminUserOperationException : Exception
    {
        public AdminUserOperationException(string message) : base(message)
        {
        }
    }

    public interface IAdminUsersService
    {
        Task<List<AdminUserResultModel>> GetAllAsync();
        Task InviteAsync(InviteAdminUserModel model, Guid actingAdminId, AdminRoleEnum actingAdminRole);
        Task<List<PendingAdminInvitationResultModel>> GetPendingInvitationsAsync();
        Task<bool> CancelInvitationAsync(Guid id);
        Task<AdminUserResultModel?> UpdateRoleAsync(Guid id, string role, Guid actingAdminId, AdminRoleEnum actingAdminRole);
        Task<bool> DeleteAsync(Guid id, Guid actingAdminId, AdminRoleEnum actingAdminRole);
        Task<AdminUserResultModel?> SetActiveAsync(Guid id, bool isActive, Guid actingAdminId, AdminRoleEnum actingAdminRole);
        Task<AdminUserResultModel?> ResetTwoFactorAsync(Guid id, AdminRoleEnum actingAdminRole);
    }
}
