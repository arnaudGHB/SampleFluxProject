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
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.Data.Dto.Receipts.Payments;
using CBS.TransactionManagement.Data.Dto.Receipts.Details;
using CBS.TransactionManagement.Repository.Receipts.Payments;
using CBS.TransactionManagement.MediatR.Accounting.Command;
using CBS.TransactionManagement.Repository.DailyStatisticBoard;
using CBS.TransactionManagement.Repository.RemittanceP;
using CBS.TransactionManagement.Data.Entity.RemittanceP;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashOut.Commands;
using CBS.TransactionManagement.MediatR.OTP.Commands;
using CBS.TransactionManagement.Repository.MongoDBManager.SerialNumberGenerator;

namespace CBS.TransactionManagement.MediatR.Operations.Cash.CashOut.Services.RemittanceP
{
    /// <summary>
    /// Handles the command to add a new Transaction.
    /// </summary>
    public class RemittanceCashoutServices : IRemittanceCashoutServices
    {
        private readonly IAccountRepository _AccountRepository;
        private readonly IWithdrawalServices _WithdrawalServices;
        private readonly IAccountingDayRepository _accountingDayRepository;
        IConfigRepository _configRepository;
        private readonly IPaymentReceiptRepository _paymentReceiptRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<RemittanceCashoutServices> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IDailyTellerRepository _tellerProvisioningAccountRepository;
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvioningHistoryRepository;
        private readonly ITellerRepository _tellerRepository;
        private readonly IRemittanceRepository _remittanceRepository;
        private readonly IDailyTransactionCodeGenerator _dailyTransactionCodeGenerator;
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
        public RemittanceCashoutServices(
            IAccountRepository AccountRepository,
            ICurrencyNotesRepository CurrencyNotesRepository,
            IConfigRepository ConfigRepository,
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<RemittanceCashoutServices> logger,
            IUnitOfWork<TransactionContext> uow,
            IDailyTellerRepository tellerProvisioningAccountRepository = null,
            ITellerRepository tellerRepository = null,
            IMediator mediator = null,
            ISubTellerProvisioningHistoryRepository subTellerProvioningHistoryRepository = null,
            IWithdrawalServices withdrawalServices = null,
            IAccountingDayRepository accountingDayRepository = null,
            IPaymentReceiptRepository paymentReceiptRepository = null,
            IGeneralDailyDashboardRepository generalDailyDashboardRepository = null,
            IRemittanceRepository remittanceRepository = null,
            IDailyTransactionCodeGenerator dailyTransactionCodeGenerator = null)
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
            _remittanceRepository=remittanceRepository;
            _dailyTransactionCodeGenerator=dailyTransactionCodeGenerator;
        }

        /// <summary>
        /// Handles the WithdrawalTransactionCommand to add a new Transaction.
        /// </summary>
        /// <param name="request">The WithdrawalTransactionCommand containing Transaction data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>

        public async Task<ServiceResponse<PaymentReceiptDto>> RemittanceCashout(AddBulkOperationWithdrawalCommand requests, DateTime accountingDate, Config config)
        {
            bool isInterBranchOperation = false; // Flag to indicate if the operation is inter-branch.
            string reference = string.Empty;
            try
            {
                string message = null;
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
                    message = $"The remittance with reference {remittance.TransactionReference} is currently in a pending state. " +
                              $"It must be validated and approved before further processing. Please contact the initiating branch for approval.";

                    await BaseUtilities.LogAndAuditAsync(message, requests, HttpStatusCodeEnum.Forbidden, LogAction.Remittance, LogLevelInfo.Warning);
                    _logger.LogInformation(message);
                    return ServiceResponse<PaymentReceiptDto>.Return403(message);
                }

                if (remittance.Status == Status.Rejected.ToString())
                {
                    message = $"The remittance with reference {remittance.TransactionReference} has been rejected and cannot be processed. " +
                              $"Please contact the remitting branch ({remittance.SourceBranchName}) for further assistance or clarification.";

                    await BaseUtilities.LogAndAuditAsync(message, requests, HttpStatusCodeEnum.Forbidden, LogAction.Remittance, LogLevelInfo.Warning);
                    _logger.LogInformation(message);
                    return ServiceResponse<PaymentReceiptDto>.Return403(message);
                }

                if (remittance.Status == Status.Withdrawn.ToString())
                {
                    message = $"The operation is not permitted. The remittance with the specified reference {remittance.TransactionReference} has already been withdrawn and processed " +
                              $"by the receiving branch: {remittance.ReceivingBranchName}. " +
                              $"Withdrawal Details: Date: {remittance.DateOfCashOut:dd/MM/yyyy hh:mm:ss}, Withdrawn By: {remittance.ReceiverName}, " +
                              $"Amount: {remittance.Amount:#,##0 XAF}, Charges: {remittance.Fee:#,##0 XAF}. " +
                              $"Teller: {remittance.ReceivingTellerName}. Please verify the transaction details and ensure no duplicate action is attempted.";

                    await BaseUtilities.LogAndAuditAsync(message, requests, HttpStatusCodeEnum.Forbidden, LogAction.Remittance, LogLevelInfo.Warning);
                    _logger.LogInformation(message);
                    return ServiceResponse<PaymentReceiptDto>.Return403(message);
                }


                if (remittance.TransferType == TransferType.Local.ToString() && remittance.Status != Status.Paid.ToString())
                {
                    message = $"The operation is not possible. The remittance with reference {remittance.TransactionReference} and Amount: {BaseUtilities.FormatCurrency(remittance.Amount)} " +
                              $"& Charge: {BaseUtilities.FormatCurrency(remittance.Fee)} has not yet been paid to the cash desk by the sender at branch {remittance.SourceBranchName}. " +
                              $"However, the receiver is attempting a withdrawal at the branch: {remittance.ReceivingBranchName}. " +
                              $"This transaction is being handled by the cashier {remittance.SourceTellerName}. Please verify the transaction details to ensure no unauthorized withdrawal is processed.";

                    await BaseUtilities.LogAndAuditAsync(message, requests, HttpStatusCodeEnum.Forbidden, LogAction.Remittance, LogLevelInfo.Warning);
                    _logger.LogInformation(message);
                    return ServiceResponse<PaymentReceiptDto>.Return403(message);
                }


                var remittanceAccount = await _AccountRepository.GetAccountByAccountNumber(remittance.AccountNumber);
                var product = remittanceAccount.Product;
                if (product.OTPControl)
                {
                    var oTPCommand = new VerifyTemporalOTPCommand { OtpCode = requests.OTP, UserId = remittance.TransactionReference };
                    var otpResult = await _mediator.Send(oTPCommand);
                    if (otpResult.StatusCode != 200)
                    {
                        message = $"OTP validation failed for the remittance with reference {remittance.TransactionReference}. " +
                                  $"The provided OTP {requests.OTP} is invalid or expired. Please ensure that the correct OTP is used. " +
                                  $"If the issue persists, request a new OTP and try again. " +
                                  $"Note: The remittance has not yet been withdrawn. Please confirm the transaction details to avoid further delays.";

                        await BaseUtilities.LogAndAuditAsync(message, requests, HttpStatusCodeEnum.Forbidden, LogAction.Remittance, LogLevelInfo.Warning);
                        _logger.LogInformation(message);
                        return ServiceResponse<PaymentReceiptDto>.Return403(message);
                    }
                    remittance.IsOTPVerified=true;
                }
                if (product.AutoVerifyRemittanceReceiver)
                {
                    string verification = _remittanceRepository.ValidateReceiverIdentity(remittance, requests.ReceiverName, requests.ReceiverAddress, requests.ReceiverPhoneNumber, requests.SenderSecretCode);
                    if (verification != null)
                    {
                        message = $"Receiver identity verification failed for the remittance with reference {remittance.TransactionReference}. " +
                                  $"Details: {verification}. " +
                                  $"Please ensure that the provided information is correct. If the issue persists, contact support for assistance. " +
                                  $"Note: The remittance has not yet been withdrawn. Confirm the transaction details and try again.";

                        await BaseUtilities.LogAndAuditAsync(message, requests, HttpStatusCodeEnum.Forbidden, LogAction.Remittance, LogLevelInfo.Warning);
                        _logger.LogInformation(message);
                        return ServiceResponse<PaymentReceiptDto>.Return403(message);
                    }
                    remittance.IsAutoVerifyReceiver=true;
                }

                if (product.AutoVerifyRemittanceSender)
                {

                    string verification = _remittanceRepository.ValidateSenderIdentity(remittance, requests.SenderName, requests.SenderPhoneNumber, requests.SenderSecretCode, requests.RemittanceAmount, requests.RemittanceDate.Value);
                    if (verification != null)
                    {
                        message = $"Sender identity verification failed for the remittance with reference {remittance.TransactionReference}. " +
                                  $"Details: {verification}. " +
                                  $"Please ensure that the provided information is correct. If the issue persists, contact support for assistance. " +
                                  $"Note: The remittance has not yet been withdrawn. Confirm the transaction details and try again.";

                        await BaseUtilities.LogAndAuditAsync(message, requests, HttpStatusCodeEnum.Forbidden, LogAction.Remittance, LogLevelInfo.Warning);
                        _logger.LogInformation(message);
                        return ServiceResponse<PaymentReceiptDto>.Return403(message);
                    }
                    remittance.IsAutoVerifySender=true;
                }


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
                var branch = await GetBranch(teller.BranchId); // Retrieve branch details.
                // Calculate total amount and charges
                decimal amount = CalculateTotalAmount(requests.BulkOperations); // Calculate total amount.
                decimal customer_charges = remittance.Fee; // Calculate total charges.

                if (teller.BranchId != customer.BranchId)
                {
                    isInterBranchOperation = true;
                }                                                                         // Generate transaction reference based on branch type
                await _dailyTransactionCodeGenerator.ReserveTransactionCode(_userInfoToken.BranchCode, OperationPrefix.Cash_W_Remittance, TransactionType.WITHDRAWAL, isInterBranch: isInterBranchOperation); // Generate 

                // Retrieve currency notes
                var currencyNote = await RetrieveCurrencyNotes(reference, requests.BulkOperations.FirstOrDefault().currencyNotes); // Retrieve currency notes.

                var subTellerProvioning = _subTellerProvioningHistoryRepository.CashOutByDenomination(amount, requests.BulkOperations.FirstOrDefault().currencyNotes, teller.Id, accountingDate, customer_charges, requests.BulkOperations.FirstOrDefault().IsChargesInclussive, tellerAccount.OpenningOfDayReference);

                // Map the list of BulkOperation objects to a list of corresponding BulkDeposit objects
                var bulkWithdrawals = CurrencyNotesMapper.MapBulkOperationListToBulkDepositList(requests.BulkOperations);
                customer.BranchCode = branch.branchCode;
                var account = await _AccountRepository.GetAccountByAccountNumber(remittance.AccountNumber);

                var notes = requests.BulkOperations.FirstOrDefault().currencyNotes;
                var transactions = await ProcessBulkTransactions(bulkWithdrawals, teller, tellerAccount, isInterBranchOperation, customer, currencyNote.Data.ToList(), branch, reference, config, accountingDate, true, remittance); // Process bulk transactions.

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
                    ServiceType = TransactionType.CASH_W_Remittance.ToString(),
                    SourceOfRequest = OperationSourceType.Physical_Teller.ToString(),
                    TotalAmount = amount,
                    TotalCharges = customer_charges,
                    Transactions = transactions
                };
                // Step 21: Process payment and get receipt.
                var paymentReceipt = _paymentReceiptRepository.ProcessPaymentAsync(paymentProcessing);
                var TellerBranch = await GetBranch(teller.BranchId);
                var cashOperation = new CashOperation(teller.BranchId, paymentProcessing.Amount, paymentProcessing.TotalCharges, TellerBranch.name, TellerBranch.branchCode, CashOperationType.CashOut, LogAction.Remittance, subTellerProvioning);
                await _generalDailyDashboardRepository.UpdateOrCreateDashboardAsync(cashOperation);


                remittance.ReceivingBranchCode=branch.branchCode;
                remittance.ReceivingBranchName=branch.name;
                remittance.ReceivingBranchId=branch.id;
                remittance.ReceivingTellerId=teller.Id;
                remittance.ReceivingTellerName=teller.Name;

                remittance.CapturedOTP=requests.OTP;
                remittance.CapturedReceiverAddress=requests.ReceiverAddress;
                remittance.CapturedReceiverCNI=requests.ReceiverCNI;
                remittance.CapturedReceiverCNIDateOfExpiration=requests.ReceiverCNIDateOfExpiration;
                remittance.CapturedReceiverCNIDateOfIssue=requests.ReceiverCNIDateOfIssue;
                remittance.CapturedReceiverCNIPlcaceOfIssue=requests.ReceiverCNIPlcaceOfIssue;
                remittance.CapturedReceiverName=requests.ReceiverName;
                remittance.CapturedReceiverPhoneNumber=requests.ReceiverPhoneNumber;
                remittance.CapturedRemittanceAmount=requests.RemittanceAmount;
                remittance.CapturedSenderSecretCode=requests.SenderSecretCode;
                remittance.CapturedSenderPhoneNumber=requests.SenderPhoneNumber;
                remittance.CapturedSenderName=requests.SenderName;

                remittance.ReceiverCNI=requests.ReceiverCNI;
                remittance.ReceiverCNIIssueDate=requests.ReceiverCNIDateOfIssue;
                remittance.ReceiverCNIExpiryDate=requests.ReceiverCNIDateOfExpiration;
                remittance.ReceiverCNIPlaceOfIssue=requests.ReceiverCNIPlcaceOfIssue;

                remittance.Status=Status.Withdrawn.ToString();
                remittance.DateOfCashOut=BaseUtilities.UtcNowToDoualaTime();
                _remittanceRepository.Update(remittance);


                // Step 22: Save changes to the database.
                await _uow.SaveAsync();
                await _dailyTransactionCodeGenerator.MarkTransactionAsSuccessful(reference);
                // Step 23: Calculate total charges and prepare transaction details.
                customer_charges = transactions.Sum(x => x.Fee);
                // Step 24: Send SMS notification to customer.
                await SendSMS(remittance, branch);
                // Step 25: Post accounting entries for transactions.
                var accountingResult=await PostAccounting(account, branch, isInterBranchOperation, accountingDate, remittance, reference);
                var accountingResponseMessages = string.Empty;
                // Step 26: Prepare and return the response.
                string accountDetails = string.Join("\n", accountDeposits.Select((ad, index) =>
                    $"{index + 1}. {ad.AccountName}: {BaseUtilities.FormatCurrency(ad.Amount)}, Fee: {BaseUtilities.FormatCurrency(ad.Charge)}"));
                var paymentReceiptDto = _mapper.Map<PaymentReceiptDto>(paymentReceipt);
                if (accountingResult)
                {
                    accountingResponseMessages = $"Account Posting: Successful. A remittance cashout of {BaseUtilities.FormatCurrency(amount)} " +
                                                 $"with total charges of {BaseUtilities.FormatCurrency(paymentProcessing.TotalCharges)} " +
                                                 $"was processed by {teller.Name} [User: {dailyTeller.UserName}] for Member: {customer.Name}. " +
                                                 $"The cashout was deducted from the following accounts:\n{accountDetails}";
                }
                else
                {
                    accountingResponseMessages = $"Account Posting: Failed. A remittance cashout of {BaseUtilities.FormatCurrency(amount)} " +
                                                 $"with total charges of {BaseUtilities.FormatCurrency(paymentProcessing.TotalCharges)} " +
                                                 $"was processed by {teller.Name} [User: {dailyTeller.UserName}] for Member: {customer.Name}. " +
                                                 $"The cashout was attempted from the following accounts:\n{accountDetails}";
                }

                await BaseUtilities.LogAndAuditAsync(accountingResponseMessages, requests, HttpStatusCodeEnum.OK, LogAction.Remittance, LogLevelInfo.Information);

                _logger.LogInformation(accountingResponseMessages);

                return ServiceResponse<PaymentReceiptDto>.ReturnResultWith200(paymentReceiptDto, accountingResponseMessages);

            }
            catch (Exception e)
            {
                await _dailyTransactionCodeGenerator.RevertTransactionCode(reference);
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while commiting withdrawal operation: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, requests, HttpStatusCodeEnum.InternalServerError, LogAction.Remittance, LogLevelInfo.Error);
                return ServiceResponse<PaymentReceiptDto>.Return500(errorMessage);
            }
        }
        // Method to send SMS notification
        private async Task SendSMS(Remittance remittance, BranchDto branch)
        {
            string branchName = branch?.name ?? "";
            string remittanceType = remittance.RemittanceType ?? "General Remittance";
            string msg;

            if (remittance.ReceiverLanguage?.ToLower() == "english")
            {
                msg = $"Dear {remittance.ReceiverName}, you received {BaseUtilities.FormatCurrency(remittance.Amount)} " +
                      $"from {remittance.SenderName} via {remittanceType}. Reference: {remittance.TransactionReference}. " +
                      $"Fee: {BaseUtilities.FormatCurrency(remittance.Fee)}. Branch: {branchName}. Date: {BaseUtilities.UtcToDoualaTime(DateTime.UtcNow)}.";
            }
            else
            {
                msg = $"Cher(e) {remittance.ReceiverName}, vous avez reçu {BaseUtilities.FormatCurrency(remittance.Amount)} " +
                      $"de {remittance.SenderName} via {remittanceType}. Reference: {remittance.TransactionReference}. " +
                      $"Frais: {BaseUtilities.FormatCurrency(remittance.Fee)}. Agence: {branchName}. " +
                      $"Date: {BaseUtilities.UtcToDoualaTime(DateTime.UtcNow)}.";
            }

            var sMSPICallCommand = new SendSMSPICallCommand
            {
                messageBody = msg,
                recipient = remittance.ReceiverPhoneNumber
            };
            await _mediator.Send(sMSPICallCommand);
        }

        /// <summary>
        /// Posts accounting entries for a remittance withdrawal transaction.
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
                _logger.LogInformation($"[INFO] Initiating remittance withdrawal accounting. CorrelationId: '{correlationId}', Reference: '{reference}', Account: '{account.AccountNumber}', Amount: '{BaseUtilities.FormatCurrency(remittance.TotalAmount)}'.");

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

                if (remittance.Status != "Withdrawal")
                {
                    _logger.LogInformation($"[INFO] Skipping accounting posting. Transaction is not a withdrawal. CorrelationId: '{correlationId}', Reference: '{reference}'.");
                    return true;
                }

                // 🔍 Step 1: Create Remittance Accounting Posting Command
                var remittanceAccountingPostingCommand = new RemittanceAccountingPostingCommand
                {
                    TransactionReferenceId = reference,
                    MemberReference = account.CustomerId,
                    ProductId = account.ProductId,
                    OperationType = TransactionType.WITHDRAWAL.ToString(),
                    Source = "Physical_Teller",
                    TransactionDate = accountingDate
                };

                // 🔍 Step 2: Deducting Amount from HEAD OFFICE
                remittanceAccountingPostingCommand.AmountCollection.Add(new RemittanceAmountCollectionItem
                {
                    Amount = remittance.TotalAmount, // Deduct the total amount
                    EventAttributeName = OperationEventRubbriqueName.Principal_Saving_Account.ToString(),
                    LevelOfExecution = "HEAD_OFFICE",
                    AmountType = "MAIN_AMOUNT",
                    Naration = $"Remittance Withdrawal: Head Office Deduction. [Transaction Reference: {reference}]. [Total Amount: {BaseUtilities.FormatCurrency(remittance.TotalAmount)}]"
                });

                // 🔍 Step 3: Paying Out at BRANCH OFFICE
                remittanceAccountingPostingCommand.AmountCollection.Add(new RemittanceAmountCollectionItem
                {
                    Amount = remittance.ReceiverAmount, // Amount paid to the receiver
                    EventAttributeName = OperationEventRubbriqueName.Principal_Saving_Account.ToString(),
                    LevelOfExecution = "BRANCH_OFFICE",
                    AmountType = "MAIN_AMOUNT",
                    Naration = $"Remittance Withdrawal: Receiver Payout at Branch. [Transaction Reference: {reference}]. [Receiver Amount: {BaseUtilities.FormatCurrency(remittance.ReceiverAmount)}]"
                });

                // 🔍 Step 4: Deducting Branch Commission (Paying Branch)
                if (remittance.RecivingBranchCommision > 0)
                {
                    remittanceAccountingPostingCommand.AmountCollection.Add(new RemittanceAmountCollectionItem
                    {
                        Amount = remittance.RecivingBranchCommision,
                        EventAttributeName = OperationEventRubbriqueName.Transfer_Fee_Account.ToString(),
                        LevelOfExecution = "BRANCH_OFFICE",
                        AmountType = "COMMISSION_AMOUNT",
                        Naration = $"Remittance Withdrawal: Branch Commission Deduction. [Transaction Reference: {reference}]. [Amount: {BaseUtilities.FormatCurrency(remittance.RecivingBranchCommision)}]"
                    });
                }

                // 🔍 Step 5: Execute Accounting Posting
                _logger.LogInformation($"[INFO] Sending withdrawal accounting posting command. CorrelationId: '{correlationId}', Reference: '{reference}'.");

                var result = await _mediator.Send(remittanceAccountingPostingCommand);

                if (result.StatusCode != 200)
                {
                    string errorResponse = $"[ERROR] Withdrawal accounting posting failed. Response: {result.Message}. CorrelationId: '{correlationId}', Reference: '{reference}'.";
                    _logger.LogError(errorResponse);
                    return false;
                }

                _logger.LogInformation($"[SUCCESS] Remittance withdrawal accounting completed. CorrelationId: '{correlationId}', Reference: '{reference}'.");
                return true;
            }
            catch (Exception ex)
            {
                string errorMessage = $"[ERROR] Exception occurred during remittance withdrawal accounting. CorrelationId: '{correlationId}', Reference: '{reference}'. Details: {ex.Message}.";
                _logger.LogError(errorMessage);
                return false;
            }
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
        private async Task<List<TransactionDto>> ProcessBulkTransactions(List<BulkDeposit> requests, Teller teller, Account tellerAccount, bool isInterBranchOperation, CustomerDto customer, List<CurrencyNotesDto> currencyNotes, BranchDto branch, string reference, Config config, DateTime accountingDate, bool isRemittance, Remittance remittance)
        {
            var transactions = new List<TransactionDto>(); // Initialize list to store transactions.

            foreach (var request in requests)
            {
                var customerAccount = await _AccountRepository.GetAccount(request.AccountNumber, TransactionType.WITHDRAWAL.ToString()); // Get customer account information.
                var customCharge = isRemittance ? remittance.Fee : request.Fee; // Get custom charge for the transaction.
                // Set transaction properties
                request.Customer = customer;
                request.currencyNotes = currencyNotes;
                request.Branch = branch;
                request.IsExternalOperation = false;
                request.ExternalApplicationName = remittance.RemittanceType;
                request.ExternalReference = remittance.TransactionReference;
                request.SourceType =isRemittance ? remittance.TransferSource : "Local";

                // Deposit amount into account
                var transaction = await _WithdrawalServices.Withdrawal(teller, tellerAccount, request, isInterBranchOperation, _userInfoToken.BranchID, customer.BranchId, customCharge, reference, customerAccount, config, false, accountingDate, isRemittance, remittance);

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
