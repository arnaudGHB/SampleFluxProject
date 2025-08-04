using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
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
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.Data.Dto.Receipts.Payments;
using CBS.TransactionManagement.Data.Dto.Receipts.Details;
using CBS.TransactionManagement.Repository.Receipts.Payments;
using CBS.TransactionManagement.MediatR.Accounting.Command;
using CBS.TransactionManagement.Repository.DailyStatisticBoard;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashOut.Commands;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.MediatR.UtilityServices;

namespace CBS.TransactionManagement.MediatR.Operations.Cash.CashOut.Services.NormalCashOutP
{
    /// <summary>
    /// Handles the command to add a new Transaction.
    /// </summary>
    public class NormalCashoutServices : INormalCashoutServices
    {
        private readonly IAccountRepository _AccountRepository;
        private readonly IWithdrawalServices _WithdrawalServices;
        private readonly IAccountingDayRepository _accountingDayRepository;
        IConfigRepository _configRepository;
        private readonly IPaymentReceiptRepository _paymentReceiptRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<NormalCashoutServices> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IDailyTellerRepository _tellerProvisioningAccountRepository;
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvioningHistoryRepository;
        private readonly ITellerRepository _tellerRepository;
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository;
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
        public NormalCashoutServices(
            IAccountRepository AccountRepository,
            ICurrencyNotesRepository CurrencyNotesRepository,
            IConfigRepository ConfigRepository,
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<NormalCashoutServices> logger,
            IUnitOfWork<TransactionContext> uow,
            IDailyTellerRepository tellerProvisioningAccountRepository = null,
            ITellerRepository tellerRepository = null,
            IMediator mediator = null,
            ISubTellerProvisioningHistoryRepository subTellerProvioningHistoryRepository = null,
            IWithdrawalServices withdrawalServices = null,
            IAccountingDayRepository accountingDayRepository = null,
            IPaymentReceiptRepository paymentReceiptRepository = null,
            IGeneralDailyDashboardRepository generalDailyDashboardRepository = null,
            IAPIUtilityServicesRepository utilityServicesRepository = null)
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
            _utilityServicesRepository=utilityServicesRepository;
        }



        /// <summary>
        /// Handles cash withdrawal operations, processes bulk withdrawal requests, and posts accounting entries.
        /// </summary>
        /// <param name="requests">The bulk operation withdrawal command containing withdrawal details.</param>
        /// <param name="accountingDate">The accounting date for the transaction.</param>
        /// <param name="config">System configuration details.</param>
        /// <returns>A service response containing the payment receipt details or an error message.</returns>
        public async Task<ServiceResponse<PaymentReceiptDto>> Cashout(AddBulkOperationWithdrawalCommand requests, DateTime accountingDate, Config config)
        {
            bool isInterBranchOperation = false; // Flag to indicate if the operation is inter-branch.

            try
            {
                // Step 1: Verify active teller and check if the accounting day is open.
                var dailyTeller = await _tellerProvisioningAccountRepository.GetAnActiveSubTellerForTheDate();
                await _subTellerProvioningHistoryRepository.CheckIfDayIsStillOpened(dailyTeller.TellerId, accountingDate);

                // Step 2: Retrieve teller information and check operational rights.
                var teller = await _tellerRepository.RetrieveTeller(dailyTeller);
                await _tellerRepository.CheckTellerOperationalRights(teller, requests.OperationType, requests.IsCashOperation);

                // Step 3: Retrieve teller and customer accounts.
                var tellerAccount = await _AccountRepository.RetrieveTellerAccount(teller);
                var customer = await GetCustomer(requests);
                var branch = await GetBranch(customer.BranchId);

                // Step 4: Check for inter-branch operation.
                if (teller.BranchId != customer.BranchId)
                {
                    isInterBranchOperation = true;
                }

                // Step 5: Generate a unique transaction reference based on branch type.
                string reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, TransactionType.WITHDRAWAL.ToString(), isInterBranchOperation);

                // Step 6: Calculate the total withdrawal amount and charges.
                decimal amount = CalculateTotalAmount(requests.BulkOperations);
                decimal customerCharges = CalculateTotalCharges(requests.BulkOperations);

                // Step 7: Retrieve currency notes and record cash-out by denomination.
                var currencyNote = await RetrieveCurrencyNotes(reference, requests.BulkOperations.FirstOrDefault().currencyNotes);
                var subTellerProvisioning = _subTellerProvioningHistoryRepository.CashOutByDenomination(
                    amount,
                    requests.BulkOperations.FirstOrDefault().currencyNotes,
                    teller.Id,
                    accountingDate,
                    customerCharges,
                    requests.BulkOperations.FirstOrDefault().IsChargesInclussive,
                    tellerAccount.OpenningOfDayReference
                );

                // Step 8: Map bulk operations to withdrawal requests.
                var bulkWithdrawals = CurrencyNotesMapper.MapBulkOperationListToBulkDepositList(requests.BulkOperations);
                customer.BranchCode = branch.branchCode;
                var notes = requests.BulkOperations.FirstOrDefault().currencyNotes;

                // Step 9: Process bulk withdrawal transactions.
                var transactions = await ProcessBulkTransactions(
                    bulkWithdrawals,
                    teller,
                    tellerAccount,
                    isInterBranchOperation,
                    customer,
                    currencyNote.Data.ToList(),
                    branch,
                    reference,
                    config,
                    accountingDate
                );

                // Step 10: Prepare payment receipt and transaction details.
                var accountDeposits = new List<AccountDeposit>();
                var paymentReceipts = new List<PaymentDetailObject>();

                foreach (var transaction in transactions)
                {
                    paymentReceipts.Add(new PaymentDetailObject
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

                // Step 11: Create a payment processing request.
                var paymentProcessing = new PaymentProcessingRequest
                {
                    AccountingDay = accountingDate,
                    Amount = amount,
                    MemberName = customer.Name,
                    NotesRequest = notes,
                    OperationType = OperationType.Cash.ToString(),
                    OperationTypeGrouping = TransactionType.CASH_WITHDRAWAL.ToString(),
                    PortalUsed = OperationSourceType.Web_Portal.ToString(),
                    PaymentDetails = paymentReceipts,
                    ServiceType = TransactionType.CASH_WITHDRAWAL.ToString(),
                    SourceOfRequest = OperationSourceType.Physical_Teller.ToString(),
                    TotalAmount = amount,
                    TotalCharges = customerCharges,
                    Transactions = transactions
                };

                // Step 12: Process the payment and get the receipt.
                var paymentReceipt = _paymentReceiptRepository.ProcessPaymentAsync(paymentProcessing);

                // Step 13: Update the daily dashboard with the cash-out operation.
                var tellerBranch = await GetBranch(teller.BranchId);
                var cashOperation = new CashOperation(
                    teller.BranchId,
                    paymentProcessing.Amount,
                    paymentProcessing.TotalCharges,
                    tellerBranch.name,
                    tellerBranch.branchCode,
                    CashOperationType.CashOut,
                    LogAction.WithdrawalProcessed,
                    subTellerProvisioning
                );
                await _generalDailyDashboardRepository.UpdateOrCreateDashboardAsync(cashOperation);

                // Step 14: Save changes to the database.
                await _uow.SaveAsync();

                // Step 15: Send an SMS notification to the customer.
                await SendSMS(accountDeposits, amount, reference, customer, branch, customerCharges);

                // Step 16: Post accounting entries for the transactions.
                var accountingResponseMessages = await PostAccounting(transactions, branch, isInterBranchOperation, customer, accountingDate);

                // Step 17: Prepare the response message.
                string accountDetails = string.Join("\n", accountDeposits.Select((ad, index) =>
                    $"{index + 1}. {ad.AccountName}: {BaseUtilities.FormatCurrency(ad.Amount)}, Fee: {BaseUtilities.FormatCurrency(ad.Charge)}"));
                var paymentReceiptDto = _mapper.Map<PaymentReceiptDto>(paymentReceipt);

                string successMessage = accountingResponseMessages == null
                    ? $"Account Posting: Successful. A cash withdrawal of {BaseUtilities.FormatCurrency(amount)} with total charges of {BaseUtilities.FormatCurrency(paymentProcessing.TotalCharges)} was processed by {teller.Name} [User: {dailyTeller.UserName}] for Member: {customer.Name}. The withdrawal was deducted from the following accounts:\n{accountDetails}"
                    : $"Account Posting: Failed. A cash withdrawal of {BaseUtilities.FormatCurrency(amount)} with total charges of {BaseUtilities.FormatCurrency(paymentProcessing.TotalCharges)} was processed by {teller.Name} [User: {dailyTeller.UserName}] for Member: {customer.Name}. The withdrawal was attempted from the following accounts:\n{accountDetails}";

                // Step 18: Log the success and audit.
                await BaseUtilities.LogAndAuditAsync(successMessage, requests, HttpStatusCodeEnum.OK, LogAction.WithdrawalProcessed, LogLevelInfo.Information);
                _logger.LogInformation(successMessage);

                // Return the success response.
                return ServiceResponse<PaymentReceiptDto>.ReturnResultWith200(paymentReceiptDto, successMessage);
            }
            catch (Exception e)
            {
                // Step 19: Handle any errors and log them.
                var errorMessage = $"Error occurred while processing the withdrawal: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, requests, HttpStatusCodeEnum.InternalServerError, LogAction.WithdrawalProcessed, LogLevelInfo.Error);
                return ServiceResponse<PaymentReceiptDto>.Return500(errorMessage);
            }
        }




        /// <summary>
        /// Handles the WithdrawalTransactionCommand to add a new Transaction.
        /// </summary>
        /// <param name="request">The WithdrawalTransactionCommand containing Transaction data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>

        //public async Task<ServiceResponse<PaymentReceiptDto>> Cashout(AddBulkOperationWithdrawalCommand requests, DateTime accountingDate, Config config)
        //{
        //    bool isInterBranchOperation = false; // Flag to indicate if the operation is inter-branch.

        //    try
        //    {
        //        // Check if the user account serves as a teller today
        //        var dailyTeller = await _tellerProvisioningAccountRepository.GetAnActiveSubTellerForTheDate();
        //        // Check if the accounting day is still open
        //        await _subTellerProvioningHistoryRepository.CheckIfDayIsStillOpened(dailyTeller.TellerId, accountingDate);
        //        // Retrieve teller information
        //        var teller = await _tellerRepository.RetrieveTeller(dailyTeller); // Retrieve teller details.
        //        // Check teller rights.
        //        await _tellerRepository.CheckTellerOperationalRights(teller, requests.OperationType, requests.IsCashOperation);
        //        // Retrieve sub teller account
        //        var tellerAccount = await _AccountRepository.RetrieveTellerAccount(teller); // Retrieve teller's account.
        //        // Retrieve customer information
        //        var customer = await GetCustomer(requests); // Retrieve customer details.
        //        // Retrieve branch information
        //        var branch = await GetBranch(customer.BranchId); // Retrieve branch details.
        //        // Calculate total amount and charges
        //        decimal amount = CalculateTotalAmount(requests.BulkOperations); // Calculate total amount.
        //        decimal customer_charges = CalculateTotalCharges(requests.BulkOperations); // Calculate total charges.
        //        if (teller.BranchId != customer.BranchId)
        //        {
        //            isInterBranchOperation = true;
        //        }                                                                         // Generate transaction reference based on branch type
        //        string reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, TransactionType.WITHDRAWAL.ToString(), isInterBranchOperation); // Generate transaction reference.

        //        // Retrieve currency notes
        //        var currencyNote = await RetrieveCurrencyNotes(reference, requests.BulkOperations.FirstOrDefault().currencyNotes); // Retrieve currency notes.

        //        var subTellerProvioning = _subTellerProvioningHistoryRepository.CashOutByDenomination(amount, requests.BulkOperations.FirstOrDefault().currencyNotes, teller.Id, accountingDate, customer_charges, requests.BulkOperations.FirstOrDefault().IsChargesInclussive, tellerAccount.OpenningOfDayReference);

        //        // Map the list of BulkOperation objects to a list of corresponding BulkDeposit objects
        //        var bulkWithdrawals = CurrencyNotesMapper.MapBulkOperationListToBulkDepositList(requests.BulkOperations);
        //        customer.BranchCode = branch.branchCode;

        //        var notes = requests.BulkOperations.FirstOrDefault().currencyNotes;
        //        var transactions = await ProcessBulkTransactions(bulkWithdrawals, teller, tellerAccount, isInterBranchOperation, customer, currencyNote.Data.ToList(), branch, reference, config, accountingDate); // Process bulk transactions.

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
        //            OperationTypeGrouping = TransactionType.CASH_WITHDRAWAL.ToString(),
        //            PortalUsed = OperationSourceType.Web_Portal.ToString(),
        //            PaymentDetails = paymentReciepts,
        //            ServiceType = TransactionType.CASH_WITHDRAWAL.ToString(),
        //            SourceOfRequest = OperationSourceType.Physical_Teller.ToString(),
        //            TotalAmount = amount,
        //            TotalCharges = customer_charges,
        //            Transactions = transactions
        //        };
        //        // Step 21: Process payment and get receipt.
        //        var paymentReceipt = _paymentReceiptRepository.ProcessPaymentAsync(paymentProcessing);
        //        var TellerBranch = await GetBranch(teller.BranchId);
        //        var cashOperation = new CashOperation(teller.BranchId, paymentProcessing.Amount, paymentProcessing.TotalCharges, TellerBranch.name, TellerBranch.branchCode, CashOperationType.CashOut, LogAction.WithdrawalProcessed, subTellerProvioning);
        //        await _generalDailyDashboardRepository.UpdateOrCreateDashboardAsync(cashOperation);

        //        // Step 22: Save changes to the database.
        //        await _uow.SaveAsync();
        //        // Step 23: Calculate total charges and prepare transaction details.
        //        customer_charges = transactions.Sum(x => x.Fee);
        //        // Step 24: Send SMS notification to customer.
        //        await SendSMS(accountDeposits, amount, reference, customer, branch, customer_charges);
        //        // Step 25: Post accounting entries for transactions.
        //        var accountingResponseMessages = await PostAccounting(transactions, branch, isInterBranchOperation, customer, accountingDate);
        //        // Step 26: Prepare and return the response.
        //        string accountDetails = string.Join("\n", accountDeposits.Select((ad, index) =>
        //            $"{index + 1}. {ad.AccountName}: {BaseUtilities.FormatCurrency(ad.Amount)}, Fee: {BaseUtilities.FormatCurrency(ad.Charge)}"));
        //        var paymentReceiptDto = _mapper.Map<PaymentReceiptDto>(paymentReceipt);
        //        if (accountingResponseMessages == null)
        //        {
        //            accountingResponseMessages = $"Account Posting: Successful. A cash withdrawal of {BaseUtilities.FormatCurrency(amount)} with total charges of {BaseUtilities.FormatCurrency(paymentProcessing.TotalCharges)} was processed by {teller.Name} [User: {dailyTeller.UserName}] for Member: {customer.Name}. The withdrawal was deducted from the following accounts:\n{accountDetails}";
        //        }
        //        else
        //        {
        //            accountingResponseMessages = $"Account Posting: Failed. A cash withdrawal of {BaseUtilities.FormatCurrency(amount)} with total charges of {BaseUtilities.FormatCurrency(paymentProcessing.TotalCharges)} was processed by {teller.Name} [User: {dailyTeller.UserName}] for Member: {customer.Name}. The withdrawal was attempted from the following accounts:\n{accountDetails}";
        //        }
        //        await BaseUtilities.LogAndAuditAsync(accountingResponseMessages, requests, HttpStatusCodeEnum.OK, LogAction.WithdrawalProcessed, LogLevelInfo.Information);

        //        _logger.LogInformation(accountingResponseMessages);

        //        return ServiceResponse<PaymentReceiptDto>.ReturnResultWith200(paymentReceiptDto, accountingResponseMessages);

        //    }
        //    catch (Exception e)
        //    {
        //        // Log error and return 500 Internal Server Error response with error message
        //        var errorMessage = $"Error occurred while commiting withdrawal operation: {e.Message}";
        //        _logger.LogError(errorMessage);
        //        await BaseUtilities.LogAndAuditAsync(errorMessage, requests, HttpStatusCodeEnum.InternalServerError, LogAction.WithdrawalProcessed, LogLevelInfo.Error);
        //        return ServiceResponse<PaymentReceiptDto>.Return500(errorMessage);
        //    }
        //}
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
                msg = $"Withdrawal of {BaseUtilities.FormatCurrency(amount)} from account(s):\n{accountDetails} was successful.\n" +
                      $"Reference: {reference}. Total charge: {BaseUtilities.FormatCurrency(charge)}. Date: {BaseUtilities.UtcToDoualaTime(DateTime.UtcNow)}. Thank you.";
            }
            else
            {
                msg = $"Retrait de {BaseUtilities.FormatCurrency(amount)} des comptes:\n{accountDetails} réussi.\n" +
                      $"Référence: {reference}. Frais totaux : {BaseUtilities.FormatCurrency(charge)}. Date : {BaseUtilities.UtcToDoualaTime(DateTime.UtcNow)}. Merci.";
            }

            // Create command to send SMS
            var sMSPICallCommand = new SendSMSPICallCommand
            {
                messageBody = msg,
                recipient = customer.Phone
            };
            await _utilityServicesRepository.PushNotification(customer.CustomerId, PushNotificationTitle.CASH_OUT, msg);

            // Send command to _mediator
            await _mediator.Send(sMSPICallCommand);
        }

        /// <summary>
        /// Posts a list of transactions to the accounting system, creating corresponding accounting postings for each transaction.
        /// </summary>
        /// <param name="transactions">The list of transactions to be posted.</param>
        /// <param name="branch">The branch where the transactions occur.</param>
        /// <param name="isInterBranchOperation">Indicates if the transactions are inter-branch operations.</param>
        /// <param name="customer">The customer associated with the transactions.</param>
        /// <param name="accountingDate">The date for the accounting entry.</param>
        /// <returns>A string containing any accounting response messages or errors.</returns>
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
        /// This function handles both inter-branch and regular branch transactions, optionally including withdrawal form fees.
        /// </summary>
        private AddAccountingPostingCommand MakeAccountingPosting(
            decimal amount,
            Account account,
            TransactionDto transaction,
            BranchDto branch,
            bool isInterBranch,
            string legalForm,
            DateTime accountingDate)
        {
            string IB = isInterBranch ? "IB" : "LB";
            var addAccountingPostingCommand = new AddAccountingPostingCommand
            {
                AccountHolder = account.AccountName,
                OperationType = TransactionType.WITHDRAWAL.ToString(),
                AccountNumber = account.AccountNumber,
                ProductId = account.ProductId,
                ProductName = account.Product.Name,
                Naration = $"{IB}-Cashout Transaction | Account: {account.AccountType} | Amount: {BaseUtilities.FormatCurrency(amount)} | Reference: {transaction.TransactionReference} | Date: {accountingDate:dd-MM-yyyy}",
                TransactionReferenceId = transaction.TransactionReference,
                IsInterBranchTransaction = isInterBranch,
                ExternalBranchCode = branch.branchCode,
                ExternalBranchId = branch.id,
                TransactionDate = accountingDate,
                MemberReference = transaction.CustomerId,
                AmountCollection = new List<AmountCollection>(),
                AmountEventCollections = new List<AmountEventCollection>()
            };

            // Adds the principal amount to the collection.
            AddPrincipalAmount(addAccountingPostingCommand.AmountCollection, amount, isInterBranch, transaction.TransactionReference);

            if (isInterBranch)
            {
                // Adds collections specific to inter-branch transactions.
                AddInterBranchCollections(addAccountingPostingCommand.AmountCollection, transaction, addAccountingPostingCommand.TransactionReferenceId);
            }
            else
            {
                // Adds collections specific to regular branch transactions.
                AddRegularBranchCollections(addAccountingPostingCommand.AmountCollection, transaction, addAccountingPostingCommand.TransactionReferenceId);
            }

            // Adds withdrawal form fees, if applicable.
            AddWithdrawalFormFee(addAccountingPostingCommand.AmountEventCollections, transaction.WithrawalFormCharge, legalForm, account);

            return addAccountingPostingCommand;
        }

        /// <summary>
        /// Adds the principal amount to the collection with a standardized pipe-separated narration.
        /// </summary>
        private void AddPrincipalAmount(List<AmountCollection> amountCollection, decimal amount, bool isInterBranch,string reference)
        {
            string IB = isInterBranch ? "IB" : "LB";
            amountCollection.Add(new AmountCollection
            {
                Amount = Math.Abs(amount),
                IsPrincipal = true,
                EventAttributeName = OperationEventRubbriqueName.Principal_Saving_Account.ToString(),
                IsInterBankOperationPrincipalCommission = isInterBranch,
                Naration = $"Principal Amount {IB}-Cashout | Amount: {BaseUtilities.FormatCurrency(amount)} | Reference: {reference}"
            });
        }

        /// <summary>
        /// Adds collections specific to inter-branch transactions with references.
        /// </summary>
        private void AddInterBranchCollections(List<AmountCollection> amountCollection, TransactionDto transaction, string reference)
        {
            string IB = transaction.IsInterBrachOperation ? "IB" : "LB";
            amountCollection.AddRange(new[]
            {
        CreateAmountCollection(transaction.SourceBranchCommission, SharingWithPartner.SourceBrachCommission_Account.ToString(),
            $"Source Branch {IB}-Cash-Out Commission for {transaction.AccountType} | Amount: {BaseUtilities.FormatCurrency(transaction.SourceBranchCommission)} | Reference: {reference}"),
        CreateAmountCollection(transaction.DestinationBranchCommission, SharingWithPartner.DestinationBranchCommission_Account.ToString(),
            $"Destination Branch {IB}-Cash-Out Commission for {transaction.AccountType} | Amount: {BaseUtilities.FormatCurrency(transaction.DestinationBranchCommission)} | Reference: {reference}"),
        CreateAmountCollection(transaction.HeadOfficeCommission, SharingWithPartner.HeadOfficeShareCashOutCommission.ToString(),
            $"Head Office {IB}-Cash-Out Commission for {transaction.AccountType} | Amount: {BaseUtilities.FormatCurrency(transaction.HeadOfficeCommission)} | Reference: {reference}"),
        CreateAmountCollection(transaction.FluxAndPTMCommission, SharingWithPartner.FluxAndPTMShareCashOutCommission.ToString(),
            $"Flux and PTM {IB}-Cash-Out Commission for {transaction.AccountType} | Amount: {BaseUtilities.FormatCurrency(transaction.FluxAndPTMCommission)} | Reference: {reference}"),
        CreateAmountCollection(transaction.CamCCULCommission, SharingWithPartner.CamCCULShareCashOutCommission.ToString(),
            $"CamCCUL {IB}-Cash-Out Commission for {transaction.AccountType} | Amount: {BaseUtilities.FormatCurrency(transaction.CamCCULCommission)} | Reference: {reference}")
    });
        }

        /// <summary>
        /// Adds collections specific to regular branch transactions with references.
        /// </summary>
        private void AddRegularBranchCollections(List<AmountCollection> amountCollection, TransactionDto transaction, string reference)
        {
            string IB = transaction.IsInterBrachOperation ? "IB" : "LB";
            amountCollection.AddRange(new[]
            {
        CreateAmountCollection(transaction.OperationCharge, OperationEventRubbriqueName.CashOut_Commission_Account.ToString(),
            $"Branch {IB}-Cash-Out Commission for {transaction.AccountType} | Amount: {BaseUtilities.FormatCurrency(transaction.OperationCharge)} | Reference: {reference}"),
        CreateAmountCollection(transaction.HeadOfficeCommission, SharingWithPartner.HeadOfficeShareCashOutCommission.ToString(),
            $"Head Office {IB}-Cash-Out Commission for {transaction.AccountType} | Amount: {BaseUtilities.FormatCurrency(transaction.HeadOfficeCommission)} | Reference: {reference}"),
        CreateAmountCollection(transaction.FluxAndPTMCommission, SharingWithPartner.FluxAndPTMShareCashOutCommission.ToString(),
            $"Flux and PTM {IB}-Cash-Out Commission for {transaction.AccountType} | Amount: {BaseUtilities.FormatCurrency(transaction.FluxAndPTMCommission)} | Reference: {reference}"),
        CreateAmountCollection(transaction.CamCCULCommission, SharingWithPartner.CamCCULShareCashOutCommission.ToString(),
            $"CamCCUL {IB}-Cash-Out Commission for {transaction.AccountType} | Amount: {BaseUtilities.FormatCurrency(transaction.CamCCULCommission)} | Reference: {reference}")
    });
        }

        /// <summary>
        /// Creates an AmountCollection instance with a pipe-separated narration.
        /// </summary>
        private AmountCollection CreateAmountCollection(decimal amount, string eventAttributeName, string narration)
        {
            return new AmountCollection
            {
                Amount = amount,
                IsPrincipal = false,
                EventAttributeName = eventAttributeName,
                IsInterBankOperationPrincipalCommission = false,
                Naration = narration
            };
        }

        /// <summary>
        /// Adds withdrawal form fees to the event collections with a pipe-separated narration.
        /// </summary>
        private void AddWithdrawalFormFee(List<AmountEventCollection> amountEventCollections, decimal withdrawalFormCharge, string legalForm, Account account)
        {
            amountEventCollections.Add(new AmountEventCollection
            {
                Amount = withdrawalFormCharge,
                EventCode = legalForm == LegalForms.Physical_Person.ToString() ? account.Product.EventCodePhysicalPersonWithdrawalFormFee : account.Product.EventCodeMoralPersonWithdrawalFormFee,
                Naration = $"Cashout Withdrawal Form Fee | Legal Form: {legalForm} | Fee: {BaseUtilities.FormatCurrency(withdrawalFormCharge)} | Account: {account.AccountName}"
            });
        }


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
                var transaction = await _WithdrawalServices.Withdrawal(teller, tellerAccount, request, isInterBranchOperation, _userInfoToken.BranchID, customer.BranchId, customCharge, reference, customerAccount, config, false, accountingDate, false, null);

                transactions.Add(transaction); // Add transaction to list.
            }

            return transactions; // Return list of transactions.
        }
        // Method to retrieve customer information
        private async Task<CustomerDto> GetCustomer(AddBulkOperationWithdrawalCommand requests)
        {
            string customerId = requests.BulkOperations.FirstOrDefault().CustomerId; // Get customer ID from request.
            var customerCommandQuery = new GetCustomerCommandQuery { CustomerID = customerId }; // Create command to get customer.
            var customerResponse = await _mediator.Send(customerCommandQuery); // Send command to _mediator.

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
            var branchCommandQuery = new GetBranchByIdCommand { BranchId = BranchId }; // Create command to get branch.
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

    }
}
