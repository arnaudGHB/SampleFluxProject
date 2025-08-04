using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Helper.Helper;
using Microsoft.IdentityModel.Tokens;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.MediatR;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Repository.AccountServices;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.Data.Dto.Receipts.Payments;
using CBS.TransactionManagement.Data.Dto.Receipts.Details;
using DocumentFormat.OpenXml.ExtendedProperties;
using CBS.TransactionManagement.Repository.Receipts.Payments;
using CBS.TransactionManagement.MediatR.Accounting.Command;
using CBS.TransactionManagement.MediatR.Commands;
using CBS.TransactionManagement.Repository.DailyStatisticBoard;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to add a new Transaction.
    /// </summary>
    public class AddBulkOperationWithdrawalCommandHandler : IRequestHandler<AddBulkOperationWithdrawalCommand, ServiceResponse<PaymentReceiptDto>>
    {
        private readonly IAccountRepository _AccountRepository;
        private readonly IWithdrawalServices _WithdrawalServices;
        private readonly IAccountingDayRepository _accountingDayRepository;
        IConfigRepository _configRepository;
        private readonly IPaymentReceiptRepository _paymentReceiptRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddBulkOperationWithdrawalCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IDailyTellerRepository _tellerProvisioningAccountRepository;
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvioningHistoryRepository;
        private readonly ITellerRepository _tellerRepository;
        public IMediator _mediator { get; set; }
        private readonly IGeneralDailyDashboardRepository _generalDailyDashboardRepository; // Repository for accessing account data.

        /// <summary>
        /// Constructor for initializing the WithdrawalTransactionCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Transaction data access.</param>
        /// <param name="TransactionRepository">Repository for Transaction data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddBulkOperationWithdrawalCommandHandler(
            IAccountRepository AccountRepository,
            ICurrencyNotesRepository CurrencyNotesRepository,
            IConfigRepository ConfigRepository,
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<AddBulkOperationWithdrawalCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IDailyTellerRepository tellerProvisioningAccountRepository = null,
            ITellerRepository tellerRepository = null,
            IMediator mediator = null,
            ISubTellerProvisioningHistoryRepository subTellerProvioningHistoryRepository = null,
            IWithdrawalServices withdrawalServices = null,
            IAccountingDayRepository accountingDayRepository = null,
            IPaymentReceiptRepository paymentReceiptRepository = null,
            IGeneralDailyDashboardRepository generalDailyDashboardRepository = null)
        {
            _AccountRepository = AccountRepository;
            _configRepository = ConfigRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _tellerProvisioningAccountRepository = tellerProvisioningAccountRepository;
            _tellerRepository = tellerRepository;
            _mediator = mediator;
            _subTellerProvioningHistoryRepository = subTellerProvioningHistoryRepository;
            _WithdrawalServices = withdrawalServices;
            _accountingDayRepository = accountingDayRepository;
            _paymentReceiptRepository = paymentReceiptRepository;
            _generalDailyDashboardRepository = generalDailyDashboardRepository;
        }

        /// <summary>
        /// Handles the WithdrawalTransactionCommand to add a new Transaction.
        /// </summary>
        /// <param name="request">The WithdrawalTransactionCommand containing Transaction data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>

        public async Task<ServiceResponse<PaymentReceiptDto>> Handle(AddBulkOperationWithdrawalCommand requests, CancellationToken cancellationToken)
        {
            bool isInterBranchOperation = false; // Flag to indicate if the operation is inter-branch.

            try
            {
                var accountingDate = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);

                // Check if system configuration is set.
                var config = await _configRepository.GetConfigAsync(OperationSourceType.Web_Portal.ToString());
                // Check if the user account serves as a teller today
                var dailyTeller = await _tellerProvisioningAccountRepository.GetAnActiveSubTellerForTheDate();
                // Check if the accounting day is still open
                await _subTellerProvioningHistoryRepository.CheckIfDayIsStillOpened(dailyTeller.TellerId, accountingDate);
                // Retrieve teller information
                var teller = await _tellerRepository.RetrieveTeller(dailyTeller); // Retrieve teller details.
                // Check teller rights.
                await _tellerRepository.CheckTellerOperationalRights(teller, requests.OperationType, requests.IsCashOperation);
                // Retrieve sub teller account
                var tellerAccount = await _AccountRepository.RetrieveTellerAccount(teller); // Retrieve teller's account.
                // Retrieve customer information
                var customer = await GetCustomer(requests); // Retrieve customer details.
                // Retrieve branch information
                var branch = await GetBranch(customer.BranchId); // Retrieve branch details.
                // Calculate total amount and charges
                decimal amount = CalculateTotalAmount(requests.BulkOperations); // Calculate total amount.
                decimal customer_charges = CalculateTotalCharges(requests.BulkOperations); // Calculate total charges.
                if (teller.BranchId != customer.BranchId)
                {
                    isInterBranchOperation = true;
                }                                                                         // Generate transaction reference based on branch type
                string reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, TransactionType.WITHDRAWAL.ToString(), isInterBranchOperation); // Generate transaction reference.

                // Retrieve currency notes
                var currencyNote = await RetrieveCurrencyNotes(reference, requests.BulkOperations.FirstOrDefault().currencyNotes); // Retrieve currency notes.

                var subTellerProvioning= _subTellerProvioningHistoryRepository.CashOutByDenomination(amount, requests.BulkOperations.FirstOrDefault().currencyNotes, teller.Id, accountingDate, customer_charges, requests.BulkOperations.FirstOrDefault().IsChargesInclussive,tellerAccount.OpenningOfDayReference);

                // Map the list of BulkOperation objects to a list of corresponding BulkDeposit objects
                var bulkWithdrawals = CurrencyNotesMapper.MapBulkOperationListToBulkDepositList(requests.BulkOperations);
                customer.BranchCode = branch.branchCode;

                var notes = requests.BulkOperations.FirstOrDefault().currencyNotes;
                var transactions = await ProcessBulkTransactions(bulkWithdrawals, teller, tellerAccount, isInterBranchOperation, customer, currencyNote.Data.ToList(), branch, reference, config, accountingDate); // Process bulk transactions.

                // Step 19: Prepare payment receipt details.
                var accountDeposits = new List<AccountDeposit>();
                var paymentReciepts = new List<PaymentDetailObject>();
                // Add account deposit details.
                foreach (var transaction in transactions)
                {
                    paymentReciepts.Add(new PaymentDetailObject
                    {
                        AccountNumber = transaction.AccountNumber,
                        Fee = transaction.Fee,
                        Amount = transaction.Amount,
                        Interest = 0,
                        LoanCapital = 0,
                        SericeOrEventName = transaction.Account.AccountName,
                        VAT = 0,
                        Balance = 0
                    });
                    accountDeposits.Add(new AccountDeposit
                    {
                        AccountName = transaction.Account.AccountName,
                        Amount = transaction.Amount,
                        Charge = transaction.Fee
                    });
                }

                // Step 20: Create payment processing request object.
                var paymentProcessing = new PaymentProcessingRequest
                {
                    AccountingDay = accountingDate,
                    Amount = amount,
                    MemberName = customer.Name,
                    NotesRequest = notes,
                    OperationType = OperationType.Cash.ToString(),
                    OperationTypeGrouping = TransactionType.CASH_WITHDRAWAL.ToString(),
                    PortalUsed = OperationSourceType.Web_Portal.ToString(),
                    PaymentDetails = paymentReciepts,
                    ServiceType = TransactionType.CASH_WITHDRAWAL.ToString(),
                    SourceOfRequest = OperationSourceType.Physical_Teller.ToString(),
                    TotalAmount = amount,
                    TotalCharges = customer_charges,
                    Transactions = transactions
                };
                // Step 21: Process payment and get receipt.
                var paymentReceipt = _paymentReceiptRepository.ProcessPaymentAsync(paymentProcessing);
                var TellerBranch = await GetBranch(teller.BranchId);
                var cashOperation = new CashOperation(teller.BranchId, paymentProcessing.Amount, paymentProcessing.TotalCharges, TellerBranch.name, TellerBranch.branchCode, CashOperationType.CashOut, LogAction.WithdrawalProcessed, subTellerProvioning);
                await _generalDailyDashboardRepository.UpdateOrCreateDashboardAsync(cashOperation);

                // Step 22: Save changes to the database.
                await _uow.SaveAsync();
                // Step 23: Calculate total charges and prepare transaction details.
                customer_charges = transactions.Sum(x => x.Fee);
                // Step 24: Send SMS notification to customer.
                //await SendSMS(accountDeposits, amount, reference, customer, branch, customer_charges);
                // Step 25: Post accounting entries for transactions.
                var accountingResponseMessages = await PostAccounting(transactions, branch, isInterBranchOperation, customer,  accountingDate);
                // Step 26: Prepare and return the response.
                string accountDetails = string.Join("\n", accountDeposits.Select((ad, index) =>
                    $"{index + 1}. {ad.AccountName}: {BaseUtilities.FormatCurrency(ad.Amount)}, Fee: {BaseUtilities.FormatCurrency(ad.Charge)}"));
                var paymentReceiptDto = _mapper.Map<PaymentReceiptDto>(paymentReceipt);
                if (accountingResponseMessages == null)
                {
                    accountingResponseMessages = $"Account Posting: Successful. A cash withdrawal of {BaseUtilities.FormatCurrency(amount)} with total charges of {BaseUtilities.FormatCurrency(paymentProcessing.TotalCharges)} was processed by {teller.Name} [User: {dailyTeller.UserName}] for Member: {customer.Name}. The withdrawal was deducted from the following accounts:\n{accountDetails}";
                }
                else
                {
                    accountingResponseMessages = $"Account Posting: Failed. A cash withdrawal of {BaseUtilities.FormatCurrency(amount)} with total charges of {BaseUtilities.FormatCurrency(paymentProcessing.TotalCharges)} was processed by {teller.Name} [User: {dailyTeller.UserName}] for Member: {customer.Name}. The withdrawal was attempted from the following accounts:\n{accountDetails}";
                }
                await BaseUtilities.LogAndAuditAsync(accountingResponseMessages, requests, HttpStatusCodeEnum.OK, LogAction.WithdrawalProcessed, LogLevelInfo.Information);

                _logger.LogInformation(accountingResponseMessages);

                return ServiceResponse<PaymentReceiptDto>.ReturnResultWith200(paymentReceiptDto, accountingResponseMessages);

            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while commiting withdrawal operation: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, requests, HttpStatusCodeEnum.InternalServerError, LogAction.WithdrawalProcessed, LogLevelInfo.Error);
                return ServiceResponse<PaymentReceiptDto>.Return500(errorMessage);
            }
        }
        // Method to send SMS notification
        private async Task SendSMS(List<AccountDeposit> accountWithdrawals, decimal amount, string reference, CustomerDto customer, BranchDto branch, decimal charge)
        {
            // Format account details with numbering and line breaks
            string accountDetails = string.Join("\n", accountWithdrawals.Select((aw, index) => $"{index + 1}. {aw.AccountName}: {BaseUtilities.FormatCurrency(aw.Amount)}, Fee: {BaseUtilities.FormatCurrency(aw.Charge)}"));

            // Initialize message variable
            string msg;
            string branchName = branch?.name ?? "";

            // Check the customer's language preference
            if (customer.Language.ToLower() == "english")
            {
                msg = $"Dear {customer.FirstName}, withdrawal of {BaseUtilities.FormatCurrency(amount)} from account(s):\n{accountDetails} was successful.\n" +
                      $"Reference: {reference}. Total charge: {BaseUtilities.FormatCurrency(charge)}. Date: {BaseUtilities.UtcToDoualaTime(DateTime.UtcNow)}. Thank you.";
            }
            else
            {
                msg = $"Cher(e) {customer.FirstName}, retrait de {BaseUtilities.FormatCurrency(amount)} des comptes:\n{accountDetails} réussi.\n" +
                      $"Référence: {reference}. Frais totaux : {BaseUtilities.FormatCurrency(charge)}. Date : {BaseUtilities.UtcToDoualaTime(DateTime.UtcNow)}. Merci.";
            }

            // Create command to send SMS
            var sMSPICallCommand = new SendSMSPICallCommand
            {
                messageBody = msg,
                recipient = customer.Phone
            };

            // Send command to mediator
            await _mediator.Send(sMSPICallCommand);
        }
        // Method to post accounting entries
        //private async Task<string> PostAccounting(List<TransactionDto> transactions, BranchDto branch, bool isInterBranchOperation, CustomerDto customer,DateTime accountingDay)
        //{
        //    string accountingResponseMessages = null; // Initialize variable to store accounting response messages.

        //    foreach (var transaction in transactions)
        //    {
        //        var apiRequest = MakeAccountingPosting(transaction.Amount, transaction.Account, transaction, branch, isInterBranchOperation, customer.LegalForm, accountingDay); // Create accounting posting request.
        //        var result = await _mediator.Send(apiRequest); // Send request to mediator.

        //        if (result.StatusCode != 200)
        //        {
        //            accountingResponseMessages += $"{result.Message}, "; // Append error message to response messages.
        //        }
        //    }

        //    return accountingResponseMessages; // Return accounting response messages.
        //}
        /// <summary>
        /// Posts accounting entries for a list of transactions, handling inter-branch operations and customer legal forms.
        /// </summary>
        /// <param name="transactions">List of transactions to process.</param>
        /// <param name="branch">The branch information for the transactions.</param>
        /// <param name="isInterBranchOperation">Flag to indicate if the operation is inter-branch.</param>
        /// <param name="customer">The customer information for legal form usage.</param>
        /// <param name="accountingDate">The accounting date for the transactions.</param>
        /// <returns>A string with accounting response messages if any errors occur.</returns>
        private async Task<string> PostAccounting(List<TransactionDto> transactions, BranchDto branch, bool isInterBranchOperation, CustomerDto customer, DateTime accountingDate)
        {
            string accountingResponseMessages = null; // Initialize variable to store accounting response messages.
            var addAccountingPostings = new AddAccountingPostingCommandList(); // Initialize the list of accounting postings.
            int transactionIndex = 0; // Transaction index for tracking the first transaction.
            addAccountingPostings.OperationType = OperationType.Withdrawal.ToString();
            // Iterate through each transaction
            foreach (var transaction in transactions)
            {
                transactionIndex++; // Increment transaction index

                // Determine the amount to post: if charges are inclusive, add the transaction fee to the amount.
                decimal amountToPost = Math.Abs(transaction.Amount);
                if (transaction.IsChargesIclussive)
                {
                    amountToPost = Math.Abs(transaction.Amount) + transaction.Fee;
                }

                // Create an accounting posting for the transaction
                var addAccountingPosting = MakeAccountingPosting(
                    amountToPost, // Use the calculated amount to post
                    transaction.Account,
                    transaction,
                    branch,
                    isInterBranchOperation,
                    customer.LegalForm,
                    accountingDate
                );

                // Add the created posting to the list of accounting postings
                addAccountingPostings.MakeAccountPostingCommands.Add(addAccountingPosting);
            }

            // Send the list of accounting postings to the mediator and await the result
            var result = await _mediator.Send(addAccountingPostings);

            // If the result status is not successful, add the error message to the response
            if (result.StatusCode != 200)
            {
                accountingResponseMessages = $"{result.Message}, "; // Append error message to the response.
            }

            return accountingResponseMessages; // Return any accumulated accounting response messages.
        }

        // Method to create an accounting posting command
        private AddAccountingPostingCommand MakeAccountingPosting(decimal Amount, Account account, TransactionDto transaction, BranchDto branch, bool IsInterBranch, string legalForm, DateTime accountingDate)
        {
            // Create a new AddAccountingPostingCommand instance
            var addAccountingPostingCommand = new AddAccountingPostingCommand
            {
                AccountHolder = account.AccountName,
                OperationType = TransactionType.WITHDRAWAL.ToString(),
                AccountNumber = account.AccountNumber,
                ProductId = account.ProductId,
                ProductName = account.Product.Name,
                Naration = transaction.Note,
                TransactionReferenceId = transaction.TransactionReference,
                IsInterBranchTransaction = IsInterBranch,
                ExternalBranchCode = branch.branchCode,
                ExternalBranchId = branch.id,
                TransactionDate = accountingDate,
                AmountCollection = new List<AmountCollection>(),
                AmountEventCollections = new List<AmountEventCollection>()
            };

            // Add amount collections based on IsInterBranch flag
            if (IsInterBranch)
            {
                // For inter-branch transactions
                addAccountingPostingCommand.AmountCollection.Add(new AmountCollection
                {
                    Amount = Math.Abs(Amount),
                    IsPrincipal = true,
                    EventAttributeName = OperationEventRubbriqueName.Principal_Saving_Account.ToString(),
                    IsInterBankOperationPrincipalCommission = IsInterBranch
                });
                addAccountingPostingCommand.AmountCollection.Add(new AmountCollection
                {
                    Amount = transaction.SourceBranchCommission,
                    IsPrincipal = false,
                    HasPaidCommissionByCash = transaction.IsChargesIclussive ? false : true,
                    EventAttributeName = SharingWithPartner.SourceBrachCommission_Account.ToString(),
                    IsInterBankOperationPrincipalCommission = IsInterBranch
                });
                addAccountingPostingCommand.AmountCollection.Add(new AmountCollection
                {
                    Amount = transaction.DestinationBranchCommission,
                    IsPrincipal = false,
                    HasPaidCommissionByCash = transaction.IsChargesIclussive ? false:true,
                    EventAttributeName = SharingWithPartner.DestinationBranchCommission_Account.ToString(),
                    IsInterBankOperationPrincipalCommission = IsInterBranch
                });
                // Add amount event collections for inter- branch transactions
                addAccountingPostingCommand.AmountEventCollections.Add(new AmountEventCollection
                {
                    Amount = transaction.WithrawalFormCharge, // Set the building contribution amount
                    EventCode = legalForm == LegalForms.Physical_Person.ToString() ? account.Product.EventCodePhysicalPersonWithdrawalFormFee : account.Product.EventCodeMoralPersonWithdrawalFormFee, // Set the event code
                    Naration = $"Payment of withdrawal form fee. [Saving Withdrawal Form Fee: {BaseUtilities.FormatCurrency(transaction.WithrawalFormCharge)}]", // Set the narration
                });
            }
            else
            {
                // For regular branch transactions
                addAccountingPostingCommand.AmountCollection.Add(new AmountCollection
                {
                    Amount = Math.Abs(Amount),
                    IsPrincipal = true,
                    EventAttributeName = OperationEventRubbriqueName.Principal_Saving_Account.ToString(),
                    IsInterBankOperationPrincipalCommission = false
                });
                addAccountingPostingCommand.AmountCollection.Add(new AmountCollection
                {
                    Amount = transaction.OperationCharge,
                    IsPrincipal = false,
                    HasPaidCommissionByCash = transaction.IsChargesIclussive ? false : true,
                    EventAttributeName = OperationEventRubbriqueName.Withdrawal_Fee_Account.ToString(),
                    IsInterBankOperationPrincipalCommission = false
                });
                // Add amount event collections for regular branch transactions
                addAccountingPostingCommand.AmountEventCollections.Add(new AmountEventCollection
                {
                    Amount = transaction.WithrawalFormCharge, // Set the building contribution amount
                    EventCode = legalForm == LegalForms.Physical_Person.ToString() ? account.Product.EventCodePhysicalPersonWithdrawalFormFee : account.Product.EventCodeMoralPersonWithdrawalFormFee, // Set the event code
                    Naration = $"Payment of withdrawal form fee. [Saving Withdrawal Form Fee: {BaseUtilities.FormatCurrency(transaction.WithrawalFormCharge)}]", // Set the narration
                });
            }

            return addAccountingPostingCommand;
        }
        // Method to retrieve currency notes
        private async Task<ServiceResponse<List<CurrencyNotesDto>>> RetrieveCurrencyNotes(string reference, CurrencyNotesRequest currencyNotesRequest)
        {
            var addCurrencyNotesCommand = new AddCurrencyNotesCommand { CurrencyNote = currencyNotesRequest, Reference = reference }; // Create command to add currency notes.
            var currencyNoteResponse = await _mediator.Send(addCurrencyNotesCommand); // Send command to mediator.

            if (currencyNoteResponse.StatusCode != 200)
            {
                return ServiceResponse<List<CurrencyNotesDto>>.Return403(""); // Return error response if currency notes retrieval fails.
            }
            return currencyNoteResponse; // Return currency notes data.
        }
        // Method to process bulk transactions
        private async Task<List<TransactionDto>> ProcessBulkTransactions(List<BulkDeposit> requests, Teller teller, Account tellerAccount, bool isInterBranchOperation, CustomerDto customer, List<CurrencyNotesDto> currencyNotes, BranchDto branch, string reference, Config config, DateTime accountingDate)
        {
            var transactions = new List<TransactionDto>(); // Initialize list to store transactions.

            foreach (var request in requests)
            {
                var customerAccount = await _AccountRepository.GetAccount(request.AccountNumber, TransactionType.WITHDRAWAL.ToString()); // Get customer account information.
                var customCharge = request.Fee; // Get custom charge for the transaction.
                // Set transaction properties
                request.Customer = customer;
                request.currencyNotes = currencyNotes;
                request.Branch = branch;
                request.IsExternalOperation = false;
                request.ExternalApplicationName = "N/A";
                request.ExternalReference = "N/A";
                request.SourceType = "Local";

                // Deposit amount into account
                var transaction = await _WithdrawalServices.Withdrawal(teller, tellerAccount, request, isInterBranchOperation, _userInfoToken.BranchID, customer.BranchId, customCharge, reference, customerAccount, config, false, accountingDate,false,null);

                transactions.Add(transaction); // Add transaction to list.
            }

            return transactions; // Return list of transactions.
        }
        // Method to retrieve customer information
        private async Task<CustomerDto> GetCustomer(AddBulkOperationWithdrawalCommand requests)
        {
            string customerId = requests.BulkOperations.FirstOrDefault().CustomerId; // Get customer ID from request.
            var customerCommandQuery = new GetCustomerCommandQuery { CustomerID = customerId }; // Create command to get customer.
            var customerResponse = await _mediator.Send(customerCommandQuery); // Send command to mediator.

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
        private async Task<BranchDto> GetBranch(string BranchId)
        {
            var branchCommandQuery = new GetBranchByIdCommand { branchId = BranchId }; // Create command to get branch.
            var branchResponse = await _mediator.Send(branchCommandQuery); // Send command to mediator.

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

    }
}
