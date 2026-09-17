namespace Fisd.Application.Models.result.Media
{
    public class MediaFileResultModel
    {
        public Guid Id { get; set; }
        public string Category { get; set; }
        public string OriginalFileName { get; set; }
        public string ContentType { get; set; }
        public long FileSizeBytes { get; set; }
        public string Url { get; set; }
        public string? AltText { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
