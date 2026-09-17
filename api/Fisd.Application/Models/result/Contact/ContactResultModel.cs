namespace Fisd.Application.Models.result.Contact
{
    public class ContactResultModel
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public string? Href { get; set; }
        public bool IsVisible { get; set; }
        public string Status { get; set; }
        public int DisplayOrder { get; set; }
    }
}
