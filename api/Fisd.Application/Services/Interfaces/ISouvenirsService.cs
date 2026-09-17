using Fisd.Application.Models.receive.Souvenir;
using Fisd.Application.Models.result.Souvenir;

namespace Fisd.Application.Services.Interfaces
{
    public interface ISouvenirsService
    {
        Task<List<SouvenirResultModel>> GetAllAsync();
        Task<List<SouvenirPhotoResultModel>> GetPhotosAsync(Guid souvenirId);
        Task<SouvenirResultModel> CreateAsync(SaveSouvenirModel model);
        Task<SouvenirResultModel?> UpdateAsync(Guid id, SaveSouvenirModel model);
        Task<bool> DeleteAsync(Guid id);
        Task<SouvenirPhotoResultModel?> AddPhotoAsync(Guid souvenirId, SaveSouvenirPhotoModel model);
        Task<bool> DeletePhotoAsync(Guid souvenirId, Guid photoId);
    }
}
