using Fisd.Application.Models.result.Diagnostics;
using Fisd.Application.Services.Interfaces;
using Fisd.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fisd.Application.Services
{
    public class SystemHealthService : ISystemHealthService
    {
        private readonly FisdDbContext _dbContext;

        public SystemHealthService(FisdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SystemHealthResultModel> GetSystemHealthAsync()
        {
            var databaseHealthy = await _dbContext.Database.CanConnectAsync();
            if (!databaseHealthy)
            {
                return new SystemHealthResultModel { DatabaseHealthy = false };
            }

            var since = DateTimeOffset.UtcNow.AddHours(-24);

            var errorCount24h = await _dbContext.ErrorLogs.CountAsync(l => l.Logged >= since && (l.Level == "Error" || l.Level == "Fatal"));
            var warningCount24h = await _dbContext.ErrorLogs.CountAsync(l => l.Logged >= since && l.Level == "Warn");
            var activeAdminCount = await _dbContext.AdminUsers.CountAsync(u => u.IsActive);
            var inactiveAdminCount = await _dbContext.AdminUsers.CountAsync(u => !u.IsActive);
            var pendingInvitationCount = await _dbContext.AdminUserInvitations.CountAsync(i => i.AcceptedAt == null && i.ExpiresAt > DateTimeOffset.UtcNow);

            var recentErrors = await _dbContext.ErrorLogs
                .Where(l => l.Level == "Error" || l.Level == "Fatal")
                .OrderByDescending(l => l.Logged)
                .Take(10)
                .Select(l => new ErrorLogResultModel
                {
                    Id = l.Id,
                    Logged = l.Logged,
                    Level = l.Level,
                    Message = l.Message,
                    Logger = l.Logger,
                    RequestMethod = l.RequestMethod,
                    RequestUrl = l.RequestUrl,
                    UserEmail = l.UserEmail,
                    HasException = l.Exception != null
                })
                .ToListAsync();

            return new SystemHealthResultModel
            {
                DatabaseHealthy = true,
                ErrorCount24h = errorCount24h,
                WarningCount24h = warningCount24h,
                ActiveAdminCount = activeAdminCount,
                InactiveAdminCount = inactiveAdminCount,
                PendingInvitationCount = pendingInvitationCount,
                RecentErrors = recentErrors
            };
        }
    }
}
