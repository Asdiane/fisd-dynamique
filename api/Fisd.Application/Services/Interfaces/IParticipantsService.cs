using Fisd.Application.Models.receive.Participant;
using Fisd.Application.Models.result.Participant;

namespace Fisd.Application.Services.Interfaces
{
    public interface IParticipantsService
    {
        Task<List<PublicParticipantResultModel>> GetVisibleAsync();
        Task<List<ParticipantResultModel>> GetAllAsync();
        Task<ParticipantResultModel> CreateAsync(SaveParticipantModel model);
        Task<ParticipantResultModel?> UpdateAsync(Guid id, SaveParticipantModel model);
        Task<bool> DeleteAsync(Guid id);
    }
}
