namespace Fisd.Application.Models.result.EngagementAction
{
    // Admin-facing - both languages, for the edit form.
    public class EngagementActionResultModel
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
        public string Status { get; set; }
        public int DisplayOrder { get; set; }
    }

    // Public-facing - resolved to the visitor's language.
    public class PublicEngagementActionResultModel
    {
        public Guid Id { get; set; }
        public string Anchor { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Icon { get; set; }
        public string ImageUrl { get; set; }
        public string Link { get; set; }
        public string Cta { get; set; }
        public string Detail { get; set; }
        public int DisplayOrder { get; set; }
    }
}
