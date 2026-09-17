namespace Fisd.Application.Models.receive.Auth
{
    public class TwoFactorVerifyModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string TotpCode { get; set; }
    }
}
