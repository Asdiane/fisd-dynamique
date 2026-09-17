using Fisd.Application.Models.receive.Article;
using Fisd.Application.Models.result.Article;
using Fisd.Application.Services.Interfaces;
using Fisd.Persistence;
using Fisd.Persistence.Entities.Content;
using Fisd.Persistence.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fisd.Application.Services
{
    public class ArticlesService : IArticlesService
    {
        private const string PublishedStatus = "published";
        private const string DraftStatus = "draft";

        private readonly FisdDbContext _dbContext;

        public ArticlesService(FisdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ArticleResultModel>> GetPublishedAsync()
        {
            var entities = await _dbContext.Articles
                .Where(a => a.Status == ArticleStatusEnum.Published)
                .OrderByDescending(a => a.PublishedAt)
                .ToListAsync();
            return entities.Select(ToResultModel).ToList();
        }

        public async Task<ArticleResultModel?> GetByIdAsync(Guid id)
        {
            var entity = await _dbContext.Articles.FirstOrDefaultAsync(a => a.Id == id);
            return entity == null ? null : ToResultModel(entity);
        }

        public async Task<List<ArticleResultModel>> GetAllAsync()
        {
            var entities = await _dbContext.Articles.OrderByDescending(a => a.CreatedAt).ToListAsync();
            return entities.Select(ToResultModel).ToList();
        }

        public async Task<ArticleResultModel> CreateAsync(SaveArticleModel model)
        {
            var now = DateTimeOffset.UtcNow;
            var status = ParseStatus(model.Status);
            var entity = new ArticleEntity
            {
                Id = Guid.NewGuid(),
                Title = model.Title,
                Excerpt = model.Excerpt,
                Content = model.Content,
                ImageUrl = model.ImageUrl,
                Status = status,
                PublishedAt = status == ArticleStatusEnum.Published ? now : null,
                CreatedAt = now,
                UpdatedAt = now
            };

            _dbContext.Articles.Add(entity);
            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        // publishedAt is only ever set the first time an article is published, and is never
        // cleared afterward (even if it's saved back to draft) - so a republish keeps its
        // original publish date.
        public async Task<ArticleResultModel?> UpdateAsync(Guid id, SaveArticleModel model)
        {
            var entity = await _dbContext.Articles.FirstOrDefaultAsync(a => a.Id == id);
            if (entity == null)
            {
                return null;
            }

            var status = ParseStatus(model.Status);
            entity.Title = model.Title;
            entity.Excerpt = model.Excerpt;
            entity.Content = model.Content;
            entity.ImageUrl = model.ImageUrl;
            entity.Status = status;
            entity.PublishedAt = status == ArticleStatusEnum.Published ? (model.PublishedAt ?? DateTimeOffset.UtcNow) : model.PublishedAt;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _dbContext.Articles.FirstOrDefaultAsync(a => a.Id == id);
            if (entity == null)
            {
                return false;
            }

            _dbContext.Articles.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private static ArticleStatusEnum ParseStatus(string status) =>
            status == PublishedStatus ? ArticleStatusEnum.Published : ArticleStatusEnum.Draft;

        private static ArticleResultModel ToResultModel(ArticleEntity entity) => new()
        {
            Id = entity.Id,
            Title = entity.Title,
            Slug = entity.Slug,
            Excerpt = entity.Excerpt,
            Content = entity.Content,
            ImageUrl = entity.ImageUrl,
            Status = entity.Status == ArticleStatusEnum.Published ? PublishedStatus : DraftStatus,
            PublishedAt = entity.PublishedAt,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
