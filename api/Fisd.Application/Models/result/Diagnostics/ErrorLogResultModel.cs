namespace Fisd.Application.Models.result.Diagnostics
{
    public class ErrorLogResultModel
    {
        public int Id { get; set; }
        public DateTimeOffset Logged { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string? Logger { get; set; }
        public string? RequestMethod { get; set; }
        public string? RequestUrl { get; set; }
        public string? UserEmail { get; set; }
        public bool HasException { get; set; }
    }

    public class ErrorLogDetailResultModel : ErrorLogResultModel
    {
        public string? Exception { get; set; }
    }

    public class ErrorLogPageResultModel
    {
        public List<ErrorLogResultModel> Items { get; set; } = [];
        public int TotalCount { get; set; }
    }
}
