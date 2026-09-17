namespace Fisd.Persistence.Entities.Diagnostics
{
    // One row per email actually attempted, captured automatically by GlobalEmailService -
    // no explicit logging call site needed at any caller (invitations, password resets, etc.).
    public class EmailSentHistoryEntity
    {
        public long Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string SentFrom { get; set; }
        public string SentTo { get; set; }
        public bool EmailSent { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
