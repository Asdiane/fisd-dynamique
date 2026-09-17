namespace Fisd.Application.Models.result.Auth
{
    public class AdminInvitationInfoResultModel
    {
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
    }
}
