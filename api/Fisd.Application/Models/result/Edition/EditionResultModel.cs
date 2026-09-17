namespace Fisd.Application.Models.result.Edition
{
    // Admin-facing - both languages, for the edit form.
    public class EditionResultModel
    {
        public Guid Id { get; set; }
        public int Year { get; set; }
        public string BadgeFr { get; set; }
        public string BadgeEn { get; set; }
        public string TitleFr { get; set; }
        public string TitleEn { get; set; }
        public string TextFr { get; set; }
        public string TextEn { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string? LocationLabelFr { get; set; }
        public string? LocationLabelEn { get; set; }
        public string? TicketingUrl { get; set; }
        public bool IsVisible { get; set; }
        public string Status { get; set; }
        public bool IsCurrent { get; set; }
    }

    // Public-facing - resolved to the visitor's language.
    public class PublicEditionResultModel
    {
        public Guid Id { get; set; }
        public int Year { get; set; }
        public string Badge { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string? LocationLabel { get; set; }
        public string? TicketingUrl { get; set; }
        public bool IsCurrent { get; set; }
    }
}
