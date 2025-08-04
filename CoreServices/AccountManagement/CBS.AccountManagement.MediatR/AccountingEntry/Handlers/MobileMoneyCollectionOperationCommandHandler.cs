using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;
using CBS.AccountManagement.Data.Dto.AccountingEntryDto;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.ExceptionHandler;
using Microsoft.Extensions.Configuration;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Data;
using CBS.AccountManagement.MediatR.AccountingEntry.Commands;

namespace CBS.AccountManagement.MediatR.Handlers
{
    public class MobileMoneyCollectionOperationCommandHandler : IRequestHandler<MobileMoneyCollectionOperationCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICashMovementTrackingConfigurationRepository _chartOfAccountRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MobileMoneyCollectionOperationCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ITransactionDataRepository _transactionDataRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;
        private readonly IMediator _mediator;


        public MobileMoneyCollectionOperationCommandHandler(
            ITransactionDataRepository transactionDataRepository,
             IAccountingEntryRepository accountingEntryRepository,
    IOperationEventAttributeRepository operationEventAttributeRepository,
    IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
             IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<MobileMoneyCollectionOperationCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken,
            ICashMovementTrackingConfigurationRepository chartOfAccountRepository, IAccountingEntriesServices accountingEntriesServices, IMediator mediator)
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
            _mediator = mediator;
            _accountingService = accountingEntriesServices;
        }

        public async Task<ServiceResponse<bool>> Handle(MobileMoneyCollectionOperationCommand command, CancellationToken cancellationToken)
        {
            try
            {
                List<Data.AccountingEntry> AccountingEntries = new List<Data.AccountingEntry>();
                List<Data.AccountingEntryDto> AccountingEntriesDto = new List<Data.AccountingEntryDto>();

                Data.Account TellerAccount = new Data.Account();
                if (await _accountingService.TransactionExists(command.TransactionReferenceId))
                {
                   var errorMessage = $"An entry has already been posted with this transaction Ref:{command.TransactionReferenceId}.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.Create, LogLevelInfo.Information);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }

                TellerAccount = await _accountingService.GetAccount(command.TellerSources, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                if (TellerAccount == null)
                {
                    var errorMessage = $"There is not an account for {_userInfoToken.BranchName} for the eventcode {command.TellerSources}.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(),
                        command, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                foreach (var item in command.PaymentCollection)
                {
                    Data.Account Account = new Data.Account();
                    if (item.IsComission)
                    {

                        Account = await _accountingService.GetAccount(item.GetOperationEventCode(), _userInfoToken.BranchId, _userInfoToken.BranchCode);
                        if (Account == null)
                        {
                            var errorMessage = $"There is not an account for {_userInfoToken.BranchName} for the eventcode {command.TellerSources}.";
                            _logger.LogError(errorMessage);

                            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(),
                                command, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                            return ServiceResponse<bool>.Return404(errorMessage);
                        }
                    }
                    else
                    {
                        Account = await _accountingService.GetAccount(item.GetOperationEventCode(item.ProductId), _userInfoToken.BranchId, _userInfoToken.BankCode);


                        if (Account == null)
                        {
                            var errorMessage = $"There is not an account for {_userInfoToken.BranchName} for the eventcode {command.TellerSources}.";
                            _logger.LogError(errorMessage);

                            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(),
                                command, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                            return ServiceResponse<bool>.Return404(errorMessage);
                        }
                    }
                    var list = await _accountingService.CashMovementAsync(item.Naration,command.MemberReference,command.TransactionDate, TellerAccount, Account, item.Amount, "MobileMoneyOperationCommand", command.TransactionReferenceId, _userInfoToken.BranchId);
                    AccountingEntriesDto.AddRange(list);

                }


                AccountingEntries = _mapper.Map(AccountingEntriesDto, AccountingEntries);

                _accountingEntryRepository.AddRange(AccountingEntries);



                if (_accountingService.EvaluateDoubleEntryRule(AccountingEntries))
                {
                    await _uow.SaveAsync();
                }
                else
                {
                    return ServiceResponse<bool>.Return422("Accounting double entry rule not validated contact administrator");
                }
                return ServiceResponse<bool>.ReturnResultWith200(true, "Transaction Completed Successfully");

            }
            catch (Exception ex)
            {

                return ServiceResponse<bool>.Return403(false, ex.Message);
            }
        }

        private bool CheckIfCommissionIsApplied(List<AmountCollection> amountCollection)
        {
            return amountCollection.Where(x => x.IsInterBankOperationPrincipalCommission == true).Any() && amountCollection.Count() > 0;
        }




        private OperationEventAttributeTypes GetOperationEventAttributeTypes(string operationType)
        {
            if (operationType.ToLower().Contains(OperationEventAttributeTypes.deposit.ToString()))
            {
                return OperationEventAttributeTypes.deposit;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.withdrawal.ToString()))
            {
                return OperationEventAttributeTypes.withdrawal;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.transfer.ToString()))
            {
                return OperationEventAttributeTypes.transfer;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.cashreplenishment.ToString()))
            {
                return OperationEventAttributeTypes.cashreplenishment;
            }
            else
            {
                return OperationEventAttributeTypes.none;
            }
        }

        private TransactionCode GetTransCode(string operationType)
        {
            if (operationType.ToLower().Contains(OperationEventAttributeTypes.deposit.ToString()))
            {
                return TransactionCode.CINT;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.withdrawal.ToString()))
            {
                return TransactionCode.COUT;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.transfer.ToString()))
            {
                return TransactionCode.TRANS;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.cashreplenishment.ToString()))
            {
                return TransactionCode.CRP;
            }
            else
            {
                return TransactionCode.None;
            }
        }
        private decimal GetPrincipal(List<AmountCollection> amountCollection)
        {
            return amountCollection.Where(x => x.IsPrincipal == true).FirstOrDefault().Amount;
        }

        private async Task LogError(string message, Exception ex, MakeAccountPostingCommand command)
        {
            // Common logic for logging errors and auditing
            _logger.LogError(ex, message);
            // Assuming APICallHelper.AuditLogger is a static utility method for auditing
            await APICallHelper.AuditLogger(_userInfoToken.Email, nameof(MakeAccountPostingCommand), command, message, "Error", 500, _userInfoToken.Token);
        }



    }


}