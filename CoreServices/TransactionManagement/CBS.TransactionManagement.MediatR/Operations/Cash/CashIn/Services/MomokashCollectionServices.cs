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
using CBS.TransactionManagement.Repository.AccountServices;
using CBS.TransactionManagement.Repository.MemberAccountConfiguration;
using CBS.TransactionManagement.Data.Entity.OtherCashInP;
using CBS.TransactionManagement.MediatR.LoanRepayment.Command;
using CBS.TransactionManagement.Data.Dto.LoanRepayment;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.Repository.Receipts.Payments;
using CBS.TransactionManagement.Data.Dto.Receipts.Details;
using CBS.TransactionManagement.Data.Dto.Receipts.Payments;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Commands;
using DocumentFormat.OpenXml.ExtendedProperties;
using Azure;
using DocumentFormat.OpenXml.Bibliography;
using CBS.TransactionManagement.MediatR.Accounting.Command;
using CBS.TransactionManagement.MediatR.UtilityServices;

namespace CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Services.NewFolder
{

    public class MomokashCollectionServices : IMomokashCollectionServices
    {
        private readonly IAccountRepository _accountRepository; // Repository for accessing account data.
        private readonly UserInfoToken _userInfoToken; // User information token.
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvioningHistoryRepository; // Repository for accessing teller data.
        private readonly ITellerRepository _tellerRepository; // Repository for accessing teller data.
        private readonly IConfigRepository _configRepository;
        private readonly IDepositServices _depositServices;
        private readonly IPaymentReceiptRepository _paymentReceiptRepository;
        private readonly IMapper _mapper;
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository;
        public IMediator _mediator { get; set; } // Mediator for handling requests.
        private readonly ILogger<MomokashCollectionServices> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for transaction context.

        public MomokashCollectionServices(
            IAccountRepository AccountRepository,
            UserInfoToken UserInfoToken,
            ILogger<MomokashCollectionServices> logger,
            IUnitOfWork<TransactionContext> uow,
            ITellerRepository tellerRepository = null,
            IMediator mediator = null,
            ISubTellerProvisioningHistoryRepository subTellerProvioningHistoryRepository = null,
            IDepositServices depositServices = null,
            IConfigRepository configRepository = null,
            IPaymentReceiptRepository paymentReceiptRepository = null,
            IMapper mapper = null,
            IAPIUtilityServicesRepository utilityServicesRepository = null)
        {
            _userInfoToken = UserInfoToken; // Initialize user information token.
            _accountRepository = AccountRepository; // Initialize account repository.
            _logger = logger; // Initialize logger.
            _uow = uow; // Initialize unit of work.
            _tellerRepository = tellerRepository; // Initialize teller repository.
            _mediator = mediator; // Initialize _mediator.
            _subTellerProvioningHistoryRepository = subTellerProvioningHistoryRepository;
            _depositServices = depositServices;
            _configRepository = configRepository;
            _paymentReceiptRepository = paymentReceiptRepository;
            _mapper = mapper;
            _utilityServicesRepository=utilityServicesRepository;
        }



        /// <summary>
        /// Handles Momo cash collection, processing deposits, posting accounting entries, and sending notifications.
        /// </summary>
        /// <param name="requests">The request containing bulk operations for Momo cash collection.</param>
        /// <param name="accountingDate">The date for accounting day processing.</param>
        /// <returns>A service response containing the payment receipt details.</returns>
        public async Task<ServiceResponse<PaymentReceiptDto>> MomokashCollection(AddBulkOperationDepositCommand requests, DateTime accountingDate)
        {
            string logReference = BaseUtilities.GenerateUniqueNumber();  // Unique reference for logging and auditing
            try
            {
                bool isInterBranchOperation = false;  // Flag to determine if the operation involves multiple branches

                // 1. Validate system configuration
                var config = await _configRepository.GetConfigAsync(OperationSourceType.Web_Portal.ToString());

                // 2. Retrieve and validate teller information
                var teller = await _tellerRepository.GetTellerByType(requests.BulkOperations.FirstOrDefault().SourceType);
                await _tellerRepository.CheckTellerOperationalRights(teller, requests.OperationType, requests.IsCashOperation);

                // 3. Retrieve teller account and customer details
                var tellerAccount = await _accountRepository.RetrieveTellerAccount(teller);
                var customer = await GetCustomer(requests);
                var branch = await GetBranch(customer);

                // 4. Check if inter-branch operation is occurring
                if (teller.BranchId != customer.BranchId)
                {
                    isInterBranchOperation = true;
                    string errorMessage = "Inter-branch operation is not allowed for Momo cash collection.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, requests, HttpStatusCodeEnum.Forbidden, LogAction.MomocashCollectionDepositProcessed, LogLevelInfo.Warning, logReference);
                    //return ServiceResponse<PaymentReceiptDto>.Return403(errorMessage);
                }

                // 5. Generate transaction reference and calculate total amounts and charges
                string reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, TransactionType.MomokcashCollection.ToString(), isInterBranchOperation);
                decimal amount = CalculateTotalAmount(requests.BulkOperations);
                decimal customerCharges = CalculateTotalCharges(requests.BulkOperations);

                // 6. Retrieve currency notes and map bulk deposits
                var currencyNote = await RetrieveCurrencyNotes(reference, requests.BulkOperations.FirstOrDefault().currencyNotes);
                var bulkDeposits = CurrencyNotesMapper.MapBulkOperationListToBulkDepositList(requests.BulkOperations);

                // 7. Process transactions in bulk
                var transactions = await ProcessBulkTransactions(
                    bulkDeposits, teller, tellerAccount, isInterBranchOperation, customer, currencyNote.Data.ToList(), branch, reference, config, customerCharges, false, true, accountingDate);

                // 8. Prepare payment receipt details
                var accountDeposits = transactions.Select(tx => new AccountDeposit
                {
                    AccountName = tx.Account.AccountName,
                    Amount = tx.Amount,
                    Charge = tx.Fee
                }).ToList();

                var paymentReciepts = transactions.Select(tx => new PaymentDetailObject
                {
                    AccountNumber = tx.AccountNumber,
                    Fee = tx.Fee,
                    Amount = tx.Amount,
                    SericeOrEventName = tx.Account.AccountName,
                    VAT = 0,
                    Balance = 0
                }).ToList();

                // 9. Create payment processing request and process the payment
                var paymentProcessing = new PaymentProcessingRequest
                {
                    AccountingDay = accountingDate,
                    Amount = amount,
                    MemberName = customer.Name,
                    NotesRequest = requests.BulkOperations.FirstOrDefault().currencyNotes,
                    OperationType = OperationType.Cash.ToString(),
                    OperationTypeGrouping = TransactionType.CASH_IN.ToString(),
                    PortalUsed = OperationSourceType.Web_Portal.ToString(),
                    PaymentDetails = paymentReciepts,
                    ServiceType = TransactionType.MomokcashCollection.ToString(),
                    SourceOfRequest = OperationSourceType.BackOffice_Operation.ToString(),
                    TotalAmount = amount,
                    TotalCharges = transactions.Sum(x => x.Fee),
                    Transactions = transactions
                };

                var paymentReceipt = _paymentReceiptRepository.ProcessPaymentAsync(paymentProcessing);
                await _uow.SaveAsync();

                // 10. Send SMS notification
                await SendSMS(accountDeposits, amount, reference, customer, branch, customerCharges);

                // 11. Post accounting entries
                bool accountingStatus = await PostAccounting(transactions, branch, isInterBranchOperation, accountingDate, teller.OperationEventCode);

                // 12. Prepare success message and response
                string accountDetails = string.Join("\n", accountDeposits.Select((ad, index) =>
                    $"{index + 1}. {ad.AccountName}: {BaseUtilities.FormatCurrency(ad.Amount)}, Fee: {BaseUtilities.FormatCurrency(ad.Charge)}"));

                string accountingResponseMessage = accountingStatus
                    ? $"Account Posting: Successful. A momo cash deposit of {BaseUtilities.FormatCurrency(amount)} with total charges of {BaseUtilities.FormatCurrency(paymentProcessing.TotalCharges)}. To Member: {customer.Name}. The deposit was allocated to the following accounts:\n{accountDetails}"
                    : $"Account Posting: Failed. A momo cash deposit of {BaseUtilities.FormatCurrency(amount)} with total charges of {BaseUtilities.FormatCurrency(paymentProcessing.TotalCharges)}. To Member: {customer.Name}. The deposit was allocated to the following accounts:\n{accountDetails}";

                _logger.LogInformation(accountingResponseMessage);
                await BaseUtilities.LogAndAuditAsync(accountingResponseMessage, requests, HttpStatusCodeEnum.OK, LogAction.MomocashCollectionDepositProcessed, LogLevelInfo.Information, logReference);

                var paymentReceiptDto = _mapper.Map<PaymentReceiptDto>(paymentReceipt);
                return ServiceResponse<PaymentReceiptDto>.ReturnResultWith200(paymentReceiptDto, accountingResponseMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error occurred while processing momo cash collection: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, requests, HttpStatusCodeEnum.InternalServerError, LogAction.MomocashCollectionDepositProcessed, LogLevelInfo.Error, logReference);
                return ServiceResponse<PaymentReceiptDto>.Return500(errorMessage);
            }
        }

        //public async Task<ServiceResponse<PaymentReceiptDto>> MomokashCollection(AddBulkOperationDepositCommand requests, DateTime accountingDate)
        //{

        //    try
        //    {


        //        bool isInterBranchOperation = false; // Flag to indicate if the operation is inter-branch.

        //        // Check if system configuration is set.
        //        var config = await _configRepository.GetConfigAsync(OperationSourceType.Web_Portal.ToString());

        //        // Retrieve teller information.
        //        var teller = await _tellerRepository.GetTellerByType(requests.BulkOperations.FirstOrDefault().SourceType);
        //        // Check teller rights.
        //        await _tellerRepository.CheckTellerOperationalRights(teller, requests.OperationType, requests.IsCashOperation);

        //        // Retrieve sub teller account.
        //        var tellerAccount = await _accountRepository.RetrieveTellerAccount(teller);

        //        // Retrieve customer information.
        //        var customer = await GetCustomer(requests);

        //        // Retrieve branch information.
        //        var branch = await GetBranch(customer);
        //        if (teller.BranchId != customer.BranchId)
        //        {
        //            // Set the operation as inter-branch
        //            isInterBranchOperation = true;

        //            // Prepare an error message
        //            var errorMessage = $"Inter-branch operation is not allowed for Momo cash collection.";

        //            // Log and audit the error message
        //            await BaseUtilities.LogAndAuditAsync(
        //                errorMessage,
        //                requests,
        //                HttpStatusCodeEnum.Forbidden,
        //                LogAction.DepositProcessed,
        //                LogLevelInfo.Error
        //            );

        //            // Return a 403 service response with the error details
        //            return ServiceResponse<PaymentReceiptDto>.Return403(errorMessage);
        //        }

        //        // Generate transaction reference based on branch type.
        //        string reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, TransactionType.MomokcashCollection.ToString(), isInterBranchOperation);

        //        // Calculate total amount and charges.
        //        decimal amount = CalculateTotalAmount(requests.BulkOperations);
        //        decimal customer_charges = CalculateTotalCharges(requests.BulkOperations);

        //        // Retrieve currency notes.
        //        var currencyNote = await RetrieveCurrencyNotes(reference, requests.BulkOperations.FirstOrDefault().currencyNotes);
        //        var notes = requests.BulkOperations.FirstOrDefault().currencyNotes;
        //        //_subTellerProvioningHistoryRepository.CashInByDinomination(amount, requests.BulkOperations.FirstOrDefault().currencyNotes, teller.Id, accountingDate);
        //        // Map the list of BulkOperation objects to a list of corresponding BulkDeposit objects.
        //        var bulkDeposits = CurrencyNotesMapper.MapBulkOperationListToBulkDepositList(requests.BulkOperations);

        //        // Process each transaction in the bulk.
        //        var transactions = await ProcessBulkTransactions(bulkDeposits, teller, tellerAccount, isInterBranchOperation, customer, currencyNote.Data.ToList(), branch, reference, config, customer_charges, false, true, accountingDate);
        //        // Step 19: Prepare payment receipt details.
        //        var accountDeposits = new List<AccountDeposit>();
        //        var paymentReciepts = new List<PaymentDetailObject>();

        //        // Add account deposit details.
        //        foreach (var transaction in transactions)
        //        {
        //            paymentReciepts.Add(new PaymentDetailObject
        //            {
        //                AccountNumber = transaction.AccountNumber,
        //                Fee = transaction.Fee,
        //                Amount = transaction.Amount,
        //                Interest = 0,
        //                LoanCapital = 0,
        //                SericeOrEventName = transaction.Account.AccountName,
        //                VAT = 0,
        //                Balance = 0
        //            });
        //            accountDeposits.Add(new AccountDeposit
        //            {
        //                AccountName = transaction.Account.AccountName,
        //                Amount = transaction.Amount,
        //                Charge = transaction.Fee
        //            });
        //        }
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
        //            ServiceType = TransactionType.MomokcashCollection.ToString(),
        //            SourceOfRequest = OperationSourceType.BackOffice_Operation.ToString(),
        //            TotalAmount = amount,
        //            TotalCharges = transactions.Sum(x => x.Fee),
        //            Transactions = transactions
        //        };
        //        // Step 21: Process payment and get receipt.
        //        var paymentReceipt = _paymentReceiptRepository.ProcessPaymentAsync(paymentProcessing);
        //        // Step 22: Save changes to the database.
        //        await _uow.SaveAsync();
        //        // Step 23: Calculate total charges and prepare transaction details.
        //        customer_charges = transactions.Sum(x => x.Fee);
        //        // Step 24: Send SMS notification to customer.
        //        await SendSMS(accountDeposits, amount, reference, customer, branch, customer_charges);
        //        // Step 25: Post accounting entries for transactions.
        //        var status = await PostAccounting(transactions, branch, isInterBranchOperation, accountingDate, teller.OperationEventCode);
        //        // Step 26: Prepare and return the response.
        //        string accountDetails = string.Join("\n", accountDeposits.Select((ad, index) =>
        //            $"{index + 1}. {ad.AccountName}: {BaseUtilities.FormatCurrency(ad.Amount)}, Fee: {BaseUtilities.FormatCurrency(ad.Charge)}"));
        //        var paymentReceiptDto = _mapper.Map<PaymentReceiptDto>(paymentReceipt);
        //        string accountingResponseMessages = string.Empty;
        //        if (status)
        //        {
        //            accountingResponseMessages = $"Account Posting: Successful. A momo cash deposit of {BaseUtilities.FormatCurrency(amount)} with total charges of {BaseUtilities.FormatCurrency(paymentProcessing.TotalCharges)}. To Member: {customer.Name}. The deposit was allocated to the following accounts:\n{accountDetails}";
        //        }
        //        else
        //        {
        //            accountingResponseMessages = $"Account Posting: Failed. A momo cash deposit of {BaseUtilities.FormatCurrency(amount)} with total charges of {BaseUtilities.FormatCurrency(paymentProcessing.TotalCharges)} To Member: {customer.Name}. The deposit was allocated to the following accounts:\n{accountDetails}";
        //        }
        //        _logger.LogInformation(accountingResponseMessages);
        //        // Log the success and audit.
        //        await BaseUtilities.LogAndAuditAsync(accountingResponseMessages, requests, HttpStatusCodeEnum.OK, LogAction.DepositProcessed, LogLevelInfo.Information);

        //        return ServiceResponse<PaymentReceiptDto>.ReturnResultWith200(paymentReceiptDto, accountingResponseMessages);

        //    }
        //    catch (Exception e)
        //    {
        //        // Log error and return 500 Internal Server Error response with error message.
        //        var errorMessage = $"Error occurred while deposit momo cash collection: {e.Message}";
        //        _logger.LogError(errorMessage);
        //        // Log the error and audit.
        //        await BaseUtilities.LogAndAuditAsync(errorMessage, requests,HttpStatusCodeEnum.InternalServerError, LogAction.DepositProcessed, LogLevelInfo.Error);
        //        return ServiceResponse<PaymentReceiptDto>.Return500(e);
        //    }
        //}

        // Method to send SMS notification
        private async Task SendSMS(List<AccountDeposit> accountDeposits, decimal amount, string reference, CustomerDto customer, BranchDto branch, decimal charge)
        {
            // Format account details with numbering and line breaks
            string accountDetails = string.Join("\n", accountDeposits.Select((ad, index) => $"{index + 1}. {ad.AccountName}: {BaseUtilities.FormatCurrency(ad.Amount)}, Fee: {BaseUtilities.FormatCurrency(ad.Charge)}"));


            // Initialize message variable
            string msg;
            string branchName = branch?.name ?? "";

            // Check the customer's language preference
            if (customer.Language.ToLower() == "english")
            {
                msg = $"Dear {customer.FirstName}, deposit of {BaseUtilities.FormatCurrency(amount)} into account(s):\n{accountDetails} was successful.\n" +
       $"Transaction Reference: {reference}. Total charge: {BaseUtilities.FormatCurrency(charge)}. Date: {BaseUtilities.UtcToDoualaTime(DateTime.UtcNow)}. Thank you.";

            }
            else
            {
                msg = $"Cher(e) {customer.FirstName}, dépôt de {BaseUtilities.FormatCurrency(amount)} sur les comptes:\n{accountDetails} réussi.\n" +
      $"Référence de la transaction : {reference}. Frais totaux : {BaseUtilities.FormatCurrency(charge)}. Date : {BaseUtilities.UtcToDoualaTime(DateTime.UtcNow)}. Merci.";

            }

            // Create command to send SMS
            var sMSPICallCommand = new SendSMSPICallCommand
            {
                messageBody = msg,
                recipient = customer.Phone
            };
            await _utilityServicesRepository.PushNotification(customer.CustomerId, PushNotificationTitle.CASH_IN_MOMO_CASH, msg);
            // Send command to _mediator
            await _mediator.Send(sMSPICallCommand);
        }

        // Method to retrieve customer information
        private async Task<CustomerDto> GetCustomer(AddBulkOperationDepositCommand requests)
        {
            string customerId = requests.BulkOperations.FirstOrDefault().CustomerId; // Get customer ID from request.
            var customerCommandQuery = new GetCustomerCommandQuery { CustomerID = customerId }; // Create command to get customer.
            var customerResponse = await _mediator.Send(customerCommandQuery); // Send command to _mediator.

            if (customerResponse == null)
            {
                var errorMessage = "Error; Null";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            // Check if customer information retrieval was successful
            if (customerResponse.StatusCode != 200)
            {
                var errorMessage = "Failed getting member's information";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }

            var customer = customerResponse.Data; // Get customer data from response.
            customer.Name = $"{customer.FirstName} {customer.LastName}";

            return customer; // Return customer data.
        }

        // Method to retrieve branch information
        private async Task<BranchDto> GetBranch(CustomerDto customer)
        {
            var branchCommandQuery = new GetBranchByIdCommand { BranchId = customer.BranchId }; // Create command to get branch.
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
        // Method to calculate total amount from bulk operations
        private decimal CalculateTotalAmount(IEnumerable<BulkOperation> bulkOperations)
        {
            return bulkOperations.Sum(x => x.Total); // Calculate total amount.
        }
        // Method to calculate total charges from bulk operations
        private decimal CalculateTotalCharges(IEnumerable<BulkOperation> bulkOperations)
        {
            return bulkOperations.Sum(x => x.Fee); // Calculate total charges.
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
        /// <summary>
        /// Processes bulk transactions for deposits, including Momo cash collection or regular deposits.
        /// </summary>
        /// <param name="requests">List of bulk deposit requests.</param>
        /// <param name="teller">Teller performing the operation.</param>
        /// <param name="tellerAccount">Teller's account for the deposit.</param>
        /// <param name="isInterBranchOperation">Flag to indicate if the operation involves multiple branches.</param>
        /// <param name="customer">Customer information associated with the deposits.</param>
        /// <param name="currencyNotes">Details of the currency notes for the transaction.</param>
        /// <param name="branch">Branch information related to the customer.</param>
        /// <param name="reference">Unique transaction reference for the deposit.</param>
        /// <param name="config">Configuration settings for the operation.</param>
        /// <param name="customer_charges">Total charges applicable to the transactions.</param>
        /// <param name="IsLoanRepayment">Flag indicating if the operation is a loan repayment.</param>
        /// <param name="IsMomocashCollection">Flag indicating if the operation is Momo cash collection.</param>
        /// <param name="accountingDate">Date of the accounting entry for the operation.</returns>
        /// <returns>A list of transaction DTOs representing the processed transactions.</returns>
        private async Task<List<TransactionDto>> ProcessBulkTransactions(
            List<BulkDeposit> requests,
            Teller teller,
            Account tellerAccount,
            bool isInterBranchOperation,
            CustomerDto customer,
            List<CurrencyNotesDto> currencyNotes,
            BranchDto branch,
            string reference,
            Config config,
            decimal customer_charges,
            bool IsLoanRepayment,
            bool IsMomocashCollection,
            DateTime accountingDate)
        {
            // Initialize the list to store processed transactions.
            var transactions = new List<TransactionDto>();
            string transactionType = TransactionType.DEPOSIT.ToString();

            // Process each bulk deposit request.
            foreach (var request in requests)
            {
                // 1. Retrieve customer account details for the given account number.
                var customerAccount = await _accountRepository.GetAccount(request.AccountNumber, transactionType);
                if (customerAccount == null)
                {
                    string errorMessage = $"Customer account '{request.AccountNumber}' not found. Transaction skipped.";
                    _logger.LogWarning(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.MomocashCollectionDepositProcessed, LogLevelInfo.Warning);
                    continue; // Skip this transaction if the account is not found.
                }

                // 2. Set up custom charge and other request-specific details.
                var customCharge = request.Fee;
                request.Customer = customer;
                request.currencyNotes = currencyNotes;
                request.Branch = branch;
                request.IsExternalOperation = false;
                request.ExternalApplicationName = "N/A";  // No external application for this operation.
                request.ExternalReference = "N/A";       // No external reference for this operation.
                request.SourceType = OperationSourceType.Web_Portal.ToString();

                // 3. Call the deposit service to process the transaction.
                var transaction = await _depositServices.Deposit(
                    teller,
                    tellerAccount,
                    request,
                    isInterBranchOperation,
                    _userInfoToken.BranchID,
                    customer.BranchId,
                    customCharge,
                    reference,
                    customerAccount,
                    config,
                    customer.Name,
                    IsMomocashCollection,
                    accountingDate,
                    false,
                    null);

                // 4. Set additional transaction properties.
                transaction.IsChargesIclussive = request.IsChargesInclussive;

                // 5. Add the successfully processed transaction to the list.
                transactions.Add(transaction);
            }

            // 6. Return the list of successfully processed transactions.
            return transactions;
        }

        //private async Task<List<TransactionDto>> ProcessBulkTransactions(List<BulkDeposit> requests, Teller teller, Account tellerAccount, bool isInterBranchOperation, CustomerDto customer, List<CurrencyNotesDto> currencyNotes, BranchDto branch, string reference, Config config, decimal customer_charges, bool IsLoanRepayment, bool IsMomocashCollection, DateTime accountingDate)
        //{
        //    var transactions = new List<TransactionDto>(); // Initialize list to store transactions.
        //    string transactionType = TransactionType.DEPOSIT.ToString();
        //    foreach (var request in requests)
        //    {

        //        var customerAccount = await _accountRepository.GetAccount(request.AccountNumber, transactionType); // Get customer account information.
        //        var customCharge = request.Fee; // Get custom charge for the transaction.
        //        request.Customer = customer;
        //        request.currencyNotes = currencyNotes;
        //        request.Branch = branch;
        //        request.IsExternalOperation = false;
        //        request.ExternalApplicationName = "N/A";
        //        request.ExternalReference = "N/A";
        //        request.SourceType = OperationSourceType.Web_Portal.ToString();
        //        var transaction = await _depositServices.Deposit(teller, tellerAccount, request, isInterBranchOperation, _userInfoToken.BranchID, customer.BranchId, customCharge, reference, customerAccount, config, customer.Name, IsMomocashCollection, accountingDate,false,null);
        //        transaction.IsChargesIclussive = request.IsChargesInclussive;
        //        transactions.Add(transaction); // Add transaction to list.
        //    }

        //    return transactions; // Return list of transactions.
        //}





        // Method to post accounting transactions
        private async Task<bool> PostAccounting(List<TransactionDto> transactions, BranchDto branch, bool isInterBranchOperation, DateTime accountingDate, string eventCode)
        {
            // Create a new instance of MobileMoneyCollectionOperationCommand to hold the accounting posting command
            var addAccountingPostingCommand = new MobileMoneyCollectionOperationCommand
            {
                TransactionReferenceId = transactions.FirstOrDefault()?.TransactionReference, // Use the first transaction's reference
                PaymentCollection = new List<PaymentCollection>(), // Initialize the PaymentCollection list
                TransactionDate = accountingDate, // Set the accounting date
                TellerSources = TellerSources.Virtual_Teller_Momo_cash_Collection.ToString() // Specify the teller source
            };

            // Iterate through each transaction and prepare the accounting postings
            foreach (var transaction in transactions)
            {
                // Calculate the amount to post
                decimal amountToPost = Math.Abs(transaction.Amount);
                if (transaction.IsChargesIclussive)
                {
                    // Include the fee in the amount if charges are inclusive
                    amountToPost += transaction.Fee;
                }

                // Generate payment collections for the transaction
                var paymentCollections = MakeAccountingPosting(
                    amountToPost,         // Amount to post
                    transaction.Account,  // Associated account
                    transaction,          // Transaction details
                    eventCode             // Event code
                );

                // Add generated payment collections to the posting command
                addAccountingPostingCommand.PaymentCollection.AddRange(paymentCollections);
            }

            // Send the posting command to the mediator and process the result
            var result = await _mediator.Send(addAccountingPostingCommand);

            // Return true for success (status code 200), otherwise false
            return result.StatusCode == 200;
        }

        // Helper method to create accounting postings for principal and commission
        private List<PaymentCollection> MakeAccountingPosting(decimal amount, Account account, TransactionDto transaction, string eventCode)
        {
            // Create a list to hold the payment collections
            var payments = new List<PaymentCollection>
    {
        // Add the principal payment
        new PaymentCollection
        {
            Amount = amount, // Principal amount
            EventCode = OperationEventRubbriqueName.Principal_Saving_Account.ToString(), // Event code for principal
            IsComission = false, // Not a commission
            ProductId = account.ProductId, // Product ID from the account
            Narration = $"Cashin Momocash Collection | Type: Principal | Account: {account.AccountName} | Product: {account.Product.Name} | Amount: {BaseUtilities.FormatCurrency(amount)} | Reference: {transaction.TransactionReference}"
        },

        // Add the commission payment
        new PaymentCollection
        {
            Amount = transaction.OperationCharge, // Commission amount
            EventCode = eventCode, // Event code for the commission
            IsComission = true, // This is a commission
            ProductId = account.ProductId, // Product ID from the account
            Narration = $"Cashin Momocash Collection | Type: Commission | Account: {account.AccountName} | Product: {account.Product.Name} | Amount: {BaseUtilities.FormatCurrency(transaction.OperationCharge)} | Reference: {transaction.TransactionReference}"
        }
    };

            // Return the list of payments
            return payments;
        }
    }
}
