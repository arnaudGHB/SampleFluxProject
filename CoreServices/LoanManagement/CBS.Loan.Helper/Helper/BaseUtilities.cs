using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Helper.Helper
{


    public static class BaseUtilities
    {
        /// <summary>
        /// Generates a unique insurance number with a specific prefix and maximum size.
        /// </summary>
        /// <param name="maxSize">Maximum size of the generated number.</param>
        /// <param name="prefix">Prefix for the generated number.</param>
        /// <returns>Generated insurance number.</returns>
        public static string GenerateInsuranceUniqueNumber(int maxSize, string prefix)
        {
            const string chars = "1234567890";
            return GenerateUniqueNumber(maxSize, chars, prefix);
        }

        public static decimal RoundUpValue(decimal value, int decimalPlaces = 0)
        {
            decimal multiplier = (decimal)Math.Pow(10, decimalPlaces);
            return Math.Ceiling(value * multiplier) / multiplier;
        }
        public static decimal GetChargeRoundedUp(decimal amount, decimal PercentageValue, string FeeBase = "Range", decimal Charge=0)
        {
            
            decimal value = 0;

            // Determine the value based on whether it's a range or percentage
            if (FeeBase == "Range")
            {
                value = Charge;
            }
            else
            {
                value = PercentageValue / 100 * amount;
            }
            value=RoundUpValue(value);
            return value;
        }
        public static string FormatCurrencyx(decimal amount, int decimalPlaces=1)
        {
            // Create a CultureInfo object for French (CFA Franc) with the appropriate currency symbol
            CultureInfo cultureInfo = new CultureInfo("fr-FR");
            cultureInfo.NumberFormat.CurrencySymbol = "FCFA";

            // Format amount as currency with the specified number of decimal places using the custom culture
            return string.Format(cultureInfo, "{0:C" + decimalPlaces + "}", amount);
        }
        public static string FormatCurrency(decimal amount, string currencySymbol = "XAF", int decimalPlaces = 1)
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}{1:N" + decimalPlaces + "}", currencySymbol, amount);
        }

        /// <summary>
        /// Generates a unique number with a specific maximum size.
        /// </summary>
        /// <param name="maxSize">Maximum size of the generated number.</param>
        /// <returns>Generated number.</returns>
        public static string GenerateUniqueNumber(int maxSize)
        {
            const string chars = "1234567890";
            return GenerateUniqueNumber(maxSize, chars);
        }
        /// <summary>
        /// Generates a unique number up to 15 digits.
        /// </summary>
        /// <returns>Generated number.</returns>
        public static string GenerateUniqueNumber()
        {
            const string chars = "1234567890";
            return GenerateUniqueNumber(15, chars);
        }
        public static string ConvertToyyyyMMdd(string inputDate)
        {
            // Define array of input date formats to try parsing
            string[] formats = { "dd-MM-yyyy", "dd/MM/yyyy", "MM/dd/yyyy", "MM-dd-yyyy" };

            // Parse the input date using DateTime.ParseExact with formats
            DateTime parsedDate = DateTime.ParseExact(inputDate, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);

            // Format the parsed date to yyyyMMdd
            return parsedDate.ToString("yyyyMMdd");
        }
        /// <summary>
        /// Logs an action and audits it by sending the log information to the audit trail API.
        /// </summary>
        /// <param name="message">A descriptive message detailing the action being logged.</param>
        /// <param name="request">The request object associated with the action, which may contain relevant data.</param>
        /// <param name="statusCode">The HTTP status code representing the outcome of the action, defined in <see cref="HttpStatusCodeEnum"/>.</param>
        /// <param name="logAction">The specific action being logged, represented as a <see cref="LogAction"/> enumeration.</param>
        /// <param name="logLevel">The severity level of the log entry, represented as a <see cref="LogLevelInfo"/> enumeration.</param>
        /// <param name="corrolationid">It can represent your transaction id.</param>
        public static async Task LogAndAuditAsync(string message, object request, HttpStatusCodeEnum statusCode, LogAction logAction, LogLevelInfo logLevel, string corrolationid = null)
        {
            // Call the AuditLogger method to record the action and associated details asynchronously.
            await APICallHelper.AuditLogger(logAction.ToString(), request, message, logLevel.ToString(), (int)statusCode, corrolationid);
        }
        public static async Task LogAndAuditAsync(string message, object request, HttpStatusCodeEnum statusCode, LogAction logAction, LogLevelInfo logLevel, string userName, string token,string corrolationid = null)
        {
            // Call the AuditLogger method to record the action and associated details asynchronously.
            await APICallHelper.AuditLogger(logAction.ToString(), request, message, logLevel.ToString(), (int)statusCode,corrolationid,userName,token);
        }
        public static string ConvertToyyyyMMdd(DateTime inputDate)
        {
            // Format the inputDate to yyyyMMdd
            return inputDate.ToString("yyyyMMdd");
        }
        public static string GetFileSize(string filePath)
        {
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                return "File not found";
            }

            // Get the file info
            var fileInfo = new FileInfo(filePath);

            // Get the file size in bytes
            long fileSizeInBytes = fileInfo.Length;

            // Convert bytes to kilobytes
            double fileSizeInKB = fileSizeInBytes / 1024.0;

            // Convert kilobytes to megabytes
            double fileSizeInMB = fileSizeInKB / 1024.0;

            // Return the file size in a readable format
            if (fileSizeInMB >= 1)
            {
                return $"{fileSizeInMB:F2} MB";
            }
            else
            {
                return $"{fileSizeInKB:F2} KB";
            }
        }

        /// <summary>
        /// Adds '237' prefix to the MSISDN if it doesn't already start with it.
        /// </summary>
        /// <param name="msisdn">Mobile number without prefix.</param>
        /// <returns>MSISDN with '237' prefix.</returns>
        public static string Add237Prefix(string msisdn)
        {
            const string prefix = "237";
            if (!msisdn.StartsWith(prefix))
            {
                msisdn = $"{prefix}{msisdn}";
            }
            return msisdn;
        }

        private static string GenerateUniqueNumber(int maxSize, string allowedChars, string prefix = "")
        {
            char[] chars = allowedChars.ToCharArray();
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[maxSize];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % chars.Length]);
            }
            return $"{prefix}{result.ToString()}";
        }
        /// <summary>
        /// Converts a DateTime to UTC if it's in Unspecified kind.
        /// </summary>
        /// <param name="dateTime">The DateTime value.</param>
        /// <returns>UTC representation of the input DateTime.</returns>
        public static DateTime ToUtc(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
                dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            return dateTime.ToUniversalTime();
        }
        public static int GetCurrentAge(this DateTimeOffset dateTimeOffset,
          DateTimeOffset? dateOfDeath)
        {
            var dateToCalculateTo = DateTime.UtcNow;

            if (dateOfDeath != null)
            {
                dateToCalculateTo = dateOfDeath.Value.UtcDateTime;
            }

            int age = dateToCalculateTo.Year - dateTimeOffset.Year;

            if (dateToCalculateTo < dateTimeOffset.AddYears(age))
            {
                age--;
            }

            return age;
        }
        private static DateTime ConvertToDoualaTime(DateTime utcDateTime)
        {
            // Get the time zone info for Cameroon (UTC+1, "W. Central Africa Standard Time")
            TimeZoneInfo cameroonTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");

            // Convert the UTC DateTime to Cameroon time
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, cameroonTimeZone);
        }

        // Converts current UTC time to Douala time
        public static DateTime UtcNowToDoualaTime()
        {
            return ConvertToDoualaTime(DateTime.UtcNow);
        }

        // Converts a given UTC DateTime to Douala time
        public static DateTime UtcToDoualaTime(this DateTime? dateTime)
        {
            return ConvertToDoualaTime(DateTime.UtcNow);
        }
    }
    public static class InsuranceCodeGenerator
    {
        // Function to generate a unique 7-digit insurance code
        public static string GenerateInsuranceCode(List<string> existingCodes)
        {
            string newCode = GenerateRandomCode();

            // Check if the generated code already exists in the provided list
            while (existingCodes.Contains(newCode))
            {
                newCode = GenerateSequentialCode(existingCodes);
            }

            return newCode;
        }



        // Helper function to generate a random 7-digit code
        private static string GenerateRandomCode()
        {
            Random random = new Random();
            int code = random.Next(1000000, 10000000); // Generates a random number between 1,000,000 and 9,999,999
            return code.ToString();
        }

        // Helper function to generate a sequential code based on existing codes
        private static string GenerateSequentialCode(List<string> existingCodes)
        {
            int lastCode = existingCodes.Count > 0 ? int.Parse(existingCodes.Last()) : 9999999;
            int nextCode = lastCode + 1;
            return nextCode.ToString();
        }
    }


}
