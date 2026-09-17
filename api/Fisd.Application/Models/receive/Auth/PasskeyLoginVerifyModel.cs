using Fido2NetLib;

namespace Fisd.Application.Models.receive.Auth
{
    public class PasskeyLoginVerifyModel
    {
        public string ChallengeId { get; set; }
        public AuthenticatorAssertionRawResponse AssertionResponse { get; set; }
    }
}
