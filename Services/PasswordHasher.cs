using System;
using System.Security.Cryptography;

namespace Backend.Services
{
    /// <summary>
    /// Salted PBKDF2 password hashing. Uses only built-in .NET cryptography,
    /// so it needs no extra NuGet package. Stored format:
    /// {iterations}.{base64(salt)}.{base64(hash)}
    /// </summary>
    public static class PasswordHasher
    {
        private const int SaltSize = 16;       // 128 bit
        private const int HashSize = 32;       // 256 bit
        private const int Iterations = 100_000;

        public static string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool Verify(string password, string storedHash)
        {
            var parts = storedHash.Split('.', 3);
            if (parts.Length != 3) return false;

            if (!int.TryParse(parts[0], out var iterations)) return false;

            byte[] salt, expectedHash;
            try
            {
                salt = Convert.FromBase64String(parts[1]);
                expectedHash = Convert.FromBase64String(parts[2]);
            }
            catch (FormatException)
            {
                return false;
            }

            var actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expectedHash.Length);
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
    }
}
