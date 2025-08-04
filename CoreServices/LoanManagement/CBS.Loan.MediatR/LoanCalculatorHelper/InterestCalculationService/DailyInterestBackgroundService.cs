using System;
using System.Threading;
using System.Threading.Tasks;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanCalculatorHelper.InterestCalculationService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Functions;

public class DailyInterestBackgroundService : BackgroundService
{
    private readonly ILogger<DailyInterestBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly BackgroundServiceState _state;
    private static readonly object _lock = new object(); // Lock to prevent overlapping executions
    private bool _isExecuting;
    private DateTime _nextExecutionTime;
    private bool _isInterestAlreadyCalculatedNewExecution = false;

    // Constructor to initialize dependencies
    public DailyInterestBackgroundService(
        ILogger<DailyInterestBackgroundService> logger,
        IServiceProvider serviceProvider,
        BackgroundServiceState state)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _state = state;
        _nextExecutionTime = GetNextExecutionTime(); // Initialize next execution time
    }

    // Override ExecuteAsync to run background tasks
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Daily Interest Background Service is starting.");
        _state.IsRunning = true;

        try
        {
            // Start the initial interest calculation if needed
            await PerformInitialCalculationIfNeededAsync(stoppingToken);

            // Start the heartbeat task
            var heartbeatTask = StartHeartbeatAsync(stoppingToken);

            // Wait until the stopping token is cancelled
            await Task.Delay(Timeout.Infinite, stoppingToken);

            // Ensure the heartbeat task is also completed
            await heartbeatTask;
        }
        catch (TaskCanceledException)
        {
            // Task was cancelled, usually due to stopping the application
        }
        finally
        {
            _state.IsRunning = false;
            _logger.LogInformation("Daily Interest Background Service is stopping.");
        }
    }

    // Perform the initial interest calculation if the current time is within the allowed execution window
    private async Task PerformInitialCalculationIfNeededAsync(CancellationToken stoppingToken)
    {
        if (DateTime.Now <= _nextExecutionTime)
        {
            _logger.LogInformation("Current time is less than or equal to the next execution time. Executing interest calculation.");
            await PerformInterestCalculationAsync(stoppingToken);
            _nextExecutionTime = GetNextExecutionTime();
            _logger.LogInformation("Next execution time set to: {NextExecutionTime}", _nextExecutionTime);
        }
    }

    // Calculate the next execution time for 2 AM
    private DateTime GetNextExecutionTime()
    {
        var now = BaseUtilities.UtcNowToDoualaTime();
        var nextExecutionTime = new DateTime(now.Year, now.Month, now.Day, 2, 0, 0, DateTimeKind.Local);

        if (_isInterestAlreadyCalculatedNewExecution)
        {
            nextExecutionTime = nextExecutionTime.AddDays(1);
        }
        else if (now > nextExecutionTime)
        {
            nextExecutionTime = nextExecutionTime.AddDays(1);
        }

        _logger.LogInformation("Next execution time calculated: {NextExecutionTime}", nextExecutionTime);

        return nextExecutionTime;
    }

    // Perform the interest calculation
    private async Task PerformInterestCalculationAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (TryEnterExecution())
            {
                try
                {
                    _logger.LogInformation("Execution time reached. Calculating daily interest...");
                    await CalculateDailyInterestAsync(stoppingToken);
                    _state.LastSuccessfulRun = DateTime.Now;
                    _state.LastErrorMessage = null;
                    _logger.LogInformation("Daily interest calculation completed successfully.");
                }
                catch (Exception ex)
                {
                    _state.LastErrorMessage = ex.Message;
                    _logger.LogError(ex, "Error occurred while calculating daily interest.");
                }
                finally
                {
                    ExitExecution();
                }
            }
            else
            {
                _logger.LogInformation("Interest calculation is already in progress. Skipping execution.");
            }
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Task was canceled.");
        }
        catch (Exception ex)
        {
            _state.LastErrorMessage = ex.Message;
            _logger.LogError(ex, "Background service encountered an exception.");
        }
    }

    // Attempt to acquire the execution lock
    private bool TryEnterExecution()
    {
        lock (_lock)
        {
            if (!_isExecuting)
            {
                _isExecuting = true;
                return true;
            }
            return false;
        }
    }

    // Release the execution lock
    private void ExitExecution()
    {
        lock (_lock)
        {
            _isExecuting = false;
        }
    }

    // Perform daily interest calculation
    private async Task CalculateDailyInterestAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Creating scope for interest calculation.");
        using (var scope = _serviceProvider.CreateScope())
        {
            var interestService = scope.ServiceProvider.GetRequiredService<IInterestForLoanCalculationServices>();
            _logger.LogInformation("Calling CalculateDailyInterestAndUpdateAsync.");
            await interestService.CalculateDailyInterestAndUpdateAsync(stoppingToken);
            _isInterestAlreadyCalculatedNewExecution = true;
        }
        _logger.LogInformation("Interest calculation scope disposed.");
    }

    // Start the heartbeat task
    private async Task StartHeartbeatAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Heartbeat: Service is alive at {Time}", DateTime.Now);

            if (DateTime.Now >= _nextExecutionTime)
            {
                await PerformInterestCalculationAsync(stoppingToken);
                _nextExecutionTime = GetNextExecutionTime();
                _logger.LogInformation($"Next execution time calculated and set at: {_nextExecutionTime}");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}

