using Fisd.Application.Models.receive.Contact;
using Fisd.Application.Models.result.Contact;

namespace Fisd.Application.Services.Interfaces
{
    public interface IContactsService
    {
        Task<List<ContactResultModel>> GetVisibleAsync();
        Task<List<ContactResultModel>> GetAllAsync();
        Task<ContactResultModel> CreateAsync(SaveContactModel model);
        Task<ContactResultModel?> UpdateAsync(Guid id, SaveContactModel model);
        Task<bool> DeleteAsync(Guid id);
    }
}
