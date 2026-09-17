using Fisd.Application.Models.receive.Contact;
using Fisd.Application.Models.result.Contact;
using Fisd.Application.Services.Interfaces;
using Fisd.Persistence;
using Fisd.Persistence.Entities.Content;
using Fisd.Persistence.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fisd.Application.Services
{
    public class ContactsService : IContactsService
    {
        private readonly FisdDbContext _dbContext;

        public ContactsService(FisdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ContactResultModel>> GetVisibleAsync()
        {
            var entities = await _dbContext.Contacts
                .Where(c => c.IsVisible && c.Status == ContentStatusEnum.Published)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
            return entities.Select(ToResultModel).ToList();
        }

        public async Task<List<ContactResultModel>> GetAllAsync()
        {
            var entities = await _dbContext.Contacts.OrderBy(c => c.DisplayOrder).ToListAsync();
            return entities.Select(ToResultModel).ToList();
        }

        public async Task<ContactResultModel> CreateAsync(SaveContactModel model)
        {
            var entity = new ContactEntity
            {
                Id = Guid.NewGuid(),
                Type = ParseType(model.Type),
                Label = model.Label,
                Value = model.Value,
                Href = model.Href,
                IsVisible = model.IsVisible,
                Status = ParseStatus(model.Status),
                DisplayOrder = model.DisplayOrder
            };

            _dbContext.Contacts.Add(entity);
            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<ContactResultModel?> UpdateAsync(Guid id, SaveContactModel model)
        {
            var entity = await _dbContext.Contacts.FirstOrDefaultAsync(c => c.Id == id);
            if (entity == null)
            {
                return null;
            }

            entity.Type = ParseType(model.Type);
            entity.Label = model.Label;
            entity.Value = model.Value;
            entity.Href = model.Href;
            entity.IsVisible = model.IsVisible;
            entity.Status = ParseStatus(model.Status);
            entity.DisplayOrder = model.DisplayOrder;

            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _dbContext.Contacts.FirstOrDefaultAsync(c => c.Id == id);
            if (entity == null)
            {
                return false;
            }

            _dbContext.Contacts.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private static ContactTypeEnum ParseType(string type) =>
            Enum.TryParse<ContactTypeEnum>(type, ignoreCase: true, out var parsed) ? parsed : ContactTypeEnum.Email;

        private static ContentStatusEnum ParseStatus(string status) =>
            status == "published" ? ContentStatusEnum.Published : ContentStatusEnum.Draft;

        private static ContactResultModel ToResultModel(ContactEntity entity) => new()
        {
            Id = entity.Id,
            Type = entity.Type.ToString().ToLowerInvariant(),
            Label = entity.Label,
            Value = entity.Value,
            Href = entity.Href,
            IsVisible = entity.IsVisible,
            Status = entity.Status == ContentStatusEnum.Published ? "published" : "draft",
            DisplayOrder = entity.DisplayOrder
        };
    }
}
