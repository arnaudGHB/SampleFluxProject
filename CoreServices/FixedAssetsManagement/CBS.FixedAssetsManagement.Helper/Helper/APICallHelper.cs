using CBS.FixedAssetsManagement.Helper.DataModel;
using CBS.APICaller.Helper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using CBS.FixedAssetsManagement.Data;

using System.Net.Http;
using CBS.APICaller.Helper.LoginModel.Authenthication;

namespace CBS.FixedAssetsManagement.Helper
{
    public abstract class APICallHelper
    {

        public static async Task AuditLogger<T>(string userName, string action, T objectToSerialize, string detailMessage, string level, int statusCode, string token)
        where T : class
        {
            // Environment variable-based configuration
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            // Assuming you set the environment variables LOGGING_AUDITTRAILS_BASEURL, etc.
            string baseUrl = configuration["Logging:AuditTrails:BaseUrl"];
            string auditTrailEndpoint = configuration["Logging:AuditTrails:AuditTrailEndpoint"];
            string microserviceName = configuration["Logging:AuditTrails:MicroserviceName"];

            // Serialize the object
            string stringifyObject = objectToSerialize != null ? JsonConvert.SerializeObject(objectToSerialize) : null;

            // Create AuditTrailLogger instance
            AuditTrailLogger logger = new AuditTrailLogger(action, userName, microserviceName, stringifyObject, detailMessage, level, statusCode);

            // Call API with ApiCallerHelper
            var apiCallerHelper = new ApiCallerHelper(baseUrl, token);
            await apiCallerHelper.PostAsync(auditTrailEndpoint, logger);
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

  

    }
}