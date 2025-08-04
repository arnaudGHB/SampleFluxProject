using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.HELPER.Helper
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public static class PinSecurity
    {
        /// <summary>
        /// Hashes a PIN using PBKDF2 with HMACSHA512 for secure storage.
        /// </summary>
        /// <param name="pin">The plain-text PIN to be hashed.</param>
        /// <returns>The hashed PIN as a Base64 encoded string.</returns>
        public static string HashPin(string pin)
        {
            if (string.IsNullOrEmpty(pin))
            {
                throw new ArgumentException("PIN cannot be null or empty.", nameof(pin));
            }

            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[16];
                rng.GetBytes(salt);

                using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(pin, salt, 100000, HashAlgorithmName.SHA512))
                {
                    byte[] hash = rfc2898DeriveBytes.GetBytes(32);

                    // Combine the salt and hash for storage
                    byte[] hashBytes = new byte[48];
                    Array.Copy(salt, 0, hashBytes, 0, 16);
                    Array.Copy(hash, 0, hashBytes, 16, 32);

                    return Convert.ToBase64String(hashBytes);
                }
            }
        }

        /// <summary>
        /// Verifies a provided PIN against a stored hashed PIN.
        /// </summary>
        /// <param name="inputPin">The plain-text PIN provided during login.</param>
        /// <param name="storedHashedPin">The stored hashed PIN.</param>
        /// <returns>True if the PIN matches the hash; otherwise, false.</returns>
        public static bool VerifyPin(string inputPin, string storedHashedPin)
        {
            if (string.IsNullOrEmpty(inputPin) || string.IsNullOrEmpty(storedHashedPin))
            {
                throw new ArgumentException("Input PIN and stored hashed PIN cannot be null or empty.");
            }

            byte[] hashBytes = Convert.FromBase64String(storedHashedPin);

            // Extract the salt from the stored hash
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(inputPin, salt, 100000, HashAlgorithmName.SHA512))
            {
                byte[] hash = rfc2898DeriveBytes.GetBytes(32);

                // Compare the stored hash with the hash of the input PIN
                for (int i = 0; i < 32; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
