using Fisd.Persistence.Enums;

namespace Fisd.Persistence.Entities.Content
{
    public class ContactEntity
    {
        public Guid Id { get; set; }
        public ContactTypeEnum Type { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public string? Href { get; set; }
        public bool IsVisible { get; set; }
        public ContentStatusEnum Status { get; set; }
        public int DisplayOrder { get; set; }
    }
}
