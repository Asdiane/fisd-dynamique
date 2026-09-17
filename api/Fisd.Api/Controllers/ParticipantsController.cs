using Fisd.Application.Models.receive.Participant;
using Fisd.Application.Models.result.Participant;
using Fisd.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fisd.Api.Controllers
{
    [ApiController]
    [Route("api/participants")]
    public class ParticipantsController : ControllerBase
    {
        private readonly IParticipantsService _participantsService;

        public ParticipantsController(IParticipantsService participantsService)
        {
            _participantsService = participantsService;
        }

        [HttpGet("visible")]
        [AllowAnonymous]
        public async Task<ActionResult<List<PublicParticipantResultModel>>> GetVisible() => Ok(await _participantsService.GetVisibleAsync());

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<ParticipantResultModel>>> GetAll() => Ok(await _participantsService.GetAllAsync());

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ParticipantResultModel>> Create([FromBody] SaveParticipantModel model) =>
            Ok(await _participantsService.CreateAsync(model));

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<ParticipantResultModel>> Update(Guid id, [FromBody] SaveParticipantModel model)
        {
            var updated = await _participantsService.UpdateAsync(id, model);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id) =>
            await _participantsService.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
