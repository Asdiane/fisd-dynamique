using System.Security.Claims;
using Fisd.Persistence.Common;

namespace Fisd.Api.Infrastructure
{
    public class HttpContextCurrentAdminAccessor : ICurrentAdminAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextCurrentAdminAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? GetCurrentAdminUserId()
        {
            var idClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
            return Guid.TryParse(idClaim, out var id) ? id : null;
        }
    }
}
