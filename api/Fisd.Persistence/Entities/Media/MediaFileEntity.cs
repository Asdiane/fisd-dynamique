namespace Fisd.Persistence.Entities.Media
{
    // A row per uploaded file, tracked in the DB (not just written to disk) so the admin media
    // library can list, browse and delete every image that was ever uploaded - a single-tenant
    // media library, so no per-product attachment linking is needed here.
    public class MediaFileEntity
    {
        public Guid Id { get; set; }
        public string Category { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public string ContentType { get; set; }
        public long FileSizeBytes { get; set; }
        public string StoragePath { get; set; }
        public string PublicUrl { get; set; }
        public string? AltText { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
