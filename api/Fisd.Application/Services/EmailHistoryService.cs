using Fisd.Application.Models.receive.Diagnostics;
using Fisd.Application.Models.result.Diagnostics;
using Fisd.Application.Services.Interfaces;
using Fisd.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fisd.Application.Services
{
    public class EmailHistoryService : IEmailHistoryService
    {
        private readonly FisdDbContext _dbContext;

        public EmailHistoryService(FisdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EmailHistoryPageResultModel> GetHistoryAsync(EmailHistoryQueryModel query)
        {
            var history = _dbContext.EmailSentHistory.AsNoTracking().AsQueryable();

            if (query.Status == "sent")
            {
                history = history.Where(e => e.EmailSent);
            }
            else if (query.Status == "failed")
            {
                history = history.Where(e => !e.EmailSent);
            }

            var totalCount = await history.CountAsync();
            var page = Math.Max(query.Page, 1);
            var pageSize = Math.Clamp(query.PageSize, 1, 100);

            var items = await history
                .OrderByDescending(e => e.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EmailHistoryResultModel
                {
                    Id = e.Id,
                    Subject = e.Subject,
                    SentFrom = e.SentFrom,
                    SentTo = e.SentTo,
                    EmailSent = e.EmailSent,
                    ErrorMessage = e.ErrorMessage,
                    CreatedAt = e.CreatedAt
                })
                .ToListAsync();

            return new EmailHistoryPageResultModel { Items = items, TotalCount = totalCount };
        }

        public async Task<EmailHistoryDetailResultModel?> GetDetailAsync(long id)
        {
            var entry = await _dbContext.EmailSentHistory.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (entry == null)
            {
                return null;
            }

            return new EmailHistoryDetailResultModel
            {
                Id = entry.Id,
                Subject = entry.Subject,
                SentFrom = entry.SentFrom,
                SentTo = entry.SentTo,
                EmailSent = entry.EmailSent,
                ErrorMessage = entry.ErrorMessage,
                CreatedAt = entry.CreatedAt,
                Body = entry.Body
            };
        }
    }
}
