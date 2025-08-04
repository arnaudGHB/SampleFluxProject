using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Dto;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.AccountingEntry.Commands;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CBS.AccountManagement.MediatR.Commands.AddTransferEventCommand;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace CBS.AccountManagement.MediatR.Handlers
{


    public class QueryLastTransactionForReconciliationHandler : IRequestHandler<QueryLastTransactionForReconciliation, ServiceResponse<string>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICashMovementTrackingConfigurationRepository _chartOfAccountRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<QueryLastTransactionForReconciliationHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ITransactionDataRepository _transactionDataRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;
        private readonly PathHelper _pathHelper;

        public QueryLastTransactionForReconciliationHandler(
            ITransactionDataRepository transactionDataRepository,
             IAccountingEntryRepository accountingEntryRepository,
    IOperationEventAttributeRepository operationEventAttributeRepository,
    IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
            IMediator mediator,
             IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<QueryLastTransactionForReconciliationHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken,
            ICashMovementTrackingConfigurationRepository chartOfAccountRepository, IAccountingEntriesServices accountingEntriesServices, PathHelper? pathHelper)
        {
            _accountRepository = accountRepository;
            _operationEventAttributeRepository = operationEventAttributeRepository;
            _accountingEntryRepository = accountingEntryRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _transactionDataRepository = transactionDataRepository;
            _userInfoToken = userInfoToken;
            _configuration = configuration;
            _chartOfAccountRepository = chartOfAccountRepository;
            _accountingRuleEntryRepository = accountingRuleEntryRepository;
            _pathHelper = pathHelper;
             _accountingService = accountingEntriesServices;
            _mediator = mediator;
        }

        public async Task<ServiceResponse<string>> Handle(QueryLastTransactionForReconciliation command, CancellationToken cancellationToken)
        {
           
            string errorMessage = "";
            try
            {
                var modelList = await APICallHelper.GetTransactions(_pathHelper, _userInfoToken, command.LastlyProcessedTransactionId);
                foreach (var item in modelList.Data)
                {
                    await processTransactionEntry(item);
                }



                return ServiceResponse<string>.ReturnResultWith200("", "Transaction Completed Successfully");
            }
            catch (Exception ex)
            {

                // Log error and return 500 Internal Server Error response with error message
                errorMessage = $" {ex.Message}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddTransferEventCommand",
                    command, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                _logger.LogError(errorMessage);
                return ServiceResponse<string>.Return500(ex, errorMessage);
            }
        }

        private async Task processTransactionEntry(TransactionTrackerDto item)
        {
             
           var entries =  _accountingEntryRepository.FindBy(model=>model.ReferenceID == item.TransactionReferenceId);
            if (entries.Any())
            {
            }
            else
            {
               var RequestObject = GetRequestObject(item);

              var result= (ServiceResponse<bool>)await _mediator.Send(RequestObject);
                if (result.Data)
                {
                    // Update transactions service 
                }
                else
                {
                    throw new Exception("data reconciliation failed"+ RequestObject);
                }
            }
            
        }

        private object GetRequestObject(TransactionTrackerDto item)
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



        private object GetRequestObjectUpdate(TransactionTrackerDto item)
        {
            if (item.CommandJsonObject == null)
                throw new Exception("CommandJsonObject is null");

            var commandTypeMapping = new Dictionary<string, Type>
            {
                { CommandDataType.AddTransferEventCommand, typeof(AddTransferEventCommand) },
                { CommandDataType.AddTransferToNonMemberEventCommand, typeof(AddTransferToNonMemberEventCommand) },
                { CommandDataType.AddTransferWithdrawalEventCommand, typeof(AddTransferWithdrawalEventCommand) },
                { CommandDataType.AutoPostingEventCommand, typeof(AutoPostingEventCommand) },
                { CommandDataType.LoanApprovalPostingCommand, typeof(LoanApprovalPostingCommand) },
                { CommandDataType.MakeAccountPostingCommand, typeof(MakeAccountPostingCommand) },
                { CommandDataType.LoanDisbursementPostingCommand, typeof(LoanDisbursementPostingCommand) },
      
                { CommandDataType.ReverseAccountingEntryCommand, typeof(ReverseAccountingEntryCommand) },
                { CommandDataType.ClosingOfMemberAccountCommand, typeof(ClosingOfMemberAccountCommand) },
                { CommandDataType.MakeBulkAccountPostingCommand, typeof(MakeBulkAccountPostingCommand) },
                { CommandDataType.MobileMoneyCollectionOperationCommand, typeof(MobileMoneyCollectionOperationCommand) },
                { CommandDataType.MobileMoneyOperationCommand, typeof(MobileMoneyOperationCommand) },
                { CommandDataType.MobileMoneyManagementPostingCommand, typeof(MobileMoneyManagementPostingCommand) },
                { CommandDataType.DailyCollectionMonthlyCommisionEventCommand, typeof(DailyCollectionMonthlyCommisionEventCommand) },
                { CommandDataType.DailyCollectionMonthlyPayableEventCommand, typeof(DailyCollectionMonthlyPayableEventCommand) },
                { CommandDataType.DailyCollectionPostingEventCommand, typeof(DailyCollectionPostingEventCommand) },
                { CommandDataType.LoanRefundPostingCommand, typeof(LoanRefundPostingCommand) }
                
            };

            if (commandTypeMapping.TryGetValue(item.CommandDataType, out var commandType))
            {
                return JsonConvert.DeserializeObject(item.CommandJsonObject, commandType);
            }

            throw new Exception($"Unsupported CommandDataType: {item.CommandDataType}");
        }


        private async Task LogError(string message, Exception ex, QueryLastTransactionForReconciliation command)
        {
            // Common logic for logging errors and auditing
            _logger.LogError(ex, message);
            // Assuming APICallHelper.AuditLogger is a static utility method for auditing
            await APICallHelper.AuditLogger(_userInfoToken.Email, nameof(AddTransferEventCommand), command, message, "Error", 500, _userInfoToken.Token);
        }

    }
}
