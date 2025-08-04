using CBS.SystemConfiguration.Helper.DataModel;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using CBS.SystemConfiguration.Helper;
using CBS.SystemConfiguration.Data;
using CBS.APICaller.Helper;
 
using CBS.APICaller.Helper.LoginModel.Authenthication;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.IIS.Core;
using System.Collections.Generic;
using System.Diagnostics;

namespace CBS.SystemConfiguration.Helper
{
    public abstract class APICallHelper
    {
      


        public static async Task AuditLogger<T>(string userName, string action, T objectToSerialize, string detailMessage, string level, int statuscode, string token)
            where T : class
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string baseUrl = configuration.GetSection("Logging:AuditTrails:BaseUrl").Value;
            string auditTrailEndpoint = configuration.GetSection("Logging:AuditTrails:AuditTrailEndpoint").Value;
            string microserviceName = configuration.GetSection("Logging:AuditTrails:MicroserviceName").Value;
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
        public static async Task AuditLogger<T>(string action, T objectToSerialize, string detailMessage, string level, int statuscode)
            where T : class
        {
            // Assuming you have access to HttpContext
            var httpContext = new HttpContextAccessor().HttpContext;

            // Retrieve userName and token from session
            string userName = httpContext.Session.GetString("UserName"); // Adjust key as needed
            string token = httpContext.Session.GetString("Token"); // Adjust key as needed

            // Build configuration settings from appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            // Retrieve API endpoint details from configuration
            string baseUrl = configuration.GetSection("Logging:AuditTrails:BaseUrl").Value;
            string auditTrailEndpoint = configuration.GetSection("Logging:AuditTrails:AuditTrailEndpoint").Value;
            string microserviceName = configuration.GetSection("Logging:AuditTrails:MicroserviceName").Value;
            string stringifyObject = objectToSerialize != null ? JsonConvert.SerializeObject(objectToSerialize) : null;
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


        public static async Task<Branch> GetBankInfos(PathHelper _pathHelper, UserInfoToken userInfoToken,string branchId, string token)
        {
            try
            {
                //var login = await AuthenthicationFromIdentityServer(_pathHelper);
                string url = string.Format(_pathHelper.BankManagement_GetBranchInfoEndPoint, branchId);
                var ApiCallerHelper = new ApiCallerHelper(_pathHelper.BankManagementBaseUrl, token);
                var responseobj = await ApiCallerHelper.GetAsync<Branch>(url);
                return responseobj;
            }
            catch (Exception ex)
            {
                await AuditLogger<Branch>(userInfoToken.Email, "GetBranchInfo", null, ex.Message, "Error", 500, userInfoToken.Token);
                throw (ex);
            }
        
        }

        public static async Task<List<Branch>> GetAllBranchInfos(PathHelper _pathHelper, UserInfoToken userInfoToken)
        {
            try
            {
                //var login = await AuthenthicationFromIdentityServer(_pathHelper);
                string url = string.Format(_pathHelper.BankManagement_GetAllBranchInfo);
                var ApiCallerHelper = new ApiCallerHelper(url, userInfoToken.Token);
                var responseobj = await ApiCallerHelper.GetAsync<ServiceResponse<List<Branch>>>(url);
                return responseobj.Data;
            }
            catch (Exception ex)
            {
                await AuditLogger<List<Branch>>(userInfoToken.Email, "GetBranchInfo", null, ex.Message, "Error", 500, userInfoToken.Token);
                throw (ex);
            }

        }


        public static async Task<T> PostCashInOrCashOperation<T>(PathHelper _pathHelper, string token, string stringifyJsonObject, string endPoint)
        {
            var ApiCallerHelper = new ApiCallerHelper(_pathHelper.BankTransactionCashINAndCashOutURL, token);
        
            var responseobj = await ApiCallerHelper.PostAsync<T>(endPoint, stringifyJsonObject);
            return responseobj;

        }

        public static async Task<CallBackRespose> UploadExcelDocument(AddDocumentUploadedCommand command,string apiUrl,string token)
        {
            try
            {
                var additionalParams = new Dictionary<string, string>
                {
                    { "OperationID", command.OperationID },
                    { "DocumentId", command.DocumentId ?? "" },
                    { "DocumentType", command.DocumentType },
                    { "ServiceType", command.ServiceType },
                    { "CallBackBaseUrl", command.CallBackBaseUrl },
                    { "CallBackEndPoint", command.CallBackEndPoint },
                    { "RemoteFilePath", command.RemoteFilePath },
                    { "IsSynchronus", command.IsSynchronus.ToString() }
                };
                var ApiCallerHelper = new ApiCallerHelper(apiUrl, token);

                var response = await ApiCallerHelper.PostFilesAndParamsAsync<CallBackRespose>(apiUrl, additionalParams, command.FormFiles);

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading Document: {ex.Message}");
                throw ex;
            }
        }


        public static async Task<List<Branch>> GetAllBankInfos(PathHelper _pathHelper, UserInfoToken userInfoToken,   string token)
        {
            try
            {
                //var login = await AuthenthicationFromIdentityServer(_pathHelper);
                string url = string.Format(_pathHelper.BankManagement_GetAllBranchInfo);
                var ApiCallerHelper = new ApiCallerHelper(url, token);
                var responseobj = await ApiCallerHelper.GetAsync<List<Branch>>(url);
                return responseobj;
            }
            catch (Exception ex)
            {
                await AuditLogger<Branch>(userInfoToken.Email, "GetBranchInfo", null, ex.Message, "Error", 500, userInfoToken.Token);
                throw (ex);
            }


        }
 

 

    }
}