using Fisd.Application.Models.receive.Diagnostics;
using Fisd.Application.Models.result.Diagnostics;

namespace Fisd.Application.Services.Interfaces
{
    public interface IAuditLogService
    {
        Task<AuditLogPageResultModel> GetAuditLogsAsync(AuditLogQueryModel query);
        Task<AuditLogDetailResultModel?> GetAuditLogDetailAsync(long id);
    }
}
