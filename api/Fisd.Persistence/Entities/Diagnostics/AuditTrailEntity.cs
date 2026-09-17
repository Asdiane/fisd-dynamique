namespace Fisd.Persistence.Entities.Diagnostics
{
    // One row per changed entity per SaveChanges call, captured automatically by
    // FisdDbContext.SaveChangesAsync - no explicit logging call site needed anywhere.
    public class AuditTrailEntity
    {
        public long Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public Guid? AdminUserId { get; set; }
        public string TableName { get; set; }
        public string Action { get; set; }
        public string? KeyValues { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
    }
}
