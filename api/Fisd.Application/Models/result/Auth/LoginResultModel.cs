namespace Fisd.Application.Models.result.Auth
{
    public class LoginResultModel
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public string? Token { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public bool RequiresTwoFactor { get; set; }
        public bool RequiresTwoFactorSetup { get; set; }
        public string? TwoFactorSecret { get; set; }
        public string? TwoFactorQrUri { get; set; }
    }
}
