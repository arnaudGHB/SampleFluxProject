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
using CBS.TransactionManagement.Repository.Receipts.Payments;
using CBS.TransactionManagement.Data.Dto.Receipts.Details;
using CBS.TransactionManagement.Data.Dto.Receipts.Payments;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Commands;
using AutoMapper.Internal;
using CBS.TransactionManagement.MediatR.Accounting.Command;
using CBS.TransactionManagement.otherCashIn.Commands;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository.DailyStatisticBoard;
using CBS.TransactionManagement.MediatR.UtilityServices;

namespace CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Services
{

    public class NormalDepositServices : INormalDepositServices
    {

        private readonly IGeneralDailyDashboardRepository _generalDailyDashboardRepository; // Repository for accessing account data.
        private readonly IAccountRepository _accountRepository; // Repository for accessing account data.
        private readonly UserInfoToken _userInfoToken; // User information token.
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvioningHistoryRepository; // Repository for accessing teller data.
        private readonly IDailyTellerRepository _tellerProvisioningAccountRepository; // Repository for accessing teller provisioning account data.
        private readonly ITellerRepository _tellerRepository; // Repository for accessing teller data.
        private readonly IDepositServices _depositServices;
        private readonly IPaymentReceiptRepository _paymentReceiptRepository;
        private readonly IMapper _mapper;
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository;
        private readonly IMemberAccountActivationRepository _memberAccountActivationRepository;
        public IMediator _mediator { get; set; } // Mediator for handling requests.
        private readonly ILogger<NormalDepositServices> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for transaction context.

        public NormalDepositServices(
            IAccountRepository AccountRepository,
            UserInfoToken UserInfoToken,
            ILogger<NormalDepositServices> logger,
            IUnitOfWork<TransactionContext> uow,
            IDailyTellerRepository tellerProvisioningAccountRepository = null,
            ITellerRepository tellerRepository = null,
            IMediator mediator = null,
            ISubTellerProvisioningHistoryRepository subTellerProvioningHistoryRepository = null,
            IDepositServices depositServices = null,
            IMemberAccountActivationRepository memberAccountActivationRepository = null,
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
            _depositServices = depositServices;
            _memberAccountActivationRepository = memberAccountActivationRepository;
            _paymentReceiptRepository = paymentReceiptRepository;
            _mapper = mapper;
            _generalDailyDashboardRepository = generalDailyDashboardRepository;
            _utilityServicesRepository=utilityServicesRepository;
        }
        /// <summary>
        /// Handles deposit transactions, including membership validation, transaction processing, and payment receipt generation.
        /// </summary>
        /// <param name="requests">Bulk deposit operation request.</param>
        /// <param name="accountingDate">The accounting day for the deposit.</param>
        /// <param name="config">Configuration object for the deposit process.</param>
        /// <returns>A service response containing the payment receipt details.</returns>
        public async Task<ServiceResponse<PaymentReceiptDto>> MakeDeposit(AddBulkOperationDepositCommand requests, DateTime accountingDate, Config config)
        {
            string reference = string.Empty;  // Transaction reference
            try
            {
                // Step 1: Initialize flags for inter-branch operation and first deposit.
                bool isInterBranchOperation = false;
                bool isFirstDeposit = false;

                // Step 3: Verify if the user's account is acting as a teller today.
                var dailyTeller = await _tellerProvisioningAccountRepository.GetAnActiveSubTellerForTheDate();

                // Step 4: Check if the accounting day remains open for the teller.
                await _subTellerProvioningHistoryRepository.CheckIfDayIsStillOpened(dailyTeller.TellerId, accountingDate);

                // Step 5: Retrieve teller info based on the deposit type.
                var teller = requests.DepositType == "CashInMomocashCollection"
                    ? await _tellerRepository.GetTellerByType(requests.BulkOperations.FirstOrDefault().SourceType)
                    : await _tellerRepository.RetrieveTeller(dailyTeller);

                // Step 6: Check teller's operational rights for the requested operation.
                await _tellerRepository.CheckTellerOperationalRights(teller, requests.OperationType, requests.IsCashOperation);

                // Step 7: Retrieve the account associated with the teller.
                var tellerAccount = await _accountRepository.RetrieveTellerAccount(teller);

                // Step 8: Retrieve customer details.
                var customer = await GetCustomer(requests);

                // Step 9: Initialize membership subscription amount.
                decimal memberSubscriptionAmount = 0;

                // Step 11: Get customer's branch details.
                var branch = await GetBranch(customer.BranchId);
                customer.BranchCode = branch.branchCode;
                customer.BranchName = branch.name;

                // Step 12: Determine if the transaction involves inter-branch activity.
                isInterBranchOperation = teller.BranchId != customer.BranchId;

                // Step 13: Generate a unique transaction reference.
                reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, TransactionType.DEPOSIT.ToString(), isInterBranchOperation);

                var memberFeePolicy = new MemberFeePolicyDto();

                // Step 10: Validate membership status for deposit eligibility.
                if (customer.MembershipApprovalStatus == MembershipApprovalStatus.Awaits_Validation.ToString())
                {
                    decimal membershipAmountEntered = await GetMemberShipAmount(requests.BulkOperations);
                    memberFeePolicy = await _memberAccountActivationRepository.GetMemberSubcription(customer);
                    memberSubscriptionAmount = memberFeePolicy.Amount;

                    if (membershipAmountEntered != memberSubscriptionAmount)
                    {
                        var errorMessage = $"Incorrect membership amount entered. Required: {BaseUtilities.FormatCurrency(memberSubscriptionAmount)}";
                        await BaseUtilities.LogAndAuditAsync(errorMessage, requests, HttpStatusCodeEnum.BadRequest, LogAction.DepositProcessed, LogLevelInfo.Warning, reference);
                        throw new InvalidOperationException(errorMessage);
                    }

                    isFirstDeposit = true;
                }

                // Step 14: Calculate total deposit amount and customer charges.
                decimal amount = CalculateTotalAmount(requests.BulkOperations);
                decimal customerCharges = CalculateTotalCharges(requests.BulkOperations, memberSubscriptionAmount);
                amount -= memberSubscriptionAmount;

                // Step 15: Handle cases where the deposit amount is zero.
                if (amount == 0)
                {
                    var errorMessage = "Initial payment includes only the membership fee, which is insufficient to open the account.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, requests, HttpStatusCodeEnum.BadRequest, LogAction.DepositProcessed, LogLevelInfo.Warning, reference);
                    throw new InvalidOperationException(errorMessage);
                }

                // Step 16: Process currency notes for the transaction.
                var currencyNote = await RetrieveCurrencyNotes(reference, requests.BulkOperations.FirstOrDefault().currencyNotes);
                var subTellerProvisioning = _subTellerProvioningHistoryRepository.CashInByDinomination(amount, requests.BulkOperations.FirstOrDefault().currencyNotes, teller.Id, accountingDate, tellerAccount.OpenningOfDayReference);

                // Step 17: Map bulk operations to deposit objects.
                var bulkDeposits = CurrencyNotesMapper.MapBulkOperationListToBulkDepositList(requests.BulkOperations);

                // Step 18: Execute bulk transactions.
                var transactions = await ProcessBulkTransactions(bulkDeposits, teller, tellerAccount, isInterBranchOperation, customer, currencyNote.Data.ToList(), branch, reference, config, customerCharges, false, false, accountingDate);

                // Step 19: Activate membership for first-time deposits.
                var memberActivationResponses = new List<MemberActivationResponse>();
                if (isFirstDeposit)
                {
                    if (isInterBranchOperation)
                    {
                        var errorMessage = "New member registration cannot be processed through inter-branch operations.";
                        await BaseUtilities.LogAndAuditAsync(errorMessage, requests, HttpStatusCodeEnum.BadRequest, LogAction.DepositProcessed, LogLevelInfo.Warning, reference);
                        throw new InvalidOperationException(errorMessage);
                    }

                    memberActivationResponses = await ActivateMembership(customer.CustomerId, memberSubscriptionAmount, isFirstDeposit, customer, memberFeePolicy, branch.branchCode);
                }

                // Step 20: Prepare payment receipt details.
                var accountDeposits = new List<AccountDeposit>();
                var paymentReceipts = new List<PaymentDetailObject>();

                foreach (var transaction in transactions)
                {
                    paymentReceipts.Add(new PaymentDetailObject { AccountNumber = transaction.AccountNumber, Fee = transaction.Fee, Amount = transaction.Amount, SericeOrEventName = transaction.Account.AccountName });
                    accountDeposits.Add(new AccountDeposit { AccountName = transaction.Account.AccountName, Amount = transaction.Amount, Charge = transaction.Fee });
                }

                foreach (var activationResponse in memberActivationResponses)
                {
                    paymentReceipts.Add(new PaymentDetailObject { AccountNumber = customer.CustomerId, Fee = activationResponse.Paid, SericeOrEventName = activationResponse.ServiceName, Balance = activationResponse.Balance });
                }

                // Step 21: Create payment processing request.
                var paymentProcessing = new PaymentProcessingRequest
                {
                    AccountingDay = accountingDate,
                    Amount = amount,
                    MemberName = customer.Name,
                    NotesRequest = requests.BulkOperations.FirstOrDefault()?.currencyNotes,
                    OperationType = OperationType.Cash.ToString(),
                    OperationTypeGrouping = TransactionType.CASH_IN.ToString(),
                    PaymentDetails = paymentReceipts,
                    ServiceType = TransactionType.CASH_IN.ToString(),
                    SourceOfRequest = OperationSourceType.Physical_Teller.ToString(),
                    TotalAmount = amount,
                    TotalCharges = transactions.Sum(x => x.Fee),
                    Transactions = transactions
                };

                // Step 22: Process payment and update dashboard.
                var paymentReceipt = _paymentReceiptRepository.ProcessPaymentAsync(paymentProcessing);
                var tellerBranch = await GetBranch(teller.BranchId);
                var cashOperation = new CashOperation(teller.BranchId, paymentProcessing.Amount, paymentProcessing.TotalCharges, tellerBranch.name, tellerBranch.branchCode, CashOperationType.CashIn, LogAction.DepositProcessed, subTellerProvisioning);
                await _generalDailyDashboardRepository.UpdateOrCreateDashboardAsync(cashOperation, reference);

                // Step 23: Commit changes to the database.
                await _uow.SaveAsync();

                // Step 24: Send SMS notifications.
                await SendSMS(accountDeposits, amount, reference, customer, branch, customerCharges, isFirstDeposit, memberActivationResponses, memberSubscriptionAmount);

                // Step 25: Post accounting entries and prepare response.
                var accountingResponseMessages = await PostAccounting(transactions, branch, isInterBranchOperation, memberActivationResponses, accountingDate);
                var paymentReceiptDto = _mapper.Map<PaymentReceiptDto>(paymentReceipt);

                string successMessage = $"Deposit of {BaseUtilities.FormatCurrency(amount)} processed by {teller.Name} for Member: {customer.Name}.";
                await BaseUtilities.LogAndAuditAsync(successMessage, requests, HttpStatusCodeEnum.OK, LogAction.DepositProcessed, LogLevelInfo.Information, reference);
                _logger.LogInformation(successMessage);

                return ServiceResponse<PaymentReceiptDto>.ReturnResultWith200(paymentReceiptDto, successMessage);
            }
            catch (Exception e)
            {
                // Step 27: Handle errors and log the exception.
                var errorMessage = $"Error during deposit: {e.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, requests, HttpStatusCodeEnum.InternalServerError, LogAction.DepositProcessed, LogLevelInfo.Error, reference);
                _logger.LogError(errorMessage);
                return ServiceResponse<PaymentReceiptDto>.Return500(errorMessage);
            }
        }


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
            //EnsureCustomerMembershipApproved(customer); // Ensure customer membership is approved.

            return customer; // Return customer data.
        }
        // Method to activate membership
        private async Task<List<MemberActivationResponse>> ActivateMembership(string customerId, decimal amount, bool isFirstDeposit, CustomerDto customer, MemberFeePolicyDto memberFeePolicy,string branchCode)
        {
            // If it's the first deposit, activate membership
            if (isFirstDeposit)
            {
                var activateMembershipCommand = new ActivateMembershipCommand(customerId, amount, customer.BranchId, customer.BankId, memberFeePolicy, branchCode);
                var membershipActivateResponse = await _mediator.Send(activateMembershipCommand);
                // Check if the activation was successful
                if (membershipActivateResponse.StatusCode != 200)
                {
                    var errorMessage = "Failed to activate membership.";
                    _logger.LogError(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }
                return membershipActivateResponse.Data;
            }
            return null;
        }

      
        // Method to retrieve branch information
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
        // Method to retrieve branch information
        private decimal CalculateTotalAmount(IEnumerable<BulkOperation> bulkOperations)
        {
            return bulkOperations.Sum(x => x.Total); // Calculate total amount.
        }
        private async Task<decimal> GetMemberShipAmount(IEnumerable<BulkOperation> bulkOperations)
        {
            // Retrieve the first CustomerId from bulk operations
            var customerId = bulkOperations.FirstOrDefault()?.CustomerId;

            if (customerId == null)
            {
                throw new InvalidOperationException("No valid customer information provided in the bulk operations.");
            }

            // Fetch subscription accounts based on the customer ID
            var subscriptionAccounts = await _accountRepository.FindBy(x => x.CustomerId == customerId).Include(x => x.Product).ToListAsync();

            // Check if any of the accounts are of the Membership type
            var membershipAccount = subscriptionAccounts.FirstOrDefault(a => a.AccountType == AccountType.Membership.ToString());

            // If a membership account is found, return the amount
            if (membershipAccount != null)
            {


                return bulkOperations.Where(x => x.AccountType == membershipAccount.Product?.Name).FirstOrDefault()?.Amount ?? 0;
            }
            else
            {
                // If no membership account is found, throw an exception with an elaborate message
                var errorMessage = $"Membership subscription account not found for customer ID: {customerId}. " +
                                   "Please ensure that a valid membership account is associated with this customer before proceeding. " +
                                   "If you believe this is an error, contact system administration for further assistance.";

                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
        }


        // Method to calculate total charges from bulk operations
        private decimal CalculateTotalCharges(IEnumerable<BulkOperation> bulkOperations, decimal memberSubcriptionAmount)
        {
            decimal total = bulkOperations.Sum(x => x.Fee) + memberSubcriptionAmount;
            return total; // Calculate total charges.
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
        private async Task<List<TransactionDto>> ProcessBulkTransactions(List<BulkDeposit> requests, Teller teller, Account tellerAccount, bool isInterBranchOperation, CustomerDto customer, List<CurrencyNotesDto> currencyNotes, BranchDto branch, string reference, Config config, decimal customer_charges, bool IsLoanRepayment, bool IsMomocashCollection, DateTime accountingDate)
        {
            var transactions = new List<TransactionDto>(); // Initialize list to store transactions.
            string transactionType = IsLoanRepayment == false ? TransactionType.DEPOSIT.ToString() : TransactionType.CASHIN_LOAN_REPAYMENT.ToString();
            foreach (var request in requests)
            {

                var customerAccount = await _accountRepository.GetAccount(request.AccountNumber, transactionType); // Get customer account information.
                var customCharge = request.Fee; // Get custom charge for the transaction.
                                                // Set transaction properties
                request.Customer = customer;
                request.currencyNotes = currencyNotes;
                request.Branch = branch;
                request.IsSWS = false;
                request.IsExternalOperation = false;
                request.ExternalApplicationName = "N/A";
                request.ExternalReference = "N/A";
                request.SourceType = OperationSourceType.Web_Portal.ToString();
                var transaction = await _depositServices.Deposit(teller, tellerAccount, request, isInterBranchOperation, _userInfoToken.BranchID, customer.BranchId, customCharge, reference, customerAccount, config, customer.Name, IsMomocashCollection, accountingDate,false,null);
                transaction.IsChargesIclussive = request.IsChargesInclussive;
                transactions.Add(transaction); // Add transaction to list.
                if (customerAccount.AccountType == AccountType.Membership.ToString())
                {
                    _accountRepository.DeleteAccount(customerAccount);
                }

            }

            return transactions; // Return list of transactions.
        }

        // Method to send SMS notification
        private async Task SendSMS(List<AccountDeposit> accountDeposits, decimal amount, string reference, CustomerDto customer, BranchDto branch, decimal charge, bool IsFirstDeposit, List<MemberActivationResponse> memberActivations, decimal membershipAmount)
        {
            // Format account details with numbering and line breaks
            string accountDetails = string.Join("\n", accountDeposits.Select((ad, index) => $"{index + 1}. {ad.AccountName}: {BaseUtilities.FormatCurrency(ad.Amount)}, Fee: {BaseUtilities.FormatCurrency(ad.Charge)}"));

            // Format service details with numbering and line breaks
            string serviceDetails = string.Join("\n", memberActivations.Select((ma, index) => $"{index + 1}. {ma.ServiceName}: {BaseUtilities.FormatCurrency(ma.Fee)}"));

            // Initialize message variable
            string msg;
            string branchName = branch?.name ?? "";

            // Check the customer's language preference
            if (customer.Language.ToLower() == "english")
            {
                // Construct the SMS message in English
                if (IsFirstDeposit)
                {
                    msg = $"Dear {customer.FirstName}, your membership with {branchName} is activated with an initial deposit of {BaseUtilities.FormatCurrency(amount)} into account(s):\n{accountDetails}.\n" +
                          $"Membership {BaseUtilities.FormatCurrency(membershipAmount)}\nDetails:\n{serviceDetails}.\n" +
                          $"Transaction Reference: {reference}. Total charge: {BaseUtilities.FormatCurrency(charge)}. Date: {BaseUtilities.UtcToDoualaTime(DateTime.UtcNow)}. Thank you.";
                }
                else
                {
                    msg = $"Dear {customer.FirstName}, deposit of {BaseUtilities.FormatCurrency(amount)} into account(s):\n{accountDetails} was successful.\n" +
                          $"Transaction Reference: {reference}. Total charge: {BaseUtilities.FormatCurrency(charge)}. Date: {BaseUtilities.UtcToDoualaTime(DateTime.UtcNow)}. Thank you.";
                }
            }
            else
            {
                // Construct the SMS message in French
                if (IsFirstDeposit)
                {
                    msg = $"Cher(e) {customer.FirstName}, votre adhésion à {branchName} est activée avec un dépôt initial de {BaseUtilities.FormatCurrency(amount)} sur les comptes:\n{accountDetails}.\n" +
                          $"Les frais d'adhésion {BaseUtilities.FormatCurrency(membershipAmount)}\nDetails\n{serviceDetails}.\n" +
                          $"Référence de la transaction : {reference}. Frais totaux : {BaseUtilities.FormatCurrency(charge)}. Date : {BaseUtilities.UtcToDoualaTime(DateTime.UtcNow)}. Merci.";
                }
                else
                {
                    msg = $"Cher(e) {customer.FirstName}, dépôt de {BaseUtilities.FormatCurrency(amount)} sur les comptes:\n{accountDetails} réussi.\n" +
                          $"Référence de la transaction : {reference}. Frais totaux : {BaseUtilities.FormatCurrency(charge)}. Date : {BaseUtilities.UtcToDoualaTime(DateTime.UtcNow)}. Merci.";
                }
            }

            // Create command to send SMS
            var sMSPICallCommand = new SendSMSPICallCommand
            {
                messageBody = msg,
                recipient = customer.Phone
            };
            await _utilityServicesRepository.PushNotification(customer.CustomerId,PushNotificationTitle.CASH_IN, msg);
            // Send command to _mediator
            await _mediator.Send(sMSPICallCommand);
        }

        /// <summary>
        /// Posts a list of transactions to the accounting system, creating corresponding accounting postings for each transaction.
        /// Supports member registration inclusivity and regular transactions.
        /// </summary>
        /// <param name="transactions">The list of transactions to be posted.</param>
        /// <param name="branch">The branch where the transactions occur.</param>
        /// <param name="isInterBranchOperation">Indicates if the transactions are inter-branch operations.</param>
        /// <param name="memberActivations">List of member activation responses, if applicable.</param>
        /// <param name="accountingDate">The date for the accounting entry.</param>
        /// <returns>A string containing any accounting response messages or errors.</returns>
        private async Task<string> PostAccounting(List<TransactionDto> transactions, BranchDto branch, bool isInterBranchOperation, List<MemberActivationResponse> memberActivations, DateTime accountingDate)
        {
            string accountingResponseMessages = null; // Initialize variable to store accounting response messages.
            var addAccountingPostings = new AddAccountingPostingCommandList(); // Initialize the list of accounting postings.
            int transactionIndex = 0; // Transaction index for tracking the first transaction.
            bool isMemberRegistrationInclusive = false; // Flag to indicate if member registration is inclusive in the transaction.
            addAccountingPostings.OperationType = OperationType.Deposit.ToString();

            // Iterate through each transaction
            foreach (var transaction in transactions)
            {
                if (transaction.Account.AccountType != AccountType.Membership.ToString())
                {
                    transactionIndex++; // Increment transaction index

                    // Determine the amount to post: if charges are inclusive, add the transaction fee to the amount.
                    decimal amountToPost = Math.Abs(transaction.Amount);
                    if (transaction.IsChargesIclussive)
                    {
                        amountToPost = Math.Abs(transaction.Amount) + transaction.Fee;
                    }

                    // Check if the first transaction involves member registration inclusivity.
                    if (transactionIndex == 1 && memberActivations.Any())
                    {
                        isMemberRegistrationInclusive = true;
                    }
                    else
                    {
                        isMemberRegistrationInclusive = false;
                    }

                    // Create an accounting posting for the transaction
                    var addAccountingPosting = MakeAccountingPosting(
                        amountToPost,
                        transaction.Account,
                        transaction,
                        branch,
                        isInterBranchOperation,
                        memberActivations,
                        accountingDate,
                        isMemberRegistrationInclusive
                    );

                    // Add the created posting to the list of accounting postings
                    addAccountingPostings.MakeAccountPostingCommands.Add(addAccountingPosting);
                }
            }

            // Send the list of accounting postings to the _mediator and await the result
            var result = await _mediator.Send(addAccountingPostings);

            // If the result status is not successful, add the error message to the response
            if (result.StatusCode != 200)
            {
                accountingResponseMessages = $"{result.Message}, "; // Append error message to the response.
            }

            return accountingResponseMessages; // Return any accumulated accounting response messages.
        }

        /// <summary>
        /// Creates and initializes an AddAccountingPostingCommand instance based on the provided transaction details.
        /// Supports both inter-branch and regular transactions, optionally including member registration fees.
        /// </summary>
        private AddAccountingPostingCommand MakeAccountingPosting(
            decimal amount,
            Account account,
            TransactionDto transaction,
            BranchDto branch,
            bool isInterBranch,
            List<MemberActivationResponse> memberActivations,
            DateTime accountingDate,
            bool isMemberRegistrationInclusive)
        {
            string IB = isInterBranch ? "IB" : "LB";
            // Initialize the accounting posting command.
            var addAccountingPostingCommand = new AddAccountingPostingCommand
            {
                AccountHolder = account.AccountName, // Account holder's name.
                OperationType = TransactionType.DEPOSIT.ToString(), // Type of transaction.
                AccountNumber = account.AccountNumber, // Account number involved in the transaction.
                ProductId = account.ProductId, // Associated product ID.
                ProductName = account.Product?.Name ?? account.AccountType, // Product name or account type.
                Naration = $"{IB}-Cashin Transaction | Account: {account.AccountName} | Amount: {BaseUtilities.FormatCurrency(amount)} | Reference: {transaction.TransactionReference} | Date: {accountingDate:dd-MM-yyyy}", // Pipe-separated narration.
                TransactionReferenceId = transaction.TransactionReference, // Reference ID for the transaction.
                IsInterBranchTransaction = isInterBranch, // Indicates if the transaction is inter-branch.
                ExternalBranchCode = branch.branchCode, // Code for the external branch.
                ExternalBranchId = branch.id, // ID of the external branch.
                AmountEventCollections = new List<AmountEventCollection>(), // Initialize the event collection list.
                AmountCollection = new List<AmountCollection>(), // Initialize the amount collection list.
                TransactionDate = accountingDate, // Set the transaction date.
                MemberReference = transaction.CustomerId // Member reference ID.
            };

            // Adds the principal amount to the collection.
            AddPrincipalAmount(addAccountingPostingCommand.AmountCollection, amount, isInterBranch, transaction.AccountType, transaction.TransactionReference);

            // Handle inter-branch or regular branch-specific collections.
            if (isInterBranch)
            {
                AddInterBranchCollections(addAccountingPostingCommand.AmountCollection, transaction, addAccountingPostingCommand.TransactionReferenceId);
            }
            else
            {
                AddRegularBranchCollections(addAccountingPostingCommand.AmountCollection, transaction, addAccountingPostingCommand.TransactionReferenceId);

                // Include member registration fees if applicable.
                if (isMemberRegistrationInclusive)
                {
                    AddMemberRegistrationFees(addAccountingPostingCommand.AmountEventCollections, memberActivations, addAccountingPostingCommand.TransactionReferenceId);
                }
            }

            return addAccountingPostingCommand;
        }

        /// <summary>
        /// Adds the principal amount to the collection with a standardized pipe-separated narration.
        /// </summary>
        private void AddPrincipalAmount(List<AmountCollection> amountCollection, decimal amount, bool isInterBranch, string accountType, string reference)
        {
            string IB = isInterBranch ? "IB" : "LB";
            // Create and add a principal amount entry.
            amountCollection.Add(new AmountCollection
            {
                Amount = amount, // Principal transaction amount.
                IsPrincipal = true, // Mark this as a principal transaction.
                EventAttributeName = OperationEventRubbriqueName.Principal_Saving_Account.ToString(), // Event name for principal account.
                IsInterBankOperationPrincipalCommission = isInterBranch, // Indicates inter-branch commission.
                Naration = $"Principal Amount {IB}-Cashin | Account Type: {accountType} | Amount: {BaseUtilities.FormatCurrency(amount)} | Reference: {reference}" // Pipe-separated narration.
            });
        }

        /// <summary>
        /// Adds inter-branch collections to the amount collection with pipe-separated narrations.
        /// </summary>
        private void AddInterBranchCollections(List<AmountCollection> amountCollection, TransactionDto transaction, string reference)
        {
            string IB = transaction.IsInterBrachOperation ? "IB" : "LB";
            amountCollection.AddRange(new[]
            {
        CreateAmountCollection(transaction.SourceBranchCommission, SharingWithPartner.SourceBrachCommission_Account.ToString(),
            $"Source Branch {IB}-Cash-In Commission for {transaction.AccountType} | Amount: {BaseUtilities.FormatCurrency(transaction.SourceBranchCommission)} | Reference: {reference}"),
        CreateAmountCollection(transaction.DestinationBranchCommission, SharingWithPartner.DestinationBranchCommission_Account.ToString(),
            $"Destination Branch {IB}-Cash-In Commission for {transaction.AccountType} | Amount: {BaseUtilities.FormatCurrency(transaction.DestinationBranchCommission)} | Reference: {reference}"),
        CreateAmountCollection(transaction.CamCCULCommission, SharingWithPartner.CamCCULShareCashInCommission.ToString(),
            $"CamCCUL {IB}-Cash-In Commission for {transaction.AccountType} | Amount: {BaseUtilities.FormatCurrency(transaction.CamCCULCommission)} | Reference: {reference}"),
        CreateAmountCollection(transaction.FluxAndPTMCommission, SharingWithPartner.FluxAndPTMShareCashInCommission.ToString(),
            $"Flux and PTM {IB}-Cash-In Commission for {transaction.AccountType} | Amount: {BaseUtilities.FormatCurrency(transaction.FluxAndPTMCommission)} | Reference: {reference}"),
        CreateAmountCollection(transaction.HeadOfficeCommission, SharingWithPartner.HeadOfficeShareCashInCommission.ToString(),
            $"Head Office {IB}-Cash-In Commission for {transaction.AccountType} | Amount: {BaseUtilities.FormatCurrency(transaction.HeadOfficeCommission)} | Reference: {reference}")
    });
        }

        /// <summary>
        /// Adds regular branch collections to the amount collection with pipe-separated narrations.
        /// </summary>
        private void AddRegularBranchCollections(List<AmountCollection> amountCollection, TransactionDto transaction, string reference)
        {
            string IB = transaction.IsInterBrachOperation ? "IB" : "LB";
            amountCollection.AddRange(new[]
            {
        CreateAmountCollection(transaction.SourceBranchCommission, OperationEventRubbriqueName.CashIn_Commission_Account.ToString(),
            $"Source {IB}-Branch Cash-In Commission for {transaction.AccountType} | Amount: {BaseUtilities.FormatCurrency(transaction.SourceBranchCommission)} | Reference: {reference}"),
        CreateAmountCollection(transaction.CamCCULCommission, SharingWithPartner.CamCCULShareCashInCommission.ToString(),
            $"CamCCUL {IB}-Cash-In Commission for {transaction.AccountType} | Amount: {BaseUtilities.FormatCurrency(transaction.CamCCULCommission)} | Reference: {reference}"),
        CreateAmountCollection(transaction.FluxAndPTMCommission, SharingWithPartner.FluxAndPTMShareCashInCommission.ToString(),
            $"Flux and PTM {IB}-Cash-In Commission for {transaction.AccountType} | Amount: {BaseUtilities.FormatCurrency(transaction.FluxAndPTMCommission)} | Reference: {reference}"),
        CreateAmountCollection(transaction.HeadOfficeCommission, SharingWithPartner.HeadOfficeShareCashInCommission.ToString(),
            $"Head Office {IB}-Cash-In Commission for {transaction.AccountType} | Amount: {BaseUtilities.FormatCurrency(transaction.HeadOfficeCommission)} | Reference: {reference}")
    });
        }

        /// <summary>
        /// Creates a collection entry for a given amount and event attribute with pipe-separated narration.
        /// </summary>
        private AmountCollection CreateAmountCollection(decimal amount, string eventAttributeName, string narration)
        {
            return new AmountCollection
            {
                Amount = amount, // Monetary amount.
                IsPrincipal = false, // Not a principal transaction.
                EventAttributeName = eventAttributeName, // Associated event name.
                IsInterBankOperationPrincipalCommission = false, // Default value for principal commission flag.
                Naration = narration // Pipe-separated narration.
            };
        }

        /// <summary>
        /// Adds fees related to member registration to the event collections with pipe-separated narrations.
        /// </summary>
        private void AddMemberRegistrationFees(List<AmountEventCollection> amountEventCollections, List<MemberActivationResponse> memberActivations, string reference)
        {
            foreach (var response in memberActivations)
            {
                amountEventCollections.Add(new AmountEventCollection
                {
                    Amount = response.Paid, // Amount paid for member activation.
                    EventCode = response.EventCode, // Event code associated with the activation.
                    Naration = $"Member Activation Fee | Service: {response.ServiceName} | Amount: {BaseUtilities.FormatCurrency(response.Paid)} | Reference: {reference}" // Pipe-separated narration.
                });
            }
        }





    }
}
