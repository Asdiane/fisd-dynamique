namespace Fisd.Persistence.Entities.Security
{
    public class PasswordResetTokenEntity
    {
        public Guid Id { get; set; }
        public Guid AdminUserId { get; set; }
        public string TokenHash { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
        public DateTimeOffset? UsedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
