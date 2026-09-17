using Fisd.Application.Models.receive.Speaker;
using Fisd.Application.Models.result.Speaker;

namespace Fisd.Application.Services.Interfaces
{
    public interface ISpeakersService
    {
        Task<List<PublicSpeakerResultModel>> GetVisibleAsync();
        Task<List<PublicSpeakerResultModel>> GetVisibleByEditionAsync(Guid editionId);
        Task<List<SpeakerResultModel>> GetAllAsync();
        Task<SpeakerResultModel> CreateAsync(SaveSpeakerModel model);
        Task<SpeakerResultModel?> UpdateAsync(Guid id, SaveSpeakerModel model);
        Task<bool> DeleteAsync(Guid id);
    }
}
