namespace Fisd.Application.Models.result.Diagnostics
{
    public class AuditLogResultModel
    {
        public long Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string? AdminUserEmail { get; set; }
        public string TableName { get; set; }
        public string Action { get; set; }
        public string? KeyValues { get; set; }
    }

    public class AuditLogDetailResultModel : AuditLogResultModel
    {
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
    }

    public class AuditLogPageResultModel
    {
        public List<AuditLogResultModel> Items { get; set; } = [];
        public int TotalCount { get; set; }
    }
}
