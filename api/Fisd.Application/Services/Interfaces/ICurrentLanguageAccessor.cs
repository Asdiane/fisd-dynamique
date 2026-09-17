namespace Fisd.Application.Services.Interfaces
{
    // Resolves which language ("fr" or "en") the current request wants for bilingual content
    // fields. Kept as an interface so services stay testable without a real HttpContext.
    public interface ICurrentLanguageAccessor
    {
        string GetLanguage();
    }
}
