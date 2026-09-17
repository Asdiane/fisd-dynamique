using Fisd.Persistence.Enums;

namespace Fisd.Persistence.Entities.Security
{
    public class AdminUserInvitationEntity
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public AdminRoleEnum Role { get; set; }
        public string TokenHash { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
        public DateTimeOffset? AcceptedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public Guid InvitedByAdminUserId { get; set; }
    }
}
