namespace Fisd.Persistence.Entities.Diagnostics
{
    public class TicketAttachmentEntity
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public string ContentType { get; set; }
        public long FileSizeBytes { get; set; }
        public string StoragePath { get; set; }
        public string PublicUrl { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
