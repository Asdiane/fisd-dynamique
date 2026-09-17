namespace Fisd.Application.Models.receive.Partner
{
    public class SavePartnerModel
    {
        public string Name { get; set; }
        public string? Label { get; set; }
        public string LogoUrl { get; set; }
        public bool IsVisible { get; set; }
        public string Status { get; set; }
        public int DisplayOrder { get; set; }
    }
}
