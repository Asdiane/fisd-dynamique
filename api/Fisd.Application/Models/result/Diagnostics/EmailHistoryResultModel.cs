namespace Fisd.Application.Models.result.Diagnostics
{
    public class EmailHistoryResultModel
    {
        public long Id { get; set; }
        public string Subject { get; set; }
        public string SentFrom { get; set; }
        public string SentTo { get; set; }
        public bool EmailSent { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    public class EmailHistoryDetailResultModel : EmailHistoryResultModel
    {
        public string Body { get; set; }
    }

    public class EmailHistoryPageResultModel
    {
        public List<EmailHistoryResultModel> Items { get; set; } = [];
        public int TotalCount { get; set; }
    }
}
