using Fisd.Application.Models.receive.Pillar;
using Fisd.Application.Models.result.Pillar;
using Fisd.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fisd.Api.Controllers
{
    [ApiController]
    [Route("api/pillars")]
    public class PillarsController : ControllerBase
    {
        private readonly IPillarsService _pillarsService;

        public PillarsController(IPillarsService pillarsService)
        {
            _pillarsService = pillarsService;
        }

        [HttpGet("visible")]
        [AllowAnonymous]
        public async Task<ActionResult<List<PublicPillarResultModel>>> GetVisible() => Ok(await _pillarsService.GetVisibleAsync());

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<PillarResultModel>>> GetAll() => Ok(await _pillarsService.GetAllAsync());

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PillarResultModel>> Create([FromBody] SavePillarModel model) =>
            Ok(await _pillarsService.CreateAsync(model));

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<PillarResultModel>> Update(Guid id, [FromBody] SavePillarModel model)
        {
            var updated = await _pillarsService.UpdateAsync(id, model);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id) =>
            await _pillarsService.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
