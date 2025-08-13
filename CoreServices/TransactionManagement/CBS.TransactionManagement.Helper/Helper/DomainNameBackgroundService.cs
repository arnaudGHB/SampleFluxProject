using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Helper.Helper
{
    public class DomainNameBackgroundService : IHostedService
    {
        private readonly IDomainNameService _domainNameService;
        private readonly IConfiguration _configuration;

        public DomainNameBackgroundService(IDomainNameService domainNameService, IConfiguration configuration)
        {
            _domainNameService = domainNameService;
            _configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var domainName = await _domainNameService.GetDomainNameAsync();
            _configuration["ServerSettings:DomainName"] = domainName;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // No action required on stop
            return Task.CompletedTask;
        }
    }
    public interface IDomainNameService
    {
        Task<string> GetDomainNameAsync();
    }

    public class DomainNameService : IDomainNameService
    {
        private readonly ILogger<DomainNameService> _logger;

        public DomainNameService(ILogger<DomainNameService> logger)
        {
            _logger = logger;
        }

        public async Task<string> GetDomainNameAsync()
        {
            try
            {
                var domainName = await Task.Run(() => System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).HostName);
                return domainName;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching domain name: {ex.Message}");
                return string.Empty;
            }
        }
    }

}
