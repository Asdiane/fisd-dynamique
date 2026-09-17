namespace Fisd.Application.Models.result.Slide
{
    // Admin-facing - carries both languages so the edit form can show them side by side.
    public class SlideResultModel
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
        public string Status { get; set; }
        public int DisplayOrder { get; set; }
    }

    // Public-facing - already resolved to the visitor's language, so the site never has to
    // know about the Fr/En split.
    public class PublicSlideResultModel
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public string? FocalPoint { get; set; }
        public string Kicker { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string? Place { get; set; }
        public int DisplayOrder { get; set; }
    }
}
