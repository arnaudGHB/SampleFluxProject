using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.BackgroundServices
{
    public class ReconciliationBackgroundServiceHealthCheck : IHealthCheck
    {
        private readonly BackgroundServiceState _state;

        // Constructor to initialize the state dependency
        public ReconciliationBackgroundServiceHealthCheck(BackgroundServiceState state)
        {
            _state = state;
        }

        // Method to check the health of the background service
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            // Check if the background service is running
            if (!_state.IsRunning)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("Background service is not running."));
            }

            // Check if there are any error messages
            if (!string.IsNullOrEmpty(_state.LastErrorMessage))
            {
                return Task.FromResult(HealthCheckResult.Unhealthy($"Background service encountered an error: {_state.LastErrorMessage}"));
            }

            // Check if the last successful run was within the acceptable threshold
            var lastRunTime = DateTime.UtcNow - _state.LastSuccessfulRun;
            if (lastRunTime > TimeSpan.FromHours(24)) // Adjusted threshold
            {
                return Task.FromResult(HealthCheckResult.Degraded("Background service last successful run was over 24 hours ago."));
            }

            // Return healthy if all checks pass
            return Task.FromResult(HealthCheckResult.Healthy("Background service is running and healthy."));
        }
    }
}