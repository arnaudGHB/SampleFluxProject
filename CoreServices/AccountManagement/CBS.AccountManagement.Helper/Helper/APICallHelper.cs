using CBS.AccountManagement.Helper.DataModel;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Dto;
using CBS.APICaller.Helper;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.IIS.Core;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Web.Administration;
using System;
using System.Net;

namespace CBS.AccountManagement.Helper
{
    public abstract class APICallHelper
    {
      

        public static async Task<LoginDto> AuthenthicationFromIdentityServer(PathHelper _pathHelper, string username, string password)
        {
            AuthRequest auth = new AuthRequest(username, password);
            var ApiCallerHelper = new ApiCallerHelper(_pathHelper.IdentityServerBaseUrl);
            var responseobj = await ApiCallerHelper.PostAsync<LoginDto>(_pathHelper.IdentityBaseUrl, auth);
            return responseobj;
        }
        //public static async Task<T> AuthenthicationFromIdentityServer<T>(PathHelper _pathHelper, string Uri, object data)
        //{

        //    var login = await AuthenthicationFromIdentityServer(_pathHelper);
        //    var ApiCallerHelper = new ApiCallerHelper(_pathHelper.IdentityServerBaseUrl, login.bearerToken);
        //    var responseobj = await ApiCallerHelper.PostAsync<T>(Uri, data);
        //    return responseobj;
        //}
        public static async Task<T> AuthenthicationAuto<T>(PathHelper _pathHelper, string stringifyJsonObject)
        {
            try
            {
                var ApiCallerHelper = new ApiCallerHelper(_pathHelper.IdentityBaseUrl);
                var responseobj = await ApiCallerHelper.PostAsync<T>(_pathHelper.LoginURL, stringifyJsonObject);
                return responseobj;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public static async Task<CallBackRespose> UploadTrialBalanceDocument(IFormFile formFile, string operationId, string documentType, string documentId, string serviceType, string callBackBaseUrl, string callBackEndPoint, string remoteFilePath, bool isSynchronous, string apiUrl, string token)
        {
            try
            {
                if (formFile == null)
                    throw new ArgumentNullException(nameof(formFile));

                // Create a new FormFileCollection and add the single file
                var formFiles = new FormFileCollection();
                formFiles.Add(formFile);

                var additionalParams = new Dictionary<string, string>
        {
            { "OperationID", operationId },
            { "DocumentId", documentId ?? "" },
            { "DocumentType", documentType },
            { "ServiceType", serviceType },
            { "CallBackBaseUrl", callBackBaseUrl },
            { "CallBackEndPoint", callBackEndPoint },
            { "RemoteFilePath", remoteFilePath },
            { "IsSynchronus", isSynchronous.ToString() }
        };

                var ApiCallerHelper = new ApiCallerHelper(apiUrl, token);
                var response = await ApiCallerHelper.PostFilesAndParamsAsync<CallBackRespose>(apiUrl, additionalParams, formFiles);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading document: {ex.Message}");
                throw;
            }
        }

        

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
        public static async Task SyetemAuditLogger<T>(string action, T objectToSerialize, string detailMessage, string level, int statuscode)
            where T : class
        {
            // Assuming you have access to HttpContext
            var httpContext = new HttpContextAccessor().HttpContext;



            // Build configuration settings from appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Retrieve SystemAuthentication from session
         
            string baseUrl = configuration.GetSection("FileUploadServer:IdentityServerBaseUrl").Value;
            string authInfo = configuration.GetSection("SystemAuthentication").Value;
            string userName = "BACKGROUND-SERVICE";

            //var user = JsonConvert.DeserializeObject<SystemUser>(authInfo);
            var serviceResponse = await APICallHelper.AuthenthicationAuto<ServiceResponse<LoginDto>>(new PathHelper(configuration), authInfo);
            string token = serviceResponse.Data.bearerToken;
       // Retrieve API endpoint details from configuration
           string AuthBaseUrl = configuration.GetSection("Logging:AuditTrails:BaseUrl").Value;
            string auditTrailEndpoint = configuration.GetSection("Logging:AuditTrails:AuditTrailEndpoint").Value;
            string microserviceName = configuration.GetSection("Logging:AuditTrails:MicroserviceName").Value;

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

        //private async static Task<string> GetTokenAsync(string? baseUrl, string? authInfo)
        //{

        //    //if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpirationTime)
        //    //{
        //    //    return _cachedToken;
        //    //}

        //    var client = _httpClientFactory.CreateClient();
        //    var request = new HttpRequestMessage(HttpMethod.Post, _configuration["AuthSettings:TokenUrl"]);

        //    var credentials = new
        //    {
        //        Username = _configuration["AuthSettings:Username"],
        //        Password = _configuration["AuthSettings:Password"]
        //    };

        //    request.Content = new StringContent(
        //        JsonConvert.Serialize(credentials),
        //        System.Text.Encoding.UTF8,
        //        "application/json");

        //    var response = await client.SendAsync(request);
        //    response.EnsureSuccessStatusCode();

        //    var tokenResponse = await JsonConvert.DeserializeObject<TokenResponse>(
        //        await response.Content.ReadAsStreamAsync());

        //    if (tokenResponse == null)
        //    {
        //        throw new Exception("Failed to deserialize token response");
        //    }

        //    _cachedToken = tokenResponse.Token;
        //    _tokenExpirationTime = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);

        //    return _cachedToken;
        //}




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

        public static async Task<T> TransactionTrackerAccountingAPICalls<T>(PathHelper _pathHelper, string token, string stringifyJsonObject,string Id, string endPoint)
        {
            var ApiCallerHelper = new ApiCallerHelper(_pathHelper.TransactionTrackerAccounting, token);
            var responseobj = await ApiCallerHelper.PutAsync<T>(string.Format(endPoint, Id), stringifyJsonObject);
            return responseobj;
        }


        public static async Task<List<TransactionTrackerDto>> GetAllTrackedTransaction(PathHelper _pathHelper, UserInfoToken userInfoToken, QueryParameters parameters)
        {
            try
            {
               
                //var login = await AuthenthicationFromIdentityServer(_pathHelper);
                string url = string.Format(_pathHelper.GetAllTrackedTransactionAtAccounting,parameters.ToString());
                var ApiCallerHelper = new ApiCallerHelper(url, userInfoToken.Token);
                var responseobj = await ApiCallerHelper.GetAsync<ServiceResponse<List<TransactionTrackerDto>>>(url);
                return responseobj.Data;
            }
            catch (Exception ex)
            {
                await AuditLogger<List<Branch>>(userInfoToken.Email, "GetBranchInfo", null, ex.Message, "Error", 500, userInfoToken.Token);
                throw (ex);
            }

        }
        public static async Task<ServiceResponse<User>> GetUser(PathHelper _pathHelper,string UserId,string Token)
        {
            
                       var ApiCallerHelper = new ApiCallerHelper(string.Format(_pathHelper.UserInfoUrl, UserId), Token);
            var responseobj = await ApiCallerHelper.GetAsync<ServiceResponse<User>>(string.Format(_pathHelper.UserInfoUrl, UserId));
            return responseobj;
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
        public static async Task<List<Branch>> GetAllLoanProducts(PathHelper _pathHelper, UserInfoToken userInfoToken)
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


        public static async Task<T> PostCashInOrCashdenominationOperation<T>(PathHelper _pathHelper, string token, string stringifyJsonObject, string endPoint)
        {
            var ApiCallerHelper = new ApiCallerHelper(_pathHelper.CashInOrCashOutDenomination, token);
        
            var responseobj = await ApiCallerHelper.PostAsync<T>(endPoint, stringifyJsonObject);
            return responseobj;

        }

        public static async Task<CallBackRespose> UploadExcelDocument(AddDocumentUploadedCommand command,string apiUrl,string token)
        {
            try
            {
              // Replace with your actual API endpoint

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
                Console.WriteLine($"Error uploading document: {ex.Message}");
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

        public static async Task  SendPrimaryTellerCallBackInfos(CashReplenishmentCallBackModel command, PathHelper _pathHelper, UserInfoToken userInfoToken)
        {
            try
            {
                var ApiCallerHelper = new ApiCallerHelper(_pathHelper.TellerProvisioning_EndPoint, userInfoToken.Token);
                await ApiCallerHelper.PostAsync(_pathHelper.TellerProvisioning_EndPoint, command);
               
            }
            catch (Exception ex)
            {
                await AuditLogger<Bank>(userInfoToken.Email, "SendCashReplenishmentCallBack", null, ex.Message, "Error", 500, userInfoToken.Token);
                throw (ex);
            }


        }

     
        public static async Task<ServiceResponse<List<TransactionTrackerDto>>> GetTransactions(PathHelper _pathHelper, UserInfoToken userInfoToken,string transactionId)
        {
            try
            {
                //var login = await AuthenthicationFromIdentityServer(_pathHelper);
                string url = string.Format(_pathHelper.TransactionManagement_GetAllTransaction, transactionId);
                var ApiCallerHelper = new ApiCallerHelper(url, userInfoToken.Token);
                var responseobj = await ApiCallerHelper.GetAsync<ServiceResponse<List<TransactionTrackerDto>>>(url);
                return responseobj;
            }
            catch (Exception ex)
            {
                await AuditLogger<Branch>(userInfoToken.Email, "GetBranchInfo", null, ex.Message, "Error", 500, userInfoToken.Token);
                throw (ex);
            }


        }

        public static async Task<TransactionTrack> GetUnReconciledTransactions(PathHelper _pathHelper, UserInfoToken userInfoToken, QueryParameters QueryParameter)
        {
            try
            {
              
                string url = string.Format(_pathHelper.GetAllTrackedTransactionAtAccounting, QueryParameter.ToString());

                var serviceResponse = await APICallHelper.AuthenthicationAuto<ServiceResponse<LoginDto>>(_pathHelper, _pathHelper.LoadUserInfo);
                string token = serviceResponse.Data.bearerToken;
                var ApiCallerHelper = new ApiCallerHelper(url, token);
                var responseobj = await ApiCallerHelper.GetAsync<TransactionTrack>(url);
                return responseobj;
            }
            catch (Exception ex)
            {
                await AuditLogger<Branch>(userInfoToken.Email, "GetBranchInfo", null, ex.Message, "Error", 500, userInfoToken.Token);
                throw (ex);
            }


        }

        public static async Task<ServiceResponse<DateTime>> GetAccountingDateOpen(PathHelper _pathHelper, UserInfoToken userInfoToken, string branchId)
        {
            try
            {
                //var login = await AuthenthicationFromIdentityServer(_pathHelper);
                string url = string.Format(_pathHelper.GetAccountingDateUrl, userInfoToken.IsHeadOffice ? branchId : userInfoToken.BranchId); 
                var ApiCallerHelper = new ApiCallerHelper(url, userInfoToken.Token);
                var responseobj = await ApiCallerHelper.GetAsync<ServiceResponse<DateTime>>(url);
                if (responseobj.StatusCode==(200))
                {
                    await BaseUtilities.LogAndAuditAsync(responseobj.Message, responseobj, HttpStatusCodeEnum.InternalServerError, LogAction.APICallHelper, LogLevelInfo.Error);
                    return responseobj;
                }
                else
                {
                    var ex= new Exception(responseobj.Message);
                  await  BaseUtilities.LogAndAuditAsync(responseobj.Message, ex, HttpStatusCodeEnum.InternalServerError, LogAction.Read, LogLevelInfo.Error);
               
                    throw new Exception(responseobj.Message);
                  
                }
 
            }
            catch (Exception ex)
            {
              
                throw (ex);
            }


        }
    }
}