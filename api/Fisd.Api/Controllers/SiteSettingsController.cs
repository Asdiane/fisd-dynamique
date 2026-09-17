using Fisd.Application.Models.receive.SiteSettings;
using Fisd.Application.Models.result.SiteSettings;
using Fisd.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fisd.Api.Controllers
{
    [ApiController]
    [Route("api/site-settings")]
    public class SiteSettingsController : ControllerBase
    {
        private readonly ISiteSettingsService _siteSettingsService;

        public SiteSettingsController(ISiteSettingsService siteSettingsService)
        {
            _siteSettingsService = siteSettingsService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<SiteSettingsResultModel>> Get() => Ok(await _siteSettingsService.GetAsync());

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<SiteSettingsResultModel>> Update([FromBody] SaveSiteSettingsModel model) =>
            Ok(await _siteSettingsService.UpdateAsync(model));
    }
}
