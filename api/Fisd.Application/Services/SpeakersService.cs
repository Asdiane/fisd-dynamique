using Fisd.Application.Models.receive.Speaker;
using Fisd.Application.Models.result.Speaker;
using Fisd.Application.Services.Interfaces;
using Fisd.Persistence;
using Fisd.Persistence.Entities.Content;
using Fisd.Persistence.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fisd.Application.Services
{
    public class SpeakersService : ISpeakersService
    {
        private readonly FisdDbContext _dbContext;
        private readonly ICurrentLanguageAccessor _currentLanguageAccessor;

        public SpeakersService(FisdDbContext dbContext, ICurrentLanguageAccessor currentLanguageAccessor)
        {
            _dbContext = dbContext;
            _currentLanguageAccessor = currentLanguageAccessor;
        }

        public async Task<List<PublicSpeakerResultModel>> GetVisibleAsync()
        {
            var entities = await _dbContext.Speakers
                .Where(s => s.IsVisible && s.Status == ContentStatusEnum.Published)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();
            var isEn = _currentLanguageAccessor.GetLanguage() == "en";
            return entities.Select(e => ToPublicResultModel(e, isEn)).ToList();
        }

        public async Task<List<PublicSpeakerResultModel>> GetVisibleByEditionAsync(Guid editionId)
        {
            var entities = await _dbContext.Speakers
                .Where(s => s.IsVisible && s.Status == ContentStatusEnum.Published && s.EditionId == editionId)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();
            var isEn = _currentLanguageAccessor.GetLanguage() == "en";
            return entities.Select(e => ToPublicResultModel(e, isEn)).ToList();
        }

        public async Task<List<SpeakerResultModel>> GetAllAsync()
        {
            var entities = await _dbContext.Speakers.OrderBy(s => s.DisplayOrder).ToListAsync();
            return entities.Select(ToResultModel).ToList();
        }

        public async Task<SpeakerResultModel> CreateAsync(SaveSpeakerModel model)
        {
            var entity = new SpeakerEntity
            {
                Id = Guid.NewGuid(),
                EditionId = model.EditionId,
                Name = model.Name,
                RoleFr = model.RoleFr,
                RoleEn = model.RoleEn,
                ImageUrl = model.ImageUrl,
                DescriptionFr = model.DescriptionFr,
                DescriptionEn = model.DescriptionEn,
                IsVisible = model.IsVisible,
                Status = ParseStatus(model.Status),
                DisplayOrder = model.DisplayOrder
            };

            _dbContext.Speakers.Add(entity);
            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<SpeakerResultModel?> UpdateAsync(Guid id, SaveSpeakerModel model)
        {
            var entity = await _dbContext.Speakers.FirstOrDefaultAsync(s => s.Id == id);
            if (entity == null)
            {
                return null;
            }

            entity.EditionId = model.EditionId;
            entity.Name = model.Name;
            entity.RoleFr = model.RoleFr;
            entity.RoleEn = model.RoleEn;
            entity.ImageUrl = model.ImageUrl;
            entity.DescriptionFr = model.DescriptionFr;
            entity.DescriptionEn = model.DescriptionEn;
            entity.IsVisible = model.IsVisible;
            entity.Status = ParseStatus(model.Status);
            entity.DisplayOrder = model.DisplayOrder;

            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _dbContext.Speakers.FirstOrDefaultAsync(s => s.Id == id);
            if (entity == null)
            {
                return false;
            }

            _dbContext.Speakers.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private static ContentStatusEnum ParseStatus(string status) =>
            status == "published" ? ContentStatusEnum.Published : ContentStatusEnum.Draft;

        private static SpeakerResultModel ToResultModel(SpeakerEntity entity) => new()
        {
            Id = entity.Id,
            EditionId = entity.EditionId,
            Name = entity.Name,
            RoleFr = entity.RoleFr,
            RoleEn = entity.RoleEn,
            ImageUrl = entity.ImageUrl,
            DescriptionFr = entity.DescriptionFr,
            DescriptionEn = entity.DescriptionEn,
            IsVisible = entity.IsVisible,
            Status = entity.Status == ContentStatusEnum.Published ? "published" : "draft",
            DisplayOrder = entity.DisplayOrder
        };

        private static PublicSpeakerResultModel ToPublicResultModel(SpeakerEntity entity, bool isEn) => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Role = isEn ? entity.RoleEn : entity.RoleFr,
            ImageUrl = entity.ImageUrl,
            Description = isEn ? entity.DescriptionEn : entity.DescriptionFr,
            DisplayOrder = entity.DisplayOrder
        };
    }
}
