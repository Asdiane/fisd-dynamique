using Fisd.Application.Models.receive.Article;
using Fisd.Application.Models.result.Article;

namespace Fisd.Application.Services.Interfaces
{
    public interface IArticlesService
    {
        Task<List<ArticleResultModel>> GetPublishedAsync();
        Task<ArticleResultModel?> GetByIdAsync(Guid id);
        Task<List<ArticleResultModel>> GetAllAsync();
        Task<ArticleResultModel> CreateAsync(SaveArticleModel model);
        Task<ArticleResultModel?> UpdateAsync(Guid id, SaveArticleModel model);
        Task<bool> DeleteAsync(Guid id);
    }
}
