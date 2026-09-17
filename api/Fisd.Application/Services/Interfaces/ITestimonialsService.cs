using Fisd.Application.Models.receive.Testimonial;
using Fisd.Application.Models.result.Testimonial;

namespace Fisd.Application.Services.Interfaces
{
    public interface ITestimonialsService
    {
        Task<List<PublicTestimonialResultModel>> GetVisibleAsync();
        Task<List<TestimonialResultModel>> GetAllAsync();
        Task<TestimonialResultModel> CreateAsync(SaveTestimonialModel model);
        Task<TestimonialResultModel?> UpdateAsync(Guid id, SaveTestimonialModel model);
        Task<bool> DeleteAsync(Guid id);
    }
}
