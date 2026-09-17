using Fisd.Persistence.Enums;

namespace Fisd.Persistence.Entities.Content
{
    public class EngagementActionEntity
    {
        public Guid Id { get; set; }
        public string Anchor { get; set; }
        public string TitleFr { get; set; }
        public string TitleEn { get; set; }
        public string TextFr { get; set; }
        public string TextEn { get; set; }
        public string Icon { get; set; }
        public string ImageUrl { get; set; }
        public string Link { get; set; }
        public string CtaFr { get; set; }
        public string CtaEn { get; set; }
        public string DetailFr { get; set; }
        public string DetailEn { get; set; }
        public bool IsVisible { get; set; }
        public ContentStatusEnum Status { get; set; }
        public int DisplayOrder { get; set; }
    }
}
