using Fisd.Application.Models.receive.SiteSettings;
using Fisd.Application.Models.result.SiteSettings;
using Fisd.Application.Services.Interfaces;
using Fisd.Persistence;
using Fisd.Persistence.Entities.Settings;
using Microsoft.EntityFrameworkCore;

namespace Fisd.Application.Services
{
    public class SiteSettingsService : ISiteSettingsService
    {
        private const int DefaultSlideDurationSeconds = 5;

        private readonly FisdDbContext _dbContext;

        public SiteSettingsService(FisdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SiteSettingsResultModel> GetAsync()
        {
            var entity = await GetOrCreateAsync();
            return ToResultModel(entity);
        }

        public async Task<SiteSettingsResultModel> UpdateAsync(SaveSiteSettingsModel model)
        {
            var entity = await GetOrCreateAsync();
            entity.SlideDurationSeconds = model.SlideDurationSeconds;
            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        // There is always exactly one row - if it's ever missing (a fresh DB before the seed
        // migration ran, for instance), create it on the fly with the default rather than 500.
        private async Task<SiteSettingsEntity> GetOrCreateAsync()
        {
            var entity = await _dbContext.SiteSettings.FirstOrDefaultAsync();
            if (entity != null)
            {
                return entity;
            }

            entity = new SiteSettingsEntity { Id = Guid.NewGuid(), SlideDurationSeconds = DefaultSlideDurationSeconds };
            _dbContext.SiteSettings.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        private static SiteSettingsResultModel ToResultModel(SiteSettingsEntity entity) => new()
        {
            SlideDurationSeconds = entity.SlideDurationSeconds
        };
    }
}
