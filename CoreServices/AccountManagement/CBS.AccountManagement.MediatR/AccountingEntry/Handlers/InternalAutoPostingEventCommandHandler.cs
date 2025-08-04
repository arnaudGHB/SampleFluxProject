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
    public class InternalAutoPostingEventCommandHandler : IRequestHandler<InternalAutoPostingEventCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<InternalAutoPostingEventCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ITransactionDataRepository _transactionDataRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;
        private readonly IMediator _mediator;
 

        public InternalAutoPostingEventCommandHandler(
            ITransactionDataRepository transactionDataRepository,
             IAccountingEntryRepository accountingEntryRepository,
    IOperationEventAttributeRepository operationEventAttributeRepository,
    IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
             IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<InternalAutoPostingEventCommandHandler> logger,
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

        public async Task<ServiceResponse<bool>> Handle(InternalAutoPostingEventCommand command, CancellationToken cancellationToken)
        {
            List<Data.AccountingEntry> AccountingEntries = new List<Data.AccountingEntry>();
            List<Data.AccountingEntryDto> AccountingEntriesDto = new List<Data.AccountingEntryDto>();
            string errorMessages = string.Empty;
            try
            {
             
                string branchId =  _userInfoToken.BranchId;
                string branchCode = _userInfoToken.BranchCode;
               

                var accounts = await _accountingService.GetCashMovementAccountWithBookingDirectionByEventCode( command.EventCode, branchId, branchCode);
                if (ValidateDeterminantAccountBalance(accounts, command.Amount))
                {
       
                    if (await _accountingService.TransactionExists(command.TransactionReference))
                    {
                          errorMessages = $"An entry has already been posted with this transaction Ref:{command.TransactionReference}.";
                        await BaseUtilities.LogAndAuditAsync(errorMessages, command, HttpStatusCodeEnum.Conflict, LogAction.InternalAutoPostingEventCommand, LogLevelInfo.Information);
                        return ServiceResponse<bool>.Return409(errorMessages);
                    }
                    var entries = await _accountingService.GetCashMovementAccountByEventCode(command.EventCode, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                    AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(command.Naration,GetMemberReference(command.EventCode, _userInfoToken), BaseUtilities.UtcToLocal(),
                                                                                           accounts.BalancingAccount, accounts.DeterminantAccount, Convert.ToDecimal(command.Amount),
                                                                                           "CashRequsitionCommand", command.TransactionReference, _userInfoToken.BranchId));
                    AccountingEntries = _mapper.Map(AccountingEntriesDto, AccountingEntries);

                    _accountingEntryRepository.AddRange(AccountingEntries);
                }




                if (_accountingService.EvaluateDoubleEntryRule(AccountingEntries))
                {
                    await _uow.SaveAsyncWithOutAffectingBranchId();
                }
                else
                {
                    errorMessages = "Accounting double entry rule not validated contact administrator";
                    await BaseUtilities.LogAndAuditAsync(errorMessages, command, HttpStatusCodeEnum.Forbidden, LogAction.InternalAutoPostingEventCommand, LogLevelInfo.Warning);

                    return ServiceResponse<bool>.Return403(false, errorMessages);
                }
                errorMessages = "Transaction Completed Successfully";

                await BaseUtilities.LogAndAuditAsync(errorMessages, command, HttpStatusCodeEnum.OK, LogAction.LoanDisbursementPostingCommand, LogLevelInfo.Information);

                return ServiceResponse<bool>.ReturnResultWith200(true, errorMessages);

            }
            catch (Exception ex)
            {

                return ServiceResponse<bool>.Return403(false, ex.Message);
            }
        }

        private string GetMemberReference(string eventCode, UserInfoToken userInfoToken)
        {
            string code = "";
            var words = eventCode.Split('-');
            foreach (var item in words)
            {
                code = code + item[0];
            }
            return code + "-" + _userInfoToken.BranchCode + "-" + _userInfoToken.FullName;
        }

        private string GenerateReference(string eventCode)
        {
            string  prefix = string.Empty;
          var result = eventCode.Split('_');
            foreach (var item in result) 
            { 

                 prefix = prefix + item[0];
            
            }
            return BaseUtilities.GenerateInsuranceUniqueNumber(15,prefix+"-");

        }
        private string GenerateNaration(Data.Account sourceAccount, Data.Account destinationAccount, InternalAutoPostingEventCommand command, string reference)
        {
            return $"{command.EventCode} of XAF {command.Amount} from {sourceAccount.AccountNumberCU}-{sourceAccount.AccountName} to " +
              $"{destinationAccount.AccountNumberCU}-{destinationAccount.AccountName} Ref:{reference}";

        }
        private bool ValidateDeterminantAccountBalance((Data.Account sourceAccount, Data.Account destinationAccount, AccountOperationType BookingDirection) accounts, decimal amount)
        {
            var modeDic = accounts.BookingDirection.ToString().ToLower().Equals(AccountOperationType.DEBIT.ToString().ToLower())?accounts.sourceAccount:accounts.destinationAccount;
            if ((modeDic.CurrentBalance-amount)>0)
            {
                return true;
            }
            else
            {
                throw new InvalidOperationException($"The current balance {modeDic.AccountNumberCU}-{modeDic.AccountName} is {modeDic.CurrentBalance} not enough for processing");
            }
        }

        private bool CheckIfCommissionIsApplied(List<AmountCollection> amountCollection)
        {
            return amountCollection.Where(x => x.IsInterBankOperationPrincipalCommission == true).Any() && amountCollection.Count() > 0;
        }


        //private async Task<Data.Account> GetHomeLiaisonAccount(Data.ChartOfAccount accountChart, string BranchId,string BranchCodeX)
        //{
        //    var result = _accountRepository.FindBy(x => x.ChartOfAccountId == (accountChart.Id) && x.BranchId == (BranchId));
        //    if (result.Any())
        //    {
        //        return result.FirstOrDefault();
        //    }
        //    else
        //    {
        //        var branch = await _accountingService.GetBranchCodeAsync(BranchId);

        //        var model = new Commands.AddAccountCommand
        //        {
        //            AccountOwnerId = BranchId,
        //            BranchCodeX = BranchCodeX,
        //            AccountName = accountChart.LabelEn + " " + branch.name,
        //            AccountNumberManagementPosition = accountNumberManagementPosition,
        //            AccountNumber = accountChart.AccountNumber,
        //            AccountNumberNetwok = (accountChart.AccountNumber.PadRight(6, '0') + branch.bank.bankCode + BranchCodeX).PadRight(12, '0')+ accountNumberManagementPosition,
        //            AccountNumberCU = (accountChart.AccountNumber.PadRight(6, '0') + _userInfoToken.BranchCodeX).PadRight(9, '0') + accountNumberManagementPosition,
        //            ChartOfAccountId = accountChart.Id,
        //            AccountCategoryId = accountChart.AccountCartegoryId
        //        };
        //        await _mediator.Send(model);
        //        result = _accountRepository.FindBy(x => x.ChartOfAccountId == (accountChart.Id) && x.AccountOwnerId == (BranchId));
        //        return result.FirstOrDefault();
        //    }


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