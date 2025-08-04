
﻿using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.APICaller.Helper;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace CBS.CUSTOMER.HELPER.Helper
{
    public abstract class APICallHelper
    {

        public static async Task AuditLogger<T>(string userName, string action, T objectToSerialize, string detailMessage, string level, int statuscode, string token,string corolationid=null)
          where T : class
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string baseUrl = configuration.GetSection("AuditTrails:BaseUrl").Value;
            string auditTrailEndpoint = configuration.GetSection("AuditTrails:AuditTrailEndpoint").Value;
            string microserviceName = configuration.GetSection("AuditTrails:MicroserviceName").Value;
            string stringifyObject = objectToSerialize != null ? JsonConvert.SerializeObject(objectToSerialize) : null;
            AuditTrailLogger logger = new AuditTrailLogger(action, userName, microserviceName, stringifyObject, detailMessage, level, statuscode, corolationid);

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
        public static async Task AuditLogger<T>(string action, T objectToSerialize, string detailMessage, string level, int statuscode,string corolationid=null)
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
            AuditTrailLogger logger = new AuditTrailLogger(action, userName, microserviceName, stringifyObject, detailMessage, level, statuscode, corolationid);

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
        public static async Task<LoginDto> AuthenthicationFromIdentityServer(PathHelper _pathHelper)
          {

              AuthRequest auth = new AuthRequest(_pathHelper.UserName, _pathHelper.Password);
              var ApiCallerHelper = new ApiCallerHelper(_pathHelper.IdentityServerBaseUrl);
              var responseobj = await ApiCallerHelper.PostAsync<LoginDto>(_pathHelper.AuthenthicationUrl, auth);
              return responseobj;
          }

        public static async Task<T> PostData<T>(string baseURL, string url, string stringifyJsonObject, string token)
        {
            var ApiCallerHelper = new ApiCallerHelper(baseURL, token);
            var responseobj = await ApiCallerHelper.PostAsync<T>(url, stringifyJsonObject);
            return responseobj;
        } 
        
        public static async Task<T> GetData<T>(string baseURL, string url, string token)
        {
            var ApiCallerHelper = new ApiCallerHelper(baseURL, token);
            var responseobj = await ApiCallerHelper.GetAsync<T>(url);
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

        public static async Task<T> CreateAccount<T>(string BaseURL, string Uri, object data, string bearerToken)
        {

            // var login =await AuthenthicationFromIdentityServer(_pathHelper);
            var ApiCallerHelper = new ApiCallerHelper(BaseURL, bearerToken);
            var responseobj = await ApiCallerHelper.PostAsync<T>(Uri, data);
            return responseobj;
        }


        public static async Task<T> AuthenthicationFromIdentityServer<T>(PathHelper _pathHelper,string bearerToken, string Uri, object data)
        {

             // var login =await AuthenthicationFromIdentityServer(_pathHelper);
            var ApiCallerHelper = new ApiCallerHelper(_pathHelper.IdentityServerBaseUrl, bearerToken);
            var responseobj = await ApiCallerHelper.PostAsync<T>(Uri, data);
            return responseobj;
        }

        public static async Task<T> WithOutAuthenthicationFromOtherServer<T>(string Uri, object data)
        {


            var ApiCallerHelper = new ApiCallerHelper(null, null);
            var responseobj = await ApiCallerHelper.PostAsync<T>(Uri, data);
            return responseobj;
        } 
        
        public static async Task<T> WithAuthenthicationFromOtherServer<T>(string bearerToken, string Uri, object data)
        {

          //  var login = await AuthenthicationFromIdentityServer(_pathHelper);
            var ApiCallerHelper = new ApiCallerHelper(null, bearerToken);
            var responseobj = await ApiCallerHelper.PostAsync<T>(Uri, data);
            return responseobj;
        }




    }
}
