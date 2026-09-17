using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Fisd.Application.Models.receive.AdminUser;
using Fisd.Application.Models.result.AdminUser;
using Fisd.Application.Services.Interfaces;
using Fisd.Persistence.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fisd.Api.Controllers
{
    [ApiController]
    [Route("api/admin-users")]
    [Authorize(Roles = "SuperAdmin,PlatformAdmin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly IAdminUsersService _adminUsersService;

        public AdminUsersController(IAdminUsersService adminUsersService)
        {
            _adminUsersService = adminUsersService;
        }

        private Guid ActingAdminId => Guid.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        private AdminRoleEnum ActingAdminRole => Enum.Parse<AdminRoleEnum>(User.FindFirst(ClaimTypes.Role)!.Value);

        [HttpGet]
        public async Task<ActionResult<List<AdminUserResultModel>>> GetAll() => Ok(await _adminUsersService.GetAllAsync());

        [HttpPost("invite")]
        public async Task<IActionResult> Invite([FromBody] InviteAdminUserModel model)
        {
            try
            {
                await _adminUsersService.InviteAsync(model, ActingAdminId, ActingAdminRole);
                return NoContent();
            }
            catch (AdminUserOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("invitations")]
        public async Task<ActionResult<List<PendingAdminInvitationResultModel>>> GetPendingInvitations() =>
            Ok(await _adminUsersService.GetPendingInvitationsAsync());

        [HttpDelete("invitations/{id:guid}")]
        public async Task<IActionResult> CancelInvitation(Guid id) =>
            await _adminUsersService.CancelInvitationAsync(id) ? NoContent() : NotFound();

        [HttpPut("{id:guid}/role")]
        public async Task<ActionResult<AdminUserResultModel>> UpdateRole(Guid id, [FromBody] UpdateAdminRoleModel model)
        {
            try
            {
                var updated = await _adminUsersService.UpdateRoleAsync(id, model.Role, ActingAdminId, ActingAdminRole);
                return updated == null ? NotFound() : Ok(updated);
            }
            catch (AdminUserOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                return await _adminUsersService.DeleteAsync(id, ActingAdminId, ActingAdminRole) ? NoContent() : NotFound();
            }
            catch (AdminUserOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id:guid}/status")]
        [Authorize(Roles = "PlatformAdmin")]
        public async Task<ActionResult<AdminUserResultModel>> SetActive(Guid id, [FromBody] SetAdminActiveModel model)
        {
            try
            {
                var updated = await _adminUsersService.SetActiveAsync(id, model.IsActive, ActingAdminId, ActingAdminRole);
                return updated == null ? NotFound() : Ok(updated);
            }
            catch (AdminUserOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{id:guid}/reset-2fa")]
        [Authorize(Roles = "PlatformAdmin")]
        public async Task<ActionResult<AdminUserResultModel>> ResetTwoFactor(Guid id)
        {
            try
            {
                var updated = await _adminUsersService.ResetTwoFactorAsync(id, ActingAdminRole);
                return updated == null ? NotFound() : Ok(updated);
            }
            catch (AdminUserOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
