using Fisd.Application.Models.receive.Edition;
using Fisd.Application.Models.result.Edition;

namespace Fisd.Application.Services.Interfaces
{
    public interface IEditionsService
    {
        Task<List<EditionResultModel>> GetAllAsync();
        Task<List<PublicEditionResultModel>> GetVisibleAsync();
        Task<PublicEditionResultModel?> GetCurrentAsync();
        Task<List<ProgramDayResultModel>> GetDaysAsync(Guid editionId);
        Task<List<ScheduleItemResultModel>> GetScheduleAsync(Guid dayId);

        Task<EditionResultModel> CreateAsync(SaveEditionModel model);
        Task<EditionResultModel?> UpdateAsync(Guid id, SaveEditionModel model);
        Task<bool> DeleteAsync(Guid id);
        Task<EditionResultModel?> SetCurrentAsync(Guid id);

        Task<ProgramDayResultModel?> AddDayAsync(Guid editionId, SaveProgramDayModel model);
        Task<ProgramDayResultModel?> UpdateDayAsync(Guid editionId, Guid dayId, SaveProgramDayModel model);
        Task<bool> DeleteDayAsync(Guid editionId, Guid dayId);

        Task<ScheduleItemResultModel?> AddScheduleItemAsync(Guid dayId, SaveScheduleItemModel model);
        Task<ScheduleItemResultModel?> UpdateScheduleItemAsync(Guid dayId, Guid itemId, SaveScheduleItemModel model);
        Task<bool> DeleteScheduleItemAsync(Guid dayId, Guid itemId);
    }
}
