using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2016.Excel;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;

namespace CBS.TransactionManagement.Helper
{


    public static class BaseUtilities
    {
        private static readonly TimeZoneInfo DoualaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");

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
        public static string GenerateCustomerCode(string input, string branchCode)
        {
            // Convert input to an integer (assuming input is numeric)
            if (int.TryParse(input, out int number))
            {
                // Ensure the number is non-negative and format it to 7 digits
                if (number >= 0)
                {
                    string customerCode = $"{branchCode}{number.ToString("D7")}";
                    return customerCode;
                }
                else
                {
                    throw new ArgumentException("Input must be a non-negative integer.");
                }
            }
            else
            {
                throw new ArgumentException("Input must be a numeric string.");
            }
        }

        public static decimal RoundUpToOne(decimal value)
        {
            return Math.Max(1.0m, Math.Ceiling(value)); // Always rounds up to at least 1
        }

        // Overload for double
        public static double RoundUpToOne(double value)
        {
            return Math.Max(1.0, Math.Ceiling(value)); // Always rounds up to at least 1
        }

        public static string ToQueryString<T>(T obj)
        {
            var properties = from p in typeof(T).GetProperties()
                             where p.GetValue(obj, null) != null
                             select FormatProperty(p.Name, p.GetValue(obj));

            return string.Join("&", properties.Where(x => !string.IsNullOrEmpty(x)).ToArray());
        }

        private static string FormatProperty(string name, object value)
        {
            if (value is IEnumerable<string> stringCollection)
            {
                // Format string collections with each value treated as a separate query parameter
                return string.Join("&", stringCollection.Select(item => $"{name}={Uri.EscapeDataString(item)}"));
            }
            else if (value is IEnumerable<object> collection)
            {
                // Handle non-string collections as comma-separated values
                return $"{name}={Uri.EscapeDataString(string.Join(",", collection))}";
            }
            else if (value is string strValue)
            {
                // Ensure strings are properly encoded
                return $"{name}={Uri.EscapeDataString(strValue)}";
            }
            else if (value is DateTime dateTime)
            {
                // Format DateTime values in ISO 8601 format
                return $"{name}={Uri.EscapeDataString(dateTime.ToString("o"))}";
            }
            else
            {
                // Default handling for other types
                return $"{name}={Uri.EscapeDataString(value.ToString())}";
            }
        }

        public static string GetMessageFromJson(string input)
        {
            try
            {
                // Check if the input contains a JSON structure
                int jsonStartIndex = input.IndexOf("{");
                if (jsonStartIndex != -1)
                {
                    // Extract the JSON substring
                    string jsonPart = input.Substring(jsonStartIndex);

                    // Parse the JSON
                    var jsonObject = JObject.Parse(jsonPart);

                    // Find the "message" key
                    return FindMessage(jsonObject) ?? "No specific message found in the error response.";
                }

                return "No JSON found in the input string.";
            }
            catch (Exception ex)
            {
                return $"Error parsing JSON: {ex.Message}";
            }
        }

        private static string FindMessage(JToken token)
        {
            if (token == null) return null;

            // If the token is an object, look for the "message" key
            if (token.Type == JTokenType.Object)
            {
                foreach (var property in token.Children<JProperty>())
                {
                    if (property.Name.Equals("message", StringComparison.OrdinalIgnoreCase))
                    {
                        return property.Value.ToString();
                    }

                    // Recursively search nested objects
                    var result = FindMessage(property.Value);
                    if (!string.IsNullOrEmpty(result))
                    {
                        return result;
                    }
                }
            }
            // If the token is an array, search each element
            else if (token.Type == JTokenType.Array)
            {
                foreach (var item in token.Children())
                {
                    var result = FindMessage(item);
                    if (!string.IsNullOrEmpty(result))
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        public static string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
        }

        public static Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
        {
            {".txt", "text/plain"},
            {".pdf", "application/pdf"},
            {".doc", "application/vnd.ms-word"},
            {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
            {".xls", "application/vnd.ms-excel"},
            {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
            {".png", "image/png"},
            {".jpg", "image/jpeg"},
            {".jpeg", "image/jpeg"},
            {".gif", "image/gif"},
            {".csv", "text/csv"}
        };
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
        public static async Task LogAndAuditAsync(string message, object request, HttpStatusCodeEnum statusCode, LogAction logAction, LogLevelInfo logLevel, string userName, string token, string corrolationid = null)
        {
            // Call the AuditLogger method to record the action and associated details asynchronously.
            await APICallHelper.AuditLogger(logAction.ToString(), request, message, logLevel.ToString(), (int)statusCode, corrolationid, userName, token);
        }
        /// <summary>
        /// Performs a fuzzy match between two strings to determine if they sufficiently match based on a threshold.
        /// </summary>
        /// <param name="storedValue">The stored value in the database.</param>
        /// <param name="providedValue">The value provided for validation.</param>
        /// <param name="threshold">The minimum match ratio (0.0 to 1.0).</param>
        /// <returns>True if the match ratio meets or exceeds the threshold, otherwise false.</returns>
        public static bool IsFuzzyMatch(string storedValue, string providedValue, double threshold)
        {
            int maxLength = Math.Max(storedValue.Length, providedValue.Length);
            int editDistance = ComputeLevenshteinDistance(storedValue, providedValue);

            double similarity = 1.0 - ((double)editDistance / maxLength);
            return similarity >= threshold;
        }

        /// <summary>
        /// Computes the Levenshtein Distance (edit distance) between two strings.
        /// </summary>
        /// <param name="s1">The first string.</param>
        /// <param name="s2">The second string.</param>
        /// <returns>The number of edits required to transform one string into the other.</returns>
        public static int ComputeLevenshteinDistance(string s1, string s2)
        {
            int[,] dp = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; i++) dp[i, 0] = i;
            for (int j = 0; j <= s2.Length; j++) dp[0, j] = j;

            for (int i = 1; i <= s1.Length; i++)
            {
                for (int j = 1; j <= s2.Length; j++)
                {
                    int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;

                    dp[i, j] = Math.Min(
                        Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                        dp[i - 1, j - 1] + cost
                    );
                }
            }

            return dp[s1.Length, s2.Length];
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
        /// Generates a unique number up to 8 digits.
        /// </summary>
        /// <returns>Generated number.</returns>
        public static string GenerateUniqueNumber()
        {
            const string chars = "1234567890";
            return GenerateUniqueNumber(15, chars);
        }
        public static string GenerateAccountNumber(string customerId, string accountNUmber)
        {
            string account_number = $"{accountNUmber}{customerId}";
            return account_number;
        }
        //$"{accountNumber}{request.CustomerId}";
        public static string ComputeFileHash(IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                using (var sha256 = SHA256.Create())
                {
                    var hashBytes = sha256.ComputeHash(stream); // Compute SHA256 hash of the file.
                    return Convert.ToBase64String(hashBytes); // Convert the hash to a Base64 string.
                }
            }
        }
        // Safely get the value from a cell and convert to decimal
        public static decimal GetDecimalFromCell(IXLCell cell, decimal defaultValue = 0)
        {
            try
            {
                // Check if the cell has a numeric value before attempting conversion
                if (cell.IsEmpty() || !decimal.TryParse(cell.GetValue<string>(), out decimal result))
                {
                    return defaultValue; // Return default value if cell is empty or not convertible
                }
                return result; // Return the parsed value
            }
            catch (Exception ex)
            {
                // Log the error and return default value
                return defaultValue;
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
        public static string PartiallyEncryptAccountNumber(string accountNumber)
        {
            int visibleDigitsCount = 6;
            int encryptedDigitsCount = accountNumber.Length - visibleDigitsCount - 4; // 4 digits in the middle are left visible
            string visibleDigitsPrefix = accountNumber.Substring(0, 6); // First 6 visible digits
            string visibleDigitsSuffix = accountNumber.Substring(accountNumber.Length - 4); // Last 4 visible digits
            string encryptedDigits = new string('*', encryptedDigitsCount);
            string result = $"{visibleDigitsPrefix}{encryptedDigits}{visibleDigitsSuffix}";
            return result;
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
                result.Append(chars[b % (chars.Length)]);
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
        public static DateTime? ConvertStringToDate(string dateString, string format = "yyyy-MM-dd")
        {
            DateTime parsedDate;
            if (DateTime.TryParseExact(dateString, format,
                                       System.Globalization.CultureInfo.InvariantCulture,
                                       System.Globalization.DateTimeStyles.None,
                                       out parsedDate))
            {
                return parsedDate;
            }
            else
            {
                // Return null if the conversion fails
                return null;
            }
        }
        public static bool IsDateInRange(DateTime? dateToCheck, DateTime? startDate, DateTime? endDate)
        {
            if (!dateToCheck.HasValue || !startDate.HasValue || !endDate.HasValue)
            {
                return false;
            }

            return dateToCheck.Value.Date >= startDate.Value.Date && dateToCheck.Value.Date <= endDate.Value.Date;
        }
        public static void ValidateDates(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                throw new ArgumentException("Invalid date format for startDate or endDate. Both dates must be provided and valid.");
            }
            if (startDate > endDate)
            {
                throw new ArgumentException("Minimum value must be less than the maximum value and cannot be equal.");
            }
        }
        public static bool ValidateMinAndMaxValue(decimal minimum_value, decimal maximum_value, out string errorMessage)
        {
            if (minimum_value >= maximum_value)
            {
                errorMessage = "Minimum value must be less than the maximum value and cannot be equal.";
                return false;
            }

            errorMessage = null;
            return true;
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
        /// <summary>
        /// Converts a nullable DateTime from UTC to local time if it's in Unspecified kind.
        /// </summary>
        /// <param name="dateTime">The nullable DateTime value in UTC.</param>
        /// <returns>Local time representation of the input DateTime, or null if input is null.</returns>



        public static string ProcessTelephoneNumber(string tel)
        {
            // Remove spaces from the telephone number
            string cleanedTel = tel.Replace(" ", "");

            // Validate the telephone number has exactly 9 digits
            if (!Regex.IsMatch(cleanedTel, @"^\d{9}$"))
            {
                throw new ArgumentException("Invalid telephone number. It must contain exactly 9 digits.");
            }

            // Add country code 237 to the telephone number
            string processedTel = "237" + cleanedTel;

            return processedTel;
        }
        // Helper method to convert a given UTC DateTime to Douala time
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

        //UtcToDoualaTime
        //UtcNowToDoualaTime
        public static string FormatDateTime(DateTime dateTime)
        {
            var format = "dd/MM/yyyy HH:mm:ss";
            // Format DateTime using the specified format string (default: "dd/MM/yyyy HH:mm:ss")
            return dateTime.ToString(format);
        }

        public static string FormatCurrency(decimal amount, string currencySymbol = "XAF", int decimalPlaces = 1)
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}{1:N" + decimalPlaces + "}", currencySymbol, amount);
        }
        private static readonly string[] ones = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };

        private static readonly string[] teens = { "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };

        private static readonly string[] tens = { "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

        private static readonly string[] thousandsGroups = { "", "Thousand", "Million", "Billion" };


        public static string ConvertToWords(decimal amount)
        {
            if (amount == 0)
                return ones[0];

            if (amount < 0)
                return "minus " + ConvertToWords(Math.Abs(amount));

            string words = "";

            int thousandsGroup = 0;
            while (amount > 0)
            {
                int thousands = (int)(amount % 1000);
                if (thousands != 0)
                {
                    string thousandsWords = ConvertGroup(thousands);
                    if (thousandsGroup > 0)
                        thousandsWords += " " + thousandsGroups[thousandsGroup];

                    if (!string.IsNullOrEmpty(words))
                        words = thousandsWords + ", " + words;
                    else
                        words = thousandsWords;
                }

                amount /= 1000;
                thousandsGroup++;
            }

            return words;
        }

        private static string ConvertGroup(int number)
        {
            string groupWords = "";

            if (number >= 100)
            {
                groupWords += ones[number / 100] + " hundred";
                number %= 100;
                if (number > 0)
                    groupWords += " ";
            }

            if (number >= 20)
            {
                groupWords += tens[number / 10 - 2];
                number %= 10;
                if (number > 0)
                    groupWords += "-";
            }

            if (number > 0)
            {
                if (number < 10)
                    groupWords += ones[number];
                else
                    groupWords += teens[number - 10];
            }

            return groupWords;
        }
    }
    public class ReferenceNumberGenerator
    {
        private static DateTime _currentDate = DateTime.Today;
        private static Dictionary<string, int> _branchCounters = new Dictionary<string, int>();

        public static string GenerateReferenceNumber(string branchCode)
        {
            DateTime today = DateTime.Today;

            // Reset the counters if the date has changed
            if (today > _currentDate)
            {
                _currentDate = today;
                _branchCounters.Clear();
            }

            // Increment the counter for the specific branch
            if (!_branchCounters.ContainsKey(branchCode))
            {
                _branchCounters[branchCode] = 0;
            }

            _branchCounters[branchCode]++;

            // Format the reference number as "MMdd<BranchCode><Counter>"
            return $"{today:MMdd}{branchCode}{_branchCounters[branchCode]:D6}";
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
    public static class ShareValidator
    {
        public static void Validate(string sharingType = null, params decimal[] shares)
        {
            if (shares == null || shares.Length == 0)
            {
                throw new ArgumentException("Shares cannot be null or empty.");
            }

            decimal totalShare = shares.Sum();
            const decimal tolerance = 0.0001m; // Allow for minor precision errors

            if (Math.Abs(totalShare - 100m) > tolerance)
            {
                string typeInfo = string.IsNullOrEmpty(sharingType) ? "" : $"{sharingType} ";
                throw new InvalidOperationException($"The total share percentage of {typeInfo}must equal 100%. Current total is {totalShare}%.");
            }
        }
    }



    public static class OperationPrefixMapper
    {
        /// <summary>
        /// This dictionary maps different financial operation prefixes to their respective short codes.
        /// The acronyms are designed to be as simple and meaningful as possible:
        /// - Single-letter codes for common transactions (e.g., "D" for Deposit, "W" for Withdrawal).
        /// - Two or three-letter codes where necessary for clarity.
        /// - Unique and clear distinctions for **MTN Mobile Money (MMC) and Orange Money (OMC)**.
        /// </summary>
        private static readonly Dictionary<OperationPrefix, string> _prefixMappings = new()
{
    // Common Transactions
    { OperationPrefix.Withdrawal, "W" },  // Withdrawal
    { OperationPrefix.Deposit, "D" },  // Deposit
    { OperationPrefix.Transfer, "T" },  // Transfer
    { OperationPrefix.Loan_Disbursement, "L" },  // Loan Disbursement

    // Loan Transactions
    { OperationPrefix.Loan_Disbursement_Refinance, "LR" }, // Loan Refinance
    { OperationPrefix.CashIn_Loan_Repayment, "LP" }, // Loan Repayment (Cash In)
    { OperationPrefix.Deposit_Loan_Repayment, "LD" }, // Loan Repayment (Deposit)
    { OperationPrefix.Loan_Accrual_Interest, "LA" }, // Loan Accrual Interest

    // Remittance Transactions
    { OperationPrefix.Cash_W_Remittance, "RW" }, // Remittance Withdrawal
    { OperationPrefix.Cash_In_Remittance, "RC" }, // Remittance Cash In
    { OperationPrefix.Remittance, "R" }, // General Remittance

    // **MTN Mobile Money (MMC) Transactions**
    { OperationPrefix.MMC_In, "MMI" },  // MTN Mobile Money Cash In
    { OperationPrefix.MMC_Out, "MMO" }, // MTN Mobile Money Cash Out

    // **Orange Money (OMC) Transactions**
    { OperationPrefix.OMC_In, "OMI" },  // Orange Money Cash In
    { OperationPrefix.OMC_Out, "OMO" }, // Orange Money Cash Out

    // MomoKash Transactions
    { OperationPrefix.MomoKash_Collection, "MC" }, // MomoKash Collection
    { OperationPrefix.MomoKash_Collection_Loan_Repayment, "ML" }, // MomoKash Loan Repayment

    // TTP Transactions
    { OperationPrefix.TTP_Transfer, "TT" }, // TTP Transfer
    { OperationPrefix.TTP_Withdrawal, "TW" }, // TTP Withdrawal
    { OperationPrefix.TTP_Transfer_GAV, "TG" }, // TTP Transfer to GAV
    { OperationPrefix.TTP_Transfer_CMoney, "TC" }, // TTP Transfer to CMoney

    // Expense & Income
    { OperationPrefix.Expense, "E" }, // Expense
    { OperationPrefix.Income, "I" }, // Income
    { OperationPrefix.OtherCashIn_Expense, "OE" }, // Other Cash Expense
    { OperationPrefix.OtherCashIn_Income, "OI" }, // Other Cash Income

    // Vault & Cash Operations
    { OperationPrefix.Vault_Operation_Change, "V" }, // Vault Change Operation
    { OperationPrefix.Cash_Change_Operation_SubTill, "CS" }, // Cash Change (Sub Till)
    { OperationPrefix.Cash_Change_Operation_PrimaryTill, "CP" }, // Cash Change (Primary Till)

    // Other Transactions
    { OperationPrefix.Salary, "S" }, // Salary
    { OperationPrefix.Other, "O" }   // Other (Default case)
};


        /// <summary>
        /// Gets the prefix for an operation name and appends IB (Inter-Branch) or L (Local)
        /// </summary>
        public static string GetPrefix(OperationPrefix operationPrefix, bool isInterBranch)
        {
            string prefix = _prefixMappings.TryGetValue(operationPrefix, out var basePrefix) ? basePrefix : "O";
            return isInterBranch ? $"I{prefix}" : $"{prefix}";
        }
    }

}
