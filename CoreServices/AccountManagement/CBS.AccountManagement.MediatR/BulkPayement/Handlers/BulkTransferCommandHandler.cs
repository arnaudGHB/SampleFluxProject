using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Azure;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Math;

namespace CBS.AccountManagement.MediatR.Handlers
{
    #region Class Definition
    // Handles the command for bulk account postings
    public class BulkTransferCommandHandler : IRequestHandler<BulkTransferResultCommand, ServiceResponse<BulkResult>>
    {
        #region Fields
        // Dependency injections for required repositories, services, and utilities
        private readonly IAccountingEntryRepository _accountingEntryRepository;

        private readonly IMapper _mapper;
        private readonly ILogger<BulkTransferCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        private readonly IAccountingEntriesServices _accountingService;
        private readonly IMediator _mediator;
        #endregion

        #region Properties
        // Holds customer information
        public CustomerInfo _customerInfo { get; set; } = new CustomerInfo();
        #endregion

        #region Constructor

        // Constructor initializes the injected dependencies
        public BulkTransferCommandHandler(
            ITransactionDataRepository transactionDataRepository,
            IAccountingEntryRepository accountingEntryRepository,
            IOperationEventAttributeRepository operationEventAttributeRepository,
            IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
            IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<BulkTransferCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            UserInfoToken userInfoToken,
            IChartOfAccountManagementPositionRepository chartOfAccountRepository,
            IAccountingEntriesServices accountingEntriesServices,
            IMediator mediator)
        {

            _accountingEntryRepository = accountingEntryRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _mediator = mediator;
            _accountingService = accountingEntriesServices;
        }
        #endregion

        #region Methods
        // Main handler method for bulk account postings
        public async Task<ServiceResponse<BulkResult>> Handle(BulkTransferResultCommand command, CancellationToken cancellationToken)
        {
            List<Data.AccountingEntry> allEntries = new List<Data.AccountingEntry> { };
            string errorMessage = string.Empty;
            try
            {
                // Check for duplicate transactions
                //if (await _accountingService.TransactionExists(command.MakeAccountPostingCommands[0].TransactionReferenceId))
                //{
                //    errorMessage = $"An entry has already been posted with this transaction Ref:{command.MakeAccountPostingCommands[0].TransactionReferenceId}.";
                //    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.BulkTransferCommand, LogLevelInfo.Warning);
                //    return ServiceResponse<BulkResult>.Return409(errorMessage);
                //}


                // Process the transactions and generate accounting entries
               
                    var accountingEntries = await ProcessTransactions(command, allEntries);

                
              
                // Validate and save the accounting entries
           
                errorMessage = "Transaction Completed Successfully";
                await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.OK, LogAction.BulkTransferCommand, LogLevelInfo.Information);


                // Return success response if no issues
                return ServiceResponse<BulkResult>.ReturnResultWith200(accountingEntries);
            }
            catch (Exception ex)
            {
                // Handle exceptions and return failure response
                return ServiceResponse<BulkResult>.Return403( ex.Message);
            }
        }

        #region Helper Methods
 

        // Process multiple transactions to generate accounting entries
        private async Task<BulkResult> ProcessTransactions(BulkTransferResultCommand model, List<Data.AccountingEntry> allEntries)
        {
            var allFailedMakeAccountPostingCommand = new List<Data.BulkTransaction.MakeAccountPostingCommand>();
            //var allEntries = new List<Data.AccountingEntry>();

            try
            {
                foreach (var command in model.MakeAccountPostingCommands)
                {
                    var entries = new List<Data.AccountingEntry>();
                    // Process individual transaction and add entries
                    if (command.ProductType.ToLower().Equals("newloan") || command.ProductType.ToLower().Equals("oldloan"))
                    {
                        // LoanRefundPostingCommand command
                        var modelLoanRefund = new BulkLoanRefundPostingCommand
                        {
                            TransactionReferenceId = command.TransactionReferenceId,

                            IsOldSystemLoan = command.ProductType.ToLower().Equals("oldloan"),
                            LoanProductId = command.ToProductId,
                            BranchId = model.BranchId,

                            FromProductId = command.FromProductId,
                            MemberReference = command.MemberReference,
                            AmountCollection = command.ProductType.ToLower().Equals("newloan") ? (from item in command.LoanRefundCollections
                                                                                                  select new LoanRefundCollection
                                                                                                  {
                                                                                                      Naration = item.Naration,
                                                                                                      EventAttributeName = item.EventAttributeName,
                                                                                                      Amount = item.Amount
                                                                                                  }).ToList() : null,
                            LoanRefundCollectionAlpha = command.ProductType.ToLower().Equals("oldloan") ? LoanRefundCollectionAlpha.FromLoanRefundCollectionAlpha(command.LoanRefundCollectionAlpha) : null,
                            TransactionDate = command.TransactionDate
                        };
                        var result = await _mediator.Send(modelLoanRefund);
                        if (result.StatusCode.Equals(200))
                        {
                            entries = result.Data;
                            allEntries.AddRange(entries);
                        }
                        else
                        {
                            allFailedMakeAccountPostingCommand.Add(command);
                        }

                    }
                    else
                    {
                        entries = await ProcessSingleTransaction(command);
                        allEntries.AddRange(entries);
                    }

                }
                return new BulkResult { AccountingEntries = allEntries, MakeAccountPostingCommands = allFailedMakeAccountPostingCommand };
            }
            catch (Exception ec)
            {

              return new BulkResult { AccountingEntries = allEntries, MakeAccountPostingCommands = allFailedMakeAccountPostingCommand , message = ec.Message};
            }
        }

        // Process a single transaction to generate accounting entries
        private async Task<List<Data.AccountingEntry>> ProcessSingleTransaction(Data.BulkTransaction.MakeAccountPostingCommand command)
        {
            try
            {
                // Initialize the required accounts for the transaction
                var accounts = await InitializeAccounts(command);
                var entriesDto = new List<Data.AccountingEntryDto>();

                // Process the main transaction
                entriesDto.AddRange(await ProcessMainTransaction(command, accounts));

                // Process commissions and special fees
                entriesDto.AddRange(await ProcessCommissions(command, accounts));
                entriesDto.AddRange(await ProcessSpecialFees(command, accounts));

                // Map DTOs to accounting entries
                var entries = _mapper.Map(entriesDto, new List<Data.AccountingEntry>());

                return entries;
            }
            catch (Exception ex)
            {

                throw(ex);
            }
        }
        /// <summary>
        /// Processes special event fees
        /// </summary>
        /// <param name="command"></param>
        /// <param name="accounts"></param>
        /// <returns></returns>
        private async Task<List<AccountingEntryDto>> ProcessSpecialFees(Data.BulkTransaction.MakeAccountPostingCommand command, TransactionAccounts accounts)
        {
            var entries = new List<AccountingEntryDto>();

            foreach (var fee in command.AmountEventCollections)
            {
                var eventAccount = await _accountingService.GetAccountByEventCode(fee, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                var sourceAccount =  accounts.FromProductAccount ;

                entries.AddRange(await _accountingService.CashMovementAsync(fee.Naration, command.MemberReference,
                command.TransactionDate,
                sourceAccount,
                eventAccount,
                fee.Amount,
                "MakeAccountPostingCommand",
                command.TransactionReferenceId, _userInfoToken.BranchId));
            }

            return entries;
        }

        // Process Commision Account
        private async Task<List<AccountingEntryDto>> ProcessCommissions(Data.BulkTransaction.MakeAccountPostingCommand command, TransactionAccounts accounts)
        {
            var entries = new List<AccountingEntryDto>();
            var principalEvent = command.AmountCollection.FirstOrDefault(x => x.IsPrincipal);

            if (!command.AmountCollection.Remove(principalEvent)) return entries;
            var collection = command.AmountCollection.Where(x => x.Amount > 0);
            foreach (var amount in collection)
            {
                var (branchId, branchCode) = GetBranchInfo(command, amount);
                var commissionAccount = await _accountingService.GetTransferCommissionAccount( amount, command.ToProductId, branchId, branchCode, false);

                var sourceAccount =  accounts.ToProductAccount;

                entries.AddRange(await _accountingService.CashMovementAsync(amount.Naration, command.MemberReference,
                command.TransactionDate,
                sourceAccount,
                commissionAccount,
                amount.Amount,
                "MakeAccountPostingCommand",
                command.TransactionReferenceId, _userInfoToken.BranchId));
            }

            return entries;
        }
        public OperationEventAttributeTypes GetOperationType(string operation)
        {
            if (OperationEventAttributeTypes.deposit.ToString().Equals(operation.ToLower()))
            {
                return OperationEventAttributeTypes.deposit;
            }
            else
            {
                return OperationEventAttributeTypes.withdrawal;
            }
        }

        // get the branch information to be used
        private (string branchId, string branchCode) GetBranchInfo(Data.BulkTransaction.MakeAccountPostingCommand command, Data.BulkTransaction.AmountCollection amount) =>
        command.IsInterBranchTransaction
             ? (command.ExternalBranchId, command.ExternalBranchCode)
             : (_userInfoToken.BranchId, _userInfoToken.BranchCode);



        // Initialize the required accounts for a transaction
        private async Task<TransactionAccounts> InitializeAccounts(Data.BulkTransaction.MakeAccountPostingCommand command)
        {
            var tellerAccount = await _accountingService.GetTellerAccount(command);
            Data.BulkTransaction.AmountCollection productEvent = null;
            if (command.AmountCollection.Count()==0)
            {
                var message = $"No Amount has been passed to determine the sense of operations";
                await BaseUtilities.LogAndAuditAsync(message, command, HttpStatusCodeEnum.InternalServerError, LogAction.BulkTransferCommand_InitializeAccounts, LogLevelInfo.Error);
                throw new NullReferenceException(message);
            }
            else 
            {
                productEvent = command.AmountCollection[0];
                var productAccount = await GetProductAccount(command, productEvent);
                var HomeliaisonAwayAccount = command.IsInterBranchTransaction
                    ? await _accountingService.GetHomeliaisonAwayAccount(command)
                    : null;
                var AwayliaisonAccount = command.IsInterBranchTransaction
                 ? await _accountingService.GetAwayLiaisonAccountHome(command)
                 : null;

                return new TransactionAccounts(tellerAccount, productAccount, HomeliaisonAwayAccount, AwayliaisonAccount);
            }
            
        }

      



        // Get the product account for a specific transaction
        private async Task<Data.Account> GetProductAccount(Data.BulkTransaction.MakeAccountPostingCommand command, Data.BulkTransaction.AmountCollection productEvent)
        {
            var (branchId, branchCode) = command.IsInterBranchTransaction
                ? (command.ExternalBranchId, command.ExternalBranchCode)
                : (_userInfoToken.BranchId, _userInfoToken.BranchCode);
            string productId = productEvent.EventAttributeName == "Transfer_Fee_Account" ? command.FromProductId : command.ToProductId;
            return await _accountingService.GetProductAccount(productEvent, command.FromProductId, branchId, branchCode);
        }

        // Process the main transaction entries
        private async Task<List<AccountingEntryDto>> ProcessMainTransaction(Data.BulkTransaction.MakeAccountPostingCommand command, TransactionAccounts accounts)
        {
       
            return command.IsInterBranchTransaction
                ? await ProcessInterBranchTransaction(command, accounts)
                : await ProcessLocalTransaction(command, accounts);
        }

     
        // process local transaction 
        private async Task<List<AccountingEntryDto>> ProcessLocalTransaction(Data.BulkTransaction.MakeAccountPostingCommand command, TransactionAccounts accounts)
        {
            Data.BulkTransaction.AmountCollection amountCollection = new Data.BulkTransaction.AmountCollection();
            var model = command.AmountCollection.Where(x => x.EventAttributeName== "Transfer_Fee_Account");
            if (model.Any())
            {
                amountCollection = model.FirstOrDefault();
            }
            else
            {
                var ProductEventCode = command.AmountCollection.Where(x => x.IsPrincipal);
                amountCollection = ProductEventCode.Any() ? ProductEventCode.FirstOrDefault() : amountCollection;
            }
  
            return await _accountingService.CashMovementAsync(amountCollection.Naration, command.MemberReference,
                      command.TransactionDate, accounts.FromProductAccount, accounts.ToProductAccount,
                      amountCollection.Amount, "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId);
             
        }
        // process Inter Branch Transaction
        private async Task<List<AccountingEntryDto>> ProcessInterBranchTransaction(Data.BulkTransaction.MakeAccountPostingCommand command, TransactionAccounts accounts)
        {
            var entries = new List<AccountingEntryDto>();

            var ProductEventCode = command.AmountCollection.Where(x => x.IsPrincipal).FirstOrDefault();
          
                entries.AddRange(await _accountingService.CashMovementAsync(ProductEventCode.Naration, command.MemberReference,
                    command.TransactionDate, accounts.FromProductAccount, accounts.AwayLiaisonAccount,
                    command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId, false, command.ExternalBranchId));

                entries.AddRange(await _accountingService.CashMovementAsync(ProductEventCode.Naration, command.MemberReference,
                    command.TransactionDate, accounts.HomeLiaisonAccountAway, accounts.ToProductAccount,
                    command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId, command.IsInterBranchTransaction, command.ExternalBranchId));
          

            return entries;
        }

        #endregion

        #region Nested Types
        // Record structure to group transaction accounts
        public record TransactionAccounts(Data.Account FromProductAccount, Data.Account ToProductAccount, Data.Account HomeLiaisonAccountAway, Data.Account AwayLiaisonAccount);
        #endregion

        // Additional processing methods omitted for brevity...
        #endregion
    }
    #endregion
}
