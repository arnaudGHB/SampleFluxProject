using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Data.Entity.OtherCashInP;
using CBS.TransactionManagement.MediatR.LoanRepayment.Command;
using CBS.TransactionManagement.Repository.Receipts.Payments;
using CBS.TransactionManagement.Data.Dto.Receipts.Details;
using CBS.TransactionManagement.Data.Dto.Receipts.Payments;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Commands;
using CBS.TransactionManagement.Repository.DailyStatisticBoard;
using CBS.TransactionManagement.MediatR.UtilityServices;

namespace CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Services.NewFolder
{

    public class LoanProcessingFeeServices : ILoanProcessingFeeServices
    {
        private readonly IAccountRepository _accountRepository; // Repository for accessing account data.
        private readonly UserInfoToken _userInfoToken; // User information token.
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvioningHistoryRepository; // Repository for accessing teller data.
        private readonly IDailyTellerRepository _tellerProvisioningAccountRepository; // Repository for accessing teller provisioning account data.
        private readonly ITellerRepository _tellerRepository; // Repository for accessing teller data.
        private readonly IPaymentReceiptRepository _paymentReceiptRepository;
        private readonly IMapper _mapper;
        private readonly IGeneralDailyDashboardRepository _generalDailyDashboardRepository; // Repository for accessing account data.
        private readonly ITransactionRepository _transactionRepository;
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository;
        public IMediator _mediator { get; set; } // Mediator for handling requests.
        private readonly ILogger<LoanProcessingFeeServices> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for transaction context.

        public LoanProcessingFeeServices(
            IAccountRepository AccountRepository,
            UserInfoToken UserInfoToken,
            ILogger<LoanProcessingFeeServices> logger,
            IUnitOfWork<TransactionContext> uow,
            IDailyTellerRepository tellerProvisioningAccountRepository = null,
            ITellerRepository tellerRepository = null,
            IMediator mediator = null,
            ISubTellerProvisioningHistoryRepository subTellerProvioningHistoryRepository = null,
            ITransactionRepository transactionRepository = null,
            ITellerOperationRepository tellerOperationRepository = null,
            IPaymentReceiptRepository paymentReceiptRepository = null,
            IMapper mapper = null,
            IGeneralDailyDashboardRepository generalDailyDashboardRepository = null,
            IAPIUtilityServicesRepository utilityServicesRepository = null)
        {
            _userInfoToken = UserInfoToken; // Initialize user information token.
            _accountRepository = AccountRepository; // Initialize account repository.
            _logger = logger; // Initialize logger.
            _uow = uow; // Initialize unit of work.
            _tellerProvisioningAccountRepository = tellerProvisioningAccountRepository; // Initialize teller provisioning account repository.
            _tellerRepository = tellerRepository; // Initialize teller repository.
            _mediator = mediator; // Initialize _mediator.
            _subTellerProvioningHistoryRepository = subTellerProvioningHistoryRepository;
            _transactionRepository = transactionRepository;
            _tellerOperationRepository = tellerOperationRepository;
            _paymentReceiptRepository = paymentReceiptRepository;
            _mapper = mapper;
            _generalDailyDashboardRepository = generalDailyDashboardRepository;
            _utilityServicesRepository=utilityServicesRepository;
        }

        /// <summary>
        /// Handles loan processing fee payments, processes bulk deposit transactions, confirms fee payments, and posts accounting entries.
        /// </summary>
        /// <param name="request">The bulk operation request containing payment details.</param>
        /// <param name="accountingDate">The accounting date for the transaction.</param>
        /// <param name="config">System configuration details.</param>
        /// <returns>A service response containing the payment receipt details or an error message.</returns>
        public async Task<ServiceResponse<PaymentReceiptDto>> LoanProcessingFeePayment(AddBulkOperationDepositCommand request, DateTime accountingDate, Config config)
        {
            try
            {
                // Step 1: Initialize flags for inter-branch operation and membership activation.
                bool isInterBranchOperation = false;

                // Step 2: Retrieve the active teller and verify that the accounting day is still open.
                var dailyTeller = await _tellerProvisioningAccountRepository.GetAnActiveSubTellerForTheDate();
                await _subTellerProvioningHistoryRepository.CheckIfDayIsStillOpened(dailyTeller.TellerId, accountingDate);

                // Step 3: Retrieve teller details and check operational rights.
                var teller = await _tellerRepository.RetrieveTeller(dailyTeller);
                await _tellerRepository.CheckTellerOperationalRights(teller, request.OperationType, request.IsCashOperation);
                var tellerAccount = await _accountRepository.RetrieveTellerAccount(teller);

                // Step 4: Retrieve customer and branch information.
                var customer = await _utilityServicesRepository.GetCustomer(request.BulkOperations.FirstOrDefault().CustomerId);
                var branch = await _utilityServicesRepository.GetBranch(customer.BranchId);

                // Step 5: Check for inter-branch operation.
                if (teller.BranchId != customer.BranchId)
                {
                    isInterBranchOperation = true;
                }

                // Step 6: Generate a unique transaction reference based on branch type.
                string reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, TransactionType.DEPOSIT.ToString(), isInterBranchOperation);

                // Step 7: Calculate the total payment amount and retrieve currency notes.
                decimal amount = CalculateTotalAmount(request.BulkOperations);
                var currencyNote = await RetrieveCurrencyNotes(reference, request.BulkOperations.FirstOrDefault().currencyNotes);
                var notes = request.BulkOperations.FirstOrDefault().currencyNotes;

                // Step 8: Map bulk operations to bulk deposits and retrieve the customer’s loan account.
                var bulkDeposits = CurrencyNotesMapper.MapBulkOperationListToBulkDepositList(request.BulkOperations);
                var customerAccount = await _accountRepository.GetMemberLoanAccount(customer.CustomerId);

                // Step 9: Record cash-in by denomination.
                var subTellerProvisioning = _subTellerProvioningHistoryRepository.CashInByDinomination(amount, notes, teller.Id, accountingDate, tellerAccount.OpenningOfDayReference);

                // Step 10: Process bulk transactions and map them to the required format.
                var transaction = await ProcessBulkTransactionOthers(teller, tellerAccount, isInterBranchOperation, customer, currencyNote.Data.ToList(), branch, reference, config, amount, accountingDate, customerAccount);

                // Step 11: Prepare loan payment requests for fee confirmation.
                var loanPaymentRequest = request.BulkOperations.Select(a => new LoanPaymentRequest
                {
                    Amount = a.Amount,
                    LoanApplicationFeeId = a.AccountNumber
                }).ToList();

                // Step 12: Send command to confirm fee payment using the mediator pattern.
                var feePaymentConfirmationCommand = new FeePaymentConfirmationCommand
                {
                    LoanApplicationId = request.BulkOperations.FirstOrDefault().LoanApplicationId,
                    PaymentRequests = loanPaymentRequest,
                    TransactionReference = reference,
                    Period = request.Period
                };
                var feePaymentConfirmationResponse = await _mediator.Send(feePaymentConfirmationCommand);

                // Step 13: Handle any errors during fee payment confirmation.
                if (feePaymentConfirmationResponse.StatusCode != 200)
                {
                    var errorMessage = $"Failed to pay loan fee. Loan API Error: {feePaymentConfirmationResponse.Message}";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.LoanFeeDeposit, LogLevelInfo.Error);
                    _logger.LogError(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                // Step 14: Prepare payment receipt details from fee payment confirmations.
                var accountDeposits = new List<AccountDeposit>();
                var paymentReceipts = feePaymentConfirmationResponse.Data.Select(feePayment => new PaymentDetailObject
                {
                    AccountNumber = customerAccount.AccountNumber,
                    Fee = 0,
                    Amount = feePayment.AmountPaid,
                    Interest = 0,
                    LoanCapital = 0,
                    SericeOrEventName = feePayment.FeeName,
                    VAT = 0,
                    Balance = 0
                }).ToList();

                // Step 15: Create a payment processing request.
                var transactions = new List<TransactionDto> { transaction };
                var paymentProcessing = new PaymentProcessingRequest
                {
                    AccountingDay = accountingDate,
                    Amount = amount,
                    MemberName = customer.Name,
                    NotesRequest = notes,
                    OperationType = OperationType.Cash.ToString(),
                    OperationTypeGrouping = TransactionType.CASH_IN.ToString(),
                    PortalUsed = OperationSourceType.Web_Portal.ToString(),
                    PaymentDetails = paymentReceipts,
                    ServiceType = TransactionType.CASH_IN_LOAN_FEE.ToString(),
                    SourceOfRequest = OperationSourceType.Physical_Teller.ToString(),
                    TotalAmount = amount,
                    TotalCharges = 0,
                    Transactions = transactions
                };

                // Step 16: Process payment and retrieve the receipt.
                var paymentReceipt = _paymentReceiptRepository.ProcessPaymentAsync(paymentProcessing);

                // Step 17: Update the dashboard with the loan fee operation.
                var tellerBranch = await GetBranch(teller.BranchId);
                var cashOperation = new CashOperation(teller.BranchId, paymentProcessing.Amount, paymentProcessing.TotalCharges, tellerBranch.name, tellerBranch.branchCode, CashOperationType.LoanFee, LogAction.LoanFeeDeposit, subTellerProvisioning);
                await _generalDailyDashboardRepository.UpdateOrCreateDashboardAsync(cashOperation);

                // Step 18: Save changes to the database.
                await _uow.SaveAsync();

                // Step 19: Post accounting events for the fee payments.
                var accountingResponseMessages = await PostAccountingEvent(feePaymentConfirmationResponse.Data, reference, accountingDate, customer.CustomerId, customer.Name);

                // Step 20: Prepare success or failure messages.
                string accountDetails = string.Join("\n", feePaymentConfirmationResponse.Data.Select((ad, index) => $"{index + 1}. {ad.FeeName}: {BaseUtilities.FormatCurrency(ad.AmountPaid)}"));
                var paymentReceiptDto = _mapper.Map<PaymentReceiptDto>(paymentReceipt);
                string successMessage = accountingResponseMessages == null
                    ? $"Account Posting: Successful. Loan fee payment of {BaseUtilities.FormatCurrency(amount)} was processed by {teller.Name} [User: {dailyTeller.UserName}] for Member: {customer.Name}. The fees paid include:\n{accountDetails}"
                    : $"Account Posting: Failed. Loan fee payment of {BaseUtilities.FormatCurrency(amount)} was processed by {teller.Name} [User: {dailyTeller.UserName}] for Member: {customer.Name}. The fees paid include:\n{accountDetails}";

                // Step 21: Log and audit the result.
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.LoanFeeDeposit, LogLevelInfo.Information);

                // Return success response.
                return ServiceResponse<PaymentReceiptDto>.ReturnResultWith200(paymentReceiptDto, successMessage);
            }
            catch (Exception e)
            {
                // Handle and log any errors that occur.
                var errorMessage = $"Error occurred while processing loan fee payment: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.LoanFeeDeposit, LogLevelInfo.Error);
                return ServiceResponse<PaymentReceiptDto>.Return500(e);
            }
        }

        //public async Task<ServiceResponse<PaymentReceiptDto>> LoanProcessingFeePayment(AddBulkOperationDepositCommand request, DateTime accountingDate, Config config)
        //{
        //    try
        //    {
        //        // Initialize variables
        //        bool isInterBranchOperation = false; // Flag to indicate if the operation is inter-branch.
        //        bool isFirstDeposit = false; // Flag to indicate if it's the first deposit for membership activation.

        //        // Retrieve active teller account
        //        var dailyTeller = await _tellerProvisioningAccountRepository.GetAnActiveSubTellerForTheDate();
        //        await _subTellerProvioningHistoryRepository.CheckIfDayIsStillOpened(dailyTeller.TellerId, accountingDate);
        //        var teller = await _tellerRepository.RetrieveTeller(dailyTeller);
        //        var tellerAccount = await _accountRepository.RetrieveTellerAccount(teller);
        //        // Check teller rights.
        //        await _tellerRepository.CheckTellerOperationalRights(teller, request.OperationType, request.IsCashOperation);

        //        // Retrieve customer information
        //        var customer = await _utilityServicesRepository.GetCustomer(request.BulkOperations.FirstOrDefault().CustomerId);

        //        // Retrieve branch information
        //        var branch = await _utilityServicesRepository.GetBranch(customer.BranchId);
        //        // Check if it's an inter-branch operation
        //        if (teller.BranchId != customer.BranchId)
        //        {
        //            isInterBranchOperation = true;
        //        }
        //        // Generate transaction reference based on branch type
        //        string reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, TransactionType.DEPOSIT.ToString(), isInterBranchOperation);

        //        // Calculate total amount
        //        decimal amount = CalculateTotalAmount(request.BulkOperations);

        //        // Retrieve currency notes
        //        var currencyNote = await RetrieveCurrencyNotes(reference, request.BulkOperations.FirstOrDefault().currencyNotes);
        //        var notes = request.BulkOperations.FirstOrDefault().currencyNotes;
        //        // Map BulkOperation objects to BulkDeposit objects
        //        var bulkDeposits = CurrencyNotesMapper.MapBulkOperationListToBulkDepositList(request.BulkOperations);
        //        var customerAccount = await _accountRepository.GetMemberLoanAccount(customer.CustomerId); // Get customer account information.
        //        var subTellerProvioning = new SubTellerProvioningHistory();

        //        // Process transactions in bulk
        //        var transaction = await ProcessBulkTransactionOthers(teller, tellerAccount, isInterBranchOperation, customer, currencyNote.Data.ToList(), branch, reference, config, amount, accountingDate, customerAccount);

        //        // Activate membership if it's the first deposit
        //        //var activateMember = await ActivateMembership(customer.CustomerId, memberActivationPolicy.PolicyID, memberActivationPolicy.Total, isFirstDeposit);

        //        // Prepare loan payment requests
        //        var loanPaymentRequest = request.BulkOperations.Select(a => new LoanPaymentRequest { Amount = a.Amount, LoanApplicationFeeId = a.AccountNumber }).ToList();
        //        subTellerProvioning= _subTellerProvioningHistoryRepository.CashInByDinomination(amount, request.BulkOperations.FirstOrDefault().currencyNotes, teller.Id, accountingDate, tellerAccount.OpenningOfDayReference);
        //        // Send command to _mediator for fee payment confirmation
        //        var feePaymentConfirmationCommand = new FeePaymentConfirmationCommand { LoanApplicationId = request.BulkOperations.FirstOrDefault().LoanApplicationId, PaymentRequests = loanPaymentRequest, TransactionReference = reference, Period = request.Period };
        //        var feePaymentConfirmationResponse = await _mediator.Send(feePaymentConfirmationCommand);

        //        // Handle fee payment confirmation response
        //        if (feePaymentConfirmationResponse.StatusCode != 200)
        //        {
        //            var errorMessage = $"Failed to pay loan fee. Loan API Error: {feePaymentConfirmationResponse.Message}";
        //            await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.LoanFeeDeposit, LogLevelInfo.Error);

        //            _logger.LogError(errorMessage);
        //            throw new InvalidOperationException(errorMessage);
        //        }

        //        // Step 19: Prepare payment receipt details.
        //        var accountDeposits = new List<AccountDeposit>();
        //        var paymentReciepts = new List<PaymentDetailObject>();
        //        var feePayments = feePaymentConfirmationResponse.Data;
        //        // Add account deposit details.
        //        foreach (var feePayment in feePayments)
        //        {
        //            paymentReciepts.Add(new PaymentDetailObject
        //            {
        //                AccountNumber = customerAccount.AccountNumber,
        //                Fee = 0,
        //                Amount = feePayment.AmountPaid,
        //                Interest = 0,
        //                LoanCapital = 0,
        //                SericeOrEventName = feePayment.FeeName,
        //                VAT = 0,
        //                Balance = 0
        //            });
        //        }

        //        var transactions = new List<TransactionDto>();
        //        transactions.Add(transaction);
        //        // Step 20: Create payment processing request object.
        //        var paymentProcessing = new PaymentProcessingRequest
        //        {
        //            AccountingDay = accountingDate,
        //            Amount = amount,
        //            MemberName = customer.Name,
        //            NotesRequest = notes,
        //            OperationType = OperationType.Cash.ToString(),
        //            OperationTypeGrouping = TransactionType.CASH_IN.ToString(),
        //            PortalUsed = OperationSourceType.Web_Portal.ToString(),
        //            PaymentDetails = paymentReciepts,
        //            ServiceType = TransactionType.CASH_IN_LOAN_FEE.ToString(),
        //            SourceOfRequest = OperationSourceType.Physical_Teller.ToString(),
        //            TotalAmount = amount,
        //            TotalCharges = 0,
        //            Transactions = transactions
        //        };


        //        // Step 21: Process payment and get receipt.
        //        var paymentReceipt = _paymentReceiptRepository.ProcessPaymentAsync(paymentProcessing);
        //        var TellerBranch = await GetBranch(teller.BranchId);
        //        var cashOperation = new CashOperation(teller.BranchId, paymentProcessing.Amount, paymentProcessing.TotalCharges, TellerBranch.name, TellerBranch.branchCode, CashOperationType.LoanFee, LogAction.LoanFeeDeposit, subTellerProvioning);
        //        await _generalDailyDashboardRepository.UpdateOrCreateDashboardAsync(cashOperation);

        //        // Step 22: Save changes to the database.
        //        await _uow.SaveAsync();
        //        // Step 25: Post accounting entries for transactions.
        //        var accountingResponseMessages = await PostAccountingEvent(feePayments, reference, accountingDate,customer.CustomerId,customer.Name);
        //        // Step 26: Prepare and return the response.
        //        string accountDetails = string.Join("\n", feePayments.Select((ad, index) =>
        //            $"{index + 1}. {ad.FeeName}: {BaseUtilities.FormatCurrency(ad.AmountPaid)}"));
        //        var paymentReceiptDto = _mapper.Map<PaymentReceiptDto>(paymentReceipt);
        //        if (accountingResponseMessages == null)
        //        {
        //            accountingResponseMessages = $"Account Posting: Successful. Loan fee payment of {BaseUtilities.FormatCurrency(amount)} was processed by {teller.Name} [User: {dailyTeller.UserName}] for Member: {customer.Name}. The fee paid for includes:\n{accountDetails}";
        //        }
        //        else
        //        {
        //            accountingResponseMessages = $"Account Posting: Failed. Loan fee payment of {BaseUtilities.FormatCurrency(amount)} was processed by {teller.Name} [User: {dailyTeller.UserName}] for Member: {customer.Name}. The fee paid for includes:\n{accountDetails}";

        //        }
        //        _logger.LogInformation(accountingResponseMessages);
        //        await BaseUtilities.LogAndAuditAsync(accountingResponseMessages, request, HttpStatusCodeEnum.OK, LogAction.LoanFeeDeposit, LogLevelInfo.Information);

        //        return ServiceResponse<PaymentReceiptDto>.ReturnResultWith200(paymentReceiptDto, accountingResponseMessages);
        //    }
        //    catch (Exception e)
        //    {
        //        // Log error and return 500 Internal Server Error response with error message
        //        var errorMessage = $"Error occurred while saving Transaction: {e.Message}";
        //        _logger.LogError(errorMessage);
        //        await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.LoanFeeDeposit, LogLevelInfo.Error);
        //        return ServiceResponse<PaymentReceiptDto>.Return500(e);
        //    }
        //}
        private void UpdateTellerAccountBalance(decimal depositAmount, Teller teller, Account tellerAccount, TransactionDto transaction, string eventType, bool isInterbranch, string sourceBranchId, string destinationBranchId, string Name, DateTime accountingDate)
        {
            // Update Teller Account balances for deposit
            _accountRepository.CreditAccount(tellerAccount, depositAmount);
            // Create Teller Operation for deposit
            var tellerOperationDeposit = CreateTellerOperation(depositAmount, OperationType.Credit, teller, tellerAccount, tellerAccount.Balance, transaction, eventType, isInterbranch, sourceBranchId, destinationBranchId, Name, accountingDate); // Create teller operation for deposit
            _tellerOperationRepository.Add(tellerOperationDeposit); // Add deposit operation to repository
        }
        private TellerOperation CreateTellerOperation(decimal amount, OperationType operationType, Teller teller, Account tellerAccount, decimal currentBalance, TransactionDto transaction, string eventType, bool isInterBranch, string sourceBranchId, string destinationBranchId, string Name, DateTime accountingDate)
        {
            var data = new TellerOperation
            {
                Id = BaseUtilities.GenerateUniqueNumber(),
                AccountID = tellerAccount.Id,
                AccountNumber = tellerAccount.AccountNumber,
                Amount = amount,
                Description = $"{transaction.TransactionType} of {BaseUtilities.FormatCurrency(amount)}, Account Number: {tellerAccount.AccountNumber}",
                TransactionType = TransactionType.CASH_IN_LOAN_FEE.ToString(),
                ReferenceId = ReferenceNumberGenerator.GenerateReferenceNumber(_userInfoToken.BranchCode),
                CustomerId = transaction.CustomerId,
                MemberAccountNumber = transaction.AccountNumber,
                OperationType = operationType.ToString(),
                BranchId = tellerAccount.BranchId,
                OpenOfDayReference = tellerAccount.OpenningOfDayReference,
                BankId = tellerAccount.BankId,
                CurrentBalance = currentBalance,
                Date = accountingDate,
                AccountingDate = accountingDate,
                PreviousBalance = tellerAccount.PreviousBalance,
                TellerID = teller.Id,
                TransactionReference = transaction.TransactionReference,
                UserID = _userInfoToken.Id,
                EventName = eventType,
                MemberName = Name,
                DestinationBrachId = destinationBranchId,
                SourceBranchId = sourceBranchId,
                IsInterBranch = isInterBranch,
                DestinationBranchCommission = transaction.DestinationBranchCommission,
                SourceBranchCommission = transaction.SourceBranchCommission,

            };
            return data;
        }


        private async Task<BranchDto> GetBranch(string branchid)
        {
            var branchCommandQuery = new GetBranchByIdCommand { BranchId = branchid }; // Create command to get branch.
            var branchResponse = await _mediator.Send(branchCommandQuery); // Send command to _mediator.

            // Check if branch information retrieval was successful
            if (branchResponse.StatusCode != 200)
            {
                var errorMessage = $"Failed to retrieve branch information: {branchResponse.Message}";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            return branchResponse.Data; // Return branch data.
        }
        private decimal CalculateTotalAmount(IEnumerable<BulkOperation> bulkOperations)
        {
            return bulkOperations.Sum(x => x.Total); // Calculate total amount.
        }
        // Method to retrieve currency notes
        private async Task<ServiceResponse<List<CurrencyNotesDto>>> RetrieveCurrencyNotes(string reference, CurrencyNotesRequest currencyNotesRequest)
        {
            var addCurrencyNotesCommand = new AddCurrencyNotesCommand { CurrencyNote = currencyNotesRequest, Reference = reference }; // Create command to add currency notes.
            var currencyNoteResponse = await _mediator.Send(addCurrencyNotesCommand); // Send command to _mediator.

            if (currencyNoteResponse.StatusCode != 200)
            {
                return ServiceResponse<List<CurrencyNotesDto>>.Return403(""); // Return error response if currency notes retrieval fails.
            }
            return currencyNoteResponse; // Return currency notes data.
        }
        // Method to process bulk transactions
        private async Task<TransactionDto> ProcessBulkTransactionOthers(Teller teller, Account tellerAccount, bool isInterBranchOperation, CustomerDto customer, List<CurrencyNotesDto> currencyNotes, BranchDto branch, string reference, Config config, decimal Amount, DateTime accountingDate, Account customerAccount)
        {
            string note = $"Payment of loan fee the sum of {BaseUtilities.FormatCurrency(Amount)} with reference code: {reference}";
            var otherTrans = new OtherTransaction
            {
                AccountNumber = customerAccount.AccountNumber,
                Amount = Amount,
                CustomerId = customer.CustomerId,
                CNI = customer.IDNumber,
                Id = BaseUtilities.GenerateUniqueNumber(),
                TelephoneNumber = customer.Phone,
                MemberName = customer.Name,
                BankId = customer.BankId,
                BranchId = customerAccount.BranchId,
                Credit = 0,
                Debit = 0,
                TransactionType = "LoanFeePayment",
                Description = "Loan Fee Payment.",
                Narration = note,
                SourceType = OperationSourceType.Web_Portal.ToString(),
                Direction = OperationType.Credit.ToString(),
                DateOfOperation = accountingDate,
                TellerId = teller.Id,
                AmountInWord = BaseUtilities.ConvertToWords(Amount),
                EnventName = "Loan Fee Payment",
                TransactionReference = reference,
            };
            otherTrans.Id = BaseUtilities.GenerateUniqueNumber();
            var trans = CurrencyNotesMapper.MapTransaction(otherTrans, _userInfoToken, customerAccount);
            trans.Account = null;
            trans.Teller = null;
            _transactionRepository.Add(trans);
            var transaction = CurrencyNotesMapper.MapTransaction(otherTrans, _userInfoToken, customerAccount);
            transaction.Teller = teller;
            var transactionDto = CurrencyNotesMapper.MapTransactionToDto(transaction, currencyNotes, _userInfoToken);
            transactionDto.AccountNumber = customerAccount.AccountNumber;
            transactionDto.OriginalDepositAmount = Amount;
            transactionDto.AccountType = customerAccount.AccountType;
            transactionDto.AmountInWord = BaseUtilities.ConvertToWords(Amount);
            transactionDto.Fee = 0;

            UpdateTellerAccountBalance(Amount, teller, tellerAccount, transactionDto, TransactionType.CASH_IN_LOAN_FEE.ToString(), isInterBranchOperation, teller.BranchId, customer.BranchId, customer.Name, accountingDate);

            return transactionDto; // Return list of transactions.
        }

        /// <summary>
        /// Posts accounting events for fee payments.
        /// </summary>
        /// <param name="feePayments">List of fee payment details.</param>
        /// <param name="Reference">Unique transaction reference ID.</param>
        /// <param name="accountingDay">The accounting date for the transaction.</param>
        /// <param name="MemberReference">Member reference ID associated with the transaction.</param>
        /// <param name="memberName">Name of the member associated with the transaction.</param>
        /// <returns>Returns "OK" if successful, otherwise returns the error message.</returns>
        private async Task<string> PostAccountingEvent(List<FeePaymentConfirmationDto> feePayments, string Reference, DateTime accountingDay, string MemberReference, string memberName)
        {
            try
            {
                // Initialize the command object for posting events to the accounting system.
                var addEventPosting = new AutoPostingEventCommand
                {
                    AmountEventCollections = new List<AmountEventCollection>(), // Initialize the collection of event amounts.
                    TransactionReferenceId = Reference, // Set the transaction reference ID.
                    TransactionDate = accountingDay, // Set the transaction date.
                    MemberReference = MemberReference, // Set the member reference ID.
                    Source = TellerSources.Physical_Teller.ToString(), // Set the transaction source.
                };

                // Populate the AmountEventCollections with fee payment details.
                foreach (var fee in feePayments)
                {
                    // Construct a detailed narration for the accounting event.
                    string detailedNarration =
                        $"Fee Payment | Event Code: {fee.EventCode} | Amount: {BaseUtilities.FormatCurrency(fee.AmountPaid)} | " +
                        $"Member: {memberName} (ID: {MemberReference}) | " +
                        $"Reference: {Reference} | Date: {accountingDay:yyyy-MM-dd HH:mm:ss}";

                    // Add the fee payment details to the event collection.
                    addEventPosting.AmountEventCollections.Add(new AmountEventCollection
                    {
                        Amount = fee.AmountPaid, // Set the amount paid for the fee.
                        EventCode = fee.EventCode, // Set the associated event code.
                        Naration = detailedNarration // Use the constructed detailed narration.
                    });

                    // Log each fee added for clarity and debugging purposes.
                    string feeLog = $"Prepared Fee Posting | Amount: {BaseUtilities.FormatCurrency(fee.AmountPaid)} | EventCode: {fee.EventCode} | Member: {memberName} (ID: {MemberReference}) | REF: {Reference}";
                    _logger.LogInformation(feeLog);
                    await BaseUtilities.LogAndAuditAsync(feeLog, addEventPosting, HttpStatusCodeEnum.OK, LogAction.AccountingPosting, LogLevelInfo.Information);
                }

                // Send the command to the accounting system via mediator.
                var result = await _mediator.Send(addEventPosting);

                // Check the result status and return appropriate messages.
                if (result.StatusCode != 200)
                {
                    string errorMessage = $"Accounting Event Posting Failed | REF: {Reference} | Error: {result.Message}";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, addEventPosting, HttpStatusCodeEnum.InternalServerError, LogAction.AccountingPosting, LogLevelInfo.Error);
                    return result.Message;
                }

                // Log success and return "OK".
                string successMessage = $"Accounting Event Successfully Posted | REF: {Reference} | Member: {memberName} (ID: {MemberReference})";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, addEventPosting, HttpStatusCodeEnum.OK, LogAction.AccountingPosting, LogLevelInfo.Information);
                return "OK";
            }
            catch (Exception ex)
            {
                // Handle and log unexpected exceptions.
                string exceptionMessage = $"Error Occurred While Posting Accounting Events | REF: {Reference} | Error: {ex.Message}";
                _logger.LogError(exceptionMessage, ex);
                await BaseUtilities.LogAndAuditAsync(exceptionMessage, feePayments, HttpStatusCodeEnum.InternalServerError, LogAction.AccountingPosting, LogLevelInfo.Error);
                throw;
            }
        }

    }
}
