using Fisd.Persistence.Enums;

namespace Fisd.Persistence.Entities.Content
{
    public class PartnerEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Label { get; set; }
        public string LogoUrl { get; set; }
        public bool IsVisible { get; set; }
        public ContentStatusEnum Status { get; set; }
        public int DisplayOrder { get; set; }
    }
}
