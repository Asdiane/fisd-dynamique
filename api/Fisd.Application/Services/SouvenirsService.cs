using Fisd.Application.Models.receive.Souvenir;
using Fisd.Application.Models.result.Souvenir;
using Fisd.Application.Services.Interfaces;
using Fisd.Persistence;
using Fisd.Persistence.Entities.Content;
using Fisd.Persistence.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fisd.Application.Services
{
    public class SouvenirsService : ISouvenirsService
    {
        private readonly FisdDbContext _dbContext;

        public SouvenirsService(FisdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<SouvenirResultModel>> GetAllAsync()
        {
            var entities = await _dbContext.Souvenirs.OrderByDescending(s => s.Year).ToListAsync();
            return entities.Select(ToResultModel).ToList();
        }

        public async Task<List<SouvenirPhotoResultModel>> GetPhotosAsync(Guid souvenirId)
        {
            var entities = await _dbContext.SouvenirPhotos
                .Where(p => p.SouvenirId == souvenirId)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();
            return entities.Select(ToResultModel).ToList();
        }

        public async Task<SouvenirResultModel> CreateAsync(SaveSouvenirModel model)
        {
            var entity = new SouvenirEntity
            {
                Id = Guid.NewGuid(),
                Year = model.Year,
                Title = model.Title,
                Description = model.Description,
                Status = ParseStatus(model.Status)
            };

            _dbContext.Souvenirs.Add(entity);
            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<SouvenirResultModel?> UpdateAsync(Guid id, SaveSouvenirModel model)
        {
            var entity = await _dbContext.Souvenirs.FirstOrDefaultAsync(s => s.Id == id);
            if (entity == null)
            {
                return null;
            }

            entity.Year = model.Year;
            entity.Title = model.Title;
            entity.Description = model.Description;
            entity.Status = ParseStatus(model.Status);

            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _dbContext.Souvenirs.FirstOrDefaultAsync(s => s.Id == id);
            if (entity == null)
            {
                return false;
            }

            _dbContext.Souvenirs.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<SouvenirPhotoResultModel?> AddPhotoAsync(Guid souvenirId, SaveSouvenirPhotoModel model)
        {
            var souvenirExists = await _dbContext.Souvenirs.AnyAsync(s => s.Id == souvenirId);
            if (!souvenirExists)
            {
                return null;
            }

            var entity = new SouvenirPhotoEntity
            {
                Id = Guid.NewGuid(),
                SouvenirId = souvenirId,
                ImageUrl = model.ImageUrl,
                Caption = model.Caption,
                DisplayOrder = model.DisplayOrder
            };

            _dbContext.SouvenirPhotos.Add(entity);
            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<bool> DeletePhotoAsync(Guid souvenirId, Guid photoId)
        {
            var entity = await _dbContext.SouvenirPhotos.FirstOrDefaultAsync(p => p.Id == photoId && p.SouvenirId == souvenirId);
            if (entity == null)
            {
                return false;
            }

            _dbContext.SouvenirPhotos.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private static SouvenirStatusEnum ParseStatus(string status) =>
            status == "coming_soon" ? SouvenirStatusEnum.ComingSoon : SouvenirStatusEnum.Published;

        private static SouvenirResultModel ToResultModel(SouvenirEntity entity) => new()
        {
            Id = entity.Id,
            Year = entity.Year,
            Title = entity.Title,
            Description = entity.Description,
            Status = entity.Status == SouvenirStatusEnum.ComingSoon ? "coming_soon" : "published"
        };

        private static SouvenirPhotoResultModel ToResultModel(SouvenirPhotoEntity entity) => new()
        {
            Id = entity.Id,
            SouvenirId = entity.SouvenirId,
            ImageUrl = entity.ImageUrl,
            Caption = entity.Caption,
            DisplayOrder = entity.DisplayOrder
        };
    }
}
