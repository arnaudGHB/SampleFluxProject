using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CBS.TransactionManagement.Helper.Helper
{
    public static class BalanceEncryption
    {
        private static readonly string salt = "Salty@CBS*Thug";

        public static string Encrypt(string balance, string accountNumber)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = DeriveKey(accountNumber);
            aesAlg.GenerateIV();

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            using MemoryStream msEncrypt = new MemoryStream();
            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(balance);
            }

            byte[] encryptedBytes = msEncrypt.ToArray();

            // Combine IV and encrypted data
            byte[] result = new byte[aesAlg.IV.Length + encryptedBytes.Length];
            Buffer.BlockCopy(aesAlg.IV, 0, result, 0, aesAlg.IV.Length);
            Buffer.BlockCopy(encryptedBytes, 0, result, aesAlg.IV.Length, encryptedBytes.Length);

            return Convert.ToBase64String(result);
        }

        public static string Decrypt(string cipherText, string accountNumber)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = DeriveKey(accountNumber);

            // Extract IV from the beginning of the cipherText
            byte[] iv = new byte[aesAlg.IV.Length];
            Buffer.BlockCopy(Convert.FromBase64String(cipherText), 0, iv, 0, aesAlg.IV.Length);
            aesAlg.IV = iv;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText), aesAlg.IV.Length, Convert.FromBase64String(cipherText).Length - aesAlg.IV.Length);
            using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }

        private static byte[] DeriveKey(string password)
        {
            // Convert the string to Base64
            string base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(salt));
            using Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(base64String), 10000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(16); // 16 bytes for AES-128
        }

        public static bool VerifyBalanceIntegrity(string standingBalance, string encryptedBalance, string accountNumber)
        {
            if (string.IsNullOrEmpty(encryptedBalance)) return true;
            string encryptedOriginalValue = Decrypt(encryptedBalance, accountNumber);
            decimal.Parse(encryptedOriginalValue);
            decimal.Parse(standingBalance);

            return decimal.Parse(encryptedOriginalValue).Equals(decimal.Parse(standingBalance));
            //return string.Equals(encryptedOriginalValue, standingBalance);
        }
    }
}
