namespace Fisd.Persistence.Entities.Settings
{
    // Single-row table for site-wide settings that need to be editable without a redeploy -
    // there is exactly one row, seeded with a fixed well-known Id.
    public class SiteSettingsEntity
    {
        public Guid Id { get; set; }
        public int SlideDurationSeconds { get; set; }
        public string? EditorialJson { get; set; }
    }
}
