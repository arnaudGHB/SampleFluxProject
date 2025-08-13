using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Helper.Helper
{
    public static class PinSecurity
    {
        /// <summary>
        /// Hashes a PIN using SHA256 for secure storage.
        /// </summary>
        /// <param name="pin">The plain-text PIN to be hashed.</param>
        /// <returns>The hashed PIN as a Base64 encoded string.</returns>
        public static string HashPin(string pin)
        {
            if (string.IsNullOrEmpty(pin))
            {
                throw new ArgumentException("PIN cannot be null or empty.", nameof(pin));
            }

            using (var sha256 = SHA256.Create())
            {
                var pinBytes = Encoding.UTF8.GetBytes(pin);
                var hashBytes = sha256.ComputeHash(pinBytes);
                return Convert.ToBase64String(hashBytes);
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

            // Hash the input PIN and compare it to the stored hash
            string hashedInputPin = HashPin(inputPin);
            return string.Equals(hashedInputPin, storedHashedPin, StringComparison.Ordinal);
        }
    }
}
