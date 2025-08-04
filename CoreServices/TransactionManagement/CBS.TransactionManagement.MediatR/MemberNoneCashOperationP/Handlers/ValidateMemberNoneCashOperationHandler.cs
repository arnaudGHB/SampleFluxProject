using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Repository.MemberNoneCashOperationP;
using CBS.TransactionManagement.MediatR.MemberNoneCashOperationP.Commands;
using CBS.TransactionManagement.Data.Entity.MemberNoneCashOperationP;

namespace CBS.TransactionManagement.MediatR.MemberNoneCashOperationP.Handlers
{

    /// <summary>
    /// Handles the command to validate a Member None Cash Operation request.
    /// </summary>
    public class ValidateMemberNoneCashOperationHandler : IRequestHandler<ValidateMemberNoneCashOperationCommand, ServiceResponse<bool>>
    {
        private readonly IMemberNoneCashOperationRepository _noneCashOperationRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<ValidateMemberNoneCashOperationHandler> _logger;
        private readonly UserInfoToken _userInfoToken;
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly ITellerRepository _tellerRepository; // Repository for accessing teller data.
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly IAccountingDayRepository _accountingDayRepository;
        private readonly ITransactionRepository _transactionRepository; // Repository for accessing Transaction data.
        public IMediator _mediator { get; set; }

        public ValidateMemberNoneCashOperationHandler(
            IMemberNoneCashOperationRepository noneCashOperationRepository,
            IAccountRepository accountRepository,
            ILogger<ValidateMemberNoneCashOperationHandler> logger,
            UserInfoToken userInfoToken,
            IUnitOfWork<TransactionContext> uow,
            ITellerRepository tellerRepository,
            ITellerOperationRepository tellerOperationRepository,
            IAccountingDayRepository accountingDayRepository,
            ITransactionRepository transactionRepository,
            IMediator mediator)
        {
            _noneCashOperationRepository = noneCashOperationRepository;
            _accountRepository = accountRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _uow = uow;
            _tellerRepository=tellerRepository;
            _tellerOperationRepository=tellerOperationRepository;
            _accountingDayRepository=accountingDayRepository;
            _transactionRepository=transactionRepository;
            _mediator=mediator;
        }

        /// <summary>
        /// Validates a member's None-Cash operation (Deposit/Withdrawal) by checking its status, 
        /// verifying user permissions, performing the respective operation, and updating its status 
        /// as either Approved or Rejected.
        /// </summary>
        /// <param name="request">The command containing the operation ID and validation details.</param>
        /// <param name="cancellationToken">A token to handle request cancellation.</param>
        /// <returns>
        /// A ServiceResponse<bool> indicating the success or failure of the validation along with a message.
        /// </returns>
        public async Task<ServiceResponse<bool>> Handle(ValidateMemberNoneCashOperationCommand request, CancellationToken cancellationToken)
        {
            // Generate a unique reference for logging and tracking purposes.
            string logReference = BaseUtilities.GenerateUniqueNumber();
            string errorMessage = string.Empty;

            try
            {
                // Step 1: Validate the provided operation ID.
                if (string.IsNullOrWhiteSpace(request.OperationId))
                {
                    errorMessage = "Operation ID cannot be null or empty.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.NoneCashOperationValidation, LogLevelInfo.Warning, logReference);
                    return ServiceResponse<bool>.Return400(errorMessage);
                }

                // Step 2: Retrieve the operation by ID and ensure it is not deleted.
                var memberNoneCashOperation = await _noneCashOperationRepository.FindAsync(request.OperationId);
                if (memberNoneCashOperation == null || memberNoneCashOperation.IsDeleted)
                {
                    errorMessage = $"Member None Cash Operation with ID {request.OperationId} not found.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.NoneCashOperationValidation, LogLevelInfo.Warning, logReference);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                // Step 3: Validate that the current user is not the initiator of the operation.
                if (memberNoneCashOperation.InitiatedByUSerId == _userInfoToken.Id)
                {
                    errorMessage = "The user who initiated the operation cannot validate it.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.NoneCashOperationValidation, LogLevelInfo.Warning, logReference);
                    return ServiceResponse<bool>.Return403(errorMessage);
                }

                // Step 4: Ensure the operation is in a pending state before validation.
                if (memberNoneCashOperation.ApprovalStatus != Status.Pending.ToString())
                {
                    errorMessage = "Only pending operations can be validated.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.NoneCashOperationValidation, LogLevelInfo.Warning, logReference);
                    return ServiceResponse<bool>.Return403(errorMessage);
                }

                // Determine if the operation is a Deposit or Withdrawal.
                string operationType = memberNoneCashOperation.BookingDirection == OperationType.Debit.ToString()
                    ? OperationType.Withdrawal.ToString()
                    : OperationType.Deposit.ToString();

                // Step 5: Retrieve the None-Cash Teller information and validate teller rights.
                var teller = await _tellerRepository.GetTellerByType(TellerType.NoneCashTeller.ToString());
                await _tellerRepository.CheckTellerOperationalRights(teller, operationType, false);

                // Step 6: Retrieve the necessary accounts (teller and member accounts).
                var tellerAccount = await _accountRepository.RetrieveTellerAccount(teller);
                var memberAccount = await _accountRepository.GetAccountByAccountNumber(memberNoneCashOperation.AccountNUmber);
                var accountingDay = _accountingDayRepository.GetCurrentAccountingDay(memberNoneCashOperation.BranchId);
                string transactionType = TransactionType.NONE_CASH_WITHDRAWAL.ToString();
                // Step 7: Perform the operation based on its type (Deposit or Withdrawal).
                if (operationType == OperationType.Deposit.ToString())
                {
                    // Credit the member's account and record the operation as a deposit.
                    _accountRepository.CreditAccount(memberAccount, memberNoneCashOperation.Amount);
                    CreateTellerOperation(memberNoneCashOperation.Amount, OperationType.Credit, teller, tellerAccount,
                        memberNoneCashOperation.TransactionReference, TransactionType.NONE_CASH_CASH_IN.ToString(),
                        OperationType.Deposit.ToString(), accountingDay, memberNoneCashOperation.MemberName,
                        memberNoneCashOperation.AccountNUmber, memberNoneCashOperation.MemberReference, false);
                    transactionType = TransactionType.NONE_CASH_CASH_IN.ToString();
                    var transaction = CreateTransaction(memberNoneCashOperation, memberAccount, teller, operationType,
                        TransactionType.NONE_CASH_CASH_IN, accountingDay);
                    _transactionRepository.Add(transaction);
                }
                else
                {
                    // Debit the member's account and record the operation as a withdrawal.
                    _accountRepository.DebitAccount(memberAccount, memberNoneCashOperation.Amount);
                    CreateTellerOperation(memberNoneCashOperation.Amount, OperationType.Debit, teller, tellerAccount,
                        memberNoneCashOperation.TransactionReference, TransactionType.NONE_CASH_WITHDRAWAL.ToString(),
                        OperationType.Withdrawal.ToString(), accountingDay, memberNoneCashOperation.MemberName,
                        memberNoneCashOperation.AccountNUmber, memberNoneCashOperation.MemberReference, false);

                    var transaction = CreateTransaction(memberNoneCashOperation, memberAccount, teller, operationType,
                        TransactionType.NONE_CASH_WITHDRAWAL, accountingDay);
                    _transactionRepository.Add(transaction);
                }

                // Step 8: Update the operation's status and audit details based on the validation result.
                memberNoneCashOperation.ApprovalStatus = request.IsApproved ? Status.Approved.ToString() : Status.Rejected.ToString();
                memberNoneCashOperation.ApprovedByUSerId = _userInfoToken.Id;
                memberNoneCashOperation.ApprovedBy = _userInfoToken.FullName;
                memberNoneCashOperation.ApprovalDate = BaseUtilities.UtcNowToDoualaTime();
                memberNoneCashOperation.ApprovalComment = request.ValidationComment;

                // Step 9: Save the changes to the database.
                _noneCashOperationRepository.Update(memberNoneCashOperation);
                await _uow.SaveAsync();

                // Step 10: If the operation is approved, initiate accounting posting.
                if (request.IsApproved)
                {
                    await MakeAccountingPosting(memberNoneCashOperation, accountingDay, memberAccount, transactionType);
                }

                // Step 11: Log the success and return a positive response.
                string successMessage = request.IsApproved
                    ? $"Operation with ID {memberNoneCashOperation.Id} approved successfully."
                    : $"Operation with ID {memberNoneCashOperation.Id} rejected successfully.";

                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.NoneCashOperationValidation, LogLevelInfo.Information, logReference);
                _logger.LogInformation(successMessage);
                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception ex)
            {
                // Step 12: Handle unexpected exceptions by logging and auditing the error.
                errorMessage = $"An error occurred while validating the operation: {ex.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.NoneCashOperationValidation, LogLevelInfo.Error, logReference);
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }

        private void CreateTellerOperation(decimal amount, OperationType operationType, Teller teller, Account tellerAccount,
    string TransactionReference, string TransactionType,
    string eventType, DateTime accountingDate, string memberName, string membersAccountnumber, string memberReference, bool isCashOperation)
        {
            var tellerOperation = new TellerOperation
            {
                Id = BaseUtilities.GenerateUniqueNumber(),
                AccountID = tellerAccount.Id,
                AccountNumber = tellerAccount.AccountNumber,
                Amount = amount,
                OperationType = operationType.ToString(),
                BranchId = tellerAccount.BranchId,
                BankId = tellerAccount.BankId,
                OpenOfDayReference = "n/a",
                CurrentBalance = tellerAccount.Balance,
                Date = accountingDate,
                AccountingDate = accountingDate,
                PreviousBalance = tellerAccount.PreviousBalance,
                TellerID = teller.Id,
                TransactionReference = TransactionReference,
                TransactionType = TransactionType,
                UserID = _userInfoToken.FullName,
                Description = $"{TransactionType} of {BaseUtilities.FormatCurrency(amount)}, Account Number: {tellerAccount.AccountNumber}",
                ReferenceId = ReferenceNumberGenerator.GenerateReferenceNumber(_userInfoToken.BranchCode),
                CustomerId = memberReference,
                MemberAccountNumber = membersAccountnumber,
                EventName = eventType,
                MemberName = memberName,
                DestinationBrachId = teller.BranchId,
                SourceBranchId = teller.BranchId,
                IsInterBranch = false,
                DestinationBranchCommission = 0,
                SourceBranchCommission = 0,
                IsCashOperation = isCashOperation,

            };
            _tellerOperationRepository.Add(tellerOperation);
        }

        private Transaction CreateTransaction(MemberNoneCashOperation noneCashOperation, Account account, Teller teller, string operationType, TransactionType transactionType, DateTime accountingDate)
        {
            try
            {
                string n_a = "N/A";
                var transactionEntityEntryFee = new Transaction
                {
                    Id = BaseUtilities.GenerateUniqueNumber(),
                    CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow),
                    Status = TransactionStatus.COMPLETED.ToString(),
                    TransactionReference = noneCashOperation.TransactionReference,
                    TransactionType = operationType,
                    Operation = transactionType.ToString(),
                    AccountNumber = account.AccountNumber,
                    PreviousBalance = account.Balance,
                    AccountId = account.Id,
                    ProductId = account.ProductId,
                    SenderAccountId = account.Id,
                    ReceiverAccountId = account.Id,
                    BankId = teller.BankId,
                    CheckName = n_a,
                    CheckNumber = n_a,
                    IsSWS = false,
                    BranchId = teller.BranchId,
                    OriginalDepositAmount = noneCashOperation.Amount,// request.Amount,
                    Fee = 0,
                    AccountingDate = accountingDate,
                    CustomerId = account.CustomerId,
                    DepositerNote = n_a,
                    DepositerTelephone = n_a,
                    DepositorIDExpiryDate = n_a,
                    DepositorIDNumber = n_a,
                    DepositorName = n_a,
                    DepositorIDIssueDate = n_a,
                    DepositorIDNumberPlaceOfIssue = n_a,
                    Tax = 0,
                    ExternalReference = noneCashOperation.TransactionReference,
                    CloseOfAccountCharge = 0,
                    OperationCharge = 0,
                    WithdrawalChargeWithoutNotification =0,
                    WithrawalFormCharge = 0,
                    Debit = noneCashOperation.BookingDirection==OperationType.Debit.ToString() ? noneCashOperation.Amount : 0,
                    Credit = noneCashOperation.BookingDirection==OperationType.Credit.ToString() ? noneCashOperation.Amount : 0,
                    Amount = -(noneCashOperation.Amount),
                    Note = $"{noneCashOperation.Note}",
                    OperationType = noneCashOperation.BookingDirection,
                    FeeType = Events.WithDrawalCharges_Fee.ToString(),
                    TellerId = teller.Id,
                    Balance = account.Balance,
                    IsInterBrachOperation = false,
                    DestinationBrachId = account.BranchId,
                    SourceBrachId = account.BranchId,
                    DestinationBranchCommission = 0,
                    SourceBranchCommission = 0,
                    IsExternalOperation = false, // Set external operation flag
                    ExternalApplicationName = n_a, // Set external application name
                    SourceType = OperationSourceType.BackOffice_Operation.ToString(), // Set source type
                    Currency = "XAF", // Set currency
                    ReceiptTitle = $"NONE CASH OPERATION ({transactionType.ToString()}) - Reference: {noneCashOperation.TransactionReference}",

                };

                return transactionEntityEntryFee; // Return the transaction entity

            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// Initiates an accounting posting for a None-Cash operation by sending a command to the mediator.
        /// The operation could be either a deposit or withdrawal based on the booking direction.
        /// </summary>
        /// <param name="noneCashOperation">The None-Cash operation details.</param>
        /// <param name="accountingDay">The current accounting day for the transaction.</param>
        /// <param name="MemberAccount">The member's account involved in the transaction.</param>
        /// <returns>
        /// A boolean indicating the success or failure of the accounting posting.
        /// Returns true if the accounting posting is successful, otherwise false.
        /// </returns>
        private async Task<bool> MakeAccountingPosting(MemberNoneCashOperation noneCashOperation, DateTime accountingDay, Account MemberAccount, string transactionType)
        {
            // Step 1: Format the narration string.
            string formattedNarration = $"{transactionType.ToString()} Transaction | " +
                                        $"Account Name: {MemberAccount.AccountName} | " +
                                        $"Account Reference: {MemberAccount.CustomerId} | " +
                                        $"Amount: {BaseUtilities.FormatCurrency(noneCashOperation.Amount)} | " +
                                        $"Reference: {noneCashOperation.TransactionReference} | " +
                                        $"Date: {accountingDay:dd-MM-yyyy}";

            // Step 2: Create and populate the MakeNonCashAccountAdjustmentCommand.
            var makeNonCashAccountAdjustment = new MakeNonCashAccountAdjustmentCommand
            {
                TransactionDate = accountingDay,
                ChartOfAccountId=  noneCashOperation.ChartOfAccountId,                       // Set the transaction dateBookingDirection
                ProductId = MemberAccount.ProductId,                      // Associate the transaction with the member's product.
                BookingDirection = noneCashOperation.BookingDirection,       // Specify if it's a deposit or withdrawal.
                Amount = noneCashOperation.Amount,                        // Set the transaction amount.
                Narration = formattedNarration,                           // Set the formatted narration.
                TransactionReference = noneCashOperation.TransactionReference,  // Set the reference for tracking.
                MemberReference = noneCashOperation.MemberReference,            // Associate the transaction with the member.
                Source = noneCashOperation.Source                               // Specify the source of the transaction.
            };

            // Step 3: Send the command to the mediator for processing.
            var result = await _mediator.Send(makeNonCashAccountAdjustment);

            // Step 4: Check if the mediator response indicates a failure.
            if (result.StatusCode != 200)
            {
                string warningMessage = $"Accounting posting failed for Transaction Reference: {noneCashOperation.TransactionReference}. " +
                                        $"Status Code: {result.StatusCode}, Message: {result.Message}";
                _logger.LogWarning(warningMessage);
                return false;  // Return false if the operation failed.
            }

            // Step 5: Return true if the accounting posting was successful.
            return true;
        }

    }

}
