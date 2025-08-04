using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Helper.Helper;
using CBS.TransactionManagement.MediatR;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Queries;
using DocumentFormat.OpenXml.Bibliography;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.Repository.DailyStatisticBoard;
using CBS.TransactionManagement.Data.Entity.AccountingDayOpening;
using CBS.TransactionManagement.MediatR.LoanDisbursementP.Commands;
using CBS.TransactionManagement.MediatR.LoanDisbursementP.Services;
using System.Runtime.InteropServices;
using CBS.TransactionManagement.MediatR.UtilityServices;
using ZstdSharp.Unsafe;

namespace CBS.TransactionManagement.MediatR.LoanDisbursementP.Handlers
{
    /// <summary>
    /// Handles the command to add a new Transfer Transaction.
    /// </summary>
    public class LoanDisbursmentCommandHandler : IRequestHandler<LoanDisbursmentCommand, ServiceResponse<bool>>
    {
        private readonly IAccountRepository _AccountRepository;
        private readonly ICurrencyNotesRepository _CurrencyNotesRepository;
        private readonly ITransactionRepository _TransactionRepository; // Repository for accessing Transaction data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<LoanDisbursmentCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly IDailyTellerRepository _tellerProvisioningAccountRepository;
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvioningHistoryRepository;
        private readonly IAccountingDayRepository _accountingDayRepository;
        private readonly IGeneralDailyDashboardRepository _generalDailyDashboardRepository; // Repository for accessing account data.
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository; // Repository for accessing account data.
        private readonly ITellerRepository _tellerRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfigRepository _ConfigRepository;
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Constructor for initializing the LoanDisbursmentCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Transaction data access.</param>
        /// <param name="TransactionRepository">Repository for Transaction data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public LoanDisbursmentCommandHandler(
            IAccountRepository AccountRepository,
            ITransactionRepository TransactionRepository,
            IMapper mapper,

            ILogger<LoanDisbursmentCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IDailyTellerRepository tellerProvisioningAccountRepository = null,
            ITellerOperationRepository tellerOperationRepository = null,
            ITellerRepository tellerRepository = null,
            UserInfoToken userInfoToken = null,
            IConfigRepository configRepository = null,
            IMediator mediator = null,
            ICurrencyNotesRepository currencyNotesRepository = null,
            ISubTellerProvisioningHistoryRepository subTellerProvioningHistoryRepository = null,
            IAccountingDayRepository accountingDayRepository = null,
            IGeneralDailyDashboardRepository generalDailyDashboardRepository = null,
            IAPIUtilityServicesRepository utilityServicesRepository = null)
        {
            _AccountRepository = AccountRepository;
            _TransactionRepository = TransactionRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _tellerProvisioningAccountRepository = tellerProvisioningAccountRepository;
            _tellerOperationRepository = tellerOperationRepository;
            _tellerRepository = tellerRepository;
            _userInfoToken = userInfoToken;
            _ConfigRepository = configRepository;
            _mediator = mediator;
            _CurrencyNotesRepository = currencyNotesRepository;
            _subTellerProvioningHistoryRepository = subTellerProvioningHistoryRepository;
            _accountingDayRepository = accountingDayRepository;
            _generalDailyDashboardRepository = generalDailyDashboardRepository;
            _utilityServicesRepository=utilityServicesRepository;
        }

        /// <summary>
        /// Handles the LoanDisbursmentCommand to process loan disbursement transactions.
        /// </summary>
        /// <param name="request">The LoanDisbursmentCommand containing transaction details.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>Returns a ServiceResponse indicating the success or failure of the transaction.</returns>
        public async Task<ServiceResponse<bool>> Handle(LoanDisbursmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Handle Refinancing Logic
                // If the loan application type is refinancing, proceed to repay the old loan before disbursing the new one.
                if (request.LoanApplicationType == LoanApplicationTypes.Refinancing.ToString())
                {
                    var data = LoanRepaymentObjectForRefinancing.MockRepaymentObject(request);
                    var repaymentResults = await _mediator.Send(data);

                    // If repayment fails, log and return an error response.
                    if (repaymentResults.StatusCode != 200)
                    {
                        var errorMessage = $"Failed to make repayments of old loan. Error: {repaymentResults.Description}";
                        await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.LoanDisbursement, LogLevelInfo.Error);
                        return ServiceResponse<bool>.ReturnResultWith200(false, errorMessage);
                    }
                }

                // Step 2: Retrieve Accounting Day and Teller Details
                // Fetch the current accounting date for the branch.
                var accountingDate = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);

                // Verify that the user has an active teller account for the day.
                var dailyTeller = await _tellerProvisioningAccountRepository.GetAnActiveSubTellerForTheDate();

                // Ensure that the teller's day is still open.
                await _subTellerProvioningHistoryRepository.CheckIfDayIsStillOpened(dailyTeller.TellerId, accountingDate);

                // Retrieve teller details and associated teller account.
                var teller = await _tellerRepository.GetTeller(dailyTeller.TellerId);
                var tellerAccount = await _AccountRepository.RetrieveTellerAccount(teller);

                // Step 3: Fetch and Validate Sender's Loan Account
                // Retrieve the loan account for the customer.
                var loanAccount = await _AccountRepository.GetMemberLoanAccount(request.CustomerId);

                // Validate that the loan account exists.
                if (loanAccount == null)
                {
                    var errorMessage = "The member does not have a loan account. Contact member service for account creation.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.LoanDisbursement, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                // Check if the loan account has sufficient balance for the disbursement.
                if (loanAccount.Balance < request.Amount)
                {
                    var errorMessage = $"Insufficient balance in the loan account. Available: {loanAccount.Balance}, Requested: {request.Amount}.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanDisbursement, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return403(errorMessage);
                }

                string senderProductName = loanAccount.Product.Name;

                // Step 4: Fetch and Validate Receiver's Account
                // Retrieve the receiver's account details using the account number provided in the request.
                var receiverAccount = await GetReceiverAccount(request.ReceiverAccountNumber);

                // Validate that the receiver's account exists.
                if (receiverAccount == null)
                {
                    var errorMessage = $"Unknown account number: {request.ReceiverAccountNumber}.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanDisbursement, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return400(errorMessage);
                }

                // Ensure that the receiver's account is of an eligible type ('Deposit' or 'Saving').
                if (!IsEligibleReceiverAccount(receiverAccount))
                {
                    var errorMessage = $"Funds can only be credited to a 'Deposit' or 'Saving' account. Receiver Account Type: {receiverAccount.AccountType}.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanDisbursement, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return403(errorMessage);
                }

                string receiverProductName = receiverAccount.Product.Name;

                // Fetch additional details for the receiver, including customer and branch information.
                var receiverCustomer = await _utilityServicesRepository.GetCustomer(receiverAccount.CustomerId);
                var receiverBranch = await _utilityServicesRepository.GetBranch(receiverAccount.BranchId);

                // Step 5: Generate Transaction Reference
                // Create a unique transaction reference based on the loan application type.
                string transactionReference = CurrencyNotesMapper.GenerateTransactionReference(
                    _userInfoToken,
                    request.LoanApplicationType == LoanApplicationTypes.Refinancing.ToString()
                        ? TransactionType.Loan_Disbursement_Refinance.ToString()
                        : TransactionType.Loan_Disbursement.ToString(),
                    false);

                // Step 6: Process Transactions
                // Create a transaction entity for the sender and debit the loan account.
                var senderTransaction = await CreateSenderTransactionEntity(request, loanAccount, receiverAccount, 0, dailyTeller.TellerId, transactionReference, accountingDate);
                _AccountRepository.DebitAccount(loanAccount, request.Amount);

                // Calculate charges for the loan and adjust the disbursed amount.
                decimal loanCharge = request.ChargCollections.Sum(x => x.Amount);
                senderTransaction.Amount = request.Amount - request.OldLoanPayment.Amount;

                // Create a transaction entity for the receiver.
                var receiverTransaction = await CreateReceiverTransactionEntity(receiverAccount, loanAccount, senderTransaction, dailyTeller.TellerId, loanCharge, accountingDate);

                // Step 7: Update Teller Account and Notify
                // Update the teller account to reflect the loan disbursement and associated charges.
                UpdateTellerAccountBalances(tellerAccount, receiverTransaction, $"{receiverCustomer.FirstName} {receiverCustomer.LastName}", accountingDate, loanCharge, request.LoanApplicationType, request.RestructuredBalance);

               await _uow.SaveAsync();

                // Notify the sender and receiver about the transaction via SMS.
                await SendTransferSMSToSenderAndReceiver(loanAccount, receiverAccount, senderTransaction, receiverCustomer);

                // Step 8: Handle Accounting Postings
                // Perform the accounting posting for the transaction based on the loan type.
                var accountingResponse = await ProcessAccountingPosting(request, senderTransaction, receiverAccount, receiverProductName, loanCharge, accountingDate, receiverCustomer);

                // Check the result of the accounting posting and return the appropriate response.
                if (accountingResponse.StatusCode == 200)
                {
                    var successMessage = "Loan disbursement completed successfully.";
                    await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.LoanDisbursement, LogLevelInfo.Information);
                    return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
                }

                return ServiceResponse<bool>.ReturnResultWith200(false, accountingResponse.Description);
            }
            catch (Exception e)
            {
                // Step 9: Handle Exceptions
                // Log and return an error response in case of an exception.
                var errorMessage = $"Loan disbursement failed: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.LoanDisbursement, LogLevelInfo.Error);
                return ServiceResponse<bool>.Return500(e);
            }
        }
        /// <summary>
        /// Processes the accounting posting for loan disbursement based on the loan application type.
        /// </summary>
        /// <param name="request">The LoanDisbursmentCommand containing transaction details.</param>
        /// <param name="senderTransaction">The sender's transaction entity.</param>
        /// <param name="receiverAccount">The receiver's account details.</param>
        /// <param name="receiverProductName">The receiver account's product name.</param>
        /// <param name="loanCharge">The total loan charges applied.</param>
        /// <param name="accountingDate">The current accounting date.</param>
        /// <param name="receiverCustomer">The receiver's customer details.</param>
        /// <returns>Returns the ServiceResponse for the accounting posting.</returns>
        private async Task<ServiceResponse<bool>> ProcessAccountingPosting(
            LoanDisbursmentCommand request,
            Transaction senderTransaction,
            Account receiverAccount,
            string receiverProductName,
            decimal loanCharge,
            DateTime accountingDate,
            CustomerDto receiverCustomer)
        {
            bool feeIsFromMemberAccount = request.IsChargeInclussive;

            // Determine the loan application type
            string loanApplicationType = request.LoanApplicationType == LoanApplicationTypes.Normal.ToString()
                ? "New Loan"
                : request.LoanApplicationType;

            if (request.LoanApplicationType == LoanApplicationTypes.Normal.ToString())
            {
                // Create accounting posting request for normal loans
                var apiRequest = MakeAccountingPosting(
                    request.Amount,
                    receiverAccount,
                    senderTransaction.TransactionReference,
                    receiverProductName,
                    request.LoanProductId,
                    accountingDate,
                    feeIsFromMemberAccount,
                    request.ChargCollections,
                    receiverCustomer,
                    loanApplicationType);

                // Send accounting posting request
                var result = await _mediator.Send(apiRequest);

                // Return the result of the accounting posting
                return result.StatusCode == 200
                    ? ServiceResponse<bool>.ReturnResultWith200(true, "Accounting posting successful.")
                    : ServiceResponse<bool>.ReturnResultWith200(false, result.Description);
            }
            else if (request.LoanApplicationType == LoanApplicationTypes.Refinancing.ToString())
            {
                // Adjust requested amount to account for loan charges
                request.RequestedAmount -= loanCharge;

                // Create accounting posting request for refinancing
                var apiRequest = MakeAccountingPostingRefinancing(
                    request.RequestedAmount,
                    request.RestructuredBalance,
                    receiverAccount,
                    senderTransaction.TransactionReference,
                    receiverProductName,
                    request.LoanProductId,
                    accountingDate,
                    feeIsFromMemberAccount,
                    request.ChargCollections,
                    receiverCustomer,
                    loanApplicationType,
                    loanCharge);

                // Send accounting posting request
                var result = await _mediator.Send(apiRequest);

                // Return the result of the accounting posting
                return result.StatusCode == 200
                    ? ServiceResponse<bool>.ReturnResultWith200(true, "Accounting posting successful.")
                    : ServiceResponse<bool>.ReturnResultWith200(false, result.Description);
            }

            // Default response for unsupported loan application types
            return ServiceResponse<bool>.ReturnResultWith200(false, "Unsupported loan application type for accounting posting.");
        }

        /// <summary>
        /// Determines if the receiver's account is eligible to receive a loan disbursement.
        /// </summary>
        /// <param name="receiverAccount">The account to validate.</param>
        /// <returns>True if the account is of type 'Deposit' or 'Saving'; otherwise, false.</returns>
        private bool IsEligibleReceiverAccount(Account receiverAccount)
        {
            // Convert the account type to lowercase for case-insensitive comparison.
            string accountType = receiverAccount.AccountType.ToLower();

            // Check if the account type is 'Deposit' or 'Saving'.
            return accountType == AccountType.Deposit.ToString().ToLower() ||
                   accountType == AccountType.Saving.ToString().ToLower();
        }


        /// <summary>
        /// Sends SMS notifications to both the sender and receiver about the transfer details.
        /// </summary>
        /// <param name="senderAccount">The sender's account details.</param>
        /// <param name="receiverAccount">The receiver's account details.</param>
        /// <param name="transaction">The transaction details.</param>
        /// <param name="receiverCustomer">The receiver's customer details.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SendTransferSMSToSenderAndReceiver(Account senderAccount, Account receiverAccount, Transaction transaction, CustomerDto receiverCustomer)
        {
            // Step 1: Prepare the SMS message for the receiver.
            // The message includes the transaction amount, account type, transaction reference, and transaction date.
            string receiverMsg = $"Dear {receiverCustomer.FirstName} {receiverCustomer.LastName}, " +
                                 $"{BaseUtilities.FormatCurrency(transaction.OriginalDepositAmount)} has been disbursed to your {senderAccount.AccountType} account.\n" +
                                 $"Transaction Reference: {transaction.TransactionReference}.\n" +
                                 $"Date and Time: {transaction.CreatedDate}.";

            // Step 2: Create a command object to send the SMS to the receiver.
            var receiverSmsCommand = new SendSMSPICallCommand
            {
                messageBody = receiverMsg, // The message body to be sent.
                recipient = receiverCustomer.Phone // The phone number of the receiver.
            };

            // Step 3: Dispatch the SMS sending command using the mediator.
            // This ensures the operation follows a clean architecture and allows for decoupled handling.
            await _mediator.Send(receiverSmsCommand);
        }
        private AddDisbursmementRefinancingCommand MakeAccountingPostingRefinancing(
      decimal amountToDisburse,
      decimal amountToRepayToCash,
      Account receiverAccount,
      string reference,
      string productName,
      string loanProductId,
      DateTime accountingDate,
      bool feeIsFromMembersAccount,
      List<ChargCollection> chargeCollections,
      CustomerDto customer,
      string applicationType, decimal loanCharges)
        {
            // Construct a simplified and clear narration for GL posting
            string narration =
                $"Loan Disbursement - {applicationType}: " +
                $"Member: {customer.FirstName} {customer.LastName} (ID: {customer.CustomerId}), " +
                $"Disbursed Amount: {BaseUtilities.FormatCurrency(amountToDisburse)} FEE From Member: {BaseUtilities.FormatCurrency(loanCharges)} , " +
                $"Repaid to Cash: {BaseUtilities.FormatCurrency(amountToRepayToCash)}, " +
                $"Reference: {reference}.";



            // Prepare the Disbursement Collections from charge collections
            var disbursementCollections = chargeCollections?.Select(charg => new DisbursementCollection
            {
                Amount = charg.Amount,
                EventCode = charg.EventCode,
                Naration =
                    $"Loan Disbursement Fee [{charg.ChargeName}] for {applicationType}: " +
                    $"Member: {customer.FirstName} {customer.LastName} (ID: {customer.CustomerId}), " +
                    $"Amount: {BaseUtilities.FormatCurrency(charg.Amount)}, Reference: {reference}."
            }).ToList() ?? new List<DisbursementCollection>();


            // Initialize the command with primary properties
            var addAccountingPostingCommand = new AddDisbursmementRefinancingCommand
            {
                Naration = narration,
                AmountToDisbursed = amountToDisburse,
                AmountToPayBackToCash = amountToRepayToCash,
                MemberReference = customer.CustomerId,
                LoanProductId = loanProductId,
                TransactionDate = accountingDate,
                IsCommissionFromMember=feeIsFromMembersAccount,
                DisbursementCollections=disbursementCollections,
                DisbursementType=applicationType,
                TransactionReferenceId = reference,
                AccountHolder = receiverAccount.AccountName,
                SavingProductId = receiverAccount.ProductId,
                SavingProductName = productName
            };

            return addAccountingPostingCommand;
        }

        private AddDisbursmementPostingCommand MakeAccountingPosting(
     decimal amount,
     Account receiverAccount,
     string reference,
     string productName,
     string loanProductId,
     DateTime accountingDate,
     bool feeIsFromMembersAccount,
     List<ChargCollection> chargeCollections,
     CustomerDto customer,
     string applicationType)
        {
            // Construct the main narration for the accounting posting
            string mainNarration =
                $"Loan Disbursement for {applicationType}: " +
                $"Member: {customer.FirstName} {customer.LastName} (ID: {customer.CustomerId}), " +
                $"Amount: {BaseUtilities.FormatCurrency(amount)}, Reference: {reference}.";

            // Prepare the Disbursement Collections from charge collections
            var disbursementCollections = chargeCollections?.Select(charg => new DisbursementCollection
            {
                Amount = charg.Amount,
                EventCode = charg.EventCode,
                Naration =
                    $"Loan Disbursement Fee [{charg.ChargeName}] for {applicationType}: " +
                    $"Member: {customer.FirstName} {customer.LastName} (ID: {customer.CustomerId}), " +
                    $"Amount: {BaseUtilities.FormatCurrency(charg.Amount)}, Reference: {reference}."
            }).ToList() ?? new List<DisbursementCollection>();

            // Initialize the AddDisbursmementPostingCommand object
            var addAccountingPostingCommand = new AddDisbursmementPostingCommand
            {
                Naration = mainNarration,
                Amount = amount,
                MemberReference = customer.CustomerId,
                LoanProductId = loanProductId,
                TransactionDate = accountingDate,
                TransactionReferenceId = reference,
                AccountHolder = receiverAccount.AccountName,
                SavingProductId = receiverAccount.ProductId,
                SavingProductName = productName,
                IsCommissionFromMember = feeIsFromMembersAccount,
                TellerSourceForLoanCommission = feeIsFromMembersAccount
                    ? TellerSources.MemberCommission.ToString()
                    : TellerSources.Physical_Teller.ToString(),
                DisbursementCollections = disbursementCollections
            };

            return addAccountingPostingCommand;
        }


        private async Task<Account> GetReceiverAccount(string accountNumber)
        {
            return await _AccountRepository
                .FindBy(a => a.AccountNumber == accountNumber)
                .Include(a => a.Product)
                    .ThenInclude(t => t.TransferParameters)
                .AsNoTracking() // Apply no tracking here
                .FirstOrDefaultAsync();
        }





        // Helper method to create sender transaction entity
        private async Task<Transaction> CreateSenderTransactionEntity(LoanDisbursmentCommand request, Account senderAccount, Account receiverAccount, decimal Charges, string tellerid, string Reference, DateTime accountingDate)
        {

            Charges = request.IsChargeInclussive ? request.ChargCollections.Sum(x => x.Amount) : 0;
            decimal Balance = senderAccount.Balance - request.Amount;

            string N_A = "N/A";
            var transaction = new Transaction
            {
                Id = BaseUtilities.GenerateUniqueNumber(), // Generate unique transaction ID
                CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow), // Set transaction creation date
                Status = TransactionStatus.COMPLETED.ToString(), // Set transaction status to completed
                TransactionReference = Reference, // Set transaction reference
                ExternalReference = request.LoanId, // Set external reference
                IsExternalOperation = false, // Set external operation flag
                ExternalApplicationName = "LoanMS", // Set external application name
                SourceType = OperationSourceType.Web_Portal.ToString(), // Set source type
                Currency = "XAF", // Set currency
                TransactionType = TransactionType.TRANSFER.ToString(),
                AccountNumber = senderAccount.AccountNumber, // Set account number
                PreviousBalance = senderAccount.Balance, // Set previous balance
                AccountId = senderAccount.Id, // Set account ID
                CustomerId = senderAccount.CustomerId, // Set customer ID
                ProductId = senderAccount.ProductId, // Set product ID
                SenderAccountId = senderAccount.Id, // Set sender account ID
                ReceiverAccountId = receiverAccount.Id, // Set receiver account ID
                BankId = senderAccount.BankId, // Set bank ID
                Operation = TransactionType.Loan_Disbursement.ToString(), // Set operation type (deposit)
                BranchId = senderAccount.BranchId, // Set branch ID
                OriginalDepositAmount = request.Amount, // Set original deposit amount including fees
                Fee = Charges,
                Tax = 0, // Set tax (assuming tax is not applicable)
                Amount = request.Amount, // Set amount (excluding fees)
                Note = $"Note: {request.Note} - The amount of {BaseUtilities.FormatCurrency(request.Amount)} has been successfully disbursed from the loan transit account to your {receiverAccount.AccountType} account. A charge of {BaseUtilities.FormatCurrency(Charges)} was applied for this transaction. The transaction reference is [{Reference}]. For further assistance or inquiries, please contact our member services team.",
                OperationType = OperationType.Debit.ToString(), // Set operation type (credit)
                FeeType = Events.None.ToString(), // Set fee type
                TellerId = tellerid, // Set teller ID
                DepositerNote = N_A, // Set depositer note
                DepositerTelephone = N_A, // Set depositer telephone
                DepositorIDNumber = N_A, // Set depositor ID number
                DepositorIDExpiryDate = N_A, // Set depositor ID expiry date
                DepositorIDIssueDate = N_A, // Set depositor ID issue date
                DepositorIDNumberPlaceOfIssue = N_A, // Set depositor ID place of issue
                IsDepositDoneByAccountOwner = true, // Set flag indicating if deposit is done by account owner
                DepositorName = N_A, // Set depositor name
                Balance = Balance, // Set balance after deposit (including original amount)
                Credit = 0, // Set credit amount (including fees if IsOfflineCharge is true, otherwise excluding fees)
                Debit = request.Amount, // Set debit amount (assuming no debit)
                DestinationBrachId = receiverAccount.BranchId,
                OperationCharge = 0,
                AccountingDate = accountingDate,
                WithrawalFormCharge = 0,  // Set destination branch ID
                SourceBrachId = receiverAccount.BranchId, // Set source branch ID
                IsInterBrachOperation = false, // Set flag indicating if inter-branch operation
                DestinationBranchCommission = 0,
                SourceBranchCommission = 0,
                ReceiptTitle = $"Loan Disbursement Receipt Reference: " + Reference,

            };
            _TransactionRepository.Add(transaction);

            return transaction; // Return the transaction entity
        }


        /// <summary>
        /// Creates and processes transactions for a receiver account, including loan charges if applicable.
        /// </summary>
        private async Task<Transaction> CreateReceiverTransactionEntity(
            Account receiverAccount,
            Account senderAccount,
            Transaction senderTransaction,
            string tellerId,
            decimal loanCharge, DateTime accountingDate)
        {
            // Constants for placeholder values
            const string N_A = "N/A";

            // Calculate balances
            var previousBalance = receiverAccount.Balance;
            var newBalance = receiverAccount.Balance + (senderTransaction.Amount - loanCharge);

            // Create the credit transaction for the receiver
            var creditTransaction = CreateTransaction(
                account: receiverAccount,
                senderAccount: senderAccount,
                transactionReference: senderTransaction.TransactionReference,
                externalReference: senderTransaction.ExternalReference,
                amount: senderTransaction.Amount,
                tellerId: tellerId,
                operationType: OperationType.Credit,
                note: $"You have received a transfer of {BaseUtilities.FormatCurrency(senderTransaction.Amount)} from your {senderAccount.AccountType} account, Number: [{senderAccount.AccountNumber}]. " +
                      $"Charges amounting to {BaseUtilities.FormatCurrency(senderTransaction.Fee)} were applied. " +
                      $"Transaction reference: [{senderTransaction.TransactionReference}].",
                balance: receiverAccount.Balance + senderTransaction.Amount,
                feeType: senderTransaction.FeeType,
                debit: 0,
                credit: senderTransaction.Amount, accountingDate
            );

            // Add the credit transaction to the repository
            _TransactionRepository.Add(creditTransaction);

            // If there's a loan charge, create a separate debit transaction
            if (loanCharge > 0)
            {
                var debitTransaction = CreateTransaction(
                    account: receiverAccount,
                    senderAccount: senderAccount,
                    transactionReference: senderTransaction.TransactionReference,
                    externalReference: senderTransaction.ExternalReference,
                    amount: loanCharge,
                    tellerId: tellerId,
                    operationType: OperationType.Debit,
                    note: $"A loan fee charge of {BaseUtilities.FormatCurrency(loanCharge)} has been deducted from your {receiverAccount.AccountType} account Number: [{receiverAccount.AccountNumber}]. " +
                          $"This deduction pertains to charges associated with the processing of your loan. " +
                          $"Transaction reference: [{senderTransaction.TransactionReference}].",
                    balance: newBalance,
                    feeType: Events.LoanFee.ToString(),
                    debit: loanCharge,
                    credit: 0, accountingDate
                );

                // Add the debit transaction to the repository
                _TransactionRepository.Add(debitTransaction);
            }

            // Credit the receiver's account with the net amount
            decimal netAmount = senderTransaction.Amount - loanCharge;
            _AccountRepository.CreditAccount(receiverAccount, netAmount);

            // Return the credit transaction as the result
            return creditTransaction;
        }

        /// <summary>
        /// Helper method to create a transaction entity with common fields.
        /// </summary>
        private Transaction CreateTransaction(
            Account account,
            Account senderAccount,
            string transactionReference,
            string externalReference,
            decimal amount,
            string tellerId,
            OperationType operationType,
            string note,
            decimal balance,
            string feeType,
            decimal debit,
            decimal credit, DateTime accountingDate)
        {
            const string N_A = "N/A";

            return new Transaction
            {
                Id = BaseUtilities.GenerateUniqueNumber(), // Generate unique transaction ID
                CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow), // Set transaction creation date
                Status = TransactionStatus.COMPLETED.ToString(), // Set transaction status to completed
                TransactionReference = transactionReference, // Set transaction reference
                ExternalReference = externalReference, // Set external reference
                IsExternalOperation = false, // Set external operation flag
                ExternalApplicationName = N_A, // Set external application name
                SourceType = OperationSourceType.BackOffice_Operation.ToString(), // Set source type
                Currency = "XAF", // Set currency
                TransactionType = TransactionType.Loan_Disbursement.ToString(), // Set transaction type
                AccountNumber = account.AccountNumber, // Set account number
                PreviousBalance = account.Balance, // Set previous balance
                AccountId = account.Id, // Set account ID
                CustomerId = account.CustomerId, // Set customer ID
                ProductId = account.ProductId, // Set product ID
                SenderAccountId = senderAccount.Id, // Set sender account ID
                ReceiverAccountId = account.Id, // Set receiver account ID
                BankId = senderAccount.BankId, // Set bank ID
                Operation = TransactionType.Loan_Disbursement.ToString(), // Set operation type
                BranchId = senderAccount.BranchId, // Set branch ID
                OriginalDepositAmount = amount, // Set deposit amount
                Fee = 0, // Set fee (assumed 0 for this helper)
                Tax = 0, // Set tax (assumed 0)
                Amount = amount, // Set transaction amount
                Note = note, // Set transaction note
                OperationType = operationType.ToString(), // Set operation type (credit/debit)
                FeeType = feeType, // Set fee type
                TellerId = tellerId, // Set teller ID
                DepositerNote = N_A, // Set depositer note
                DepositerTelephone = N_A, // Set depositer telephone
                DepositorIDNumber = N_A, // Set depositor ID number
                DepositorIDExpiryDate = N_A, // Set depositor ID expiry date
                DepositorIDIssueDate = N_A, // Set depositor ID issue date
                DepositorIDNumberPlaceOfIssue = N_A, // Set depositor ID place of issue
                IsDepositDoneByAccountOwner = true, // Set flag indicating if deposit is done by account owner
                DepositorName = N_A, // Set depositor name
                Balance = balance, // Set balance after transaction
                Credit = credit, // Set credit amount
                Debit = debit, // Set debit amount
                DestinationBrachId = account.BranchId, // Set destination branch ID
                OperationCharge = 0, // Set operation charge
                AccountingDate = accountingDate, // Set accounting date
                WithrawalFormCharge = 0, // Set withdrawal form charge
                SourceBrachId = account.BranchId, // Set source branch ID
                IsInterBrachOperation = false, // Set inter-branch operation flag
                DestinationBranchCommission = 0, // Set destination branch commission
                SourceBranchCommission = 0, // Set source branch commission
                ReceiptTitle = $"{operationType} Receipt Reference: {transactionReference}" // Set receipt title
            };
        }


        private void UpdateTellerAccountBalances(
    Account tellerAccount,
    Transaction transaction,
    string memberName,
    DateTime accountingDate,
    decimal loanCharge,
    string loanApplicationType,
    decimal paybackAmount)
        {
            if (loanApplicationType == LoanApplicationTypes.Refinancing.ToString())
            {
                if (loanCharge > 0)
                {
                    // Handle Refinancing with charges
                    decimal netChange = loanCharge - paybackAmount;
                    _AccountRepository.CreditAndDebitAccount(tellerAccount, loanCharge, paybackAmount);

                    var subTellerProvisioning = _subTellerProvioningHistoryRepository.CashInAndOutByDenomination(
                        paybackAmount, loanCharge, null, tellerAccount.TellerId, accountingDate, tellerAccount.OpenningOfDayReference);

                    var tellerOperationFee = CreateTellerOperation(
                        loanCharge, OperationType.Credit, tellerAccount, transaction, Events.LoanFee_Refinancing.ToString(), memberName, accountingDate);
                    _tellerOperationRepository.Add(tellerOperationFee);

                    var tellerOperationRepayAmount = CreateTellerOperation(
                        paybackAmount, OperationType.Debit, tellerAccount, transaction, Events.Loan_Refinancing.ToString(), memberName, accountingDate);
                    _tellerOperationRepository.Add(tellerOperationRepayAmount);
                }
                else
                {
                    // Handle Refinancing with no charges
                    _AccountRepository.CreditAccount(tellerAccount, paybackAmount);

                    var autoNotesDebit = CurrencyNotesMapper.CalculateCurrencyNotes(paybackAmount);
                    var subTellerProvisioning = _subTellerProvioningHistoryRepository.CashOutByDinomination(
                        paybackAmount, autoNotesDebit, tellerAccount.TellerId, accountingDate, tellerAccount.OpenningOfDayReference);

                    var tellerOperationRepayAmount = CreateTellerOperation(
                        paybackAmount, OperationType.Debit, tellerAccount, transaction, Events.Loan_Refinancing.ToString(), memberName, accountingDate);
                    _tellerOperationRepository.Add(tellerOperationRepayAmount);
                }
            }
            else if (loanApplicationType == LoanApplicationTypes.Normal.ToString())
            {
                if (loanCharge > 0)
                {
                    // Handle Normal Loan Type with charges
                    _AccountRepository.CreditAccount(tellerAccount, loanCharge);

                    var tellerOperationWithFeeCharges = CreateTellerOperation(
                        loanCharge, OperationType.Credit, tellerAccount, transaction, Events.LoanFee.ToString(), memberName, accountingDate);
                    _tellerOperationRepository.Add(tellerOperationWithFeeCharges);
                }
            }
            else
            {
                throw new InvalidOperationException(
                    $"Loan type '{loanApplicationType}' is not supported for this operation. Please contact support or check the loan application details.");
            }
        }




        private TellerOperation CreateTellerOperation(decimal amount, OperationType operationType, Account tellerAccount, Transaction transactionEntityEntryFee, string eventType, string memberName, DateTime AccountingDate)
        {
            return new TellerOperation
            {
                Id = BaseUtilities.GenerateUniqueNumber(),
                AccountID = tellerAccount.Id,
                AccountNumber = tellerAccount.AccountNumber,
                Amount = amount,
                OperationType = operationType.ToString(),
                BranchId = tellerAccount.BranchId,
                OpenOfDayReference = tellerAccount.OpenningOfDayReference,
                BankId = tellerAccount.BankId,
                TransactionType = eventType,
                ReferenceId = ReferenceNumberGenerator.GenerateReferenceNumber(_userInfoToken.BranchCode),
                CustomerId = transactionEntityEntryFee.CustomerId,
                MemberAccountNumber = transactionEntityEntryFee.AccountNumber,
                CurrentBalance = tellerAccount.Balance,
                Date = AccountingDate,
                AccountingDate = AccountingDate,
                PreviousBalance = tellerAccount.PreviousBalance,
                TellerID = tellerAccount.TellerId,
                TransactionReference = transactionEntityEntryFee.TransactionReference,
                UserID = _userInfoToken.Id,
                IsCashOperation=true,
                MemberName = memberName,
                DestinationBrachId = transactionEntityEntryFee.DestinationBrachId,
                DestinationBranchCommission = transactionEntityEntryFee.DestinationBranchCommission,
                IsInterBranch = transactionEntityEntryFee.IsInterBrachOperation,
                SourceBranchCommission = transactionEntityEntryFee.SourceBranchCommission,
                SourceBranchId = transactionEntityEntryFee.SourceBrachId,
                EventName = eventType,
                Description = $"{transactionEntityEntryFee.TransactionType} of {BaseUtilities.FormatCurrency(amount)} was paid to account number: {tellerAccount.AccountNumber}",
            };
        }




    }
}