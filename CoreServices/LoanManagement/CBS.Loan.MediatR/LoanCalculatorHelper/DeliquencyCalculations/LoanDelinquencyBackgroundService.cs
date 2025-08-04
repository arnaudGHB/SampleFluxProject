using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.AuthHelper;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.MediatR.LoanCalculatorHelper.DeliquencyCalculations
{
    /// <summary>
    /// Background service that processes loan delinquencies daily at a scheduled time (2 AM).
    /// The service calculates overdue loans, updates loan statuses, and logs the processing events.
    /// </summary>
    public class LoanDelinquencyBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<LoanDelinquencyBackgroundService> _logger;
        private readonly PathHelper _pathHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TokenStorageService _tokenStorageService;
        /// <summary>
        /// Initializes a new instance of the <see cref="LoanDelinquencyBackgroundService"/> class.
        /// </summary>
        /// <param name="serviceScopeFactory">Factory to create service scopes for dependency injection.</param>
        /// <param name="logger">Logger to log messages and errors during service execution.</param>
        /// <param name="pathHelper">Helper for configuration paths and URLs.</param>
        /// <param name="httpContextAccessor">Accessor for HTTP context to support authentication.</param>
        public LoanDelinquencyBackgroundService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<LoanDelinquencyBackgroundService> logger,
            PathHelper pathHelper,
            IHttpContextAccessor httpContextAccessor,
            TokenStorageService tokenStorageService)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _pathHelper = pathHelper;
            _httpContextAccessor = httpContextAccessor;
            _tokenStorageService=tokenStorageService;
        }

        /// <summary>
        /// Executes the background service, scheduling delinquency processing daily at 2 AM.
        /// Handles authentication, logging, and error recovery as needed.
        /// </summary>
        /// <param name="stoppingToken">Token to monitor for service shutdown.</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Calculate the next scheduled run at 2 AM daily.
                    var now = BaseUtilities.UtcNowToDoualaTime();
                    var nextRun = DateTime.Today.AddDays(now.Hour >= 2 ? 1 : 0).AddHours(2);
                    var delay = nextRun - now;

                    // Step 1: Authenticate the service to ensure valid access.
                    bool isAuthenticated = await AuthenticationHelper.AuthenticateAsync(_pathHelper, _logger, _tokenStorageService);
                    if (!isAuthenticated)
                    {
                        _logger.LogWarning("Authentication failed. Exiting the delinquency process.");
                        return;
                    }

                    // Step 2: Wait until the scheduled time if a delay is needed.
                    if (delay.TotalMilliseconds > 0)
                    {
                        _logger.LogInformation("Next delinquency processing scheduled for {NextRun}. Current time: {CurrentTime}", nextRun, now);
                        await BaseUtilities.LogAndAuditAsync(
                            $"Delinquency service scheduled for {nextRun}",
                            null,
                            HttpStatusCodeEnum.OK,
                            LogAction.DelinquencyServiceRun,
                            LogLevelInfo.Information, _tokenStorageService.GetFullName(), _tokenStorageService.GetToken()
                        );

                        await Task.Delay(delay, stoppingToken);
                    }

                    // Step 3: Start delinquency processing.
                    _logger.LogInformation("Starting delinquency processing at {StartTime}", BaseUtilities.UtcNowToDoualaTime());
                    await BaseUtilities.LogAndAuditAsync(
                        "Starting loan delinquency processing.",
                        null,
                        HttpStatusCodeEnum.OK,
                        LogAction.DelinquencyServiceStart,
                        LogLevelInfo.Information, _tokenStorageService.GetFullName(), _tokenStorageService.GetToken()
                    );

                    // Step 4: Create a scoped service instance for dependency injection.
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var delinquencyService = scope.ServiceProvider.GetRequiredService<IDelinquencyService>();
                        await delinquencyService.ProcessAllLoansAsync();
                    }

                    // Step 5: Log successful completion.
                    _logger.LogInformation("Delinquency processing completed successfully at {EndTime}", BaseUtilities.UtcNowToDoualaTime());
                    await BaseUtilities.LogAndAuditAsync(
                        "Delinquency processing completed successfully.",
                        null,
                        HttpStatusCodeEnum.OK,
                        LogAction.DelinquencyServiceEnd,
                        LogLevelInfo.Information, _tokenStorageService.GetFullName(), _tokenStorageService.GetToken()
                    );
                }
                catch (TaskCanceledException)
                {
                    // Log service cancellation during execution.
                    _logger.LogInformation("Delinquency background service is stopping.");
                    await BaseUtilities.LogAndAuditAsync(
                        "Delinquency service was stopped.",
                        null,
                        HttpStatusCodeEnum.OK,
                        LogAction.DelinquencyServiceStop,
                        LogLevelInfo.Warning, _tokenStorageService.GetFullName(), _tokenStorageService.GetToken()
                    );
                    return;
                }
                catch (Exception ex)
                {
                    // Log any unexpected errors and perform an audit trail.
                    string errorMessage = $"An error occurred during delinquency processing: {ex.Message}";
                    _logger.LogError(ex, errorMessage);
                    await BaseUtilities.LogAndAuditAsync(
                        errorMessage,
                        null,
                        HttpStatusCodeEnum.InternalServerError,
                        LogAction.DelinquencyServiceError,
                        LogLevelInfo.Error, _tokenStorageService.GetFullName(), _tokenStorageService.GetToken()
                    );
                }
            }
        }
    }

}
