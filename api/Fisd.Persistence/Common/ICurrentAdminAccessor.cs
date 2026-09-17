namespace Fisd.Persistence.Common
{
    // Kept ASP.NET Core-free on purpose - Fisd.Persistence is a plain class library and
    // shouldn't need a FrameworkReference to the web SDK just to know who's making a change.
    // The actual HttpContext-based implementation lives in Fisd.Api.
    public interface ICurrentAdminAccessor
    {
        Guid? GetCurrentAdminUserId();
    }
}
