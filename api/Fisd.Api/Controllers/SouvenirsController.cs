using Fisd.Application.Models.receive.Souvenir;
using Fisd.Application.Models.result.Souvenir;
using Fisd.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fisd.Api.Controllers
{
    [ApiController]
    [Route("api/souvenirs")]
    public class SouvenirsController : ControllerBase
    {
        private readonly ISouvenirsService _souvenirsService;

        public SouvenirsController(ISouvenirsService souvenirsService)
        {
            _souvenirsService = souvenirsService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<SouvenirResultModel>>> GetAll() => Ok(await _souvenirsService.GetAllAsync());

        [HttpGet("{souvenirId:guid}/photos")]
        [AllowAnonymous]
        public async Task<ActionResult<List<SouvenirPhotoResultModel>>> GetPhotos(Guid souvenirId) =>
            Ok(await _souvenirsService.GetPhotosAsync(souvenirId));

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<SouvenirResultModel>> Create([FromBody] SaveSouvenirModel model) =>
            Ok(await _souvenirsService.CreateAsync(model));

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<SouvenirResultModel>> Update(Guid id, [FromBody] SaveSouvenirModel model)
        {
            var updated = await _souvenirsService.UpdateAsync(id, model);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id) =>
            await _souvenirsService.DeleteAsync(id) ? NoContent() : NotFound();

        [HttpPost("{souvenirId:guid}/photos")]
        [Authorize]
        public async Task<ActionResult<SouvenirPhotoResultModel>> AddPhoto(Guid souvenirId, [FromBody] SaveSouvenirPhotoModel model)
        {
            var photo = await _souvenirsService.AddPhotoAsync(souvenirId, model);
            return photo == null ? NotFound() : Ok(photo);
        }

        [HttpDelete("{souvenirId:guid}/photos/{photoId:guid}")]
        [Authorize]
        public async Task<IActionResult> DeletePhoto(Guid souvenirId, Guid photoId) =>
            await _souvenirsService.DeletePhotoAsync(souvenirId, photoId) ? NoContent() : NotFound();
    }
}
