using CBS.APICaller.Helper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

using CBS.APICaller.Helper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;


namespace CBS.NLoan.Helper.Helper
{
    public abstract class APICallHelper
    {

        public static async Task<T> GetCustomer<T>(PathHelper pathHelper, string customerid, string token)
        {
            var ApiCallerHelper = new ApiCallerHelper(pathHelper.CustomerBaseUrl, token);
            var responseObj = await ApiCallerHelper.GetAsync<T>(string.Format(pathHelper.CustomerUrlGet, customerid));
            return responseObj;
        }

        public static async Task<T> PostRequest<T>(PathHelper _pathHelper, string token, string stringifyJsonObject, string endPoint)
        {
            var ApiCallerHelper = new ApiCallerHelper(_pathHelper.TransactionBaseUrl, token);
            var responseobj = await ApiCallerHelper.PostAsync<T>(endPoint, stringifyJsonObject);
            return responseobj;
        }
        public static async Task<T> PostRequest<T>(string baseUrl, string token, string stringifyJsonObject, string endPoint)
        {
            var ApiCallerHelper = new ApiCallerHelper(baseUrl, token);
            var responseobj = await ApiCallerHelper.PostAsync<T>(endPoint, stringifyJsonObject);
            return responseobj;
        }
        public static async Task<T> GetCustomerAccounts<T>(PathHelper pathHelper, string customerid, string token)
        {
            var ApiCallerHelper = new ApiCallerHelper(pathHelper.TransactionBaseUrl, token);
            var responseObj = await ApiCallerHelper.GetAsync<T>(string.Format(pathHelper.GetAccountByCustomerIDURL, customerid));
            return responseObj;
        }
        public static async Task<T> GetBranch<T>(PathHelper pathHelper, string branchid, string token)
        {
            var ApiCallerHelper = new ApiCallerHelper(pathHelper.BankConfigurationBaseUrl, token);
            var responseObj = await ApiCallerHelper.GetAsync<T>(string.Format(pathHelper.BranchGetBranchUrlEndPointUrl, branchid));
            return responseObj;
        }

        public static async Task<T> AccountingAPICalls<T>(PathHelper _pathHelper, string token, string stringifyJsonObject, string endPoint)
        {
            var ApiCallerHelper = new ApiCallerHelper(_pathHelper.AccountingBaseURL, token);
            var responseobj = await ApiCallerHelper.PostAsync<T>(endPoint, stringifyJsonObject);
            return responseobj;

        }
        public static async Task<T> SMSAPICalls<T>(PathHelper _pathHelper, string token, string stringifyJsonObject, string endPoint)
        {
            var ApiCallerHelper = new ApiCallerHelper(_pathHelper.SMSBaseURL, token);
            var responseobj = await ApiCallerHelper.PostAsync<T>(endPoint, stringifyJsonObject);
            return responseobj;
        }

        public static async Task<T> GetEconomicActivities<T>(PathHelper pathHelper, string token)
        {
            var ApiCallerHelper = new ApiCallerHelper(pathHelper.BankConfigurationBaseUrl, token);
            var responseObj = await ApiCallerHelper.GetAsync<T>(pathHelper.EconomicActivitiesEndPointURL);
            return responseObj;
        }



       
        public static async Task<T> AuthenthicationAuto<T>(PathHelper _pathHelper,string stringifyJsonObject)
        {
            var ApiCallerHelper = new ApiCallerHelper(_pathHelper.IdentityBaseUrl);
            var responseobj = await ApiCallerHelper.PostAsync<T>(_pathHelper.LoginURL, stringifyJsonObject);
            return responseobj;
        }

        public static async Task AuditLogger<T>(string userName, string action, T objectToSerialize, string detailMessage, string level, int statuscode, string token)
        where T : class
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string baseUrl = configuration.GetSection("AuditTrails:BaseUrl").Value;
            string auditTrailEndpoint = configuration.GetSection("AuditTrails:AuditTrailEndpoint").Value;
            string microserviceName = configuration.GetSection("AuditTrails:MicroserviceName").Value;
            string stringifyObject = objectToSerialize != null ? JsonConvert.SerializeObject(objectToSerialize) : null;
            AuditTrailLogger logger = new AuditTrailLogger(action, userName, microserviceName, stringifyObject, detailMessage, level, statuscode);

            var ApiCallerHelper = new ApiCallerHelper(baseUrl, token);
            await ApiCallerHelper.PostAsync(auditTrailEndpoint, logger);
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
        public static async Task AuditLogger<T>(string action, T objectToSerialize, string detailMessage, string level, int statuscode, string corollationid)
            where T : class
        {
            // Assuming you have access to HttpContext
            var httpContext = new HttpContextAccessor().HttpContext;

            // Retrieve userName and token from session
            string userName = httpContext.Session.GetString("FullName"); // Adjust key as needed
            string token = httpContext.Session.GetString("Token"); // Adjust key as needed

            // Build configuration settings from appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Retrieve API endpoint details from configuration
            string baseUrl = configuration.GetSection("AuditTrails:BaseUrl").Value;
            string auditTrailEndpoint = configuration.GetSection("AuditTrails:AuditTrailEndpoint").Value;
            string microserviceName = configuration.GetSection("AuditTrails:MicroserviceName").Value;

            // Serialize the object to JSON, handling potential nulls
            string stringifyObject = objectToSerialize != null ? JsonConvert.SerializeObject(objectToSerialize) : null;

            // Create an instance of AuditTrailLogger with the provided details
            AuditTrailLogger logger = new AuditTrailLogger(action, userName, microserviceName, stringifyObject, detailMessage, level, statuscode, corollationid);

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
        public static async Task AuditLogger<T>(
                string action,
                T objectToSerialize,
                string detailMessage,
                string level,
                int statuscode,
                string correlationId,
                string userName,
                string token)
                where T : class
        {
            try
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                // Retrieve API endpoint details from configuration
                string baseUrl = configuration.GetSection("AuditTrails:BaseUrl").Value;
                string auditTrailEndpoint = configuration.GetSection("AuditTrails:AuditTrailEndpoint").Value;
                string microserviceName = configuration.GetSection("AuditTrails:MicroserviceName").Value;

                // Serialize the object to JSON, handling potential nulls
                string stringifyObject = objectToSerialize != null ? JsonConvert.SerializeObject(objectToSerialize) : null;

                // Create the AuditTrailLogger instance
                AuditTrailLogger logger = new AuditTrailLogger(action, userName, microserviceName, stringifyObject, detailMessage, level, statuscode, correlationId);

                // Send the audit log via API
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
}
