namespace Fisd.Application.Models.receive.Diagnostics
{
    public class SaveHelpArticleModel
    {
        public string Category { get; set; }
        public string TitleFr { get; set; }
        public string TitleEn { get; set; }
        public string ContentFr { get; set; }
        public string ContentEn { get; set; }
        public bool IsVisible { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsPlatformAdminOnly { get; set; }
    }
}
