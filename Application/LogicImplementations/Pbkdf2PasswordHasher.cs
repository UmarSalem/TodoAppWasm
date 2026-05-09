using System.Security.Cryptography;
using Application.LogicInterfaces;

namespace Application.LogicImplementations
{
    public class Pbkdf2PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100_000;
        private const string Algorithm = "PBKDF2-SHA256";

        public string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] key = DeriveKey(password, salt, Iterations);

            // Store algorithm settings beside the hash so old passwords can still be verified
            // if we raise the iteration count later. The plain password is never saved.
            return string.Join(
                '$',
                Algorithm,
                Iterations,
                Convert.ToBase64String(salt),
                Convert.ToBase64String(key));
        }

        public bool Verify(string password, string passwordHash)
        {
            string[] parts = passwordHash.Split('$');
            if (parts.Length != 4 || parts[0] != Algorithm)
            {
                return false;
            }

            if (!int.TryParse(parts[1], out int iterations))
            {
                return false;
            }

            byte[] salt;
            byte[] expectedKey;
            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expectedKey = Convert.FromBase64String(parts[3]);
            }
            catch (FormatException)
            {
                return false;
            }

            byte[] actualKey = DeriveKey(password, salt, iterations);

            // Fixed-time comparison avoids leaking tiny timing clues about the stored hash.
            return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
        }

        private static byte[] DeriveKey(string password, byte[] salt, int iterations)
        {
            return Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                KeySize);
        }
    }
}
