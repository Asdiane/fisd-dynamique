namespace Fisd.Application.Security
{
    public class JwtOptions
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpiryHours { get; set; } = 12;
    }
}
