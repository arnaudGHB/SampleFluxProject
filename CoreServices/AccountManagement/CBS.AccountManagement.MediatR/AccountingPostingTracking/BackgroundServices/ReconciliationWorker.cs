using CBS.AccountManagement.Data.Dto;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Command;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CBS.AccountManagement.Data.Dto.ChartOfAccountDto;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper.Configuration.Annotations;

namespace CBS.AccountManagement.MediatR.BackgroundServices
{
    public class ReconciliationWorkerService : BackgroundService
    {
        private readonly ILogger<ReconciliationWorkerService> _logger;
        private readonly BackgroundServiceState _state;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly PeriodicTimer _timer;
        private readonly IMediator mediator;
        private readonly PathHelper _pathHelper;
        private readonly UserInfoToken _userInfoToken;
        //private readonly IAccountingEntryRepository _accountingEntryRepository;
        private string? _cachedToken;
        private DateTime _tokenExpirationTime = DateTime.MinValue;
        private readonly IServiceScopeFactory _scopeFactory;

        public ReconciliationWorkerService(ILogger<ReconciliationWorkerService> logger,
      PathHelper? pathHelper,  UserInfoToken? userInfoToken,  IHttpClientFactory? httpClientFactory, IServiceScopeFactory? scopeFactory)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            //_state = state;
              _pathHelper = pathHelper;
           
            _scopeFactory = scopeFactory;
            _userInfoToken = userInfoToken;
            _timer = new PeriodicTimer(TimeSpan.FromMinutes(5));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string message ="";
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var backgroundServiceState = scope.ServiceProvider.GetRequiredService<BackgroundServiceState>();
                    backgroundServiceState.IsRunning = true;

                    await BaseUtilities.AuthenticateLogAndAuditAsync(message, new { Request = "InitializationOfOperationEventAttributeCommand" }, HttpStatusCodeEnum.OK, LogAction.Read, CBS.AccountManagement.Helper.LogLevelInfo.Information);

                    //while (await _timer.WaitForNextTickAsync(stoppingToken))
                    //{
                    backgroundServiceState.LastSuccessfulRun = BaseUtilities.UtcToLocal();
                    message = string.Format("Reconciliation worker service is running at: {0}", DateTimeOffset.Now);
                    await ProcessApiDataAsync(stoppingToken, backgroundServiceState);
                    //}
                }

            }
            catch (OperationCanceledException ex)
            {
                _state.IsRunning = false;
                _logger.LogInformation("Reconciliation worker service is stopping");
                _state.LastErrorMessage = ex.Message;
            }
        }

        //public async Task<string> GetTokenAsync()
        //{
        //    if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpirationTime)
        //    {
        //        return _cachedToken;
        //    }

        //    var client = _httpClientFactory.CreateClient();
        //    var request = new HttpRequestMessage(HttpMethod.Post, _configuration["AuthSettings:TokenUrl"]);

        //    var credentials = new
        //    {
        //        Username = _configuration["AuthSettings:Username"],
        //        Password = _configuration["AuthSettings:Password"]
        //    };

        //    request.Content = new StringContent(
        //        JsonSerializer.Serialize(credentials),
        //        System.Text.Encoding.UTF8,
        //        "application/json");

        //    var response = await client.SendAsync(request);
        //    response.EnsureSuccessStatusCode();

        //    var tokenResponse = await JsonSerializer.DeserializeAsync<TokenResponse>(
        //        await response.Content.ReadAsStreamAsync());

        //    if (tokenResponse == null)
        //    {
        //        throw new Exception("Failed to deserialize token response");
        //    }

        //    _cachedToken = tokenResponse.Token;
        //    _tokenExpirationTime = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);

        //    return _cachedToken;
        //}
        private async Task ProcessApiDataAsync(CancellationToken stoppingToken, BackgroundServiceState backgroundServiceState)
        {
            try
            {
                var queryParam = JsonConvert.DeserializeObject<QueryParameters>(_pathHelper.LoadQueryPrameter);
                var modelList = await APICallHelper.GetUnReconciledTransactions(_pathHelper, _userInfoToken, queryParam);
                foreach (var item in modelList.data.items)
                {
                    await processTransactionEntry(item, backgroundServiceState);
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ProcessApiDataAsync");
            }
        }

        private async Task<TransactionTrackerResponse> processTransactionEntry(TransactionTrackerDto item, BackgroundServiceState backgroundServiceState)
        {
            try
            {
                string message = string.Empty;
                using (var scope = _scopeFactory.CreateScope())
                {
                var userModel = scope.ServiceProvider.GetRequiredService < UserInfoToken>();
                    //userModel = new UserInfoToken { BranchId = item.BranchId, BranchCodeX = item.BranchCodeX, FullName = item.UserFullName };
                     var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var entries = await mediator.Send(new GetAccountingEntryByReferenceIdQuery(item.TransactionReferenceId));
                    if (entries.Data.Any())
                    {
                        var model = new UpdateAccountingTransactionTrackerCommand
                        {
                            Id = item.Id,
                            DatePassed = DateTime.UtcNow,
                            HasPassed = true,
                            NumberOfRetry = item.NumberOfRetry + 1
                        };

                        var result = await mediator.Send(model);
                        if (result.Data)
                        {
                            message = string.Format("Reconciliation worker service did not work on data: {0} because entries successfully reconcile", item);

                            BaseUtilities.LogAndAuditAsync(message, item, HttpStatusCodeEnum.OK, LogAction.Read, CBS.AccountManagement.Helper.LogLevelInfo.Information);

                            return new TransactionTrackerResponse
                            {
                                Id = BaseUtilities.GenerateInsuranceUniqueNumber(18, "TKR"),
                                DatePassed = BaseUtilities.UtcToLocal(),
                                CommandDataType = item.CommandDataType,
                                NumberOfRetry = model.NumberOfRetry,
                                HasPassed = result.Data,
                                TransactionReferenceId = item.TransactionReferenceId,
                                TransactionTrackerId = item.Id
                            };
                        }
                        else
                        {
                            message = string.Format("Reconciliation worker service worked on data: {0} successfully reconcile and failed to update tracking status at {1}", item, BaseUtilities.UtcToLocal());

                            BaseUtilities.LogAndAuditAsync(message, item, HttpStatusCodeEnum.OK, LogAction.Read, CBS.AccountManagement.Helper.LogLevelInfo.Error);

                            return new TransactionTrackerResponse
                            {
                                Id = BaseUtilities.GenerateInsuranceUniqueNumber(18, "TKR"),
                                DatePassed = BaseUtilities.UtcToLocal(),
                                CommandDataType = item.CommandDataType,
                                NumberOfRetry = model.NumberOfRetry,
                                HasPassed = result.Data,
                                TransactionReferenceId = item.TransactionReferenceId,
                                TransactionTrackerId = item.Id
                            };
                        }

                    }
                    else
                    {
                        var RequestObject = GetRequestObject(item, backgroundServiceState);

                        var result = (ServiceResponse<bool>)await mediator.Send(RequestObject);
                        if (result.Data)
                        {
                            var model = new UpdateAccountingTransactionTrackerCommand
                            {
                                Id = item.Id,
                                DatePassed = DateTime.UtcNow,
                                HasPassed = true,
                                NumberOfRetry = item.NumberOfRetry + 1
                            };
                            var resulting = await mediator.Send(model);
                            message = string.Format("Reconciliation worker service worked on data: {0} and accounting entries was passed successfully at {1}", item, BaseUtilities.UtcToLocal());

                            BaseUtilities.LogAndAuditAsync(message, item, HttpStatusCodeEnum.OK, LogAction.Read, CBS.AccountManagement.Helper.LogLevelInfo.Information);

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
                            message = string.Format("Reconciliation worker service worked on data: {0} but accounting entries failed to be passed successfully at {1}", item, BaseUtilities.UtcToLocal());

                            BaseUtilities.LogAndAuditAsync(message, item, HttpStatusCodeEnum.OK, LogAction.Read, CBS.AccountManagement.Helper.LogLevelInfo.Information);

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

               
            }
            catch (Exception ex)
            {
                backgroundServiceState.IsRunning = false;

                backgroundServiceState.LastErrorMessage =string.Format( "Reonciliation Service failed reconciling item:{0} at {1} with error {2}",JsonConvert.SerializeObject(item),BaseUtilities.UtcToLocal(), ex.Message);
                throw (ex);
            }
        }

        private object GetRequestObject(TransactionTrackerDto item, BackgroundServiceState backgroundServiceState)
        { 
            try
            {
                switch (item.CommandDataType)
                {
                    case CommandDataType.CashInitializationCommand:
                        {
                            if (item.CommandJsonObject != null)
                            {
                                return JsonConvert.DeserializeObject<CashInitializationCommand>(item.CommandJsonObject);
                            }
                            else
                            {
                                throw new Exception("CommandJsonObject is null");
                            }
                        }; break;
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
            catch (Exception ex)
            {
                backgroundServiceState.IsRunning = false;

                backgroundServiceState.LastErrorMessage = string.Format("Reonciliation Service failed to deserialise  item:{0} to {1}at {2} with error {3}", JsonConvert.SerializeObject(item), item.CommandDataType, BaseUtilities.UtcToLocal(), ex.Message);

                throw (ex);
            }
        }

    }
    public class BackgroundServiceState
    {
        public bool IsRunning { get; set; }
        public DateTime LastSuccessfulRun { get; set; }
        public string LastErrorMessage { get; set; }
    }
}
