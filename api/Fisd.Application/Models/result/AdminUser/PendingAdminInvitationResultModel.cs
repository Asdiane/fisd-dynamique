namespace Fisd.Application.Models.result.AdminUser
{
    public class PendingAdminInvitationResultModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
