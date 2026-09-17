using Fisd.Application.Models.receive.Participant;
using Fisd.Application.Models.result.Participant;
using Fisd.Application.Services.Interfaces;
using Fisd.Persistence;
using Fisd.Persistence.Entities.Content;
using Fisd.Persistence.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fisd.Application.Services
{
    public class ParticipantsService : IParticipantsService
    {
        private readonly FisdDbContext _dbContext;
        private readonly ICurrentLanguageAccessor _currentLanguageAccessor;

        public ParticipantsService(FisdDbContext dbContext, ICurrentLanguageAccessor currentLanguageAccessor)
        {
            _dbContext = dbContext;
            _currentLanguageAccessor = currentLanguageAccessor;
        }

        public async Task<List<PublicParticipantResultModel>> GetVisibleAsync()
        {
            var entities = await _dbContext.Participants
                .Where(p => p.IsVisible && p.Status == ContentStatusEnum.Published)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();
            var isEn = _currentLanguageAccessor.GetLanguage() == "en";
            return entities.Select(e => ToPublicResultModel(e, isEn)).ToList();
        }

        public async Task<List<ParticipantResultModel>> GetAllAsync()
        {
            var entities = await _dbContext.Participants.OrderBy(p => p.DisplayOrder).ToListAsync();
            return entities.Select(ToResultModel).ToList();
        }

        public async Task<ParticipantResultModel> CreateAsync(SaveParticipantModel model)
        {
            var entity = new ParticipantEntity
            {
                Id = Guid.NewGuid(),
                NameFr = model.NameFr,
                NameEn = model.NameEn,
                DescriptionFr = model.DescriptionFr,
                DescriptionEn = model.DescriptionEn,
                IsVisible = model.IsVisible,
                Status = ParseStatus(model.Status),
                DisplayOrder = model.DisplayOrder
            };

            _dbContext.Participants.Add(entity);
            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<ParticipantResultModel?> UpdateAsync(Guid id, SaveParticipantModel model)
        {
            var entity = await _dbContext.Participants.FirstOrDefaultAsync(p => p.Id == id);
            if (entity == null)
            {
                return null;
            }

            entity.NameFr = model.NameFr;
            entity.NameEn = model.NameEn;
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
            var entity = await _dbContext.Participants.FirstOrDefaultAsync(p => p.Id == id);
            if (entity == null)
            {
                return false;
            }

            _dbContext.Participants.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private static ContentStatusEnum ParseStatus(string status) =>
            status == "published" ? ContentStatusEnum.Published : ContentStatusEnum.Draft;

        private static ParticipantResultModel ToResultModel(ParticipantEntity entity) => new()
        {
            Id = entity.Id,
            NameFr = entity.NameFr,
            NameEn = entity.NameEn,
            DescriptionFr = entity.DescriptionFr,
            DescriptionEn = entity.DescriptionEn,
            IsVisible = entity.IsVisible,
            Status = entity.Status == ContentStatusEnum.Published ? "published" : "draft",
            DisplayOrder = entity.DisplayOrder
        };

        private static PublicParticipantResultModel ToPublicResultModel(ParticipantEntity entity, bool isEn) => new()
        {
            Id = entity.Id,
            Name = isEn ? entity.NameEn : entity.NameFr,
            Description = isEn ? entity.DescriptionEn : entity.DescriptionFr,
            DisplayOrder = entity.DisplayOrder
        };
    }
}
