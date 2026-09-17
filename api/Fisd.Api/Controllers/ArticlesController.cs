using Fisd.Application.Models.receive.Article;
using Fisd.Application.Models.result.Article;
using Fisd.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fisd.Api.Controllers
{
    [ApiController]
    [Route("api/articles")]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticlesService _articlesService;

        public ArticlesController(IArticlesService articlesService)
        {
            _articlesService = articlesService;
        }

        [HttpGet("published")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ArticleResultModel>>> GetPublished() => Ok(await _articlesService.GetPublishedAsync());

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<ArticleResultModel>> GetById(Guid id)
        {
            var article = await _articlesService.GetByIdAsync(id);
            return article == null ? NotFound() : Ok(article);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<ArticleResultModel>>> GetAll() => Ok(await _articlesService.GetAllAsync());

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ArticleResultModel>> Create([FromBody] SaveArticleModel model)
        {
            var created = await _articlesService.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<ArticleResultModel>> Update(Guid id, [FromBody] SaveArticleModel model)
        {
            var updated = await _articlesService.UpdateAsync(id, model);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id) =>
            await _articlesService.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
