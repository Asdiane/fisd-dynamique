namespace Fisd.Application.Models.receive.Edition
{
    public class SaveEditionModel
    {
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
    }
}
