using Fisd.Application.Models.receive.Diagnostics;
using Fisd.Application.Models.result.Diagnostics;

namespace Fisd.Application.Services.Interfaces
{
    public interface IErrorLogService
    {
        Task<ErrorLogPageResultModel> GetErrorLogsAsync(ErrorLogQueryModel query);
        Task<ErrorLogDetailResultModel?> GetErrorLogDetailAsync(int id);
    }
}
