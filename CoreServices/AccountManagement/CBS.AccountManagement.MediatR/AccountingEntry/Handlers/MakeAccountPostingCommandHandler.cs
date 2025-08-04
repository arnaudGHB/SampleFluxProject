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

namespace CBS.AccountManagement.MediatR.Handlers
{
    public class MakeAccountPostingCommandHandler : IRequestHandler<MakeAccountPostingCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICashMovementTrackingConfigurationRepository _chartOfAccountRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MakeAccountPostingCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ITransactionDataRepository _transactionDataRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;
        private readonly IMediator _mediator;

        //  Commissions on treasury or InterBankOperations
        public CustomerInfo _customerInfo { get; set; } = new CustomerInfo();
        public MakeAccountPostingCommandHandler(
            ITransactionDataRepository transactionDataRepository,
             IAccountingEntryRepository accountingEntryRepository,
    IOperationEventAttributeRepository operationEventAttributeRepository,
    IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
             IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<MakeAccountPostingCommandHandler> logger,
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

        //public async Task<ServiceResponse<bool>> Handle(MakeAccountPostingCommand command, CancellationToken cancellationToken)
        //{
        //    string errorMessage = "";
        //    try
        //    {
        //        List<Data.AccountingEntry> AccountingEntries = new List<Data.AccountingEntry>();
        //        List<Data.AccountingEntryDto> AccountingEntriesDto = new List<Data.AccountingEntryDto>();
        //        Data.Account LiaisonAccount = new Data.Account();
        //        Data.Account TellerAccount = new Data.Account();
        //        Data.Account ProductAccount = new Data.Account();
        //        Data.Account CommissionAccount = new Data.Account();

        //        if (await _accountingService.TransactionExists(command.TransactionReferenceId))
        //        {
        //            errorMessage = $"An entry has already been posted with this transaction Ref:{command.TransactionReferenceId}.";
        //            await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.Create, LogLevelInfo.Information);
        //            return ServiceResponse<bool>.Return409(errorMessage);
        //        }
        //        var ProductEventCode = command.AmountCollection.Where(x => x.IsPrincipal).FirstOrDefault();
        //        TellerAccount = await _accountingService.GetTellerAccount(command);
        //        if (command.IsInterBranchTransaction)
        //        {

        //            LiaisonAccount = await _accountingService.GetHomeLiaisonAccount(command);

        //            ProductAccount = await _accountingService.GetProductAccount(ProductEventCode, command.ProductId, command.ExternalBranchId, command.ExternalBranchCode);
        //            if (command.OperationType.ToLower().Contains(OperationEventAttributeTypes.deposit.ToString()))
        //            {
        //                AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(ProductEventCode.Naration, command.MemberReference, command.TransactionDate, TellerAccount, LiaisonAccount, command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId));
        //                AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(ProductEventCode.Naration, command.MemberReference, command.TransactionDate, LiaisonAccount, ProductAccount, command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId));
        //            }
        //            else
        //            {

        //                AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(ProductEventCode.Naration, command.MemberReference, command.TransactionDate, LiaisonAccount, TellerAccount, command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId));
        //                AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(ProductEventCode.Naration, command.MemberReference, command.TransactionDate, ProductAccount, LiaisonAccount, command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId));

        //            }
        //            if (command.AmountCollection.Remove(ProductEventCode))
        //            {
        //                //if (CheckIfCommissionIsApplied(command.AmountCollection))
        //                //{
        //                foreach (var item in command.AmountCollection)
        //                {
        //                    if (item.Amount > 0)
        //                    {
        //                        var branchId = item.CheckCommissionAccountType() ? command.ExternalBranchId : _userInfoToken.BranchId;
        //                        var branchCode = item.CheckCommissionAccountType() ? command.ExternalBranchCode : _userInfoToken.BranchCode;
        //                        CommissionAccount = await _accountingService.GetCommissionAccount(item, command.ProductId, branchId, branchCode, command.IsRemittance());


        //                        //ProductAccount.ChartOfAccount = new Data.ChartOfAccount();
        //                        var sourceAccount = (item.HasPaidCommissionByCash == true) ? TellerAccount : ProductAccount;// await _accountingService.UpdateAccountBalanceAsync(ProductAccount, ProductEventCode.Amount, AccountOperationType.DEBIT, "MakeAccountPostingCommand");
        //                        var entriesDto = await _accountingService.CashMovementAsync(item.Naration, command.MemberReference, command.TransactionDate, sourceAccount, CommissionAccount, command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId);

        //                        AccountingEntriesDto.AddRange(entriesDto);
        //                    }

        //                }


        //                //}
        //            }
        //        }
        //        else
        //        {

        //            ProductAccount = await _accountingService.GetProductAccount(ProductEventCode, command.ProductId, _userInfoToken.BranchId, _userInfoToken.BranchCode);
        //            if (command.OperationType.ToLower().Contains(OperationEventAttributeTypes.deposit.ToString()))
        //            {
        //                AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(ProductEventCode.Naration, command.MemberReference, command.TransactionDate, TellerAccount, ProductAccount, command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId));

        //            }
        //            else
        //            {
        //                AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(ProductEventCode.Naration, command.MemberReference, command.TransactionDate, ProductAccount, TellerAccount, command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId));

        //            }
        //            if (command.AmountCollection.Remove(ProductEventCode))
        //            {
        //                //if (CheckIfCommissionIsApplied(command.AmountCollection) || (command.AmountCollection.Count() > 0 && command.AmountCollection[0].EventAttributeName == "Saving_Fee_Account") || (command.AmountCollection.Count() > 0 && command.AmountCollection[0].EventAttributeName == "Withdrawal_Fee_Account"))
        //                //{
        //                foreach (var item in command.AmountCollection)
        //                {
        //                    if (item.Amount > 0)
        //                    {
        //                        var branchId = item.CheckCommissionAccountType() ? command.ExternalBranchId : _userInfoToken.BranchId;
        //                        var branchCode = item.CheckCommissionAccountType() ? command.ExternalBranchCode : _userInfoToken.BranchCode;
        //                        CommissionAccount = await _accountingService.GetCommissionAccount(item, command.ProductId, branchId, branchCode, command.IsRemittance());


        //                        //ProductAccount.ChartOfAccount = new Data.ChartOfAccount();
        //                        var sourceAccount = ProductAccount;// await _accountingService.UpdateAccountBalanceAsync(ProductAccount, ProductEventCode.Amount, AccountOperationType.DEBIT, "MakeAccountPostingCommand");

        //                        var entriesDto = (await _accountingService.CashMovementAsync(item.Naration, command.MemberReference, command.TransactionDate, sourceAccount, CommissionAccount, command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId));

        //                        AccountingEntriesDto.AddRange(entriesDto);
        //                    }

        //                }


        //                // }
        //            }
        //        }

        //        foreach (var entry in command.AmountEventCollections)
        //        {
        //            var EventAccount = await _accountingService.GetAccountByEventCode(entry, _userInfoToken.BranchId, _userInfoToken.BranchCode);

        //            //ProductAccount.ChartOfAccount = new Data.ChartOfAccount();
        //            var sourceAccount = ProductAccount;// await _accountingService.UpdateAccountBalanceAsync(ProductAccount, ProductEventCode.Amount, AccountOperationType.DEBIT, "MakeAccountPostingCommand");

        //            var entriesDto = (await _accountingService.CashMovementAsync(entry.Naration, command.MemberReference, command.TransactionDate, sourceAccount, EventAccount, command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId));

        //            AccountingEntriesDto.AddRange(entriesDto);

        //        }

        //        AccountingEntries = _mapper.Map(AccountingEntriesDto, AccountingEntries);
        //        Data.TransactionData TransactionData = _accountingService.GenerateTransactionRecord(command.AccountNumber, GetOperationEventAttributeTypes(command.OperationType), GetTransCode(command.OperationType), command.TransactionReferenceId, "", ProductEventCode.Amount);
        //        _transactionDataRepository.Add(TransactionData);
        //        _accountingEntryRepository.AddRange(AccountingEntries);



        //        if (_accountingService.EvaluateDoubleEntryRule(AccountingEntries))
        //        {
        //            await _uow.SaveAsyncWithOutAffectingBranchId();
        //        }
        //        else
        //        {
        //            errorMessage = "Accounting double entry rule not validated contact administrator";
        //            await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.AccountingPosting, LogLevelInfo.Warning);


        //            return ServiceResponse<bool>.Return422(errorMessage);
        //        }
        //        errorMessage = "Transaction Completed Successfully";
        //        await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.AccountingPosting, LogLevelInfo.Warning);

        //        return ServiceResponse<bool>.ReturnResultWith200(true, errorMessage);

        //    }
        //    catch (Exception ex)
        //    {

        //        return ServiceResponse<bool>.Return403(false, ex.Message);
        //    }
        //}

        public async Task<ServiceResponse<bool>> Handle(MakeAccountPostingCommand command, CancellationToken cancellationToken)
        {
            string errorMessage = "";

            try
            {
                List<Data.AccountingEntry> AccountingEntries = new List<Data.AccountingEntry>();
                List<Data.AccountingEntryDto> AccountingEntriesDto = new List<Data.AccountingEntryDto>();

                // Check if transaction reference already exists
                if (await _accountingService.TransactionExists(command.TransactionReferenceId))
                {
                    errorMessage = $"An entry has already been posted with this transaction Ref: {command.TransactionReferenceId}.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.AccountingPosting, LogLevelInfo.Information);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }

                // Retrieve accounts
                var ProductEventCode = command.AmountCollection.FirstOrDefault(x => x.IsPrincipal);
                Data.Account TellerAccount = await _accountingService.GetTellerAccount(command);
                Data.Account ProductAccount;
                Data.Account LiaisonAccount = null;
                Data.Account CommissionAccount;

                // Handle inter-branch transactions
                if (command.IsInterBranchTransaction)
                {
                    LiaisonAccount = await _accountingService.GetHomeliaisonAwayAccount(command);
                    ProductAccount = await _accountingService.GetProductAccount(ProductEventCode, command.ProductId, command.ExternalBranchId, command.ExternalBranchCode);

                    if (command.OperationType.ToLower().Contains(OperationEventAttributeTypes.deposit.ToString()))
                    {
                        AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(ProductEventCode.Naration, command.MemberReference, command.TransactionDate, TellerAccount, LiaisonAccount, command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId));
                        AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(ProductEventCode.Naration, command.MemberReference, command.TransactionDate, LiaisonAccount, ProductAccount, command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId));
                    }
                    else
                    {
                        AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(ProductEventCode.Naration, command.MemberReference, command.TransactionDate, LiaisonAccount, TellerAccount, command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId));
                        AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(ProductEventCode.Naration, command.MemberReference, command.TransactionDate, ProductAccount, LiaisonAccount, command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId));
                    }
                }
                else
                {
                    ProductAccount = await _accountingService.GetProductAccount(ProductEventCode, command.ProductId, _userInfoToken.BranchId, _userInfoToken.BranchCode);

                    if (command.OperationType.ToLower().Contains(OperationEventAttributeTypes.deposit.ToString()))
                    {
                        AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(ProductEventCode.Naration, command.MemberReference, command.TransactionDate, TellerAccount, ProductAccount, command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId));
                    }
                    else
                    {
                        AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(ProductEventCode.Naration, command.MemberReference, command.TransactionDate, ProductAccount, TellerAccount, command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId));
                    }
                }

                // Handle additional commission or fee accounts
                if (command.AmountCollection.Remove(ProductEventCode))
                {
                    foreach (var item in command.AmountCollection.Where(x => x.Amount > 0))
                    {
                        var branchId = item.CheckCommissionAccountType() ? command.ExternalBranchId : _userInfoToken.BranchId;
                        var branchCode = item.CheckCommissionAccountType() ? command.ExternalBranchCode : _userInfoToken.BranchCode;
                        CommissionAccount = await _accountingService.GetCommissionAccount(item, command.ProductId, branchId, branchCode, command.IsRemittance());

                        var sourceAccount = item.HasPaidCommissionByCash ? TellerAccount : ProductAccount;
                        AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(item.Naration, command.MemberReference, command.TransactionDate, sourceAccount, CommissionAccount, command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId));
                    }
                }

                // Handle event-based accounts
                foreach (var entry in command.AmountEventCollections)
                {
                    var EventAccount = await _accountingService.GetAccountByEventCode(entry, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                    AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(entry.Naration, command.MemberReference, command.TransactionDate, ProductAccount, EventAccount, command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId));
                }

                // Map entries and create transaction records
                AccountingEntries = _mapper.Map(AccountingEntriesDto, AccountingEntries);
                Data.TransactionData TransactionData = _accountingService.GenerateTransactionRecord(command.AccountNumber, GetOperationEventAttributeTypes(command.OperationType), GetTransCode(command.OperationType), command.TransactionReferenceId, "", ProductEventCode.Amount);

                _transactionDataRepository.Add(TransactionData);
                _accountingEntryRepository.AddRange(AccountingEntries);

                // Validate double-entry rule
                if (_accountingService.EvaluateDoubleEntryRule(AccountingEntries))
                {
                    await _uow.SaveAsyncWithOutAffectingBranchId();
                }
                else
                {
                    errorMessage = "Accounting double entry rule not validated. Contact administrator.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.AccountingPosting, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return422(errorMessage);
                }

                errorMessage = "Transaction Completed Successfully";
                await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.OK, LogAction.AccountingPosting, LogLevelInfo.Information);

                return ServiceResponse<bool>.ReturnResultWith200(true, errorMessage);
            }
            catch (Exception ex)
            {
                errorMessage = $"Transaction failed: {ex.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.InternalServerError, LogAction.AccountingPosting, LogLevelInfo.Error);
                return ServiceResponse<bool>.Return403(false, errorMessage);
            }
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



    }

}