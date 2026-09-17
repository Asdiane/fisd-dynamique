using Fisd.Application.Models.receive.Diagnostics;
using Fisd.Application.Models.result.Diagnostics;
using Fisd.Application.Services.Interfaces;
using Fisd.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fisd.Application.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly FisdDbContext _dbContext;

        public AuditLogService(FisdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AuditLogPageResultModel> GetAuditLogsAsync(AuditLogQueryModel query)
        {
            var logs = _dbContext.AuditTrail.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.TableName))
            {
                logs = logs.Where(l => l.TableName == query.TableName);
            }
            if (!string.IsNullOrWhiteSpace(query.Action))
            {
                logs = logs.Where(l => l.Action == query.Action);
            }

            var totalCount = await logs.CountAsync();
            var page = Math.Max(query.Page, 1);
            var pageSize = Math.Clamp(query.PageSize, 1, 100);

            var rows = await logs
                .OrderByDescending(l => l.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var adminEmails = await ResolveAdminEmails(rows.Select(r => r.AdminUserId));

            var items = rows.Select(l => new AuditLogResultModel
            {
                Id = l.Id,
                Timestamp = l.Timestamp,
                AdminUserEmail = l.AdminUserId.HasValue && adminEmails.TryGetValue(l.AdminUserId.Value, out var email) ? email : null,
                TableName = l.TableName,
                Action = l.Action,
                KeyValues = l.KeyValues
            }).ToList();

            return new AuditLogPageResultModel { Items = items, TotalCount = totalCount };
        }

        public async Task<AuditLogDetailResultModel?> GetAuditLogDetailAsync(long id)
        {
            var log = await _dbContext.AuditTrail.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
            if (log == null)
            {
                return null;
            }

            var adminEmails = await ResolveAdminEmails([log.AdminUserId]);

            return new AuditLogDetailResultModel
            {
                Id = log.Id,
                Timestamp = log.Timestamp,
                AdminUserEmail = log.AdminUserId.HasValue && adminEmails.TryGetValue(log.AdminUserId.Value, out var email) ? email : null,
                TableName = log.TableName,
                Action = log.Action,
                KeyValues = log.KeyValues,
                OldValues = log.OldValues,
                NewValues = log.NewValues
            };
        }

        private async Task<Dictionary<Guid, string>> ResolveAdminEmails(IEnumerable<Guid?> adminUserIds)
        {
            var ids = adminUserIds.Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();
            if (ids.Count == 0)
            {
                return [];
            }

            return await _dbContext.AdminUsers
                .Where(u => ids.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Email);
        }
    }
}
