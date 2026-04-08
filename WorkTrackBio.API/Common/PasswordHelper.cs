using System.Security.Cryptography;

namespace WorkTrackBio.API.Common
{
    public static class PasswordHelper
    {
        private const int SaltSize = 32;
        private const int HashSize = 64;
        private const int Iterations = 100_000;
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;

        public static (string hash, string salt) HashPassword(string password)
        {
            var saltBytes = new byte[SaltSize];
            RandomNumberGenerator.Fill(saltBytes);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, Algorithm);
            var hashBytes = pbkdf2.GetBytes(HashSize);

            return (Convert.ToHexString(hashBytes), Convert.ToHexString(saltBytes));
        }

        public static bool VerifyPassword(string password, string hash, string salt)
        {
            var saltBytes = Convert.FromHexString(salt);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, Algorithm);
            var hashBytes = pbkdf2.GetBytes(HashSize);
            return string.Equals(Convert.ToHexString(hashBytes), hash, StringComparison.OrdinalIgnoreCase);
        }
    }
}