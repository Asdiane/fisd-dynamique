namespace Fisd.Persistence.Entities.Diagnostics
{
    // Backoffice-only documentation for admin users (Editor and up) - never exposed on the
    // public site. PlatformAdmin manages the content, every other admin role can read it.
    public class HelpArticleEntity
    {
        public Guid Id { get; set; }
        public string Category { get; set; }
        public string TitleFr { get; set; }
        public string TitleEn { get; set; }
        public string ContentFr { get; set; }
        public string ContentEn { get; set; }
        public bool IsVisible { get; set; }
        public int DisplayOrder { get; set; }

        // The PlatformAdmin role itself is deliberately never mentioned to Editor/SuperAdmin
        // users - they aren't meant to know it exists. Articles flagged here (oversight tooling,
        // etc.) are filtered out of the general reading list and only shown to PlatformAdmin.
        public bool IsPlatformAdminOnly { get; set; }
    }
}
