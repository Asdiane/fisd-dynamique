using Fisd.Application.Models.receive.Diagnostics;
using Fisd.Application.Models.result.Diagnostics;
using Fisd.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fisd.Api.Controllers
{
    [ApiController]
    [Route("api/email-history")]
    [Authorize(Roles = "PlatformAdmin")]
    public class EmailHistoryController : ControllerBase
    {
        private readonly IEmailHistoryService _emailHistoryService;

        public EmailHistoryController(IEmailHistoryService emailHistoryService)
        {
            _emailHistoryService = emailHistoryService;
        }

        [HttpGet]
        public async Task<ActionResult<EmailHistoryPageResultModel>> GetAll([FromQuery] EmailHistoryQueryModel query) =>
            Ok(await _emailHistoryService.GetHistoryAsync(query));

        [HttpGet("{id:long}")]
        public async Task<ActionResult<EmailHistoryDetailResultModel>> GetDetail(long id)
        {
            var detail = await _emailHistoryService.GetDetailAsync(id);
            return detail == null ? NotFound() : Ok(detail);
        }
    }
}
