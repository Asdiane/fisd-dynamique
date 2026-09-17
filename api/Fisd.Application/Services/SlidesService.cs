using Fisd.Application.Models.receive.Slide;
using Fisd.Application.Models.result.Slide;
using Fisd.Application.Services.Interfaces;
using Fisd.Persistence;
using Fisd.Persistence.Entities.Content;
using Fisd.Persistence.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fisd.Application.Services
{
    public class SlidesService : ISlidesService
    {
        private readonly FisdDbContext _dbContext;
        private readonly ICurrentLanguageAccessor _currentLanguageAccessor;

        public SlidesService(FisdDbContext dbContext, ICurrentLanguageAccessor currentLanguageAccessor)
        {
            _dbContext = dbContext;
            _currentLanguageAccessor = currentLanguageAccessor;
        }

        public async Task<List<PublicSlideResultModel>> GetVisibleAsync()
        {
            var entities = await _dbContext.Slides
                .Where(s => s.IsVisible && s.Status == ContentStatusEnum.Published)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();
            var isEn = _currentLanguageAccessor.GetLanguage() == "en";
            return entities.Select(e => ToPublicResultModel(e, isEn)).ToList();
        }

        public async Task<List<SlideResultModel>> GetAllAsync()
        {
            var entities = await _dbContext.Slides.OrderBy(s => s.DisplayOrder).ToListAsync();
            return entities.Select(ToResultModel).ToList();
        }

        public async Task<SlideResultModel> CreateAsync(SaveSlideModel model)
        {
            var entity = new SlideEntity
            {
                Id = Guid.NewGuid(),
                ImageUrl = model.ImageUrl,
                VideoUrl = model.VideoUrl,
                FocalPoint = model.FocalPoint,
                KickerFr = model.KickerFr,
                KickerEn = model.KickerEn,
                TitleFr = model.TitleFr,
                TitleEn = model.TitleEn,
                TextFr = model.TextFr,
                TextEn = model.TextEn,
                PlaceFr = model.PlaceFr,
                PlaceEn = model.PlaceEn,
                IsVisible = model.IsVisible,
                Status = ParseStatus(model.Status),
                DisplayOrder = model.DisplayOrder
            };

            _dbContext.Slides.Add(entity);
            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<SlideResultModel?> UpdateAsync(Guid id, SaveSlideModel model)
        {
            var entity = await _dbContext.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (entity == null)
            {
                return null;
            }

            entity.ImageUrl = model.ImageUrl;
            entity.VideoUrl = model.VideoUrl;
            entity.FocalPoint = model.FocalPoint;
            entity.KickerFr = model.KickerFr;
            entity.KickerEn = model.KickerEn;
            entity.TitleFr = model.TitleFr;
            entity.TitleEn = model.TitleEn;
            entity.TextFr = model.TextFr;
            entity.TextEn = model.TextEn;
            entity.PlaceFr = model.PlaceFr;
            entity.PlaceEn = model.PlaceEn;
            entity.IsVisible = model.IsVisible;
            entity.Status = ParseStatus(model.Status);
            entity.DisplayOrder = model.DisplayOrder;

            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _dbContext.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (entity == null)
            {
                return false;
            }

            _dbContext.Slides.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private static ContentStatusEnum ParseStatus(string status) =>
            status == "published" ? ContentStatusEnum.Published : ContentStatusEnum.Draft;

        private static SlideResultModel ToResultModel(SlideEntity entity) => new()
        {
            Id = entity.Id,
            ImageUrl = entity.ImageUrl,
            VideoUrl = entity.VideoUrl,
            FocalPoint = entity.FocalPoint,
            KickerFr = entity.KickerFr,
            KickerEn = entity.KickerEn,
            TitleFr = entity.TitleFr,
            TitleEn = entity.TitleEn,
            TextFr = entity.TextFr,
            TextEn = entity.TextEn,
            PlaceFr = entity.PlaceFr,
            PlaceEn = entity.PlaceEn,
            IsVisible = entity.IsVisible,
            Status = entity.Status == ContentStatusEnum.Published ? "published" : "draft",
            DisplayOrder = entity.DisplayOrder
        };

        private static PublicSlideResultModel ToPublicResultModel(SlideEntity entity, bool isEn) => new()
        {
            Id = entity.Id,
            ImageUrl = entity.ImageUrl,
            VideoUrl = entity.VideoUrl,
            FocalPoint = entity.FocalPoint,
            Kicker = isEn ? entity.KickerEn : entity.KickerFr,
            Title = isEn ? entity.TitleEn : entity.TitleFr,
            Text = isEn ? entity.TextEn : entity.TextFr,
            Place = isEn ? entity.PlaceEn : entity.PlaceFr,
            DisplayOrder = entity.DisplayOrder
        };
    }
}
