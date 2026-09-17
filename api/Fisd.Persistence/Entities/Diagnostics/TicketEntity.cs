using Fisd.Persistence.Enums;

namespace Fisd.Persistence.Entities.Diagnostics
{
    // Bug/improvement/other feedback submitted by any admin from the backoffice,
    // reviewed and triaged by PlatformAdmin.
    public class TicketEntity
    {
        public Guid Id { get; set; }
        public TicketCategoryEnum Category { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TicketStatusEnum Status { get; set; }
        public Guid CreatedByAdminUserId { get; set; }
        public string CreatedByEmail { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string? ResolutionNote { get; set; }
        public DateTimeOffset? ResolvedAt { get; set; }
    }
}
