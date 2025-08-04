using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Helper.Helper
{
    public interface IPublicIpService
    {
        Task<string> GetPublicIpAddressAsync();
    }

    public class PublicIpService : IPublicIpService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PublicIpService> _logger;

        public PublicIpService(IHttpClientFactory httpClientFactory, ILogger<PublicIpService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<string> GetPublicIpAddressAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetStringAsync("https://api.ipify.org");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching public IP address: {ex.Message}");
                return string.Empty;
            }
        }
    }

}
