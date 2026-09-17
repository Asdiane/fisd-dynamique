using Fisd.Application.Models.receive.Slide;
using Fisd.Application.Models.result.Slide;

namespace Fisd.Application.Services.Interfaces
{
    public interface ISlidesService
    {
        Task<List<PublicSlideResultModel>> GetVisibleAsync();
        Task<List<SlideResultModel>> GetAllAsync();
        Task<SlideResultModel> CreateAsync(SaveSlideModel model);
        Task<SlideResultModel?> UpdateAsync(Guid id, SaveSlideModel model);
        Task<bool> DeleteAsync(Guid id);
    }
}
