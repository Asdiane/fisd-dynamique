using Fisd.Application.Models.receive.Partner;
using Fisd.Application.Models.result.Partner;
using Fisd.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fisd.Api.Controllers
{
    [ApiController]
    [Route("api/partners")]
    public class PartnersController : ControllerBase
    {
        private readonly IPartnersService _partnersService;

        public PartnersController(IPartnersService partnersService)
        {
            _partnersService = partnersService;
        }

        [HttpGet("visible")]
        [AllowAnonymous]
        public async Task<ActionResult<List<PartnerResultModel>>> GetVisible() => Ok(await _partnersService.GetVisibleAsync());

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<PartnerResultModel>>> GetAll() => Ok(await _partnersService.GetAllAsync());

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PartnerResultModel>> Create([FromBody] SavePartnerModel model) =>
            Ok(await _partnersService.CreateAsync(model));

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<PartnerResultModel>> Update(Guid id, [FromBody] SavePartnerModel model)
        {
            var updated = await _partnersService.UpdateAsync(id, model);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id) =>
            await _partnersService.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
