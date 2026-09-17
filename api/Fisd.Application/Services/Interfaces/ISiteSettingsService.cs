using Fisd.Application.Models.receive.SiteSettings;
using Fisd.Application.Models.result.SiteSettings;

namespace Fisd.Application.Services.Interfaces
{
    public interface ISiteSettingsService
    {
        Task<SiteSettingsResultModel> GetAsync();
        Task<SiteSettingsResultModel> UpdateAsync(SaveSiteSettingsModel model);
    }
}
