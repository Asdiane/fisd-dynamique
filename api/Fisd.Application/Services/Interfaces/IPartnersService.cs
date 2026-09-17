using Fisd.Application.Models.receive.Partner;
using Fisd.Application.Models.result.Partner;

namespace Fisd.Application.Services.Interfaces
{
    public interface IPartnersService
    {
        Task<List<PartnerResultModel>> GetVisibleAsync();
        Task<List<PartnerResultModel>> GetAllAsync();
        Task<PartnerResultModel> CreateAsync(SavePartnerModel model);
        Task<PartnerResultModel?> UpdateAsync(Guid id, SavePartnerModel model);
        Task<bool> DeleteAsync(Guid id);
    }
}
