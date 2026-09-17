using Fisd.Application.Models.receive.Speaker;
using Fisd.Application.Models.result.Speaker;
using Fisd.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fisd.Api.Controllers
{
    [ApiController]
    [Route("api/speakers")]
    public class SpeakersController : ControllerBase
    {
        private readonly ISpeakersService _speakersService;

        public SpeakersController(ISpeakersService speakersService)
        {
            _speakersService = speakersService;
        }

        [HttpGet("visible")]
        [AllowAnonymous]
        public async Task<ActionResult<List<PublicSpeakerResultModel>>> GetVisible() => Ok(await _speakersService.GetVisibleAsync());

        [HttpGet("visible/edition/{editionId:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<PublicSpeakerResultModel>>> GetVisibleByEdition(Guid editionId) =>
            Ok(await _speakersService.GetVisibleByEditionAsync(editionId));

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<SpeakerResultModel>>> GetAll() => Ok(await _speakersService.GetAllAsync());

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<SpeakerResultModel>> Create([FromBody] SaveSpeakerModel model) =>
            Ok(await _speakersService.CreateAsync(model));

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<SpeakerResultModel>> Update(Guid id, [FromBody] SaveSpeakerModel model)
        {
            var updated = await _speakersService.UpdateAsync(id, model);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id) =>
            await _speakersService.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
