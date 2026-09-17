using System.Security.Cryptography;

namespace Fisd.Application.Security
{
    // PBKDF2-HMACSHA256. Format is self-describing (iterations.salt.hash, both base64) so the
    // iteration count can be raised later without invalidating hashes minted under a lower one.
    public static class PasswordHasher
    {
        private const int Iterations = 210_000;
        private const int SaltSizeBytes = 16;
        private const int HashSizeBytes = 32;

        public static string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSizeBytes);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSizeBytes);
            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool Verify(string password, string encoded)
        {
            var parts = encoded.Split('.');
            if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations))
            {
                return false;
            }

            var salt = Convert.FromBase64String(parts[1]);
            var expectedHash = Convert.FromBase64String(parts[2]);
            var actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expectedHash.Length);
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
    }
}
