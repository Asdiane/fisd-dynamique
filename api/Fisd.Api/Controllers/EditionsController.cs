using Fisd.Application.Models.receive.Edition;
using Fisd.Application.Models.result.Edition;
using Fisd.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fisd.Api.Controllers
{
    [ApiController]
    [Route("api/editions")]
    public class EditionsController : ControllerBase
    {
        private readonly IEditionsService _editionsService;

        public EditionsController(IEditionsService editionsService)
        {
            _editionsService = editionsService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<EditionResultModel>>> GetAll() => Ok(await _editionsService.GetAllAsync());

        [HttpGet("visible")]
        [AllowAnonymous]
        public async Task<ActionResult<List<PublicEditionResultModel>>> GetVisible() => Ok(await _editionsService.GetVisibleAsync());

        [HttpGet("current")]
        [AllowAnonymous]
        public async Task<ActionResult<PublicEditionResultModel>> GetCurrent()
        {
            var current = await _editionsService.GetCurrentAsync();
            return current == null ? NotFound() : Ok(current);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<EditionResultModel>> Create([FromBody] SaveEditionModel model) =>
            Ok(await _editionsService.CreateAsync(model));

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<EditionResultModel>> Update(Guid id, [FromBody] SaveEditionModel model)
        {
            var updated = await _editionsService.UpdateAsync(id, model);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id) =>
            await _editionsService.DeleteAsync(id) ? NoContent() : NotFound();

        [HttpPut("{id:guid}/current")]
        [Authorize]
        public async Task<ActionResult<EditionResultModel>> SetCurrent(Guid id)
        {
            var updated = await _editionsService.SetCurrentAsync(id);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpGet("{editionId:guid}/days")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ProgramDayResultModel>>> GetDays(Guid editionId) =>
            Ok(await _editionsService.GetDaysAsync(editionId));

        [HttpPost("{editionId:guid}/days")]
        [Authorize]
        public async Task<ActionResult<ProgramDayResultModel>> AddDay(Guid editionId, [FromBody] SaveProgramDayModel model)
        {
            var day = await _editionsService.AddDayAsync(editionId, model);
            return day == null ? NotFound() : Ok(day);
        }

        [HttpPut("{editionId:guid}/days/{dayId:guid}")]
        [Authorize]
        public async Task<ActionResult<ProgramDayResultModel>> UpdateDay(Guid editionId, Guid dayId, [FromBody] SaveProgramDayModel model)
        {
            var day = await _editionsService.UpdateDayAsync(editionId, dayId, model);
            return day == null ? NotFound() : Ok(day);
        }

        [HttpDelete("{editionId:guid}/days/{dayId:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteDay(Guid editionId, Guid dayId) =>
            await _editionsService.DeleteDayAsync(editionId, dayId) ? NoContent() : NotFound();

        [HttpGet("days/{dayId:guid}/schedule")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ScheduleItemResultModel>>> GetSchedule(Guid dayId) =>
            Ok(await _editionsService.GetScheduleAsync(dayId));

        [HttpPost("days/{dayId:guid}/schedule")]
        [Authorize]
        public async Task<ActionResult<ScheduleItemResultModel>> AddScheduleItem(Guid dayId, [FromBody] SaveScheduleItemModel model)
        {
            var item = await _editionsService.AddScheduleItemAsync(dayId, model);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPut("days/{dayId:guid}/schedule/{itemId:guid}")]
        [Authorize]
        public async Task<ActionResult<ScheduleItemResultModel>> UpdateScheduleItem(Guid dayId, Guid itemId, [FromBody] SaveScheduleItemModel model)
        {
            var item = await _editionsService.UpdateScheduleItemAsync(dayId, itemId, model);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpDelete("days/{dayId:guid}/schedule/{itemId:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteScheduleItem(Guid dayId, Guid itemId) =>
            await _editionsService.DeleteScheduleItemAsync(dayId, itemId) ? NoContent() : NotFound();
    }
}
