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
    public class CashInitializationCommandHandler : IRequestHandler<CashInitializationCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CashInitializationCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ITransactionDataRepository _transactionDataRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;
        private readonly IMediator _mediator;
        private const  string EventCode = "Vault_To_Cash";

        public CashInitializationCommandHandler(
            ITransactionDataRepository transactionDataRepository,
             IAccountingEntryRepository accountingEntryRepository,
    IOperationEventAttributeRepository operationEventAttributeRepository,
    IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
             IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<CashInitializationCommandHandler> logger,
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

        public async Task<ServiceResponse<bool>> Handle(CashInitializationCommand command, CancellationToken cancellationToken)
        {
            try
            {
                string branchId = _userInfoToken.BranchId;
                string branchCode = _userInfoToken.BranchCode;

                var accounts = await _accountingService.GetCashMovementAccountByEventCode( EventCode, branchId, branchCode);
                
                    accounts.sourceAccount.DebitBalance = command.AmountInVault;
                    accounts.sourceAccount.BeginningBalanceDebit = command.AmountInVault;
                    accounts.sourceAccount.CurrentBalance = command.AmountInVault;

                
                    accounts.destinationAccount.DebitBalance = command.AmountInHand;
                    accounts.destinationAccount.BeginningBalanceDebit = command.AmountInHand;
                    accounts.destinationAccount.CurrentBalance = command.AmountInHand;
                  
                 
            _accountRepository.UpdateInCasecade(accounts.sourceAccount);
                    _accountRepository.UpdateInCasecade(accounts.destinationAccount);
                _uow.SaveAsync();
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


        //private async Task<Data.Account> GetLiaisonAccount(Data.ChartOfAccount accountChart, string BranchId,string BranchCode)
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
        //            BranchCode = BranchCode,
        //            AccountName = accountChart.LabelEn + " " + branch.name,
        //            AccountNumberManagementPosition = accountNumberManagementPosition,
        //            AccountNumber = accountChart.AccountNumber,
        //            AccountNumberNetwok = (accountChart.AccountNumber.PadRight(6, '0') + branch.bank.bankCode + BranchCode).PadRight(12, '0')+ accountNumberManagementPosition,
        //            AccountNumberCU = (accountChart.AccountNumber.PadRight(6, '0') + _userInfoToken.BranchCode).PadRight(9, '0') + accountNumberManagementPosition,
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