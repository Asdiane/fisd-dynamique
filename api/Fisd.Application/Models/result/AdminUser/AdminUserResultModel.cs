namespace Fisd.Application.Models.result.AdminUser
{
    public class AdminUserResultModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public bool TwoFactorEnabled { get; set; }
    }
}
