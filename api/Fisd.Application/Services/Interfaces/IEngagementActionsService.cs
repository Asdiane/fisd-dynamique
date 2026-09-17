using Fisd.Application.Models.receive.EngagementAction;
using Fisd.Application.Models.result.EngagementAction;

namespace Fisd.Application.Services.Interfaces
{
    public interface IEngagementActionsService
    {
        Task<List<PublicEngagementActionResultModel>> GetVisibleAsync();
        Task<List<EngagementActionResultModel>> GetAllAsync();
        Task<EngagementActionResultModel> CreateAsync(SaveEngagementActionModel model);
        Task<EngagementActionResultModel?> UpdateAsync(Guid id, SaveEngagementActionModel model);
        Task<bool> DeleteAsync(Guid id);
    }
}
