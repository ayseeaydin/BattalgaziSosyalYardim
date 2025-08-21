using System;
using System.Security.Cryptography;
using System.Text;

namespace BattalgaziSosyalYardim.Security
{
    public static class PasswordHasher
    {
        private const int Iterations = 100_000;
        private const int SaltSize = 16;
        private const int KeySize = 32;

        public static string Hash(string password)
        {
            using var rng=RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var key= pbkdf2.GetBytes(KeySize);

            return $"PBKDF2|{Iterations}|{Convert.ToBase64String(salt)}|{Convert.ToBase64String(key)}";
        }

        public static bool Verify(string password, string encoded)
        {
            try
            {
                var parts = encoded.Split('|');
                if (parts.Length != 4 || parts[0] != "PBKDF2") return false;

                var iterations = int.Parse(parts[1]);
                var salt = Convert.FromBase64String(parts[2]);
                var expectedKey = Convert.FromBase64String(parts[3]);

                using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
                var actualKey = pbkdf2.GetBytes(expectedKey.Length);

                // zaman sabit karşılaştırma
                return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
            }
            catch { return false; }
        }
    }
}
