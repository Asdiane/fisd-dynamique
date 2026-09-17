using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Fisd.Application.Models.receive.Diagnostics;
using Fisd.Application.Models.result.Diagnostics;
using Fisd.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fisd.Api.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketsService _ticketsService;

        public TicketsController(ITicketsService ticketsService)
        {
            _ticketsService = ticketsService;
        }

        private Guid ActingAdminId => Guid.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        private string ActingAdminEmail => User.FindFirst(JwtRegisteredClaimNames.Email)?.Value ?? User.FindFirst(ClaimTypes.Email)!.Value;
        private bool ActingIsPlatformAdmin => User.FindFirst(ClaimTypes.Role)?.Value == "PlatformAdmin";

        // Any authenticated admin can submit a ticket and see their own submissions.
        [HttpPost]
        public async Task<ActionResult<TicketResultModel>> Create([FromBody] SaveTicketModel model) =>
            Ok(await _ticketsService.CreateAsync(ActingAdminId, ActingAdminEmail, model));

        [HttpGet("mine")]
        public async Task<ActionResult<List<TicketResultModel>>> GetMine() => Ok(await _ticketsService.GetMineAsync(ActingAdminId));

        // Only PlatformAdmin triages the full queue.
        [HttpGet]
        [Authorize(Roles = "PlatformAdmin")]
        public async Task<ActionResult<List<TicketResultModel>>> GetAll() => Ok(await _ticketsService.GetAllAsync());

        [HttpPut("{id:guid}/status")]
        [Authorize(Roles = "PlatformAdmin")]
        public async Task<ActionResult<TicketResultModel>> UpdateStatus(Guid id, [FromBody] UpdateTicketStatusModel model)
        {
            var updated = await _ticketsService.UpdateStatusAsync(id, model);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "PlatformAdmin")]
        public async Task<IActionResult> Delete(Guid id) =>
            await _ticketsService.DeleteAsync(id) ? NoContent() : NotFound();

        // Only the ticket's own author or PlatformAdmin can attach/remove a file - enforced in
        // the service (it needs the ticket's CreatedByAdminUserId to check).
        [HttpPost("{id:guid}/attachments")]
        [RequestSizeLimit(11 * 1024 * 1024)]
        public async Task<ActionResult<TicketAttachmentResultModel>> UploadAttachment(Guid id, IFormFile file)
        {
            try
            {
                await using var stream = file.OpenReadStream();
                var result = await _ticketsService.UploadAttachmentAsync(id, ActingAdminId, ActingIsPlatformAdmin, stream, file.ContentType, file.Length, file.FileName);
                return result == null ? NotFound() : Ok(result);
            }
            catch (InvalidMediaException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id:guid}/attachments/{attachmentId:guid}")]
        public async Task<IActionResult> DeleteAttachment(Guid id, Guid attachmentId) =>
            await _ticketsService.DeleteAttachmentAsync(id, attachmentId, ActingAdminId, ActingIsPlatformAdmin) ? NoContent() : NotFound();
    }
}
