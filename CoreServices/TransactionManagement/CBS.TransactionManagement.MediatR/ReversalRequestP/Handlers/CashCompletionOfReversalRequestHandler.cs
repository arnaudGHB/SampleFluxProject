using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.MediatR.Commands.ReversalRequestP;
using CBS.TransactionManagement.Data;
using DocumentFormat.OpenXml.Features;
using CBS.TransactionManagement.Data.Entity.ReversalRequestP;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.Data.Entity.AccountingDayOpening;

namespace CBS.TransactionManagement.MediatR.Handlers.ReversalRequestP
{
    /// <summary>
    /// Handles the command to add a new CashReplenishment.
    /// </summary>
    public class CashCompletionOfReversalRequestHandler : IRequestHandler<CashCompletionOfReversalCommand, ServiceResponse<bool>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IReversalRequestRepository _reversalRequestRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly IAccountingDayRepository _accountingDayRepository;
        private readonly IDailyTellerRepository _dailyTellerRepository;
        private readonly ITellerRepository _tellerRepository; // Repository for accessing CashReplenishment data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<CashCompletionOfReversalRequestHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Constructor for initializing the AddRemittanceHandler.
        /// </summary>
        /// <param name="CashReplenishmentRepository">Repository for CashReplenishment data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public CashCompletionOfReversalRequestHandler(
            ICashReplenishmentPrimaryTellerRepository CashReplenishmentRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper,
            ILogger<CashCompletionOfReversalRequestHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null,
            ITellerRepository tellerRepository = null,
            IAccountRepository accountRepository = null,
            IDailyTellerRepository dailyTellerRepository = null,
            ITransactionRepository transactionRepository = null,
            IReversalRequestRepository reversalRequestRepository = null,
            ITellerOperationRepository tellerOperationRepository = null,
            IAccountingDayRepository accountingDayRepository = null)
        {
            _mapper = mapper;
            _userInfoToken = UserInfoToken;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
            _tellerRepository = tellerRepository;
            _dailyTellerRepository = dailyTellerRepository;
            _transactionRepository = transactionRepository;
            _reversalRequestRepository = reversalRequestRepository;
            _accountRepository = accountRepository;
            _tellerOperationRepository = tellerOperationRepository;
            _accountingDayRepository = accountingDayRepository;
        }

        /// <summary>
        /// Handles the completion of a cash reversal process.
        /// </summary>
        /// <param name="request">The command containing the reversal request data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A service response containing the transaction details.</returns>
        public async Task<ServiceResponse<bool>> Handle(CashCompletionOfReversalCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the current accounting date for the branch
                var accountingDate = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);
                string startMessage = $"Starting reversal process for request ID {request.Id} on accounting date {accountingDate}.";
                _logger.LogInformation(startMessage);
                await BaseUtilities.LogAndAuditAsync(startMessage, request, HttpStatusCodeEnum.OK, LogAction.ReversalProcessed, LogLevelInfo.Information);

                // Step 2: Retrieve the active teller for the current date
                var dailyTeller = await _dailyTellerRepository.GetAnActiveSubTellerForTheDate();
                if (dailyTeller == null)
                {
                    string tellerNotFoundMessage = "No active teller found for the current date.";
                    _logger.LogError(tellerNotFoundMessage);
                    await BaseUtilities.LogAndAuditAsync(tellerNotFoundMessage, request, HttpStatusCodeEnum.NotFound, LogAction.ReversalProcessed, LogLevelInfo.Error);
                    return ServiceResponse<bool>.Return404(tellerNotFoundMessage);
                }

                // Step 3: Find the reversal request by ID
                var reversalRequest = await _reversalRequestRepository.FindAsync(request.Id);
                if (reversalRequest == null)
                {
                    string reversalNotFoundMessage = $"Reversal request with ID {request.Id} not found.";
                    _logger.LogError(reversalNotFoundMessage);
                    await BaseUtilities.LogAndAuditAsync(reversalNotFoundMessage, request, HttpStatusCodeEnum.NotFound, LogAction.ReversalProcessed, LogLevelInfo.Error);
                    return ServiceResponse<bool>.Return404(reversalNotFoundMessage);
                }

                // Step 4: Check if the reversal request is already treated
                if (IsRequestAlreadyTreated(reversalRequest))
                {
                    string alreadyTreatedMessage = $"Reversal request with ID {request.Id} has already been processed.";
                    _logger.LogWarning(alreadyTreatedMessage);
                    await BaseUtilities.LogAndAuditAsync(alreadyTreatedMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.ReversalProcessed, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return403(alreadyTreatedMessage);
                }

                // Step 5: Ensure the request is ready for processing
                if (!reversalRequest.RequestStatus)
                {
                    string notReadyMessage = $"Reversal request with ID {request.Id} is not yet ready for processing.";
                    _logger.LogWarning(notReadyMessage);
                    await BaseUtilities.LogAndAuditAsync(notReadyMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.ReversalProcessed, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return400(notReadyMessage);
                }

                // Step 6: Retrieve transactions associated with the reversal request
                var transactions = _transactionRepository.FindBy(x => x.TransactionReference == reversalRequest.TransactionReference).ToList();
                if (!transactions.Any())
                {
                    string noTransactionFoundMessage = $"No transactions found for reference {reversalRequest.TransactionReference}.";
                    _logger.LogError(noTransactionFoundMessage);
                    await BaseUtilities.LogAndAuditAsync(noTransactionFoundMessage, request, HttpStatusCodeEnum.NotFound, LogAction.ReversalProcessed, LogLevelInfo.Error);
                    return ServiceResponse<bool>.Return404(noTransactionFoundMessage);
                }

                // Step 7: Retrieve teller and teller account details
                var teller = await _tellerRepository.GetTeller(dailyTeller.TellerId);
                var tellerAccount = await _accountRepository.RetrieveTellerAccount(teller);

                // Step 8: Process the reversal request
                var transaction = await ProcessReversalRequest(reversalRequest, tellerAccount, teller, accountingDate, transactions);

                // Step 9: Update reversal request details and save changes
                UpdateReversalRequestDetails(reversalRequest, teller);
                await _uow.SaveAsync();

                string successMessage = $"Reversal process completed successfully for incident {reversalRequest.TransactionReference} with Amount {BaseUtilities.FormatCurrency(reversalRequest.Amount)}.";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.ReversalProcessed, LogLevelInfo.Information);

                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception e)
            {
                // Step 11: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Error occurred while processing reversal for request ID {request.Id}: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.ReversalProcessed, LogLevelInfo.Error);
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }

        // Helper method to check if the request is already treated
        private bool IsRequestAlreadyTreated(ReversalRequest reversalRequest)
        {
            return reversalRequest.Status == Status.Treated.ToString();
        }


        /// <summary>
        /// Processes the reversal request by adjusting account balances, creating transactions, 
        /// and recording teller operations if applicable.
        /// </summary>
        /// <param name="reversalRequest">The reversal request details.</param>
        /// <param name="tellerAccount">The teller's account used for the transaction.</param>
        /// <param name="teller">The teller processing the reversal.</param>
        /// <param name="accountingDate">The accounting date for the transaction.</param>
        /// <param name="transactions">A list of transactions associated with the reversal request.</param>
        /// <returns>Returns true if the reversal is successfully processed, otherwise throws an exception.</returns>
        private async Task<bool> ProcessReversalRequest(ReversalRequest reversalRequest, Account tellerAccount, Teller teller, DateTime accountingDate, List<Transaction> transactions)
        {
            try
            {
                // Step 1: Log the initiation of the reversal process
                string startMessage = $"Starting process for reversal request {BaseUtilities.FormatCurrency(reversalRequest.Amount)} with transaction reference {reversalRequest.TransactionReference}.";
                _logger.LogInformation(startMessage);
                await BaseUtilities.LogAndAuditAsync(startMessage, reversalRequest, HttpStatusCodeEnum.OK, LogAction.ReversalProcessed, LogLevelInfo.Information);

                foreach (var trans in transactions)
                {
                    // Step 2: Retrieve the account associated with the transaction
                    var account = await _accountRepository.GetAccountByAccountNumber(trans.AccountNumber);
                    if (account == null)
                    {
                        string accountNotFoundMessage = $"Account not found for account number {trans.AccountNumber}. Skipping transaction reversal.";
                        _logger.LogWarning(accountNotFoundMessage);
                        await BaseUtilities.LogAndAuditAsync(accountNotFoundMessage, reversalRequest, HttpStatusCodeEnum.NotFound, LogAction.ReversalProcessed, LogLevelInfo.Warning);
                        continue; // Skip this transaction and proceed with others
                    }
                    trans.Amount=Math.Abs(trans.Amount);
                    // Step 3: Process the debit or credit operation based on the reversal request
                    decimal amount = ProcessDebitOrCredit(reversalRequest, account, tellerAccount, trans.Amount);
                    if (amount == 0)
                    {
                        string zeroAmountMessage = $"No valid amount to reverse for transaction reference {reversalRequest.TransactionReference}.";
                        _logger.LogWarning(zeroAmountMessage);
                        await BaseUtilities.LogAndAuditAsync(zeroAmountMessage, reversalRequest, HttpStatusCodeEnum.BadRequest, LogAction.ReversalProcessed, LogLevelInfo.Warning);
                        continue;
                    }

                    // Step 4: Create a new transaction entry for the reversal
                    var transaction = CreateTransaction(trans,amount, account, teller, reversalRequest.TransactionReference, reversalRequest.DebitDirection, accountingDate);
                    _transactionRepository.AddTransaction(transaction);

                    if (trans != null && trans.AccountingDate.Date == accountingDate.Date)
                    {
                        if (transaction.Fee>0)
                        {
                            var tellerOperationx = CreateTellerOperation(transaction.Fee, reversalRequest.DebitDirection, teller, tellerAccount, tellerAccount.Balance, transaction, accountingDate);
                            _tellerOperationRepository.Add(tellerOperationx);
                        }
                        // Create and save the teller operation
                        var tellerOperation = CreateTellerOperation(amount, reversalRequest.DebitDirection, teller, tellerAccount, tellerAccount.Balance, transaction, accountingDate);
                        _tellerOperationRepository.Add(tellerOperation);
                    }


                }


                return true;
            }
            catch (Exception ex)
            {
                // Step 8: Handle errors gracefully and log exceptions
                string errorMessage = $"Error occurred while processing reversal request {reversalRequest.Id}: {ex.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, reversalRequest, HttpStatusCodeEnum.InternalServerError, LogAction.ReversalProcessed, LogLevelInfo.Error);
                throw;
            }
        }

        private void UpdateReversalRequestDetails(ReversalRequest reversalRequest, Teller teller)
        {
            reversalRequest.DateTreated = BaseUtilities.UtcNowToDoualaTime();
            reversalRequest.Status = Status.Treated.ToString();
            reversalRequest.TreatedTellerCode = teller.Code;
            reversalRequest.TreatedTellerName = teller.Name;
            reversalRequest.TreatedUserName = _userInfoToken.FullName;
            _reversalRequestRepository.Update(reversalRequest);
        }

        private decimal ProcessDebitOrCredit(ReversalRequest reversalRequest, Account account, Account tellerAccount, decimal amountRequest)
        {
            decimal amount;
            if (reversalRequest.DebitDirection == "Debit")
            {
                amount = -amountRequest;
                _accountRepository.DebitAccount(account, amountRequest);
                _accountRepository.DebitAccount(tellerAccount, amountRequest);
            }
            else
            {
                amount = amountRequest;
                _accountRepository.CreditAccount(account, amountRequest);
                _accountRepository.CreditAccount(tellerAccount, amountRequest);
            }
            return amount;
        }


        // Helper method to log transaction success
      


        private TransactionDto CreateTransaction(Transaction transaction, decimal Amount, Account account, Teller teller, string Reference, string debitDirection, DateTime accountingDate)
        {

            // Calculate balance and credit based on original amount and fees, if IsOfflineCharge is true
            decimal balance = account.Balance;
            decimal credit = debitDirection == OperationType.Credit.ToString() ? Amount : 0;
            decimal debit = debitDirection == OperationType.Debit.ToString() ? Amount : 0;

            decimal originalAmount = transaction.OriginalDepositAmount;
            string N_A = "N/A";
          
            // Create the transaction entity
            var transactionEntityEntryFee = new TransactionDto
            {
                Id = BaseUtilities.GenerateUniqueNumber(), // Generate unique transaction ID
                CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow), // Set transaction creation date
                Status = TransactionStatus.COMPLETED.ToString(), // Set transaction status to completed
                TransactionReference = Reference, // Set transaction reference
                ExternalReference = N_A, // Set external reference
                IsExternalOperation = false, // Set external operation flag
                ExternalApplicationName = N_A, // Set external application name
                SourceType = OperationSourceType.BackOffice_Operation.ToString(), // Set source type
                Currency = Currency.XAF.ToString(), // Set currency
                TransactionType = TransactionType.CASH_REVERSAL.ToString(), // Set transaction type (deposit)
                AccountNumber = account.AccountNumber, // Set account number
                PreviousBalance = account.Balance, // Set previous balance
                AccountId = account.Id, // Set account ID
                CustomerId = account.CustomerId, // Set customer ID
                ProductId = account.ProductId, // Set product ID
                SenderAccountId = account.Id, // Set sender account ID
                ReceiverAccountId = account.Id, // Set receiver account ID
                BankId = teller.BankId, // Set bank ID
                Operation = TransactionType.CASH_REVERSAL.ToString(), // Set operation type (deposit)
                BranchId = teller.BranchId, // Set branch ID
                OriginalDepositAmount = originalAmount, // Set original deposit amount including fees
                Fee = transaction.Fee, // Set fee (charges)
                Tax = 0, // Set tax (assuming tax is not applicable)
                Amount = Math.Abs(Amount), // Set amount (excluding fees)
                Note = $"Statement: An adjustment of {BaseUtilities.FormatCurrency(originalAmount)} was completed on account {account.AccountType}, Reference: {Reference}.", // Set transaction note
                OperationType = debitDirection,
                AccountingDate=accountingDate, // Set operation type (credit)
                FeeType = "N/A", // Set fee type
                TellerId = teller.Id, // Set teller ID
                DepositerNote = N_A, // Set depositer note
                DepositerTelephone = N_A, // Set depositer telephone
                DepositorIDNumber = N_A, // Set depositor ID number
                DepositorIDExpiryDate = N_A, // Set depositor ID expiry date
                DepositorIDIssueDate = N_A, // Set depositor ID issue date
                DepositorIDNumberPlaceOfIssue = N_A, // Set depositor ID place of issue
                IsDepositDoneByAccountOwner = true, // Set flag indicating if deposit is done by account owner
                DepositorName = N_A, // Set depositor name
                Balance = balance, // Set balance after deposit (including original amount)
                Credit = credit, // Set credit amount (including fees if IsOfflineCharge is true, otherwise excluding fees)
                Debit = debit, // Set debit amount (assuming no debit)
                DestinationBrachId = teller.BranchId,
                OperationCharge = 0,
                ReceiptTitle = "Cash Receipt, Transaction Reversal. Reference: " + Reference,
                WithrawalFormCharge = Amount,  // Set destination branch ID
                SourceBrachId = teller.BranchId, // Set source branch ID
                IsInterBrachOperation = false, // Set flag indicating if inter-branch operation
                DestinationBranchCommission = 0, // Calculate destination branch commission
                SourceBranchCommission = 0, // Calculate source branch commission
                CreatedBy = _userInfoToken.Id,
                CloseOfAccountCharge = 0,
                AmountInWord = BaseUtilities.ConvertToWords(Amount),
                AccountType = account.AccountType,
                Teller = teller,
                Account = account
            };

            return transactionEntityEntryFee; // Return the transaction entity
        }

        private TellerOperation CreateTellerOperation(decimal amount, string operationtype, Teller teller, Account tellerAccount, decimal currentBalance, TransactionDto transaction, DateTime AccountingDate)
        {
            amount = Math.Abs(amount);


            return new TellerOperation
            {
                Id = BaseUtilities.GenerateUniqueNumber(),
                AccountID = tellerAccount.Id,
                AccountNumber = tellerAccount.AccountNumber,
                Amount = Math.Abs(amount),
                Description = $"{TransactionType.CASH_REVERSAL.ToString()} of {BaseUtilities.FormatCurrency(amount)} to Account Number: {tellerAccount.AccountNumber}",
                OperationType = operationtype,
                OpenOfDayReference = tellerAccount.OpenningOfDayReference,
                BranchId = tellerAccount.BranchId,
                BankId = tellerAccount.BankId,
                CurrentBalance = currentBalance,
                Date = AccountingDate,
                AccountingDate=AccountingDate,
                PreviousBalance = tellerAccount.PreviousBalance,
                TellerID = teller.Id,
                TransactionReference = transaction.TransactionReference,
                UserID = _userInfoToken.Id,
                EventName = TransactionType.CASH_REVERSAL.ToString(),
                TransactionType = TransactionType.CASH_REVERSAL.ToString(),
                ReferenceId = ReferenceNumberGenerator.GenerateReferenceNumber(_userInfoToken.BranchCode),
                CustomerId = transaction.CustomerId,
                MemberAccountNumber = transaction.AccountNumber,
                DestinationBrachId = teller.BranchId,
                SourceBranchId = teller.BranchId,
                IsInterBranch = false,
                DestinationBranchCommission = transaction.DestinationBranchCommission,
                SourceBranchCommission = transaction.SourceBranchCommission,

            };
        }

    }

}
