using CBS.APICaller.Helper.LoginModel.Authenthication;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using CBS.APICaller.Helper;
using CBS.BankMGT.Data;

namespace CBS.BankMGT.Helper
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
        public static async Task<LoginDto> AuthenthicationFromIdentityServer(PathHelper _pathHelper, string username, string password)
        {

            AuthRequest auth = new AuthRequest(username, password);
            var ApiCallerHelper = new ApiCallerHelper(_pathHelper.IdentityServerBaseUrl);
            var responseobj = await ApiCallerHelper.PostAsync<LoginDto>(_pathHelper.AuthenthicationUrl, auth);
            return responseobj;
        }
        public static async Task<bool> SaveUploadData(DocumentUploaded d,string token)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json")
              .Build();
            var req = new CustomerApiRequest { baseUrl = d.BaseUrl, documentName = d.DocumentName, documentType = d.DocumentType, extension = d.Extension, id = d.OperationId,urlPath=d.UrlPath };
            string baseUrl = configuration.GetSection("CustomerService:BaseUrl").Value;
            string endpont = configuration.GetSection("CustomerService:EndPoint").Value;
            var ApiCallerHelper = new ApiCallerHelper(baseUrl, token);
            var responseobj = await ApiCallerHelper.PutAsync<ServiceResponse<bool>>(endpont, req);
            return responseobj.Data;
        }
    }
}
