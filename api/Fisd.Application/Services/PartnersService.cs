using Fisd.Application.Models.receive.Partner;
using Fisd.Application.Models.result.Partner;
using Fisd.Application.Services.Interfaces;
using Fisd.Persistence;
using Fisd.Persistence.Entities.Content;
using Fisd.Persistence.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fisd.Application.Services
{
    public class PartnersService : IPartnersService
    {
        private readonly FisdDbContext _dbContext;

        public PartnersService(FisdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PartnerResultModel>> GetVisibleAsync()
        {
            var entities = await _dbContext.Partners
                .Where(p => p.IsVisible && p.Status == ContentStatusEnum.Published)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();
            return entities.Select(ToResultModel).ToList();
        }

        public async Task<List<PartnerResultModel>> GetAllAsync()
        {
            var entities = await _dbContext.Partners.OrderBy(p => p.DisplayOrder).ToListAsync();
            return entities.Select(ToResultModel).ToList();
        }

        public async Task<PartnerResultModel> CreateAsync(SavePartnerModel model)
        {
            var entity = new PartnerEntity
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Label = model.Label,
                LogoUrl = model.LogoUrl,
                IsVisible = model.IsVisible,
                Status = ParseStatus(model.Status),
                DisplayOrder = model.DisplayOrder
            };

            _dbContext.Partners.Add(entity);
            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<PartnerResultModel?> UpdateAsync(Guid id, SavePartnerModel model)
        {
            var entity = await _dbContext.Partners.FirstOrDefaultAsync(p => p.Id == id);
            if (entity == null)
            {
                return null;
            }

            entity.Name = model.Name;
            entity.Label = model.Label;
            entity.LogoUrl = model.LogoUrl;
            entity.IsVisible = model.IsVisible;
            entity.Status = ParseStatus(model.Status);
            entity.DisplayOrder = model.DisplayOrder;

            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _dbContext.Partners.FirstOrDefaultAsync(p => p.Id == id);
            if (entity == null)
            {
                return false;
            }

            _dbContext.Partners.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private static ContentStatusEnum ParseStatus(string status) =>
            status == "published" ? ContentStatusEnum.Published : ContentStatusEnum.Draft;

        private static PartnerResultModel ToResultModel(PartnerEntity entity) => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Label = entity.Label,
            LogoUrl = entity.LogoUrl,
            IsVisible = entity.IsVisible,
            Status = entity.Status == ContentStatusEnum.Published ? "published" : "draft",
            DisplayOrder = entity.DisplayOrder
        };
    }
}
