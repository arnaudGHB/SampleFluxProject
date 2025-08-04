
using CBS.APICaller.Helper.LoginModel.Authenthication;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using CBS.APICaller.Helper;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using DocumentFormat.OpenXml.Office2016.Excel;

namespace CBS.TransactionManagement.Helper
{
    public abstract class APICallHelper
    {

        public static async Task<LoginDto> AuthenthicationFromIdentityServer(PathHelper _pathHelper)
        {

            AuthRequest auth = new AuthRequest(_pathHelper.UserName, _pathHelper.Password);
            var ApiCallerHelper = new ApiCallerHelper(_pathHelper.IdentityServerBaseUrl);
            var responseobj = await ApiCallerHelper.PostAsync<LoginDto>(_pathHelper.AuthenthicationUrl, auth);
            return responseobj;
        }


        public static async Task<CustomerMapper> GetCustomerMapperAsync(string baseUrl,string EndPoint, string customerid,string token)
        {
            var ApiCallerHelper = new ApiCallerHelper(baseUrl, token);
            var responseobj = await ApiCallerHelper.GetAsyncMember<CustomerMapper>(string.Format(EndPoint, customerid));
            return responseobj;
        }
        public static async Task<List<CustomerMapper>> GetMembersWithMatriculeByBranch(string baseUrl, string EndPoint, string customerid, string token)
        {
            var ApiCallerHelper = new ApiCallerHelper(baseUrl, token);
            var responseobj = await ApiCallerHelper.GetAsyncMember<List<CustomerMapper>>(string.Format(EndPoint, customerid));
            return responseobj;
        }
        public static async Task<T> GetCustomers<T>(string baseUrl, string EndPoint, string token)
        {
            var ApiCallerHelper = new ApiCallerHelper(baseUrl, token);
            var responseobj = await ApiCallerHelper.GetAsync<T>(string.Format(EndPoint));
            return responseobj;
        }



        public static async Task<LoginDto> AuthenthicationFromIdentityServer(PathHelper _pathHelper, string username, string password)
        {

            AuthRequest auth = new AuthRequest(username, password);
            var ApiCallerHelper = new ApiCallerHelper(_pathHelper.IdentityServerBaseUrl);
            var responseobj = await ApiCallerHelper.PostAsync<LoginDto>(_pathHelper.AuthenthicationUrl, auth);
            return responseobj;
        }
         
        public static async Task<T> GetCustomer<T>(PathHelper pathHelper, string customerid,string token)
        {
            var ApiCallerHelper = new ApiCallerHelper(pathHelper.CustomerBaseUrl, token);
            var responseObj = await ApiCallerHelper.GetAsync<T>(string.Format(pathHelper.CustomerUrlGet, customerid));
            return responseObj;
        }
        public static async Task<T> ValidateCustomerPin<T>(PathHelper _pathHelper,string stringifyJsonObject, string token)
        {
            var ApiCallerHelper = new ApiCallerHelper(_pathHelper.CustomerBaseUrl, token);
            var responseobj = await ApiCallerHelper.PostAsync<T>(_pathHelper.PinValidation, stringifyJsonObject);
            return responseobj;
        }
        public static async Task<T> PostData<T>(string baseURL,string url, string stringifyJsonObject, string token)
        {
            var ApiCallerHelper = new ApiCallerHelper(baseURL, token);
            var responseobj = await ApiCallerHelper.PostAsync<T>(url, stringifyJsonObject);
            return responseobj;
        }
        
        public static async Task<T> GetData<T>(string baseURL,string url, string token)
        {
            var ApiCallerHelper = new ApiCallerHelper(baseURL, token);
            var responseobj = await ApiCallerHelper.GetAsync<T>(url);
            return responseobj;
        }
        public static async Task<T> GetAsynch<T>(string baseURL, string endUrl, string token)
        {
            var ApiCallerHelper = new ApiCallerHelper(baseURL, token);
            var responseObj = await ApiCallerHelper.GetAsync<T>(endUrl);
            return responseObj;
        }
        public static async Task<T> ActivateMemberShip<T>(PathHelper _pathHelper, string token, string stringifyJsonObject, string endPoint)
        {
            var ApiCallerHelper = new ApiCallerHelper(_pathHelper.CustomerBaseUrl, token);
            var responseobj = await ApiCallerHelper.PutAsync<T>(endPoint, stringifyJsonObject);
            return responseobj;
        }
        public static async Task<T> GetObjectById<T>(string baseURL, string endUrl, string token,string id)
        {
            var ApiCallerHelper = new ApiCallerHelper(baseURL, token);
            var responseObj = await ApiCallerHelper.GetAsync<T>(string.Format(endUrl, id));
            return responseObj;
        }
        public static async Task<T> GetBranch<T>(PathHelper pathHelper, string branchid, string token)
        {
            var ApiCallerHelper = new ApiCallerHelper(pathHelper.BankConfigurationBaseUrl, token);
            var responseObj = await ApiCallerHelper.GetAsync<T>(string.Format(pathHelper.BranchGetBranchUrlEndPointUrl, branchid));
            return responseObj;
        }

        public static async Task<T> AccountingAPICalls<T>(PathHelper _pathHelper, string token,string stringifyJsonObject,string endPoint)
        {
            var ApiCallerHelper = new ApiCallerHelper(_pathHelper.AccountingBaseURL, token);
            var responseobj = await ApiCallerHelper.PostAsync<T>(endPoint, stringifyJsonObject);
            return responseobj;
        }
        public static async Task<T> LoanRepayment<T>(PathHelper _pathHelper, string token, string stringifyJsonObject, string endPoint)
        {
            var ApiCallerHelper = new ApiCallerHelper(_pathHelper.LoanBaseURL, token);
            var responseobj = await ApiCallerHelper.PostAsync<T>(endPoint, stringifyJsonObject);
            return responseobj;
        }
        public static async Task<T> SMSAPICalls<T>(PathHelper _pathHelper, string token, string stringifyJsonObject, string endPoint)
        {
            var ApiCallerHelper = new ApiCallerHelper(_pathHelper.SMSBaseURL, token);
            var responseobj = await ApiCallerHelper.PostAsync<T>(endPoint, stringifyJsonObject);
            return responseobj;
        }

        public static async Task AuditLogger<T>(string userName, string action, T objectToSerialize, string detailMessage, string level, int statuscode, string token,string corollationid=null)
        where T : class
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string baseUrl = configuration.GetSection("AuditTrails:BaseUrl").Value;
            string auditTrailEndpoint = configuration.GetSection("AuditTrails:AuditTrailEndpoint").Value;
            string microserviceName = configuration.GetSection("AuditTrails:MicroserviceName").Value;
            string stringifyObject = objectToSerialize != null ? JsonConvert.SerializeObject(objectToSerialize) : null;
            AuditTrailLogger logger = new AuditTrailLogger(action, userName, microserviceName, stringifyObject, detailMessage, level, statuscode, corollationid);

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
        public static async Task AuditLogger<T>(string action, T objectToSerialize, string detailMessage, string level, int statuscode,string corollationid)
            where T : class
        {
            // Assuming you have access to HttpContext
            var httpContext = new HttpContextAccessor().HttpContext;

            // Retrieve userName and token from session
            string userName = httpContext.Session?.GetString("FullName"); // Adjust key as needed
            string token = httpContext.Session?.GetString("Token"); // Adjust key as needed

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
