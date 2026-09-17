using Fisd.Application.Models.receive.Slide;
using Fisd.Application.Models.result.Slide;
using Fisd.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fisd.Api.Controllers
{
    [ApiController]
    [Route("api/slides")]
    public class SlidesController : ControllerBase
    {
        private readonly ISlidesService _slidesService;

        public SlidesController(ISlidesService slidesService)
        {
            _slidesService = slidesService;
        }

        [HttpGet("visible")]
        [AllowAnonymous]
        public async Task<ActionResult<List<PublicSlideResultModel>>> GetVisible() => Ok(await _slidesService.GetVisibleAsync());

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<SlideResultModel>>> GetAll() => Ok(await _slidesService.GetAllAsync());

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<SlideResultModel>> Create([FromBody] SaveSlideModel model) =>
            Ok(await _slidesService.CreateAsync(model));

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<SlideResultModel>> Update(Guid id, [FromBody] SaveSlideModel model)
        {
            var updated = await _slidesService.UpdateAsync(id, model);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id) =>
            await _slidesService.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
