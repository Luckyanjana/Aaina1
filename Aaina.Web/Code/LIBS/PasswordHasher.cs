using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Aaina.Web.Code.LIBS
{
    public class PasswordHasher
    {

        public static string GeneratePassword(string password, string salt)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
          password: password,
          salt: Convert.FromBase64String(salt),
          prf: KeyDerivationPrf.HMACSHA1,
          iterationCount: 10000,
          numBytesRequested: 256 / 8));

            return hashed;

        }

        public static string GenerateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);

        }

        public static bool VerifyHashedPassword(string hashedPassword, string password, string salt)
        {
            var newPassword = GeneratePassword(password, salt);
            return hashedPassword == newPassword;
        }

        internal static object GeneratePassword(object password, string salt)
        {
            throw new NotImplementedException();
        }
    }
}
