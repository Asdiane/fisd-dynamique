using Fisd.Application.Models.receive.Diagnostics;
using Fisd.Application.Models.result.Diagnostics;

namespace Fisd.Application.Services.Interfaces
{
    public interface IEmailHistoryService
    {
        Task<EmailHistoryPageResultModel> GetHistoryAsync(EmailHistoryQueryModel query);
        Task<EmailHistoryDetailResultModel?> GetDetailAsync(long id);
    }
}
