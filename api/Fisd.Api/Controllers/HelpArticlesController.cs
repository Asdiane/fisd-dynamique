using System.Security.Claims;
using Fisd.Application.Models.receive.Diagnostics;
using Fisd.Application.Models.result.Diagnostics;
using Fisd.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fisd.Api.Controllers
{
    [ApiController]
    [Route("api/help-articles")]
    [Authorize]
    public class HelpArticlesController : ControllerBase
    {
        private readonly IHelpArticlesService _helpArticlesService;

        public HelpArticlesController(IHelpArticlesService helpArticlesService)
        {
            _helpArticlesService = helpArticlesService;
        }

        private bool ActingIsPlatformAdmin => User.FindFirst(ClaimTypes.Role)?.Value == "PlatformAdmin";

        // Any authenticated admin (Editor and up) can read - this is backoffice documentation,
        // never exposed on the public site (no [AllowAnonymous] anywhere on this controller).
        // Articles flagged IsPlatformAdminOnly are filtered out below PlatformAdmin - other
        // roles are never told that role even exists.
        [HttpGet("visible")]
        public async Task<ActionResult<List<PublicHelpArticleResultModel>>> GetVisible() => Ok(await _helpArticlesService.GetVisibleAsync(ActingIsPlatformAdmin));

        [HttpGet]
        [Authorize(Roles = "PlatformAdmin")]
        public async Task<ActionResult<List<HelpArticleResultModel>>> GetAll() => Ok(await _helpArticlesService.GetAllAsync());

        [HttpPost]
        [Authorize(Roles = "PlatformAdmin")]
        public async Task<ActionResult<HelpArticleResultModel>> Create([FromBody] SaveHelpArticleModel model) =>
            Ok(await _helpArticlesService.CreateAsync(model));

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "PlatformAdmin")]
        public async Task<ActionResult<HelpArticleResultModel>> Update(Guid id, [FromBody] SaveHelpArticleModel model)
        {
            var updated = await _helpArticlesService.UpdateAsync(id, model);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "PlatformAdmin")]
        public async Task<IActionResult> Delete(Guid id) =>
            await _helpArticlesService.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
