namespace Fisd.Application.Models.result.Auth
{
    public class PasskeyActionResultModel
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public PasskeyResultModel? Passkey { get; set; }
    }
}
