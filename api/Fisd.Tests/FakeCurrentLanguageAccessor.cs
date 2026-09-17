using Fisd.Application.Services.Interfaces;

namespace Fisd.Tests
{
    public class FakeCurrentLanguageAccessor : ICurrentLanguageAccessor
    {
        private readonly string _language;

        public FakeCurrentLanguageAccessor(string language = "fr")
        {
            _language = language;
        }

        public string GetLanguage() => _language;
    }
}
