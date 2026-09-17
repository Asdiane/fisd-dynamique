namespace Fisd.Application.Models.receive.Diagnostics
{
    public class ErrorLogQueryModel
    {
        public string? Level { get; set; }
        public DateTimeOffset? From { get; set; }
        public DateTimeOffset? To { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
    }
}
