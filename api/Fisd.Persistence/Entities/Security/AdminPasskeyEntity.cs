namespace Fisd.Persistence.Entities.Security
{
    public class AdminPasskeyEntity
    {
        public Guid Id { get; set; }
        public Guid AdminUserId { get; set; }
        public string CredentialId { get; set; }
        public string PublicKey { get; set; }
        public long SignCount { get; set; }
        public string DeviceLabel { get; set; }
        public Guid? AaGuid { get; set; }
        public string? Transports { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? LastUsedOn { get; set; }
        public AdminUserEntity AdminUser { get; set; }
    }
}
