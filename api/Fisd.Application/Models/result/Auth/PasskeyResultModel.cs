namespace Fisd.Application.Models.result.Auth
{
    public class PasskeyResultModel
    {
        public Guid Id { get; set; }
        public string DeviceLabel { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? LastUsedOn { get; set; }
    }
}
