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
    public class MobileMoneyManagementPostingCommandHandler : IRequestHandler<MobileMoneyManagementPostingCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MobileMoneyOperationCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ITransactionDataRepository _transactionDataRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;
        private readonly IMediator _mediator;


        public MobileMoneyManagementPostingCommandHandler(
            ITransactionDataRepository transactionDataRepository,
             IAccountingEntryRepository accountingEntryRepository,
    IOperationEventAttributeRepository operationEventAttributeRepository,
    IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
             IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<MobileMoneyOperationCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken,
            IChartOfAccountManagementPositionRepository chartOfAccountRepository, IAccountingEntriesServices accountingEntriesServices, IMediator mediator)
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

        public async Task<ServiceResponse<bool>> Handle(MobileMoneyManagementPostingCommand command, CancellationToken cancellationToken)
        {
            try
            {
                List<Data.AccountingEntry> AccountingEntries = new List<Data.AccountingEntry>();
                List<Data.AccountingEntryDto> AccountingEntriesDto = new List<Data.AccountingEntryDto>();
                Data.Account LiaisonAccount = new Data.Account();
                Data.Account FromAccount = new Data.Account();
                Data.Account ToAccount = new Data.Account();

                if (await _accountingService.TransactionExists(command.TransactionReferenceId))
                {
                  var  errorMessage = $"An entry has already been posted with this transaction Ref:{command.TransactionReferenceId}.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.Create, LogLevelInfo.Information);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                FromAccount = await _accountingService.GetMFI_ChartOfAccount(command.ChartOfAccountIdFrom, _userInfoToken.BranchId,_userInfoToken.BranchCode);
                if (FromAccount == null)
                {
                    var errorMessage = $"There is not an account for {_userInfoToken.BranchName} for the eventcode {command.ChartOfAccountIdFrom}.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(),
                        command, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                //SupplierAccount = await _accountingService.GetAccountBasedOnChartOfAccountID(command.DestinationChartOfAccountId, _userInfoToken.BranchId, _userInfoToken.BranchCodeX);


                ToAccount = await _accountingService.GetMFI_ChartOfAccount(command.ChartOfAccountIdTo, _userInfoToken.BranchId, _userInfoToken.BranchCode);

                if (ToAccount == null)
                {
                    var errorMessage = $"There is not an account for {_userInfoToken.BranchName} with chartofaccountId {command.ChartOfAccountIdTo}.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(),
                        command, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                    AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(command.Naration,command.MemberReference, command.TransactionDate,FromAccount, ToAccount, command.Amount, "MobileMoneyManagementPostingEventCommand", command.TransactionReferenceId, _userInfoToken.BranchId));
           

                AccountingEntries = _mapper.Map(AccountingEntriesDto, AccountingEntries);
                Data.TransactionData TransactionData = _accountingService.GenerateTransactionRecord("00000000000", command.Direction,"Aux", command.TransactionReferenceId, "", command.Amount);
                _transactionDataRepository.Add(TransactionData);
                _accountingEntryRepository.AddRange(AccountingEntries);



                if (_accountingService.EvaluateDoubleEntryRule(AccountingEntries))
                {
                    await _uow.SaveAsync();
                    return ServiceResponse<bool>.ReturnResultWith200(true, "Transaction Completed Successfully");
                }
                else
                {
                    return ServiceResponse<bool>.Return422("Accounting double entry rule not validated contact administrator");
                }
            

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