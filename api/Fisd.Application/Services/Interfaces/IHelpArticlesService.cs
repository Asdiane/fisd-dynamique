using Fisd.Application.Models.receive.Diagnostics;
using Fisd.Application.Models.result.Diagnostics;

namespace Fisd.Application.Services.Interfaces
{
    public interface IHelpArticlesService
    {
        Task<List<PublicHelpArticleResultModel>> GetVisibleAsync(bool isPlatformAdmin);
        Task<List<HelpArticleResultModel>> GetAllAsync();
        Task<HelpArticleResultModel> CreateAsync(SaveHelpArticleModel model);
        Task<HelpArticleResultModel?> UpdateAsync(Guid id, SaveHelpArticleModel model);
        Task<bool> DeleteAsync(Guid id);
    }
}
