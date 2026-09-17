using Fisd.Application.Services.Interfaces;

namespace Fisd.Api.Infrastructure
{
    public class HttpContextCurrentLanguageAccessor : ICurrentLanguageAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextCurrentLanguageAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetLanguage()
        {
            var header = _httpContextAccessor.HttpContext?.Request.Headers.AcceptLanguage.ToString();
            return string.Equals(header, "en", StringComparison.OrdinalIgnoreCase) ? "en" : "fr";
        }
    }
}
