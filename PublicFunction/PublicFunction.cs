using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
namespace HafizDemoAPI
{
    public static class PubFunc
    {
        //public PubFunc() { }
        static HashAlgorithmName hashAlgorithm =  HashAlgorithmName.SHA256;
        static int keySize = 64;
        static int iterations = 350000;
        /// <summary>
        /// Hashing function , ref https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-7.0
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static ReturnHash HashPassword(string password)
        {
            // Generate a 128-bit salt using a sequence of
            // cryptographically strong random bytes.
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes
            //Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");

            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: iterations,
                numBytesRequested: keySize));
            //return hashed;
            return new ReturnHash { HashedPassword= hashed,Salt = Convert.ToBase64String(salt) };
        }

        public static bool VerifyPassword(string password, string hash, byte[] salt)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);
            return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromBase64String(hash));
        }

        public class ReturnHash
        {
            public string HashedPassword { get; set; }
            public string Salt { get; set; }
        }

    }
}
