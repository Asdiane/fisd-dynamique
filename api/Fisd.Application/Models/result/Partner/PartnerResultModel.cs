namespace Fisd.Application.Models.result.Partner
{
    public class PartnerResultModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Label { get; set; }
        public string LogoUrl { get; set; }
        public bool IsVisible { get; set; }
        public string Status { get; set; }
        public int DisplayOrder { get; set; }
    }
}
