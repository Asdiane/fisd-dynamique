using Fisd.Application.Models.receive.Testimonial;
using Fisd.Application.Models.result.Testimonial;
using Fisd.Application.Services.Interfaces;
using Fisd.Persistence;
using Fisd.Persistence.Entities.Content;
using Fisd.Persistence.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fisd.Application.Services
{
    public class TestimonialsService : ITestimonialsService
    {
        private readonly FisdDbContext _dbContext;
        private readonly ICurrentLanguageAccessor _currentLanguageAccessor;

        public TestimonialsService(FisdDbContext dbContext, ICurrentLanguageAccessor currentLanguageAccessor)
        {
            _dbContext = dbContext;
            _currentLanguageAccessor = currentLanguageAccessor;
        }

        public async Task<List<PublicTestimonialResultModel>> GetVisibleAsync()
        {
            var entities = await _dbContext.Testimonials
                .Where(t => t.IsVisible && t.Status == ContentStatusEnum.Published)
                .OrderBy(t => t.DisplayOrder)
                .ToListAsync();
            var isEn = _currentLanguageAccessor.GetLanguage() == "en";
            return entities.Select(e => ToPublicResultModel(e, isEn)).ToList();
        }

        public async Task<List<TestimonialResultModel>> GetAllAsync()
        {
            var entities = await _dbContext.Testimonials.OrderBy(t => t.DisplayOrder).ToListAsync();
            return entities.Select(ToResultModel).ToList();
        }

        public async Task<TestimonialResultModel> CreateAsync(SaveTestimonialModel model)
        {
            var entity = new TestimonialEntity
            {
                Id = Guid.NewGuid(),
                AuthorName = model.AuthorName,
                ImageUrl = model.ImageUrl,
                AuthorRoleFr = model.AuthorRoleFr,
                AuthorRoleEn = model.AuthorRoleEn,
                ContentFr = model.ContentFr,
                ContentEn = model.ContentEn,
                IsVisible = model.IsVisible,
                Status = ParseStatus(model.Status),
                DisplayOrder = model.DisplayOrder
            };

            _dbContext.Testimonials.Add(entity);
            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<TestimonialResultModel?> UpdateAsync(Guid id, SaveTestimonialModel model)
        {
            var entity = await _dbContext.Testimonials.FirstOrDefaultAsync(t => t.Id == id);
            if (entity == null)
            {
                return null;
            }

            entity.AuthorName = model.AuthorName;
            entity.ImageUrl = model.ImageUrl;
            entity.AuthorRoleFr = model.AuthorRoleFr;
            entity.AuthorRoleEn = model.AuthorRoleEn;
            entity.ContentFr = model.ContentFr;
            entity.ContentEn = model.ContentEn;
            entity.IsVisible = model.IsVisible;
            entity.Status = ParseStatus(model.Status);
            entity.DisplayOrder = model.DisplayOrder;

            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _dbContext.Testimonials.FirstOrDefaultAsync(t => t.Id == id);
            if (entity == null)
            {
                return false;
            }

            _dbContext.Testimonials.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private static ContentStatusEnum ParseStatus(string status) =>
            status == "published" ? ContentStatusEnum.Published : ContentStatusEnum.Draft;

        private static TestimonialResultModel ToResultModel(TestimonialEntity entity) => new()
        {
            Id = entity.Id,
            AuthorName = entity.AuthorName,
            ImageUrl = entity.ImageUrl,
            AuthorRoleFr = entity.AuthorRoleFr,
            AuthorRoleEn = entity.AuthorRoleEn,
            ContentFr = entity.ContentFr,
            ContentEn = entity.ContentEn,
            IsVisible = entity.IsVisible,
            Status = entity.Status == ContentStatusEnum.Published ? "published" : "draft",
            DisplayOrder = entity.DisplayOrder
        };

        private static PublicTestimonialResultModel ToPublicResultModel(TestimonialEntity entity, bool isEn) => new()
        {
            Id = entity.Id,
            AuthorName = entity.AuthorName,
            ImageUrl = entity.ImageUrl,
            AuthorRole = isEn ? entity.AuthorRoleEn : entity.AuthorRoleFr,
            Content = isEn ? entity.ContentEn : entity.ContentFr,
            DisplayOrder = entity.DisplayOrder
        };
    }
}
