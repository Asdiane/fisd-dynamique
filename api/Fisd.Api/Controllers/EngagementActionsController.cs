using Fisd.Application.Models.receive.EngagementAction;
using Fisd.Application.Models.result.EngagementAction;
using Fisd.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fisd.Api.Controllers
{
    [ApiController]
    [Route("api/engagement-actions")]
    public class EngagementActionsController : ControllerBase
    {
        private readonly IEngagementActionsService _engagementActionsService;

        public EngagementActionsController(IEngagementActionsService engagementActionsService)
        {
            _engagementActionsService = engagementActionsService;
        }

        [HttpGet("visible")]
        [AllowAnonymous]
        public async Task<ActionResult<List<PublicEngagementActionResultModel>>> GetVisible() => Ok(await _engagementActionsService.GetVisibleAsync());

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<EngagementActionResultModel>>> GetAll() => Ok(await _engagementActionsService.GetAllAsync());

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<EngagementActionResultModel>> Create([FromBody] SaveEngagementActionModel model) =>
            Ok(await _engagementActionsService.CreateAsync(model));

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<EngagementActionResultModel>> Update(Guid id, [FromBody] SaveEngagementActionModel model)
        {
            var updated = await _engagementActionsService.UpdateAsync(id, model);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id) =>
            await _engagementActionsService.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
