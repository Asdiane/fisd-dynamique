namespace Fisd.Persistence.Entities.Diagnostics
{
    // Written directly by NLog's Database target (see api/Fisd.Api/NLog.config) whenever any
    // ILogger.LogWarning/LogError/LogCritical call fires anywhere in the app - EF only reads
    // this table back for the PlatformAdmin error-log page, it never inserts into it.
    public class ErrorLogEntity
    {
        public int Id { get; set; }
        public DateTimeOffset Logged { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string? Logger { get; set; }
        public string? Exception { get; set; }
        public string? RequestMethod { get; set; }
        public string? RequestUrl { get; set; }
        public string? UserEmail { get; set; }
    }
}
