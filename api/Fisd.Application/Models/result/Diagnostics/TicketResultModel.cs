namespace Fisd.Application.Models.result.Diagnostics
{
    public class TicketResultModel
    {
        public Guid Id { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string CreatedByEmail { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string? ResolutionNote { get; set; }
        public DateTimeOffset? ResolvedAt { get; set; }
        public List<TicketAttachmentResultModel> Attachments { get; set; } = [];
    }

    public class TicketAttachmentResultModel
    {
        public Guid Id { get; set; }
        public string OriginalFileName { get; set; }
        public string ContentType { get; set; }
        public long FileSizeBytes { get; set; }
        public string Url { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
