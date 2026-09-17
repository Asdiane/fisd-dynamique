using Fisd.Application.Models.receive.Diagnostics;
using Fisd.Application.Models.result.Diagnostics;
using Fisd.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fisd.Api.Controllers
{
    [ApiController]
    [Route("api/audit-logs")]
    [Authorize(Roles = "PlatformAdmin")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogsController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<ActionResult<AuditLogPageResultModel>> GetAll([FromQuery] AuditLogQueryModel query) =>
            Ok(await _auditLogService.GetAuditLogsAsync(query));

        [HttpGet("{id:long}")]
        public async Task<ActionResult<AuditLogDetailResultModel>> GetDetail(long id)
        {
            var detail = await _auditLogService.GetAuditLogDetailAsync(id);
            return detail == null ? NotFound() : Ok(detail);
        }
    }
}
