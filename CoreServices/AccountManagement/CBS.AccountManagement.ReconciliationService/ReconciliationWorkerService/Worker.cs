using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Net.Http.Headers;
using ReconciliationWorkerService;
using CBS.AccountManagement.Data.Dto;
using CBS.AccountManagement.MediatR.Commands;
using Newtonsoft.Json;
using CBS.AccountManagement.Helper;
using MediatR;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Command;
using CBS.AccountManagement.Repository;
using Microsoft.EntityFrameworkCore;

namespace ApiProcessingService;


public class ReconciliationWorker : BackgroundService
{
    private readonly ILogger<ReconciliationWorker> _logger;
    private readonly JWTMiddleware _httpClient;
    private readonly IConfiguration _configuration;
    private readonly PeriodicTimer _timer;
    private readonly IMediator _mediator;
    private readonly PathHelper _pathHelper;
    private readonly UserInfoToken _userInfoToken;
    private readonly IAccountingEntryRepository _accountingEntryRepository;
    //
    public ReconciliationWorker(ILogger<ReconciliationWorker> logger, JWTMiddleware httpClient,
            IConfiguration configuration, PathHelper? pathHelper, IMediator? mediator, UserInfoToken? userInfoToken)
    {
        _logger = logger;
        _httpClient = httpClient;
        _configuration = configuration;
        _pathHelper = pathHelper;
        _mediator = mediator;
        _userInfoToken = userInfoToken;
        _timer = new PeriodicTimer(TimeSpan.FromMinutes(5));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (await _timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await ProcessApiDataAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Worker service is stopping");
        }
    }

    private async Task ProcessApiDataAsync(CancellationToken stoppingToken)
    {
        try
        {
            //var client = await _httpClient.GetClientAsync();
            //var response = await client.GetAsync(_configuration["ApiSettings:SourceUrl"], stoppingToken);

            //if (!response.IsSuccessStatusCode)
            //{
            //    _logger.LogError("Failed to fetch data from API. Status code: {StatusCode}", response.StatusCode);
            //    return;
            //}
            var queryParam = JsonConvert.DeserializeObject<QueryParameters>(_configuration["TrackingQueryParameter"]);
            var modelList = await APICallHelper.GetUnReconciledTransactions(_pathHelper, _userInfoToken, queryParam);
            foreach (var item in modelList.data.items)
            {
                await processTransactionEntry(item);
            }

           
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ProcessApiDataAsync");
        }
    }

    private async Task<TransactionTrackerResponse> processTransactionEntry(TransactionTrackerDto item)
    {

        var entries = await _accountingEntryRepository.FindBy(model => model.ReferenceID == item.TransactionReferenceId).ToListAsync();
        if (entries.Any())
        {
            var model = new UpdateAccountingTransactionTrackerCommand
            {
                Id = item.Id,
                DatePassed = DateTime.UtcNow,
                HasPassed = true,
                NumberOfRetry = item.NumberOfRetry+ 1
            };

            var result =  await _mediator.Send(model);
            
            return  new TransactionTrackerResponse 
            { 
                Id= BaseUtilities.GenerateInsuranceUniqueNumber(18,"TKR"),
                DatePassed= BaseUtilities.UtcToLocal(),
                CommandDataType=item.CommandDataType, 
                NumberOfRetry= model.NumberOfRetry,
                HasPassed= result.Data,
                TransactionReferenceId= item.TransactionReferenceId,
                TransactionTrackerId=item.Id
            };
        }
        else
        {
            var RequestObject = GetRequestObject(item);

            var result = (ServiceResponse<bool>)await _mediator.Send(RequestObject);
            if (result.Data)
            {
                var model = new UpdateAccountingTransactionTrackerCommand
                {
                    Id = item.Id,
                    DatePassed = DateTime.UtcNow,
                    HasPassed = true,
                    NumberOfRetry = item.NumberOfRetry + 1
                };
                var resulting = await _mediator.Send(model);

                return new TransactionTrackerResponse
                {
                    Id = BaseUtilities.GenerateInsuranceUniqueNumber(18, "TKR"),
                    DatePassed = BaseUtilities.UtcToLocal(),
                    CommandDataType = item.CommandDataType,
                    NumberOfRetry = model.NumberOfRetry,
                    HasPassed = resulting.Data,
                    TransactionReferenceId = item.TransactionReferenceId,
                    TransactionTrackerId = item.Id
                };
            }
            else
            {
                return new TransactionTrackerResponse
                {
                    Id = BaseUtilities.GenerateInsuranceUniqueNumber(18, "TKR"),
                    DatePassed = BaseUtilities.UtcToLocal(),
                    CommandDataType = item.CommandDataType,
                    NumberOfRetry = item.NumberOfRetry + 1,
                    HasPassed = false,
                    TransactionReferenceId = item.TransactionReferenceId,
                    TransactionTrackerId = item.Id
                };
            }
        }

    }

    private object GetRequestObject(TransactionTrackerDto item)
    {
        switch (item.CommandDataType)
        {

            case CommandDataType.AddTransferEventCommand:
                {
                    if (item.CommandJsonObject != null)
                    {
                        return JsonConvert.DeserializeObject<AddTransferEventCommand>(item.CommandJsonObject);
                    }
                    else
                    {
                        throw new Exception("CommandJsonObject is null");
                    }
                }; break;
            case CommandDataType.AddTransferToNonMemberEventCommand:
                {
                    if (item.CommandJsonObject != null)
                    {
                        return JsonConvert.DeserializeObject<AddTransferToNonMemberEventCommand>(item.CommandJsonObject);
                    }
                    else
                    {
                        throw new Exception("CommandJsonObject is null");
                    }
                }; break;
            case CommandDataType.AddTransferWithdrawalEventCommand:
                {
                    if (item.CommandJsonObject != null)
                    {
                        return JsonConvert.DeserializeObject<AddTransferWithdrawalEventCommand>(item.CommandJsonObject);
                    }
                    else
                    {
                        throw new Exception("CommandJsonObject is null");
                    }
                }; break;
            case CommandDataType.AutoPostingEventCommand:
                {
                    if (item.CommandJsonObject != null)
                    {
                        return JsonConvert.DeserializeObject<AutoPostingEventCommand>(item.CommandJsonObject);
                    }
                    else
                    {
                        throw new Exception("CommandJsonObject is null");
                    }
                }; break;
            case CommandDataType.LoanApprovalPostingCommand:
                {
                    if (item.CommandJsonObject != null)
                    {
                        return JsonConvert.DeserializeObject<LoanApprovalPostingCommand>(item.CommandJsonObject);
                    }
                    else
                    {
                        throw new Exception("CommandJsonObject is null");
                    };
                }; break;
            case CommandDataType.MakeAccountPostingCommand:
                {
                    if (item.CommandJsonObject != null)
                    {
                        return JsonConvert.DeserializeObject<MakeAccountPostingCommand>(item.CommandJsonObject);
                    }
                    else
                    {
                        throw new Exception("CommandJsonObject is null");
                    };
                }; break;
            case CommandDataType.LoanDisbursementPostingCommand:
                {
                    if (item.CommandJsonObject != null)
                    {
                        return JsonConvert.DeserializeObject<LoanDisbursementPostingCommand>(item.CommandJsonObject);
                    }
                    else
                    {
                        throw new Exception("CommandJsonObject is null");
                    };
                }; break;

            case CommandDataType.ReverseAccountingEntryCommand:
                {
                    if (item.CommandJsonObject != null)
                    {
                        return JsonConvert.DeserializeObject<ReverseAccountingEntryCommand>(item.CommandJsonObject);
                    }
                    else
                    {
                        throw new Exception("CommandJsonObject is null");
                    };
                }; break;
            case CommandDataType.ClosingOfMemberAccountCommand:
                {
                    if (item.CommandJsonObject != null)
                    {
                        return JsonConvert.DeserializeObject<ClosingOfMemberAccountCommand>(item.CommandJsonObject);
                    }
                    else
                    {
                        throw new Exception("CommandJsonObject is null");
                    };
                }; break;
            default:
                throw new Exception("CommandJsonObject is null");
        }
    }
 
}

 
