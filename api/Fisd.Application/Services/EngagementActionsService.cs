using Fisd.Application.Models.receive.EngagementAction;
using Fisd.Application.Models.result.EngagementAction;
using Fisd.Application.Services.Interfaces;
using Fisd.Persistence;
using Fisd.Persistence.Entities.Content;
using Fisd.Persistence.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fisd.Application.Services
{
    public class EngagementActionsService : IEngagementActionsService
    {
        private readonly FisdDbContext _dbContext;
        private readonly ICurrentLanguageAccessor _currentLanguageAccessor;

        public EngagementActionsService(FisdDbContext dbContext, ICurrentLanguageAccessor currentLanguageAccessor)
        {
            _dbContext = dbContext;
            _currentLanguageAccessor = currentLanguageAccessor;
        }

        public async Task<List<PublicEngagementActionResultModel>> GetVisibleAsync()
        {
            var entities = await _dbContext.EngagementActions
                .Where(e => e.IsVisible && e.Status == ContentStatusEnum.Published)
                .OrderBy(e => e.DisplayOrder)
                .ToListAsync();
            var isEn = _currentLanguageAccessor.GetLanguage() == "en";
            return entities.Select(e => ToPublicResultModel(e, isEn)).ToList();
        }

        public async Task<List<EngagementActionResultModel>> GetAllAsync()
        {
            var entities = await _dbContext.EngagementActions.OrderBy(e => e.DisplayOrder).ToListAsync();
            return entities.Select(ToResultModel).ToList();
        }

        public async Task<EngagementActionResultModel> CreateAsync(SaveEngagementActionModel model)
        {
            var entity = new EngagementActionEntity
            {
                Id = Guid.NewGuid(),
                Anchor = model.Anchor,
                TitleFr = model.TitleFr,
                TitleEn = model.TitleEn,
                TextFr = model.TextFr,
                TextEn = model.TextEn,
                Icon = model.Icon,
                ImageUrl = model.ImageUrl,
                Link = model.Link,
                CtaFr = model.CtaFr,
                CtaEn = model.CtaEn,
                DetailFr = model.DetailFr,
                DetailEn = model.DetailEn,
                IsVisible = model.IsVisible,
                Status = ParseStatus(model.Status),
                DisplayOrder = model.DisplayOrder
            };

            _dbContext.EngagementActions.Add(entity);
            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<EngagementActionResultModel?> UpdateAsync(Guid id, SaveEngagementActionModel model)
        {
            var entity = await _dbContext.EngagementActions.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null)
            {
                return null;
            }

            entity.Anchor = model.Anchor;
            entity.TitleFr = model.TitleFr;
            entity.TitleEn = model.TitleEn;
            entity.TextFr = model.TextFr;
            entity.TextEn = model.TextEn;
            entity.Icon = model.Icon;
            entity.ImageUrl = model.ImageUrl;
            entity.Link = model.Link;
            entity.CtaFr = model.CtaFr;
            entity.CtaEn = model.CtaEn;
            entity.DetailFr = model.DetailFr;
            entity.DetailEn = model.DetailEn;
            entity.IsVisible = model.IsVisible;
            entity.Status = ParseStatus(model.Status);
            entity.DisplayOrder = model.DisplayOrder;

            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _dbContext.EngagementActions.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null)
            {
                return false;
            }

            _dbContext.EngagementActions.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private static ContentStatusEnum ParseStatus(string status) =>
            status == "published" ? ContentStatusEnum.Published : ContentStatusEnum.Draft;

        private static EngagementActionResultModel ToResultModel(EngagementActionEntity entity) => new()
        {
            Id = entity.Id,
            Anchor = entity.Anchor,
            TitleFr = entity.TitleFr,
            TitleEn = entity.TitleEn,
            TextFr = entity.TextFr,
            TextEn = entity.TextEn,
            Icon = entity.Icon,
            ImageUrl = entity.ImageUrl,
            Link = entity.Link,
            CtaFr = entity.CtaFr,
            CtaEn = entity.CtaEn,
            DetailFr = entity.DetailFr,
            DetailEn = entity.DetailEn,
            IsVisible = entity.IsVisible,
            Status = entity.Status == ContentStatusEnum.Published ? "published" : "draft",
            DisplayOrder = entity.DisplayOrder
        };

        private static PublicEngagementActionResultModel ToPublicResultModel(EngagementActionEntity entity, bool isEn) => new()
        {
            Id = entity.Id,
            Anchor = entity.Anchor,
            Title = isEn ? entity.TitleEn : entity.TitleFr,
            Text = isEn ? entity.TextEn : entity.TextFr,
            Icon = entity.Icon,
            ImageUrl = entity.ImageUrl,
            Link = entity.Link,
            Cta = isEn ? entity.CtaEn : entity.CtaFr,
            Detail = isEn ? entity.DetailEn : entity.DetailFr,
            DisplayOrder = entity.DisplayOrder
        };
    }
}
