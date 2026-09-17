using Fisd.Persistence.Enums;

namespace Fisd.Persistence.Entities.Security
{
    public class AdminUserEntity
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public AdminRoleEnum Role { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string? TwoFactorSecret { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<AdminPasskeyEntity> Passkeys { get; set; } = new List<AdminPasskeyEntity>();
    }
}
