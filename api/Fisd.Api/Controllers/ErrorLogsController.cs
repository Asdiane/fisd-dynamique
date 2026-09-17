using Fisd.Application.Models.receive.Diagnostics;
using Fisd.Application.Models.result.Diagnostics;
using Fisd.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fisd.Api.Controllers
{
    [ApiController]
    [Route("api/error-logs")]
    [Authorize(Roles = "PlatformAdmin")]
    public class ErrorLogsController : ControllerBase
    {
        private readonly IErrorLogService _errorLogService;

        public ErrorLogsController(IErrorLogService errorLogService)
        {
            _errorLogService = errorLogService;
        }

        [HttpGet]
        public async Task<ActionResult<ErrorLogPageResultModel>> GetAll([FromQuery] ErrorLogQueryModel query) =>
            Ok(await _errorLogService.GetErrorLogsAsync(query));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ErrorLogDetailResultModel>> GetDetail(int id)
        {
            var detail = await _errorLogService.GetErrorLogDetailAsync(id);
            return detail == null ? NotFound() : Ok(detail);
        }
    }
}
