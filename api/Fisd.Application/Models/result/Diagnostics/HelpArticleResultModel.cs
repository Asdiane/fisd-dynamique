namespace Fisd.Application.Models.result.Diagnostics
{
    // PlatformAdmin-facing - both languages, for the management form.
    public class HelpArticleResultModel
    {
        public Guid Id { get; set; }
        public string Category { get; set; }
        public string TitleFr { get; set; }
        public string TitleEn { get; set; }
        public string ContentFr { get; set; }
        public string ContentEn { get; set; }
        public bool IsVisible { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsPlatformAdminOnly { get; set; }
    }

    // Reader-facing (any admin role) - resolved to the reader's language.
    public class PublicHelpArticleResultModel
    {
        public Guid Id { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int DisplayOrder { get; set; }
    }
}
