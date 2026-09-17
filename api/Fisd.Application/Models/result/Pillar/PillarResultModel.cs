namespace Fisd.Application.Models.result.Pillar
{
    // Admin-facing - both languages, for the edit form.
    public class PillarResultModel
    {
        public Guid Id { get; set; }
        public string TitleFr { get; set; }
        public string TitleEn { get; set; }
        public string TextFr { get; set; }
        public string TextEn { get; set; }
        public string ImageUrl { get; set; }
        public string Icon { get; set; }
        public bool IsVisible { get; set; }
        public string Status { get; set; }
        public int DisplayOrder { get; set; }
    }

    // Public-facing - resolved to the visitor's language.
    public class PublicPillarResultModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public string Icon { get; set; }
        public int DisplayOrder { get; set; }
    }
}
