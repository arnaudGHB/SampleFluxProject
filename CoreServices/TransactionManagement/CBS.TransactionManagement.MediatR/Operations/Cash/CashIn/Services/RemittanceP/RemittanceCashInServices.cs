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
using CBS.TransactionManagement.Repository.Receipts.Payments;
using CBS.TransactionManagement.Data.Dto.Receipts.Details;
using CBS.TransactionManagement.Data.Dto.Receipts.Payments;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Commands;
using CBS.TransactionManagement.MediatR.Accounting.Command;
using CBS.TransactionManagement.Repository.DailyStatisticBoard;
using CBS.TransactionManagement.Repository.RemittanceP;
using CBS.TransactionManagement.Data.Entity.RemittanceP;
using MongoDB.Driver;
using CBS.TransactionManagement.Repository.MongoDBManager.SerialNumberGenerator;

namespace CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Services.RemittanceP
{

    public class RemittanceCashInServices : IRemittanceCashInServices
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
        private readonly IRemittanceRepository _remittanceRepository;
        private readonly IDailyTransactionCodeGenerator _dailyTransactionCodeGenerator;

        public IMediator _mediator { get; set; } // Mediator for handling requests.
        private readonly ILogger<RemittanceCashInServices> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for transaction context.

        public RemittanceCashInServices(
            IAccountRepository AccountRepository,
            UserInfoToken UserInfoToken,
            ILogger<RemittanceCashInServices> logger,
            IUnitOfWork<TransactionContext> uow,
            IDailyTellerRepository tellerProvisioningAccountRepository = null,
            ITellerRepository tellerRepository = null,
            IMediator mediator = null,
            ISubTellerProvisioningHistoryRepository subTellerProvioningHistoryRepository = null,
            IDepositServices depositServices = null,
            IPaymentReceiptRepository paymentReceiptRepository = null,
            IMapper mapper = null,
            IGeneralDailyDashboardRepository generalDailyDashboardRepository = null,
            IRemittanceRepository remittanceRepository = null,
            IDailyTransactionCodeGenerator dailyTransactionCodeGenerator = null)
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
            _paymentReceiptRepository = paymentReceiptRepository;
            _mapper = mapper;
            _generalDailyDashboardRepository = generalDailyDashboardRepository;
            _remittanceRepository=remittanceRepository;
            _dailyTransactionCodeGenerator=dailyTransactionCodeGenerator;
        }


        public async Task<ServiceResponse<PaymentReceiptDto>> RemittanceCashIn(AddBulkOperationDepositCommand requests, DateTime accountingDate, Config config)
        {
            string reference = string.Empty;

            try
            {
                var message = string.Empty;
                var remittance = await _remittanceRepository.FindAsync(requests.Id);

                if (remittance == null)
                {
                    message = $"The remittance with the specified id {requests.Id} was not found. Please ensure the ID is correct and try again.";
                    await BaseUtilities.LogAndAuditAsync(message, requests, HttpStatusCodeEnum.NotFound, LogAction.Remittance, LogLevelInfo.Warning);
                    _logger.LogInformation(message);

                    return ServiceResponse<PaymentReceiptDto>.Return404(message);
                }

                if (remittance.Status == Status.Pending.ToString())
                {
                    message = $"The remittance with the specified reference {remittance.TransactionReference} is currently in a pending state. It must be validated and approved before proceeding further.";
                    await BaseUtilities.LogAndAuditAsync(message, requests, HttpStatusCodeEnum.Forbidden, LogAction.Remittance, LogLevelInfo.Warning);
                    _logger.LogInformation(message);
                    return ServiceResponse<PaymentReceiptDto>.Return403(message);
                }

                if (remittance.Status == Status.Rejected.ToString())
                {
                    message = $"The remittance with the specified reference {remittance.TransactionReference} has been rejected and cannot be processed. Please contact the remitting branch ({remittance.SourceBranchName}) for further assistance.";
                    await BaseUtilities.LogAndAuditAsync(message, requests, HttpStatusCodeEnum.Forbidden, LogAction.Remittance, LogLevelInfo.Warning);
                    _logger.LogInformation(message);
                    return ServiceResponse<PaymentReceiptDto>.Return403(message);
                }
                if (remittance.Status == Status.Withdrawn.ToString())
                {
                    message = $"The remittance with the specified reference {remittance.TransactionReference} has already been withdrawn and processed by the receiving branch: {remittance.ReceivingBranchName}. Further actions are not permitted.";
                    await BaseUtilities.LogAndAuditAsync(message, requests, HttpStatusCodeEnum.Forbidden, LogAction.Remittance, LogLevelInfo.Warning);
                    _logger.LogInformation(message);
                    return ServiceResponse<PaymentReceiptDto>.Return403(message);
                }
                if (remittance.Status == Status.Paid.ToString())
                {
                    message = $"The remittance with reference {remittance.TransactionReference} and Amount: {BaseUtilities.FormatCurrency(remittance.Amount)} & Charge: {BaseUtilities.FormatCurrency(remittance.Fee)} has already been marked as 'Paid' and processed by the branch: {remittance.SourceBranchName}. It appears that the same cashier, {remittance.SourceTellerName}, at branch {remittance.SourceBranchName}, is attempting to collect money from the customer twice for the same transaction. Please verify the transaction details and ensure no duplicate processing is being attempted.";
                    await BaseUtilities.LogAndAuditAsync(message, requests, HttpStatusCodeEnum.Forbidden, LogAction.Remittance, LogLevelInfo.Warning);
                    _logger.LogInformation(message);
                    return ServiceResponse<PaymentReceiptDto>.Return403(message);
                }

                if (remittance.TransferType == TransferType.Incoming_International.ToString())
                {
                    message = $"The operation is not permitted. The remittance with reference {remittance.TransactionReference} for an incoming international transfer " +
                              $"and Amount: {BaseUtilities.FormatCurrency(remittance.Amount)} is processed directly as a cashout by the receiver at the receiving branch: {remittance.ReceivingBranchName}. " +
                              $"No payment is required from the reciever. Please verify the transaction details and ensure no unauthorized action is being attempted.";

                    await BaseUtilities.LogAndAuditAsync(message, requests, HttpStatusCodeEnum.Forbidden, LogAction.Remittance, LogLevelInfo.Warning);
                    _logger.LogInformation(message);
                    return ServiceResponse<PaymentReceiptDto>.Return403(message);
                }



                // Step 1: Initialize flags for inter-branch operation and first deposit.
                bool isInterBranchOperation = false;
                bool IsFirstDeposit = false;

                // Step 3: Verify if the user's account is acting as a teller today.
                var dailyTeller = await _tellerProvisioningAccountRepository.GetAnActiveSubTellerForTheDate();

                // Step 4: Check if the accounting day remains open.
                await _subTellerProvioningHistoryRepository.CheckIfDayIsStillOpened(dailyTeller.TellerId, accountingDate);

                // Step 5: Retrieve teller info based on deposit type.
                var teller = await _tellerRepository.RetrieveTeller(dailyTeller);

                // Step 6: Check teller's rights to perform the requested operation.
                await _tellerRepository.CheckTellerOperationalRights(teller, requests.OperationType, requests.IsCashOperation);

                // Step 7: Retrieve the account associated with the teller.
                var tellerAccount = await _accountRepository.RetrieveTellerAccount(teller);

                // Step 8: Retrieve remittance details.
                var customer = await GetCustomer(requests);


                // Step 11: Get remittance's branch details.
                var branch = await GetBranch(customer.BranchId);
                customer.BranchCode = branch.branchCode;
                customer.BranchName = branch.name;


                // Step 14: Calculate total deposit amount and remittance charges.
                decimal amount = CalculateTotalAmount(requests.BulkOperations);
                decimal customer_charges = CalculateTotalCharges(requests.BulkOperations);
                // Step 12: Determine if the transaction involves inter-branch activity.
                isInterBranchOperation = teller.BranchId != customer.BranchId;

                // Step 13: Generate a unique transaction reference.
                reference = await _dailyTransactionCodeGenerator.ReserveTransactionCode(
                      _userInfoToken.BranchCode, OperationPrefix.Cash_In_Remittance, TransactionType.CASH_IN, isInterBranch: false); // Generate 
                // Step 15: Process currency notes for the transaction.
                var currencyNote = await RetrieveCurrencyNotes(reference, requests.BulkOperations.FirstOrDefault().currencyNotes);
                var subTellerProvioning = _subTellerProvioningHistoryRepository.CashInByDinomination(amount, requests.BulkOperations.FirstOrDefault().currencyNotes, teller.Id, accountingDate, tellerAccount.OpenningOfDayReference);

                // Step 16: Map bulk operations to deposit objects.
                var bulkDeposits = CurrencyNotesMapper.MapBulkOperationListToBulkDepositList(requests.BulkOperations);

                // Step 17: Execute bulk transactions.
                var transactions = await ProcessBulkTransactions(bulkDeposits, teller, tellerAccount, isInterBranchOperation, customer, currencyNote.Data.ToList(), branch, reference, config, customer_charges, false, false, accountingDate, true, remittance);

                var account = await _accountRepository.GetAccountByAccountNumber(remittance.AccountNumber);


                // Step 19: Prepare payment receipt details.
                var accountDeposits = new List<AccountDeposit>();
                var paymentReciepts = new List<PaymentDetailObject>();

                foreach (var transaction in transactions)
                {
                    paymentReciepts.Add(new PaymentDetailObject { AccountNumber = transaction.AccountNumber, Fee = transaction.Fee, Amount = transaction.Amount, SericeOrEventName = transaction.Account.AccountName });
                    accountDeposits.Add(new AccountDeposit { AccountName = transaction.Account.AccountName, Amount = transaction.Amount, Charge = transaction.Fee });
                }


                // Step 20: Create payment request for processing.
                var paymentProcessing = new PaymentProcessingRequest
                {
                    AccountingDay = accountingDate,
                    Amount = amount,
                    MemberName = customer.Name,
                    NotesRequest = requests.BulkOperations.FirstOrDefault()?.currencyNotes,
                    OperationType = OperationType.Cash.ToString(),
                    OperationTypeGrouping = TransactionType.CASH_IN.ToString(),
                    PaymentDetails = paymentReciepts,
                    ServiceType = TransactionType.CASH_IN_Remittance.ToString(),
                    SourceOfRequest = OperationSourceType.Physical_Teller.ToString(),
                    TotalAmount = amount,
                    TotalCharges = transactions.Sum(x => x.Fee),
                    Transactions = transactions
                };

                // Step 21: Process payment receipt.
                var paymentReceipt = _paymentReceiptRepository.ProcessPaymentAsync(paymentProcessing);
                var TellerBranch = await GetBranch(teller.BranchId);
                var cashOperation = new CashOperation(teller.BranchId, paymentProcessing.Amount, paymentProcessing.TotalCharges, TellerBranch.name, TellerBranch.branchCode, CashOperationType.RemittanceIn, LogAction.Remittance, subTellerProvioning);
                await _generalDailyDashboardRepository.UpdateOrCreateDashboardAsync(cashOperation);

                remittance.SourceBranchCode=branch.branchCode;
                remittance.SourceBranchName=branch.name;
                remittance.SourceBranchId=branch.id;
                remittance.SourceTellerId=teller.Id;
                remittance.SourceTellerName=teller.Name;
                remittance.Status=Status.Paid.ToString();
                remittance.DatePaidToCashDesk=BaseUtilities.UtcNowToDoualaTime();
                _remittanceRepository.Update(remittance);

                // Step 22: Commit changes.
                await _uow.SaveAsync();
                await _dailyTransactionCodeGenerator.MarkTransactionAsSuccessful(reference);

                // Step 24: Send SMS notifications.
                await SendSMSToSenderAndReceiver(remittance, branch);

                // Step 25: Post accounting entries for transactions.
                var accountingResponseMessages = await PostAccounting(account, branch, isInterBranchOperation, accountingDate,remittance,reference);

                // Step 26: Prepare response with transaction details.
                var paymentReceiptDto = _mapper.Map<PaymentReceiptDto>(paymentReceipt);
                string accountDetails = string.Join("\n", accountDeposits.Select((ad, index) => $"{index + 1}. {ad.AccountName}: {BaseUtilities.FormatCurrency(ad.Amount)}, Fee: {BaseUtilities.FormatCurrency(ad.Charge)}"));

                var successMessage = $"Account Posting: {(accountingResponseMessages == null ? "Successful" : "Failed")}. Remittance deposit of {BaseUtilities.FormatCurrency(amount)} was processed by {teller.Name} for a remittance none member collective account: {customer.Name}.\n{accountDetails}";
                await BaseUtilities.LogAndAuditAsync(successMessage, requests, HttpStatusCodeEnum.OK, LogAction.Remittance, LogLevelInfo.Information);
                _logger.LogInformation(successMessage);

                return ServiceResponse<PaymentReceiptDto>.ReturnResultWith200(paymentReceiptDto, successMessage);
            }
            catch (Exception e)
            {
                await _dailyTransactionCodeGenerator.RevertTransactionCode(reference);
                // Step 27: Error handling and logging.
                var errorMessage = $"Error during deposit: {e.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, requests, HttpStatusCodeEnum.InternalServerError, LogAction.Remittance, LogLevelInfo.Error);
                _logger.LogError(errorMessage);
                return ServiceResponse<PaymentReceiptDto>.Return500(errorMessage);
            }
        }

        //Method to retrieve remittance information
        private async Task<CustomerDto> GetCustomer(AddBulkOperationDepositCommand requests)
        {
            string customerId = requests.BulkOperations.FirstOrDefault().CustomerId; // Get remittance ID from request.
            var customerCommandQuery = new GetCustomerCommandQuery { CustomerID = customerId }; // Create command to get remittance.
            var customerResponse = await _mediator.Send(customerCommandQuery); // Send command to _mediator.

            if (customerResponse == null)
            {
                var errorMessage = "Error; Null";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            // Check if remittance information retrieval was successful
            if (customerResponse.StatusCode != 200)
            {
                var errorMessage = "Failed getting member's information";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }

            var customer = customerResponse.Data; // Get remittance data from response.
            customer.Name = $"{customer.FirstName} {customer.LastName}";
            //EnsureCustomerMembershipApproved(remittance); // Ensure remittance membership is approved.

            return customer; // Return remittance data.
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

        // Method to calculate total charges from bulk operations
        private decimal CalculateTotalCharges(IEnumerable<BulkOperation> bulkOperations)
        {
            decimal total = bulkOperations.Sum(x => x.Fee);
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
        private async Task<List<TransactionDto>> ProcessBulkTransactions(List<BulkDeposit> requests, Teller teller, Account tellerAccount, bool isInterBranchOperation, CustomerDto customer, List<CurrencyNotesDto> currencyNotes, BranchDto branch, string reference, Config config, decimal customer_charges, bool IsLoanRepayment, bool IsMomocashCollection, DateTime accountingDate, bool isRemittance, Remittance remittance)
        {
            var transactions = new List<TransactionDto>(); // Initialize list to store transactions.
            string transactionType = TransactionType.DEPOSIT.ToString();
            foreach (var request in requests)
            {

                var customerAccount = await _accountRepository.GetAccount(request.AccountNumber, transactionType); // Get remittance account information.
                var customCharge = request.Fee; // Get custom charge for the transaction.
                                                // Set transaction properties
                request.Customer = customer;
                request.currencyNotes = currencyNotes;
                request.Branch = branch;
                request.IsSWS = false;
                request.IsExternalOperation = false;
                request.ExternalApplicationName = remittance.RemittanceType;
                request.ExternalReference = remittance.TransactionReference;
                request.SourceType = OperationSourceType.Web_Portal.ToString();
                var transaction = await _depositServices.Deposit(teller, tellerAccount, request, isInterBranchOperation, _userInfoToken.BranchID, customer.BranchId, customCharge, reference, customerAccount, config, customer.Name, IsMomocashCollection, accountingDate, isRemittance, remittance);
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
        // Method to send SMS notification
        private async Task SendSMSToSenderAndReceiver(Remittance remittance, BranchDto branch)
        {
            string senderMessage;
            string receiverMessage;

            // SMS to Sender based on language
            if (remittance.ReceiverLanguage != null && remittance.ReceiverLanguage.ToLower() == "french")
            {
                senderMessage = $"Cher {remittance.SenderName}, votre transfert de {BaseUtilities.FormatCurrency(remittance.Amount)} ({remittance.RemittanceType}) a été initié avec succès. " +
                                $"Référence de la transaction : {remittance.TransactionReference}. Frais : {BaseUtilities.FormatCurrency(remittance.Fee)}. Merci d'utiliser notre service.";

                receiverMessage = $"Cher {remittance.ReceiverName}, un transfert de {BaseUtilities.FormatCurrency(remittance.Amount)} ({remittance.RemittanceType}) vous a été envoyé. " +
                                   $"Veuillez visiter une agence de {branch.Bank.name} pour retirer vos fonds. Référence de la transaction : {remittance.TransactionReference}. Le code secret de retrait est {remittance.SenderSecreteCode}. Merci.";
            }
            else
            {
                senderMessage = $"Dear {remittance.SenderName}, your remittance of {BaseUtilities.FormatCurrency(remittance.Amount)} ({remittance.RemittanceType}) has been successfully initiated. " +
                                $"Transaction Reference: {remittance.TransactionReference}. Charges: {BaseUtilities.FormatCurrency(remittance.Fee)}. Thank you for using our service.";

                receiverMessage = $"Dear {remittance.ReceiverName}, a remittance of {BaseUtilities.FormatCurrency(remittance.Amount)} ({remittance.RemittanceType}) has been sent to you. " +
                                   $"Please visit any of the {branch.Bank.name}'s branch to collect your funds. Transaction Reference: {remittance.TransactionReference} & Withdrawal Secret Code is {remittance.SenderSecreteCode}. Thank you.";
            }
           
            var senderSMSCommand = new SendSMSPICallCommand
            {
                messageBody = senderMessage,
                recipient = remittance.SenderPhoneNumber
            };

            var receiverSMSCommand = new SendSMSPICallCommand
            {
                messageBody = receiverMessage,
                recipient = remittance.ReceiverPhoneNumber
            };

            await _mediator.Send(senderSMSCommand);
            if (remittance.SendSMSTotReceiver)
            {
                await _mediator.Send(receiverSMSCommand);
            }

        }


        /// <summary>
        /// Posts accounting entries for a remittance transaction.
        /// </summary>
        /// <param name="account">The member's account involved in the remittance.</param>
        /// <param name="branch">Branch details for the transaction.</param>
        /// <param name="isInterBranch">Indicates if this is an inter-branch transaction.</param>
        /// <param name="accountingDate">The date for accounting transactions.</param>
        /// <param name="remittance">Remittance transaction details.</param>
        /// <param name="reference">Unique transaction reference.</param>
        /// <returns>Returns true if accounting is posted successfully; otherwise, false.</returns>
        private async Task<bool> PostAccounting(Account account, BranchDto branch, bool isInterBranch, DateTime accountingDate, Remittance remittance, string reference)
        {
            string correlationId = Guid.NewGuid().ToString(); // Unique tracking ID for debugging

            try
            {
                _logger.LogInformation($"[INFO] Initiating remittance accounting. CorrelationId: '{correlationId}', Reference: '{reference}', Account: '{account.AccountNumber}', Amount: '{BaseUtilities.FormatCurrency(remittance.TotalAmount)}'.");

                // Validate input parameters
                if (remittance == null)
                {
                    string errorMessage = $"[ERROR] Remittance details are null. Cannot proceed with accounting. CorrelationId: '{correlationId}'.";
                    _logger.LogError(errorMessage);
                    return false;
                }

                if (remittance.TotalAmount <= 0)
                {
                    string errorMessage = $"[ERROR] Invalid remittance amount '{remittance.TotalAmount}'. Accounting aborted. CorrelationId: '{correlationId}'.";
                    _logger.LogError(errorMessage);
                    return false;
                }

                // 🔍 Step 1: Create Remittance Accounting Posting Command
                var remittanceAccountingPostingCommand = new RemittanceAccountingPostingCommand
                {
                    TransactionReferenceId = reference,
                    MemberReference = account.CustomerId,
                    ProductId = account.ProductId,
                    OperationType = TransactionType.DEPOSIT.ToString(),
                    Source = "Physical_Teller",
                    TransactionDate = accountingDate
                };

                // 🔍 Step 2: Add Principal Amount Posting (Main Transfer Amount)
                remittanceAccountingPostingCommand.AmountCollection.Add(new RemittanceAmountCollectionItem
                {
                    Amount = remittance.TotalAmount,
                    EventAttributeName = OperationEventRubbriqueName.Principal_Saving_Account.ToString(),
                    LevelOfExecution = "BRANCH_OFFICE",
                    AmountType = "MAIN_AMOUNT",
                    Naration = $"Remittance Initiating Operation: [Transaction Reference: {reference}]. [Amount with Total charges: {BaseUtilities.FormatCurrency(remittance.TotalAmount)}] [Charges: {BaseUtilities.FormatCurrency(remittance.Fee)}]"
                });

                // 🔍 Step 3: Add Commission Fees (Branch & Head Office)
                if (remittance.SourceBranchCommision > 0)
                {
                    remittanceAccountingPostingCommand.AmountCollection.Add(new RemittanceAmountCollectionItem
                    {
                        Amount = remittance.SourceBranchCommision,
                        EventAttributeName = OperationEventRubbriqueName.Transfer_Fee_Account.ToString(),
                        LevelOfExecution = "BRANCH_OFFICE",
                        AmountType = "COMMISSION_AMOUNT",
                        Naration = $"Remittance-Initiating Branch Commision: [Transaction Reference: {reference}]. [Amount: {BaseUtilities.FormatCurrency(remittance.SourceBranchCommision)}]"
                    });
                }

                if (remittance.HeadOfficeCommision > 0)
                {
                    remittanceAccountingPostingCommand.AmountCollection.Add(new RemittanceAmountCollectionItem
                    {
                        Amount = remittance.HeadOfficeCommision,
                        EventAttributeName = OperationEventRubbriqueName.Transfer_Fee_Account.ToString(),
                        LevelOfExecution = "HEAD_OFFICE",
                        AmountType = "COMMISSION_AMOUNT",
                        Naration = $"Remittance Head Office Commision: [Transaction Reference: {reference}]. [Amount: {BaseUtilities.FormatCurrency(remittance.HeadOfficeCommision)}]"
                    });
                }

                // 🔍 Step 4: Add Receiving Branch Amount
                if (remittance.RecevingBranchTotalAmount > 0)
                {
                    remittanceAccountingPostingCommand.AmountCollection.Add(new RemittanceAmountCollectionItem
                    {
                        Amount = remittance.RecevingBranchTotalAmount,
                        EventAttributeName = OperationEventRubbriqueName.Principal_Saving_Account.ToString(),
                        LevelOfExecution = "HEAD_OFFICE",
                        AmountType = "MAIN_AMOUNT",
                        Naration = $"Remittance Receiving Branch amount with her commision: [Transaction Reference: {reference}]. [Amount: {BaseUtilities.FormatCurrency(remittance.RecevingBranchTotalAmount)}] [Receving Branch Commision: {BaseUtilities.FormatCurrency(remittance.RecivingBranchCommision)}] [Receiver Amount: {BaseUtilities.FormatCurrency(remittance.ReceiverAmount)}]"
                    });
                }

                // 🔍 Step 5: Execute Accounting Posting
                _logger.LogInformation($"[INFO] Sending accounting posting command. CorrelationId: '{correlationId}', Reference: '{reference}'.");

                var result = await _mediator.Send(remittanceAccountingPostingCommand);

                if (result.StatusCode != 200)
                {
                    string errorResponse = $"[ERROR] Accounting posting failed. Response: {result.Message}. CorrelationId: '{correlationId}', Reference: '{reference}'.";
                    _logger.LogError(errorResponse);
                    return false;
                }

                _logger.LogInformation($"[SUCCESS] Remittance accounting completed. CorrelationId: '{correlationId}', Reference: '{reference}'.");
                return true;
            }
            catch (Exception ex)
            {
                string errorMessage = $"[ERROR] Exception occurred during remittance accounting. CorrelationId: '{correlationId}', Reference: '{reference}'. Details: {ex.Message}.";
                _logger.LogError(errorMessage);
                return false;
            }
        }

    }
}
