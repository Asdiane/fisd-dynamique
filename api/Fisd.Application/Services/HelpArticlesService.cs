using Fisd.Application.Models.receive.Diagnostics;
using Fisd.Application.Models.result.Diagnostics;
using Fisd.Application.Services.Interfaces;
using Fisd.Persistence;
using Fisd.Persistence.Entities.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Fisd.Application.Services
{
    public class HelpArticlesService : IHelpArticlesService
    {
        private readonly FisdDbContext _dbContext;
        private readonly ICurrentLanguageAccessor _currentLanguageAccessor;

        public HelpArticlesService(FisdDbContext dbContext, ICurrentLanguageAccessor currentLanguageAccessor)
        {
            _dbContext = dbContext;
            _currentLanguageAccessor = currentLanguageAccessor;
        }

        public async Task<List<PublicHelpArticleResultModel>> GetVisibleAsync(bool isPlatformAdmin)
        {
            var query = _dbContext.HelpArticles.Where(a => a.IsVisible);
            if (!isPlatformAdmin)
            {
                query = query.Where(a => !a.IsPlatformAdminOnly);
            }

            var entities = await query.OrderBy(a => a.DisplayOrder).ToListAsync();
            var isEn = _currentLanguageAccessor.GetLanguage() == "en";
            return entities.Select(e => ToPublicResultModel(e, isEn)).ToList();
        }

        public async Task<List<HelpArticleResultModel>> GetAllAsync()
        {
            var entities = await _dbContext.HelpArticles.OrderBy(a => a.DisplayOrder).ToListAsync();
            return entities.Select(ToResultModel).ToList();
        }

        public async Task<HelpArticleResultModel> CreateAsync(SaveHelpArticleModel model)
        {
            var entity = new HelpArticleEntity
            {
                Id = Guid.NewGuid(),
                Category = model.Category,
                TitleFr = model.TitleFr,
                TitleEn = model.TitleEn,
                ContentFr = model.ContentFr,
                ContentEn = model.ContentEn,
                IsVisible = model.IsVisible,
                DisplayOrder = model.DisplayOrder,
                IsPlatformAdminOnly = model.IsPlatformAdminOnly
            };

            _dbContext.HelpArticles.Add(entity);
            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<HelpArticleResultModel?> UpdateAsync(Guid id, SaveHelpArticleModel model)
        {
            var entity = await _dbContext.HelpArticles.FirstOrDefaultAsync(a => a.Id == id);
            if (entity == null)
            {
                return null;
            }

            entity.Category = model.Category;
            entity.TitleFr = model.TitleFr;
            entity.TitleEn = model.TitleEn;
            entity.ContentFr = model.ContentFr;
            entity.ContentEn = model.ContentEn;
            entity.IsVisible = model.IsVisible;
            entity.DisplayOrder = model.DisplayOrder;
            entity.IsPlatformAdminOnly = model.IsPlatformAdminOnly;

            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _dbContext.HelpArticles.FirstOrDefaultAsync(a => a.Id == id);
            if (entity == null)
            {
                return false;
            }

            _dbContext.HelpArticles.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private static HelpArticleResultModel ToResultModel(HelpArticleEntity entity) => new()
        {
            Id = entity.Id,
            Category = entity.Category,
            TitleFr = entity.TitleFr,
            TitleEn = entity.TitleEn,
            ContentFr = entity.ContentFr,
            ContentEn = entity.ContentEn,
            IsVisible = entity.IsVisible,
            DisplayOrder = entity.DisplayOrder,
            IsPlatformAdminOnly = entity.IsPlatformAdminOnly
        };

        private static PublicHelpArticleResultModel ToPublicResultModel(HelpArticleEntity entity, bool isEn) => new()
        {
            Id = entity.Id,
            Category = entity.Category,
            Title = isEn ? entity.TitleEn : entity.TitleFr,
            Content = isEn ? entity.ContentEn : entity.ContentFr,
            DisplayOrder = entity.DisplayOrder
        };
    }
}
