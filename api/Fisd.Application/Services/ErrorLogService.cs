using Fisd.Application.Models.receive.Diagnostics;
using Fisd.Application.Models.result.Diagnostics;
using Fisd.Application.Services.Interfaces;
using Fisd.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fisd.Application.Services
{
    public class ErrorLogService : IErrorLogService
    {
        private readonly FisdDbContext _dbContext;

        public ErrorLogService(FisdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ErrorLogPageResultModel> GetErrorLogsAsync(ErrorLogQueryModel query)
        {
            var logs = _dbContext.ErrorLogs.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Level))
            {
                logs = logs.Where(l => l.Level == query.Level);
            }
            if (query.From.HasValue)
            {
                logs = logs.Where(l => l.Logged >= query.From.Value);
            }
            if (query.To.HasValue)
            {
                logs = logs.Where(l => l.Logged <= query.To.Value);
            }

            var totalCount = await logs.CountAsync();
            var page = Math.Max(query.Page, 1);
            var pageSize = Math.Clamp(query.PageSize, 1, 100);

            var items = await logs
                .OrderByDescending(l => l.Logged)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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

            return new ErrorLogPageResultModel { Items = items, TotalCount = totalCount };
        }

        public async Task<ErrorLogDetailResultModel?> GetErrorLogDetailAsync(int id)
        {
            var log = await _dbContext.ErrorLogs.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
            if (log == null)
            {
                return null;
            }

            return new ErrorLogDetailResultModel
            {
                Id = log.Id,
                Logged = log.Logged,
                Level = log.Level,
                Message = log.Message,
                Logger = log.Logger,
                RequestMethod = log.RequestMethod,
                RequestUrl = log.RequestUrl,
                UserEmail = log.UserEmail,
                HasException = log.Exception != null,
                Exception = log.Exception
            };
        }
    }
}
