using CBS.APICaller.Helper;
using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Helper
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
        public static void PrepareMonoDBDataForCreation(BaseEntity entity, UserInfoToken _userInfoToken, TrackerState trackerState)
        {
            string branchId="", CreatedBy=""; DateTime CreatedDate=UtcNowToDoualaTime();
            if (entity!=null)
            {
                  //branchId = entity.BranchId;
                  CreatedBy = entity.CreatedBy;
                  CreatedDate = entity.CreatedDate;
            }
           
            //var fullName = entity.fy;
            if (trackerState.Equals(TrackerState.Created))
            {
              
                entity.CreatedDate = UtcNowToDoualaTime();
                entity.CreatedBy = _userInfoToken.Id;
                entity.ModifiedBy = "NOT-Define";
                //entity.FullName = _userInfoToken.FullName;
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
                //entity.FullName = fullName;
            }
            else
            {
                entity.IsDeleted = true;
                entity.DeletedDate = UtcNowToDoualaTime();
                entity.DeletedBy = _userInfoToken.Id;
                //entity.BranchId = branchId;
 
                entity.CreatedBy = CreatedBy;
                entity.CreatedDate = CreatedDate;
                //entity.FullName = fullName;
            }
        }

        public static void PrepareMonoDBDataForCreationWithoutBranchId(BaseEntity entity, UserInfoToken _userInfoToken,string BBranchId, TrackerState trackerState)
        {
            string branchId = "", CreatedBy = ""; DateTime CreatedDate = UtcNowToDoualaTime();
            if (entity != null)
            {
                //branchId = entity.BranchId;
                CreatedBy = entity.CreatedBy;
                CreatedDate = entity.CreatedDate;
            }

            //var fullName = entity.fy;
            if (trackerState.Equals(TrackerState.Created))
            {

                entity.CreatedDate = UtcNowToDoualaTime();
                entity.CreatedBy = _userInfoToken.Id;
                entity.ModifiedBy = "NOT-Define";
                //entity.FullName = _userInfoToken.FullName;
                entity.ModifiedDate = default;
                entity.DeletedDate = default;
                entity.DeletedBy = "SYSTEM";
                entity.IsDeleted = false;
                //entity.BranchId = BBranchId;
                //entity.BankId = "NoBankId";
            }
            else if (trackerState.Equals(TrackerState.Modified))
            {
                entity.ModifiedDate = UtcNowToDoualaTime();
                entity.ModifiedBy = _userInfoToken.Id;
                //entity.BranchId = branchId;
                entity.CreatedBy = CreatedBy;
                entity.CreatedDate = CreatedDate;
                //entity.FullName = fullName;
            }
            else
            {
                entity.IsDeleted = true;
                entity.DeletedDate = UtcNowToDoualaTime();
                entity.DeletedBy = _userInfoToken.Id;
                //entity.BranchId = branchId;

                entity.CreatedBy = CreatedBy;
                entity.CreatedDate = CreatedDate;
                //entity.FullName = fullName;
            }
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
        public static DateTime ConvertStringToDateTime(string dateString)
        {
            // Define the date format expected in the input string
            string dateFormat = "dd-MM-yyyy";

            // Use DateTime.ParseExact to convert the string to DateTime
            DateTime dateTime;
            if (DateTime.TryParseExact(dateString, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {
                return dateTime; // Return the parsed DateTime object
            }
            else
            {
                throw new ArgumentException("Invalid date format or value");
                // Or handle the case where the input string doesn't match the specified format
            }
        }
        /// <summary>
        /// Converts a nullable DateTime from UTC to local time if it's in Unspecified kind.
        /// </summary>
        /// <param name="dateTime">The nullable DateTime value in UTC.</param>
        /// <returns>Local time representation of the input DateTime, or null if input is null.</returns>
        public static DateTime UtcToLocal(this DateTime? dateTime)
        {
            if (dateTime.HasValue && dateTime.Value.Kind == DateTimeKind.Unspecified)
            {
                dateTime = DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc).ToLocalTime();
            }
            return dateTime.Value;
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
            await  AuditLogger(logAction.ToString(), request, message, logLevel.ToString(), (int)statusCode);
        }

        /// <summary>
        /// Logs an action to the audit trail by sending log information to the specified audit trail API.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize for the audit log.</typeparam>
        /// <param name="action">A string representing the action being logged.</param>
        /// <param name="objectToSerialize">The object to be serialized and included in the audit log.</param>
        /// <param name="detailMessage">A detailed message providing context for the action being logged.</param>
        /// <param name="level">A string representing the log level (e.g., Information, Warning, Error).</param>
        /// <param name="statuscode">An integer representing the HTTP status code associated with the action.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task AuditLogger<T>(string action, T objectToSerialize, string detailMessage, string level, int statuscode)
            where T : class
        {
            // Assuming you have access to HttpContext
            var httpContext = new HttpContextAccessor().HttpContext;


            // Build configuration settings from appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Retrieve API endpoint details from configuration
            string baseUrl = configuration.GetSection("Logging:AuditTrails:BaseUrl").Value;
            string auditTrailEndpoint = configuration.GetSection("Logging:AuditTrails:AuditTrailEndpoint").Value;
            string microserviceName = configuration.GetSection("Logging:AuditTrails:MicroserviceName").Value;

            // Retrieve userName and token from session
            string userName = httpContext.Session.GetString("FullName"); // Adjust key as needed
            string token = httpContext.Session.GetString("Token"); // Adjust key as needed


            // Serialize the object to JSON, handling potential nulls
            string stringifyObject = objectToSerialize != null ? JsonConvert.SerializeObject(objectToSerialize) : null;

            // Create an instance of AuditTrailLogger with the provided details
            AuditTrailLogger logger = new AuditTrailLogger(action, userName, microserviceName, stringifyObject, detailMessage, level, statuscode);

            try
            {
                // Call the API to post the audit log
                var apiCallerHelper = new ApiCallerHelper(baseUrl, token);
                await apiCallerHelper.PostAsync(auditTrailEndpoint, logger);
            }
            catch (Exception ex)
            {
                // Log the exception using Debug for monitoring failures in the audit logging process
                Debug.WriteLine($"Failed to log audit trail: {ex.Message}");
            }
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
