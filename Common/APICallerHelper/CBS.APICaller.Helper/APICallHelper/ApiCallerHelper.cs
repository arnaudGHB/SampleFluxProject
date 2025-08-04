using CBS.APICaller.Helper.LoginModel.Authenthication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace CBS.APICaller.Helper
{
    /// <summary>
    /// Helper class for making API calls using HTTP methods (GET, POST, PUT, DELETE) with generic response handling.
    /// </summary>
    public class ApiCallerHelper : IDisposable
    {
        private readonly HttpClient _httpClient;

        string _baseURL = string.Empty;
        string _newbaseURL = string.Empty;
        /// <summary>
        /// Initializes a new instance of the ApiCallerHelper class.
        /// </summary>
        /// <param name="token">Optional authentication token.</param>
        /// <param name="baseUrl">Base URL of the API.</param>
        public ApiCallerHelper(string baseUrl = null, string token = null)
        {
            _httpClient = new HttpClient();

            if (!string.IsNullOrEmpty(baseUrl))
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
            }
            //if (HttpContext.Current != null && HttpContext.Current.Session != null)
            //{
            //    string newbaseUrl = GetBaseUrl(baseUrl);
            //    if (!string.IsNullOrEmpty(newbaseUrl))
            //    {
            //        _httpClient = new HttpClient();
            //        _httpClient.BaseAddress = new Uri(newbaseUrl);
            //        _baseURL = newbaseUrl;
            //        _newbaseURL = baseUrl;

            //    }
            //    else
            //    {
            //        throw new ArgumentException("Base URL cannot be null or empty.", nameof(newbaseUrl));
            //    }
            //}

            if (token != null)
            {
                AddAuthorizationHeader(_httpClient, token);
            }
        }



        /// <summary>
        /// Sends a GET request to the specified API endpoint and returns the response as the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the expected response data.</typeparam>
        /// <param name="apiUrl">API endpoint URL for the GET request.</param>
        /// <returns>Response data of type T.</returns>
        public async Task<T> GetAsync<T>(string apiUrl)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            return await HandleResponse<T>(response);
        }

        public async Task<T> GetAsyncMember<T>(string apiUrl)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            return await HandleResponseCustomer<T>(response);
        }

        //

        /// <summary>
        /// Sends a GET request to the specified API endpoint and returns the response as the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the expected response data.</typeparam>
        /// <param name="apiUrl">API endpoint URL for the GET request.</param>
        /// <returns>Response data of type T.</returns>

        /// <summary>
        /// Sends a POST request to the specified API endpoint with the provided data and returns the response as the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the expected response data.</typeparam>
        /// <param name="apiUrl">API endpoint URL for the POST request.</param>
        /// <param name="data">Data to be sent in the POST request.</param>
        /// <returns>Response data of type T.</returns>
        public async Task<T> PostAsync<T>(string apiUrl, string jsonData)
        {
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);
            return await HandleResponse<T>(response);
        }

  
        public async Task<T> PostAsync<T>(string apiUrl, object data)
        {
            string jsonData = JsonConvert.SerializeObject(data);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);
            return await HandleResponse<T>(response);
        }

        public async Task<T> PostAsync<T>(string apiUrl, HttpContent content)
        {
            //string jsonData = JsonConvert.SerializeObject(data);
            //StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);
            return await HandleResponse<T>(response);
        }

        public async Task PostAsync(string apiUrl, object data)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(data);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();

                // Optionally, you can log or perform other actions here if needed.
            }
            catch (Exception ex)
            {
                // Handle exceptions, log errors, or perform other error handling tasks.
            }
        }

       
        public async Task<T> PostFilesAndParamsAsync<T>(string apiUrl, Dictionary<string, string> additionalParams, IFormFileCollection files)
        {
            try
            {
                var formData = new MultipartFormDataContent();

                // Add additional parameters
                if (additionalParams != null)
                {
                    foreach (var param in additionalParams)
                    {
                        formData.Add(new StringContent(param.Value), param.Key);
                    }
                }

                // Add each file to the form data
                foreach (var file in files)
                {
                    var stream = file.OpenReadStream();
                    var fileContent = new StreamContent(stream);
                    formData.Add(fileContent, "AttachedFiles", file.FileName);
                }

            
                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, formData);
                return await HandleResponseCustomer<T>(response);
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request exceptions
                Console.WriteLine($"HTTP Request Error: {ex.Message}");
                throw (ex);
        
            }
            catch (Exception ex)
            {
                // Handle other exceptions or errors
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw (ex);
            }
        }

        public async void Post(string apiUrl, object data)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(data);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();

                // Optionally, you can log or perform other actions here if needed.
            }
            catch (Exception ex)
            {
                // Handle exceptions, log errors, or perform other error handling tasks.
            }
        }

        /// <summary>
        /// Sends a PUT request to the specified API endpoint with the provided data and returns the response as the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the expected response data.</typeparam>
        /// <param name="apiUrl">API endpoint URL for the PUT request.</param>
        /// <param name="data">Data to be sent in the PUT request.</param>
        /// <returns>Response data of type T.</returns>
        public async Task<T> PutAsync<T>(string apiUrl, object data)
        {
            string jsonData = JsonConvert.SerializeObject(data);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync(apiUrl, content);
            return await HandleResponse<T>(response);
        }
        public async Task<T> PutAsync<T>(string apiUrl, string jsonData)
        {
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync(apiUrl, content);
            return await HandleResponse<T>(response);
        }

        /// <summary>
        /// Sends a DELETE request to the specified API endpoint and returns the response as the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the expected response data.</typeparam>
        /// <param name="apiUrl">API endpoint URL for the DELETE request.</param>
        /// <returns>Response data of type T.</returns>
        public async Task<T> DeleteAsync<T>(string apiUrl)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync(apiUrl);
            return await HandleResponse<T>(response);
        }

        /// <summary>
        /// Handles the API response and deserializes the content into the specified generic type.
        /// </summary>
        /// <typeparam name="T">Type of the expected response data.</typeparam>
        /// <param name="response">HttpResponseMessage received from the API.</param>
        /// <returns>Response data of type T if the response is successful; default value of T otherwise.</returns>
        private async Task<T> HandleResponse<T>(HttpResponseMessage response)
        {
            string responseData = await response.Content.ReadAsStringAsync();

            try
            {
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize to T directly if T matches the API response structure
                    var result = JsonConvert.DeserializeObject<T>(responseData);

                    if (result == null)
                    {
                        throw new InvalidOperationException("Deserialization returned null. Check the response data or target type.");
                    }

                    return result;
                }
                else
                {
                    // Optionally, you can deserialize the response into a known error structure here
                    throw new HttpRequestException(
                        $"HTTP request failed with status code {response.StatusCode}. Response data: {responseData}");
                }
            }
            catch (JsonSerializationException jsonEx)
            {
                // Handle JSON-specific deserialization errors
                throw new InvalidOperationException($"Failed to deserialize response. JSON data: {responseData}", jsonEx);
            }
            catch (Exception ex)
            {
                // General exception handling
                throw new InvalidOperationException($"An error occurred while processing the response: {ex.Message}", ex);
            }
        }

        private async Task<T> HandleResponseCustomer<T>(HttpResponseMessage response)
        {
            string responseData = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                var jsonResponse = JObject.Parse(responseData);
                var message = jsonResponse["message"]?.ToString();
                var dataStr = jsonResponse["data"]?.ToString();
                dataStr = dataStr.Replace("\r", "").Replace("\t", "").Replace("\n", "");

                // Optionally, remove other escape sequences using Regex
                dataStr = Regex.Unescape(dataStr);

                return JsonConvert.DeserializeObject<T>(dataStr);
            }
            else
            {
                // Handle specific status codes
                switch (response.StatusCode)
                {
                    case HttpStatusCode.Conflict: // 409
                        throw new ConflictException($"Conflict error: {responseData}");
                    case HttpStatusCode.Forbidden: // 403
                        throw new ForbiddenException($"Forbidden error: {responseData}");
                    // Add more cases as needed
                    default:
                        throw new HttpRequestException($"Unexpected status code: {(int)response.StatusCode}, Response: {responseData}");
                }
            }
        }

        public class ConflictException : Exception
        {
            public ConflictException(string message) : base(message) { }
        }

        public class ForbiddenException : Exception
        {
            public ForbiddenException(string message) : base(message) { }
        }


        /// <summary>
        /// Adds the authorization header to the HttpClient object for authentication purposes.
        /// </summary>
        /// <param name="client">HttpClient object.</param>
        /// <param name="token">Authentication token to be added in the header.</param>
        private static void AddAuthorizationHeader(HttpClient client, string token)
        {

            // Check if token exists in the session
            if (!string.IsNullOrEmpty(token))
            {
                if (client.DefaultRequestHeaders.Authorization != null)
                {
                    // Parse the existing token if present
                    var jwtHandler = new JwtSecurityTokenHandler();
                    var existingTokenData = jwtHandler.ReadToken(client.DefaultRequestHeaders.Authorization.Parameter) as JwtSecurityToken;

                    if (existingTokenData != null && existingTokenData.ValidTo < DateTime.UtcNow)
                    {
                        // Replace the token if it's expired
                        client.DefaultRequestHeaders.Remove("Authorization");
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    }
                }
                else
                {
                    // Add the token to the request headers
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                }
            }
        }



        /// <summary>
        /// Disposes the HttpClient object when the ApiCallerHelper is no longer in use.
        /// </summary>
        public void Dispose()
        {
            _httpClient.Dispose();
        }

    }
}