using CBS.AccountManagement.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace  ReconciliationWorkerService
{
    public class JWTMiddleware
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<JWTMiddleware> _logger;
        private UserInfoToken _userInfoToken { get; set; }
        private string? _cachedToken;
        private DateTime _tokenExpirationTime = DateTime.MinValue;
        private readonly IServiceProvider _serviceProvider;
        public JWTMiddleware(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<JWTMiddleware> logger,
            UserInfoToken userInfoToken,
            IServiceProvider? serviceProvider)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _serviceProvider = serviceProvider;
        }

        public async Task<HttpClient> GetClientAsync()
        {
            if (string.IsNullOrEmpty(_cachedToken) || JWTMiddleware.IsTokenExpired(_cachedToken))
            {
                await RefreshTokenAsync();
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _cachedToken);
            return client;
        }

        private async Task RefreshTokenAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Post, _configuration["AuthSettings:TokenUrl"]);

                var credentials = new
                {
                    Username = _configuration["AuthSettings:Username"],
                    Password = _configuration["AuthSettings:Password"],
                    RemoteIp = _configuration["AuthSettings:RemoteIp"],
                    Longitude = _configuration["AuthSettings:Longitude"],
                    Latitude = _configuration["AuthSettings:Latitude"],
                };

                request.Content = new StringContent( JsonSerializer.Serialize(credentials),  System.Text.Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
          
                var token = await response.Content.ReadAsStringAsync();
                _cachedToken = token;
                using (var scope = _serviceProvider.CreateScope())
                {
                    _userInfoToken = scope.ServiceProvider.GetRequiredService<UserInfoToken>();
                    // Use userInfoToken
                }
                _userInfoToken.Token = token;

                // Extract claims and populate UserInfoToken
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken != null)
                {
                    _userInfoToken.Id = jsonToken.Claims.FirstOrDefault(c => c.Type == "Id")?.Value ?? string.Empty;
                    _userInfoToken.Email = jsonToken.Claims.FirstOrDefault(c => c.Type == "Email")?.Value ?? string.Empty;
                    _userInfoToken.BankId = jsonToken.Claims.FirstOrDefault(c => c.Type == "BankID")?.Value ?? string.Empty;
                    _userInfoToken.BranchId = jsonToken.Claims.FirstOrDefault(c => c.Type == "BranchID")?.Value ?? string.Empty;
                    _userInfoToken.FullName = jsonToken.Claims.FirstOrDefault(c => c.Type == "FullName")?.Value ?? string.Empty;
                    _userInfoToken.BranchCode = jsonToken.Claims.FirstOrDefault(c => c.Type == "BranchCode")?.Value ?? string.Empty;
                    _userInfoToken.BranchName = jsonToken.Claims.FirstOrDefault(c => c.Type == "BranchName")?.Value ?? string.Empty;
                    _userInfoToken.BankCode = jsonToken.Claims.FirstOrDefault(c => c.Type == "BankCode")?.Value ?? string.Empty;

                    var isHeadOfficeClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "IsHeadOffice")?.Value;
                    if (bool.TryParse(isHeadOfficeClaim, out bool isHeadOffice))
                    {
                        _userInfoToken.IsHeadOffice = isHeadOffice;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing JWT token");
                throw;
            }
        }

        public static bool IsTokenExpired(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken == null)
                    return true;

                return jsonToken.ValidTo < DateTime.UtcNow;
            }
            catch (Exception)
            {
                return true;
            }
        }
    }
}