namespace Fisd.Application.Models.receive.Contact
{
    public class SaveContactModel
    {
        public string Type { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public string? Href { get; set; }
        public bool IsVisible { get; set; }
        public string Status { get; set; }
        public int DisplayOrder { get; set; }
    }
}
