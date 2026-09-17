using Fisd.Persistence.Enums;

namespace Fisd.Persistence.Entities.Content
{
    public class SlideEntity
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public string? FocalPoint { get; set; }
        public string KickerFr { get; set; }
        public string KickerEn { get; set; }
        public string TitleFr { get; set; }
        public string TitleEn { get; set; }
        public string TextFr { get; set; }
        public string TextEn { get; set; }
        public string? PlaceFr { get; set; }
        public string? PlaceEn { get; set; }
        public bool IsVisible { get; set; }
        public ContentStatusEnum Status { get; set; }
        public int DisplayOrder { get; set; }
    }
}
