using Fisd.Application.Models.receive.Pillar;
using Fisd.Application.Models.result.Pillar;

namespace Fisd.Application.Services.Interfaces
{
    public interface IPillarsService
    {
        Task<List<PublicPillarResultModel>> GetVisibleAsync();
        Task<List<PillarResultModel>> GetAllAsync();
        Task<PillarResultModel> CreateAsync(SavePillarModel model);
        Task<PillarResultModel?> UpdateAsync(Guid id, SavePillarModel model);
        Task<bool> DeleteAsync(Guid id);
    }
}
