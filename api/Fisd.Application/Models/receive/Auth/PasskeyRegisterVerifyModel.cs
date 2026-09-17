using Fido2NetLib;

namespace Fisd.Application.Models.receive.Auth
{
    public class PasskeyRegisterVerifyModel
    {
        public string ChallengeId { get; set; }
        public string DeviceLabel { get; set; }
        public AuthenticatorAttestationRawResponse AttestationResponse { get; set; }
    }
}
