namespace Fisd.Application.Models.result.Diagnostics
{
    public class SystemHealthResultModel
    {
        public bool DatabaseHealthy { get; set; }
        public int ErrorCount24h { get; set; }
        public int WarningCount24h { get; set; }
        public int ActiveAdminCount { get; set; }
        public int InactiveAdminCount { get; set; }
        public int PendingInvitationCount { get; set; }
        public List<ErrorLogResultModel> RecentErrors { get; set; } = [];
    }
}
