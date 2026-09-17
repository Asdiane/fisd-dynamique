namespace Fisd.Application.Models.receive.Diagnostics
{
    public class AuditLogQueryModel
    {
        public string? TableName { get; set; }
        public string? Action { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
    }
}
