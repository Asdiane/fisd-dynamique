using Fisd.Application.Models.receive.Testimonial;
using Fisd.Application.Models.result.Testimonial;
using Fisd.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fisd.Api.Controllers
{
    [ApiController]
    [Route("api/testimonials")]
    public class TestimonialsController : ControllerBase
    {
        private readonly ITestimonialsService _testimonialsService;

        public TestimonialsController(ITestimonialsService testimonialsService)
        {
            _testimonialsService = testimonialsService;
        }

        [HttpGet("visible")]
        [AllowAnonymous]
        public async Task<ActionResult<List<PublicTestimonialResultModel>>> GetVisible() => Ok(await _testimonialsService.GetVisibleAsync());

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<TestimonialResultModel>>> GetAll() => Ok(await _testimonialsService.GetAllAsync());

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<TestimonialResultModel>> Create([FromBody] SaveTestimonialModel model) =>
            Ok(await _testimonialsService.CreateAsync(model));

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<TestimonialResultModel>> Update(Guid id, [FromBody] SaveTestimonialModel model)
        {
            var updated = await _testimonialsService.UpdateAsync(id, model);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id) =>
            await _testimonialsService.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
