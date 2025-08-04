using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.APICaller.Helper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace CBS.Communication.Helper.Helper
{
    public abstract class APICallHelper
    {
        protected static Dictionary<string, string> query_string = new Dictionary<string, string>();

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


        public static async Task<LoginDto> AuthenthicationFromIdentityServer(PathHelper _pathHelper)
        {

            AuthRequest auth = new AuthRequest(_pathHelper.UserName, _pathHelper.Password);
            var ApiCallerHelper = new ApiCallerHelper(_pathHelper.IdentityServerBaseUrl);
            var responseobj = await ApiCallerHelper.PostAsync<LoginDto>(_pathHelper.AuthenthicationUrl, auth);
            return responseobj;
        }


        /*    public static async Task<LoginDto> AuthenthicationFromIdentityServer(PathHelper _pathHelper, string Uri, string username, string password)
             {

                 AuthRequest auth = new AuthRequest(username, password);
                 var ApiCallerHelper = new ApiCallerHelper(_pathHelper.IdentityServerBaseUrl);
                 var responseobj = await ApiCallerHelper.PostAsync<LoginDto>(_pathHelper.AuthenthicationUrl, auth);
                 return responseobj;
             }*/

        public static async Task<T> AuthenthicationFromIdentityServer<T>(PathHelper _pathHelper, string Uri, object data, string bearerToken)
        {

            // var login =await AuthenthicationFromIdentityServer(_pathHelper);
            var ApiCallerHelper = new ApiCallerHelper(_pathHelper.IdentityServerBaseUrl, bearerToken);
            var responseobj = await ApiCallerHelper.PostAsync<T>(Uri, data);
            return responseobj;
        }


        public static async Task<T> AuthenthicationFromIdentityServer<T>(PathHelper _pathHelper, string Uri, object data)
        {

            var login = await AuthenthicationFromIdentityServer(_pathHelper);
            var ApiCallerHelper = new ApiCallerHelper(_pathHelper.IdentityServerBaseUrl, login.bearerToken);
            var responseobj = await ApiCallerHelper.PostAsync<T>(Uri, data);
            return responseobj;
        }

        public static async Task<T> WithOutAuthenthicationFromOtherServer<T>(string Uri, object data)
        {


            var ApiCallerHelper = new ApiCallerHelper(null, null);
            var responseobj = await ApiCallerHelper.PostAsync<T>(Uri, data);
            return responseobj;
        }

        public static async Task<T> GetRequestWithAuthentication<T>(string baseURL, string url,string token)
        {
            var ApiCallerHelper = new ApiCallerHelper(baseURL, token);
            var responseobj = await ApiCallerHelper.GetAsync<T>(url);
            return responseobj;
        }

        public static async Task<T> GetRequestWithoutAuthentication<T>(string Uri)
        {


            var ApiCallerHelper = new ApiCallerHelper(null, null);
            var responseobj = await ApiCallerHelper.GetAsync<T>(Uri);
            return responseobj;
        }

        public static  void AddQueryStringParam(string key, string value)
        {
            query_string.Add(key, value);
        }
        
        
        public static  void ClearQueryStringParams()
        {
            query_string.Clear();
        }

        public static string MakeFinalUrlWithOutResourceUrl(string endpoint)
        {
            string result = endpoint;
            if (query_string.Count == 0) return result;

            result += "?";
            foreach (KeyValuePair<string, string> entry in query_string)
            {
                result += UrlEncode(entry.Key) + '=' + UrlEncode(entry.Value) + "&";
            }
            result = result.Remove(result.Length - 1);
            return result;
        }

        private static string UrlEncode(string str)
        {
            return Uri.EscapeDataString(str);
        }

        public static Task GetRequestWithAuthentication<T>(object customerUrl, object getCMoneyMembersActivationByIdPath)
        {
            throw new NotImplementedException();
        }

        public static Task GetRequestWithAuthentication<T>(object customerUrl, string getCMoneyMembersActivationByIdPath)
        {
            throw new NotImplementedException();
        }
    }
}
