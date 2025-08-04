using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Helper.Helper
{
    public class PublicIpBackgroundService : IHostedService
    {
        private readonly IPublicIpService _publicIpService;
        private readonly IConfiguration _configuration;

        public PublicIpBackgroundService(IPublicIpService publicIpService, IConfiguration configuration)
        {
            _publicIpService = publicIpService;
            _configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var publicIp = await _publicIpService.GetPublicIpAddressAsync();
            _configuration["ServerSettings:IPAddress"] = publicIp;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // No action required on stop
            return Task.CompletedTask;
        }
    }

}
