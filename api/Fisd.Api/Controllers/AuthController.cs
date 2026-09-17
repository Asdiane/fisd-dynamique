using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Fisd.Application.Models.receive.Auth;
using Fisd.Application.Models.result.Auth;
using Fisd.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fisd.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResultModel>> Login([FromBody] LoginModel model)
        {
            // Always 200 - success/failure lives in the result envelope, not the status code, so
            // the frontend can branch on `success` without an HttpErrorResponse hiding the real
            // reason behind a generic network-error path.
            var result = await _authService.LoginAsync(model.Email, model.Password);
            return Ok(result);
        }

        [HttpPost("2fa/enable")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResultModel>> EnableTwoFactor([FromBody] TwoFactorEnableModel model)
        {
            var result = await _authService.EnableTwoFactorAsync(model.Email, model.Password, model.TotpCode);
            return Ok(result);
        }

        [HttpPost("2fa/verify")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResultModel>> VerifyTwoFactor([FromBody] TwoFactorVerifyModel model)
        {
            var result = await _authService.VerifyTwoFactorAsync(model.Email, model.Password, model.TotpCode);
            return Ok(result);
        }

        [HttpGet("me")]
        [Authorize]
        public ActionResult<object> Me()
        {
            var id = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(JwtRegisteredClaimNames.Email)?.Value ?? User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            return Ok(new { id, email, role });
        }

        [HttpPost("passkey/register/options")]
        [Authorize]
        public async Task<ActionResult<PasskeyOptionsResultModel>> GetPasskeyRegistrationOptions()
        {
            var result = await _authService.GetPasskeyRegistrationOptionsAsync(CurrentAdminUserId());
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost("passkey/register/verify")]
        [Authorize]
        public async Task<ActionResult<PasskeyActionResultModel>> VerifyPasskeyRegistration([FromBody] PasskeyRegisterVerifyModel model)
        {
            var result = await _authService.VerifyPasskeyRegistrationAsync(CurrentAdminUserId(), model);
            return Ok(result);
        }

        [HttpGet("passkey")]
        [Authorize]
        public async Task<ActionResult<List<PasskeyResultModel>>> ListPasskeys()
        {
            var result = await _authService.ListPasskeysAsync(CurrentAdminUserId());
            return Ok(result);
        }

        [HttpDelete("passkey/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeletePasskey(Guid id) =>
            await _authService.DeletePasskeyAsync(CurrentAdminUserId(), id) ? NoContent() : NotFound();

        [HttpPost("passkey/login/options")]
        [AllowAnonymous]
        public ActionResult<PasskeyOptionsResultModel> GetPasskeyLoginOptions() => Ok(_authService.GetPasskeyLoginOptions());

        [HttpPost("passkey/login/verify")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResultModel>> VerifyPasskeyLogin([FromBody] PasskeyLoginVerifyModel model)
        {
            var result = await _authService.VerifyPasskeyLoginAsync(model);
            return Ok(result);
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            await _authService.RequestPasswordResetAsync(model.Email);
            return Ok(new { success = true });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var error = await _authService.ResetPasswordAsync(model.Token, model.NewPassword);
            return Ok(new { success = error == null, error });
        }

        [HttpGet("invitations/{token}")]
        [AllowAnonymous]
        public async Task<ActionResult<AdminInvitationInfoResultModel>> GetInvitation(string token)
        {
            var info = await _authService.GetInvitationInfoAsync(token);
            return info == null ? NotFound() : Ok(info);
        }

        [HttpPost("invitations/accept")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResultModel>> AcceptInvitation([FromBody] AcceptInvitationModel model) =>
            Ok(await _authService.AcceptInvitationAsync(model.Token, model.Password));

        // ASP.NET Core's default JWT handler remaps the "sub" claim to ClaimTypes.NameIdentifier,
        // so Sub alone comes back null here - same fallback used everywhere else in the app.
        private Guid CurrentAdminUserId() => Guid.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}
