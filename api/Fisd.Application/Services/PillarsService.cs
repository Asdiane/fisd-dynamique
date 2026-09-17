using Fisd.Application.Models.receive.Pillar;
using Fisd.Application.Models.result.Pillar;
using Fisd.Application.Services.Interfaces;
using Fisd.Persistence;
using Fisd.Persistence.Entities.Content;
using Fisd.Persistence.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fisd.Application.Services
{
    public class PillarsService : IPillarsService
    {
        private readonly FisdDbContext _dbContext;
        private readonly ICurrentLanguageAccessor _currentLanguageAccessor;

        public PillarsService(FisdDbContext dbContext, ICurrentLanguageAccessor currentLanguageAccessor)
        {
            _dbContext = dbContext;
            _currentLanguageAccessor = currentLanguageAccessor;
        }

        public async Task<List<PublicPillarResultModel>> GetVisibleAsync()
        {
            var entities = await _dbContext.Pillars
                .Where(p => p.IsVisible && p.Status == ContentStatusEnum.Published)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();
            var isEn = _currentLanguageAccessor.GetLanguage() == "en";
            return entities.Select(e => ToPublicResultModel(e, isEn)).ToList();
        }

        public async Task<List<PillarResultModel>> GetAllAsync()
        {
            var entities = await _dbContext.Pillars.OrderBy(p => p.DisplayOrder).ToListAsync();
            return entities.Select(ToResultModel).ToList();
        }

        public async Task<PillarResultModel> CreateAsync(SavePillarModel model)
        {
            var entity = new PillarEntity
            {
                Id = Guid.NewGuid(),
                TitleFr = model.TitleFr,
                TitleEn = model.TitleEn,
                TextFr = model.TextFr,
                TextEn = model.TextEn,
                ImageUrl = model.ImageUrl,
                Icon = model.Icon,
                IsVisible = model.IsVisible,
                Status = ParseStatus(model.Status),
                DisplayOrder = model.DisplayOrder
            };

            _dbContext.Pillars.Add(entity);
            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<PillarResultModel?> UpdateAsync(Guid id, SavePillarModel model)
        {
            var entity = await _dbContext.Pillars.FirstOrDefaultAsync(p => p.Id == id);
            if (entity == null)
            {
                return null;
            }

            entity.TitleFr = model.TitleFr;
            entity.TitleEn = model.TitleEn;
            entity.TextFr = model.TextFr;
            entity.TextEn = model.TextEn;
            entity.ImageUrl = model.ImageUrl;
            entity.Icon = model.Icon;
            entity.IsVisible = model.IsVisible;
            entity.Status = ParseStatus(model.Status);
            entity.DisplayOrder = model.DisplayOrder;

            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _dbContext.Pillars.FirstOrDefaultAsync(p => p.Id == id);
            if (entity == null)
            {
                return false;
            }

            _dbContext.Pillars.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private static ContentStatusEnum ParseStatus(string status) =>
            status == "published" ? ContentStatusEnum.Published : ContentStatusEnum.Draft;

        private static PillarResultModel ToResultModel(PillarEntity entity) => new()
        {
            Id = entity.Id,
            TitleFr = entity.TitleFr,
            TitleEn = entity.TitleEn,
            TextFr = entity.TextFr,
            TextEn = entity.TextEn,
            ImageUrl = entity.ImageUrl,
            Icon = entity.Icon,
            IsVisible = entity.IsVisible,
            Status = entity.Status == ContentStatusEnum.Published ? "published" : "draft",
            DisplayOrder = entity.DisplayOrder
        };

        private static PublicPillarResultModel ToPublicResultModel(PillarEntity entity, bool isEn) => new()
        {
            Id = entity.Id,
            Title = isEn ? entity.TitleEn : entity.TitleFr,
            Text = isEn ? entity.TextEn : entity.TextFr,
            ImageUrl = entity.ImageUrl,
            Icon = entity.Icon,
            DisplayOrder = entity.DisplayOrder
        };
    }
}
