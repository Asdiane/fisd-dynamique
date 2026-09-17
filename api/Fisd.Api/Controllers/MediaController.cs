using Fisd.Application.Models.result.Media;
using Fisd.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fisd.Api.Controllers
{
    [ApiController]
    [Route("api/media")]
    [Authorize]
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _mediaService;

        public MediaController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        [HttpGet]
        public async Task<ActionResult<List<MediaFileResultModel>>> GetAll() => Ok(await _mediaService.GetAllAsync());

        [HttpPost("upload/{category}")]
        [RequestSizeLimit(6 * 1024 * 1024)]
        public async Task<ActionResult<MediaFileResultModel>> Upload(string category, IFormFile file, [FromForm] string? altText)
        {
            try
            {
                await using var stream = file.OpenReadStream();
                var result = await _mediaService.UploadAsync(category, stream, file.ContentType, file.Length, file.FileName, altText);
                return Ok(result);
            }
            catch (InvalidMediaException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id) =>
            await _mediaService.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
