using Fisd.Application.Models.result.Diagnostics;
using Fisd.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fisd.Api.Controllers
{
    [ApiController]
    [Route("api/system-health")]
    [Authorize(Roles = "PlatformAdmin")]
    public class SystemHealthController : ControllerBase
    {
        private readonly ISystemHealthService _systemHealthService;

        public SystemHealthController(ISystemHealthService systemHealthService)
        {
            _systemHealthService = systemHealthService;
        }

        [HttpGet]
        public async Task<ActionResult<SystemHealthResultModel>> Get() => Ok(await _systemHealthService.GetSystemHealthAsync());
    }
}
