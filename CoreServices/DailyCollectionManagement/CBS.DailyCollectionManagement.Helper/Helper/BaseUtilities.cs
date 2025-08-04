
using CBS.DailyCollectionManagement.Data.Dto;
using CBS.DailyCollectionManagement.Data.Entity;
using System.Security.Cryptography;
using System.Text;

namespace CBS.DailyCollectionManagement.Helper
{
    public static class BaseUtilities
    {
        public static long ConvertToLong(object value)
        {
            try
            {
                if (value == null)
                {
                    // Handle null value by returning 0 or a default value.
                    return 0;
                }

                // If the value is already a numeric type, convert directly.
                if (value is int || value is long || value is short || value is byte)
                {
                    return Convert.ToInt64(value);
                }

                if (value is decimal || value is double || value is float)
                {
                    // Round the value before converting to avoid truncation errors.
                    return Convert.ToInt64(Math.Round(Convert.ToDecimal(value)));
                }

                if (value is string)
                {
                    // Remove potential formatting characters like commas or currency symbols.
                    string cleanedValue = value.ToString().Replace(",", "").Replace("$", "").Trim();

                    if (decimal.TryParse(cleanedValue, out decimal parsedDecimal))
                    {
                        return Convert.ToInt64(Math.Round(parsedDecimal));
                    }
                    else
                    {
                        throw new FormatException("The string value cannot be parsed as a numeric value.");
                    }
                }

                // Attempt to convert any other object type if possible.
                if (value is IConvertible)
                {
                    return Convert.ToInt64(value);
                }

                // If none of the above conditions are met, throw an exception.
                throw new InvalidCastException("The provided value is not convertible to a long.");
            }
            catch (OverflowException)
            {
                // Handle values that are out of range for Int64.
                throw new OverflowException("The value is too large or too small to be converted to a long.");
            }
            catch (FormatException ex)
            {
                // Handle invalid formats.
                throw new FormatException($"Invalid format: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Catch any other unexpected exceptions.
                throw new Exception($"An error occurred during conversion: {ex.Message}");
            }
        }
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
        public static DateTime UtcToLocal()
        {
            var utcDateTime = DateTime.UtcNow;
            var localDateTime = utcDateTime.ToLocalTime();
            return localDateTime;
        }
        public static string GetFileSize(string filePath)
        {
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                return "File not found";
            }

            // Get the file info
            var fileInfo = new System.IO. FileInfo(filePath);

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
        /// Generates a unique number up to 15 digits.
        /// </summary>
        /// <returns>Generated number.</returns>
        public static string GenerateUniqueNumber()
        {
            const string chars = "1234567890";
            return GenerateUniqueNumber(15, chars);
        }
        /// <summary>
        /// Logs an action and audits it by sending the log information to the audit trail API.
        /// </summary>
        /// <param name="message">A descriptive message detailing the action being logged.</param>
        /// <param name="request">The request object associated with the action, which may contain relevant data.</param>
        /// <param name="statusCode">The HTTP status code representing the outcome of the action, defined in <see cref="HttpStatusCodeEnum"/>.</param>
        /// <param name="logAction">The specific action being logged, represented as a <see cref="LogAction"/> enumeration.</param>
        /// <param name="logLevel">The severity level of the log entry, represented as a <see cref="LogLevelInfo"/> enumeration.</param>
        public static async Task LogAndAuditAsync(string message, object request, HttpStatusCodeEnum statusCode, LogAction logAction, LogLevelInfo logLevel)
        {
            // Call the AuditLogger method to record the action and associated details asynchronously.
            await APICallHelper.AuditLogger(logAction.ToString(), request, message, logLevel.ToString(), (int)statusCode);
        }
        /// <summary>
        /// Logs an action and audits it by sending the log information to the audit trail API.
        /// </summary>
        /// <param name="message">A descriptive message detailing the action being logged.</param>
        /// <param name="request">The request object associated with the action, which may contain relevant data.</param>
        /// <param name="statusCode">The HTTP status code representing the outcome of the action, defined in <see cref="HttpStatusCodeEnum"/>.</param>
        /// <param name="logAction">The specific action being logged, represented as a <see cref="LogAction"/> enumeration.</param>
        /// <param name="logLevel">The severity level of the log entry, represented as a <see cref="LogLevelInfo"/> enumeration.</param>
        public static async Task AuthenticateLogAndAuditAsync(string message, object request, HttpStatusCodeEnum statusCode, LogAction logAction, LogLevelInfo logLevel)
        {
            // Call the SyetemAuditLogger method to record the action and associated details asynchronously.
            await APICallHelper.SyetemAuditLogger(logAction.ToString(), request, message, logLevel.ToString(), (int)statusCode);
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
        /// <summary>
        /// Gets all the inner exception messages
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string GetInnerExceptionMessages(Exception exception)
        {
            if (exception == null)
                return string.Empty;

            var innerMessage = "";

            if (exception.InnerException != null)
            {
                innerMessage = "\n" + GetInnerExceptionMessages(exception.InnerException);
            }

            return exception.Message + innerMessage;
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
        public static DateTime UtcToDoualaTime(this DateTime? dateTime)
        {
            if (dateTime.HasValue && dateTime.Value.Kind == DateTimeKind.Unspecified)
            {
                dateTime = DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc).ToLocalTime();
            }
            return dateTime.Value;
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
 
        public static void PrepareMonoDBDataForCreation(BaseEntity entity, UserInfoToken _userInfoToken,TrackerState trackerState)
        {
            //var branchId = entity.BranchId;
            var CreatedBy = entity.CreatedBy;
            var CreatedDate = entity.CreatedDate;
            var fullName = entity.FullName;
            if (trackerState.Equals(TrackerState.Created))
            {
                entity.CreatedDate = UtcNowToDoualaTime();
                entity.CreatedBy = _userInfoToken.Id;
                entity.ModifiedBy = "NOT-Define";
                entity.FullName = _userInfoToken.FullName;
                entity.ModifiedDate = default;
                entity.DeletedDate = default;
                entity.DeletedBy = "SYSTEM";
                entity.IsDeleted = false;
                //entity.BranchId = _userInfoToken.BranchId;
                //entity.BankId = "NoBankId";
            }
            else if (trackerState.Equals(TrackerState.Modified))
            {
                entity.ModifiedDate = UtcNowToDoualaTime();
                entity.ModifiedBy = _userInfoToken.Id;
                //entity.BranchId = branchId;
                entity.CreatedBy = CreatedBy;
                entity.CreatedDate = CreatedDate;
                entity.FullName = fullName;
            }
            else
            {
                entity.IsDeleted = true;
                entity.DeletedDate = UtcNowToDoualaTime();
                entity.DeletedBy = _userInfoToken.Id;
                //entity.BranchId = branchId;
                //entity.BranchId = branchId;
                entity.CreatedBy = CreatedBy;
                entity.CreatedDate = CreatedDate;
                entity.FullName = fullName;
            }
        }



        // Converts a given UTC DateTime to Douala time

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