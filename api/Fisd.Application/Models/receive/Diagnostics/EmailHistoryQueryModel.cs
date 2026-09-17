namespace Fisd.Application.Models.receive.Diagnostics
{
    public class EmailHistoryQueryModel
    {
        public string? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
    }
}
