using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Data.Entity.SavingProductFeeP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Helper.Helper;
using CBS.TransactionManagement.Repository.ChargesWaivedP;
using CBS.TransactionManagement.Repository.WithdrawalNotificationP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.Repository.AccountServices
{
    public class TTPTransferServices : ITTPTransferServices
    {
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IChargesWaivedRepository _chargesWaivedRepository;
        private readonly ISavingProductFeeRepository _savingProductFeeRepository;
        private readonly IWithdrawalNotificationRepository _withdrawalNotificationRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IUnitOfWork<TransactionContext> _uow;
        public IMediator _mediator { get; set; }
        private readonly ITransactionRepository _TransactionRepository; // Repository for accessing Transaction data.
        private readonly ILogger<AccountRepository> _logger; // Logger for logging handler actions and errors.

        public TTPTransferServices(ITellerOperationRepository tellerOperationRepository = null, IMediator mediator = null, ITransactionRepository transactionRepository = null, ILogger<AccountRepository> logger = null, IAccountRepository accountRepository = null, IUnitOfWork<TransactionContext> uow = null, UserInfoToken userInfoToken = null, IChargesWaivedRepository chargesWaivedRepository = null, IWithdrawalNotificationRepository withdrawalNotificationRepository = null, ISavingProductFeeRepository savingProductFeeRepository = null)
        {
            _tellerOperationRepository = tellerOperationRepository;
            _mediator = mediator;
            _TransactionRepository = transactionRepository;
            _logger = logger;
            _accountRepository = accountRepository;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _chargesWaivedRepository = chargesWaivedRepository;
            _withdrawalNotificationRepository = withdrawalNotificationRepository;
            _savingProductFeeRepository = savingProductFeeRepository;
        }

        /// <summary>
        /// Transfers funds from the sender account to the receiver account.
        /// </summary>
        /// <param name="teller">The teller initiating the transfer.</param>
        /// <param name="tellerAccount">The account associated with the teller.</param>
        /// <param name="request">The transfer request details.</param>
        /// <param name="Reference">The reference for the transfer.</param>
        /// <param name="senderAccount">The account from which funds will be transferred.</param>
        /// <param name="receiverAccount">The account to which funds will be transferred.</param>
        /// <param name="senderCustomer">The details of the customer initiating the transfer.</param>
        /// <param name="receiverCustomer">The details of the customer receiving the transfer.</param>
        /// <param name="config">The configuration settings.</param>
        /// <returns>A TransactionDto object representing the completed transfer.</returns>
        public async Task<TransactionDto> TransferTTP(Teller teller, Account tellerAccount, TransferTTP request, string Reference, Account senderAccount, Account receiverAccount, CustomerDto senderCustomer, CustomerDto receiverCustomer, string sourceType, string TPPtransferType, FeeOperationType feeOperationType)
        {
            try
            {
                // 1. Initialize variables
                bool isInterBranchOperation = false;           // Sets default operation type as non-inter-branch.

                // 2. Check if sender and receiver accounts are the same
                if (senderAccount.AccountNumber == receiverAccount.AccountNumber)
                {
                    // 3. Define and throw an error message if sender and receiver account numbers match
                    string errorMessage = $"Transfer cannot be done to the same account number. Sender: {senderAccount.AccountNumber}, Receiver: {receiverAccount.AccountNumber}";
                    throw new InvalidOperationException(errorMessage);  // Throws an InvalidOperationException to prevent same-account transfer.
                }
                // 4. Check if the transfer is an inter-branch operation
                var transferType = _accountRepository.DetermineTransferType(senderAccount, receiverAccount);

                // 5. Set operation to inter-branch and update transfer type if sender and receiver are in different branches
                isInterBranchOperation= transferType.IsInterBranch;

                // 6. Retrieve transfer parameters for the sender account
                var transferParameter = senderAccount.Product.TransferParameters.FirstOrDefault(x => x.TransferType == transferType.TransferType);

                // 7. Check if transfer parameters for the given type are not configured
                if (transferParameter == null)
                {
                    // 8. Define and throw an error if no transfer parameters are found for the sender account’s product type
                    var errorMessage = $"Transfer parameters are not configured for {senderAccount.Product.Name}. Contact your administrator to configure these parameters on {senderAccount.Product.Name}.";
                    throw new InvalidOperationException(errorMessage);  // Throws an exception to indicate missing configuration.
                }
               
                var transferCharges = new TransferCharges();
                if (senderAccount.AccountType==AccountType.Saving.ToString())
                {
                    bool hasWithdrawalNotification = false;

                    // Check for existing withdrawal notifications.
                    var withdrawalNotification = await _withdrawalNotificationRepository.GetWithdrawalNotification(
                        senderAccount.CustomerId, senderAccount.AccountNumber, request.Amount);
                    if (withdrawalNotification != null)
                    {
                        hasWithdrawalNotification = true;
                    }

                    // Fetch withdrawal form charges specific to the product.
                    var withdrawalFormCharge = senderAccount.Product.WithdrawalFormSavingFormFeeFor3PP;
                    // 9. Calculate transfer charges
                     transferCharges = await _accountRepository.CalculateTransferCharges(request.Amount, senderAccount.ProductId, feeOperationType, senderAccount.BranchId, transferType.TransferType, true, hasWithdrawalNotification, withdrawalFormCharge);

                }
                else
                {
                    // 9. Calculate transfer charges
                    transferCharges = await _accountRepository.CalculateTransferCharges(request.Amount, senderAccount.ProductId, feeOperationType, senderAccount.BranchId, transferType.TransferType, false);

                }

                // 12. Calculate total amount to transfer with charges
                decimal totalAmountToTransfer = TotalAmountToTransferWithCharges(transferCharges.TotalCharges, request.Amount);
                // 10. Check if sender has enough funds for the transfer
                _accountRepository.CheckBalanceForTransferCharges(request.Amount,senderAccount, transferParameter, totalAmountToTransfer, senderCustomer.LegalForm);
               
               
                // Computes the total amount to be debited, including the transfer amount and applicable charges.

                // 13. Create and process sender transaction
                var senderTransactionEntity = await CreateSenderTransactionEntity(request, senderAccount, transferParameter, receiverAccount, transferCharges, "XAF", teller.Id, isInterBranchOperation, senderCustomer.BranchId, receiverCustomer.BranchId, totalAmountToTransfer, Reference, TPPtransferType);
                // Creates a sender transaction entity recording details of the transfer, including sender and receiver info, charges, and branches.

                // 14. Update sender account properties
                UpdateSenderAccountProperties(senderAccount, totalAmountToTransfer);
                // Adjusts sender account properties to reflect the new balance after the transfer.

                // 15. Create and process receiver transaction
                var receiverTransactionEntity = await CreateReceiverTransactionEntity(request, receiverAccount, transferParameter, senderAccount, senderTransactionEntity);
                // Generates a receiver transaction entity to record receipt of the transfer.

                // 16. Update receiver account properties
                UpdateReceiverAccountProperties(receiverAccount, request);
                // Updates receiver account details after the transfer.

                // 17. Map sender transaction to DTO
                var transactionDto = CurrencyNotesMapper.MapTransactionToDto(senderTransactionEntity, null, _userInfoToken);
                // Maps the sender transaction entity to a TransactionDto, enabling easy response or display.

                // 18. Create teller operation
                var tellerOperation = CreateTellerOperation(request.Amount, OperationType.Transfer.ToString(), teller, tellerAccount, senderTransactionEntity, TransactionType.TTP_TRANSFER.ToString(), isInterBranchOperation, senderCustomer.BranchId, receiverCustomer.BranchId, senderCustomer.FirstName, senderAccount.AccountNumber);
                // Records a teller operation for auditing purposes with details like operation type, amount, involved branches, and customer name.

                // 19. Add teller operation to repository
                _tellerOperationRepository.Add(tellerOperation);  // Saves the teller operation for persistence.

                // 20. Return the transaction DTO
                return transactionDto;  // Returns the DTO, representing successful completion of the transfer.
            }
            catch (Exception ex)
            {
                // 21. Log any errors that occur during the transfer process
                _logger.LogError($"Error occurred while processing transfer: {ex.Message}");

                // 22. Rethrow the exception for higher-level handling
                throw ex;  // Propagates the exception up to higher-level error handlers.
            }
        }

        /// <summary>
        /// Updates the properties of the sender's account based on the total transfer amount including charges.
        /// </summary>
        /// <param name="senderAccount">The sender's account to update.</param>
        /// <param name="TotalAmountToTransferWithCharges">The total amount to transfer including charges.</param>
        private void UpdateSenderAccountProperties(Account senderAccount, decimal TotalAmountToTransferWithCharges)
        {
            // Use the repository to update the existing Account entity
            _accountRepository.DebitAccount(senderAccount, TotalAmountToTransferWithCharges);
        }

        /// <summary>
        /// Creates and returns a transaction entity for the receiver of the TTP transfer.
        /// </summary>
        /// <param name="request">The TTP transfer request.</param>
        /// <param name="receiverAccount">The receiver's account.</param>
        /// <param name="transferLimit">The transfer parameter limit.</param>
        /// <param name="senderAccount">The sender's account.</param>
        /// <param name="senderTransaction">The transaction entity of the sender.</param>
        /// <returns>The transaction entity for the receiver.</returns>
        private async Task<Transaction> CreateReceiverTransactionEntity(TransferTTP request, Account receiverAccount, TransferParameter transferLimit, Account senderAccount, Transaction senderTransaction)
        {
            // Create a new Transaction entity for the receiver
            var receiverTransactionEntity = new Transaction();
            // Convert UTC to local time and set it in the entity
            receiverTransactionEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            // Generate a unique transaction ID
            receiverTransactionEntity.Id = BaseUtilities.GenerateUniqueNumber();
            // Use the sender's transaction reference for the receiver's transaction
            receiverTransactionEntity.TransactionReference = senderTransaction.TransactionReference;
            // Set the transaction type and operation
            receiverTransactionEntity.TransactionType = transferLimit.TransferType;
            receiverTransactionEntity.Operation = TransactionType.TTP_TRANSFER.ToString();
            // Store the receiver's previous balance before the transfer
            receiverTransactionEntity.PreviousBalance = receiverAccount.Balance;
            // Calculate the new balance after receiving the transfer
            receiverTransactionEntity.Balance = receiverAccount.Balance + request.Amount;
            // Set various properties of the receiver's transaction entity
            receiverTransactionEntity.AccountId = receiverAccount.Id;
            receiverTransactionEntity.SenderAccountId = senderAccount.Id;
            receiverTransactionEntity.AccountNumber = receiverAccount.AccountNumber;
            receiverTransactionEntity.ReceiverAccountId = receiverAccount.Id;
            receiverTransactionEntity.ProductId = receiverAccount.ProductId;
            receiverTransactionEntity.Status = "COMPLETED";
            receiverTransactionEntity.Note ??=
                $"{receiverAccount.CustomerName}, you have successfully received a transfer of {BaseUtilities.FormatCurrency(request.Amount)} from account number {senderAccount.AccountNumber} to your account ({receiverAccount.AccountNumber}). " +
                $"The transaction reference is {senderTransaction.TransactionReference}. Please verify your account to confirm receipt.";
            receiverTransactionEntity.BankId = receiverAccount.BankId;
            receiverTransactionEntity.BranchId = receiverAccount.BranchId;
            receiverTransactionEntity.OriginalDepositAmount = request.Amount;
            receiverTransactionEntity.Fee = 0;
            receiverTransactionEntity.Tax = 0;
            receiverTransactionEntity.Amount = request.Amount;
            receiverTransactionEntity.CustomerId = receiverAccount.CustomerId;
            receiverTransactionEntity.OperationType = OperationType.Credit.ToString();
            receiverTransactionEntity.FeeType = Events.None.ToString();
            receiverTransactionEntity.TellerId = senderTransaction.TellerId;
            receiverTransactionEntity.Debit = 0;
            receiverTransactionEntity.Credit = request.Amount;
            receiverTransactionEntity.SourceType = senderTransaction.SourceType;
            receiverTransactionEntity.IsInterBrachOperation = senderTransaction.IsInterBrachOperation;
            receiverTransactionEntity.SourceBrachId = senderTransaction.BranchId;
            receiverTransactionEntity.DestinationBrachId = senderTransaction.DestinationBrachId;
            receiverTransactionEntity.DestinationBranchCommission = senderTransaction.DestinationBranchCommission;
            receiverTransactionEntity.SourceBranchCommission = senderTransaction.SourceBranchCommission;
            // Add the receiver's transaction entity to the repository
            _TransactionRepository.Add(receiverTransactionEntity);
            // Return the receiver's transaction entity
            return receiverTransactionEntity;
        }

        /// <summary>
        /// Updates the properties of the receiver's account based on the transfer request.
        /// </summary>
        /// <param name="receiverAccount">The receiver's account to update.</param>
        /// <param name="request">The transfer request containing the transfer amount.</param>
        private void UpdateReceiverAccountProperties(Account receiverAccount, TransferTTP request)
        {
            // Use the repository to update the existing Account entity
            _accountRepository.CreditAccount(receiverAccount, request.Amount);
        }

        /// <summary>
        /// Logs an error message and performs an audit operation.
        /// </summary>
        /// <param name="userEmail">The email of the user associated with the error.</param>
        /// <param name="logAction">The action to log.</param>
        /// <param name="request">The TTP transfer request associated with the error.</param>
        /// <param name="errorMessage">The error message to log.</param>
        /// <param name="logLevel">The level of the log message.</param>
        /// <param name="statusCode">The HTTP status code associated with the error.</param>
        /// <param name="userToken">The token of the user associated with the error.</param>
        private async Task LogErrorAndAudit(string userEmail, string logAction, TransferTTP request, string errorMessage, string logLevel, int statusCode, string userToken)
        {
            // Log the error message
            _logger.LogError(errorMessage);
            // Perform an audit log
            await APICallHelper.AuditLogger(userEmail, logAction, request, errorMessage, logLevel, statusCode, userToken);
        }

        /// <summary>
        /// Creates a sender transaction entity for the TTP transfer operation.
        /// </summary>
        /// <param name="request">The TTP transfer request.</param>
        /// <param name="senderAccount">The sender's account.</param>
        /// <param name="transferLimit">The transfer parameter containing transfer limits.</param>
        /// <param name="receiverAccount">The receiver's account.</param>
        /// <param name="transferCharges">The charges associated with the transfer.</param>
        /// <param name="currencyid">The currency ID.</param>
        /// <param name="tellerId">The ID of the teller performing the transfer.</param>
        /// <param name="isInternalTransfer">A flag indicating if the transfer is internal or not.</param>
        /// <param name="sourceBranchId">The ID of the source branch.</param>
        /// <param name="destinationBranchId">The ID of the destination branch.</param>
        /// <param name="totalAmounToTransfer">The total amount to transfer including charges.</param>
        /// <returns>The created sender transaction entity.</returns>
        private async Task<Transaction> CreateSenderTransactionEntity(TransferTTP request, Account senderAccount, TransferParameter transferLimit, Account receiverAccount, TransferCharges transferCharges, string currencyid, string tellerId, bool isInternalTransfer, string sourceBranchId, string destinationBranchId, decimal totalAmounToTransfer, string Reference, string TPPtransferType)
        {
            var senderTransactionEntity = new Transaction();

            // Set transaction properties
            senderTransactionEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            senderTransactionEntity.Id = BaseUtilities.GenerateUniqueNumber();
            senderTransactionEntity.TransactionReference = Reference;
            senderTransactionEntity.TransactionType = TPPtransferType;
            senderTransactionEntity.Operation = TransactionType.TTP_TRANSFER.ToString();
            senderTransactionEntity.PreviousBalance = senderAccount.Balance;
            senderTransactionEntity.Balance = senderAccount.Balance - totalAmounToTransfer;
            senderTransactionEntity.AccountId = senderAccount.Id;
            senderTransactionEntity.AccountNumber = senderAccount.AccountNumber;
            senderTransactionEntity.SenderAccountId = senderAccount.Id;
            senderTransactionEntity.ReceiverAccountId = receiverAccount.Id;
            senderTransactionEntity.Status = "COMPLETED";
            senderTransactionEntity.ProductId = senderAccount.ProductId;
            senderTransactionEntity.BankId = senderAccount.BankId;
            senderTransactionEntity.BranchId = senderAccount.BranchId;
            senderTransactionEntity.OriginalDepositAmount = request.Amount;
            senderTransactionEntity.Fee = transferCharges.TotalCharges;
            senderTransactionEntity.Tax = 0;
            senderTransactionEntity.Credit = 0;
            senderTransactionEntity.TellerId = tellerId;
            senderTransactionEntity.Debit = totalAmounToTransfer;
            senderTransactionEntity.Amount = -(totalAmounToTransfer);
            senderTransactionEntity.CustomerId = senderAccount.CustomerId;
            senderTransactionEntity.OperationType = OperationType.Debit.ToString();
            senderTransactionEntity.FeeType = Events.ChargeOfTransfer.ToString();
            senderTransactionEntity.SourceType = request.SourceType;
            senderTransactionEntity.SourceBrachId = sourceBranchId;
            senderTransactionEntity.IsInterBrachOperation = isInternalTransfer;
            senderTransactionEntity.DestinationBrachId = destinationBranchId;
            senderTransactionEntity.DestinationBranchCommission = isInternalTransfer ? XAFWallet.CalculateCommission(transferLimit.DestinationBranchOfficeShare, transferCharges.TotalCharges) : 0;
            senderTransactionEntity.SourceBranchCommission = isInternalTransfer ? XAFWallet.CalculateCommission(transferLimit.SourceBrachOfficeShare, transferCharges.TotalCharges) : transferCharges.TotalCharges;
            senderTransactionEntity.Note ??=
                $"{senderAccount.CustomerName}, a transfer of {BaseUtilities.FormatCurrency(request.Amount)} has been successfully initiated from your account ({senderAccount.AccountNumber}) to account number {receiverAccount.AccountNumber}. " +
                $"An additional charge of {BaseUtilities.FormatCurrency(transferCharges.TotalCharges)} has been applied for this transaction, bringing the total debit from your account to {BaseUtilities.FormatCurrency(totalAmounToTransfer)}. " +
                $"Transaction ID: {senderTransactionEntity.TransactionReference}. Please review your account balance for confirmation.";

            // Add the sender transaction entity to the repository
            _TransactionRepository.Add(senderTransactionEntity);

            return senderTransactionEntity;
        }

        /// <summary>
        /// Checks if the sender's account has enough funds for a transfer based on the provided amount, charges, and member type.
        /// </summary>
        /// <param name="account">The sender's account to check funds for.</param>
        /// <param name="amount">The transfer amount.</param>
        /// <param name="charges">The transfer charges.</param>
        /// <param name="memberType">The type of member (physical or moral person).</param>
        /// <returns>True if the sender's account has enough funds for the transfer; otherwise, false.</returns>
        private bool CheckIfSenderHasEnoughFundForTransfer(Account account, decimal amount, decimal charges, string memberType)
        {
            // Check if the member type is a physical person
            if (memberType == MemberType.Physical_Person.ToString())
            {
                // Calculate the minimum required balance for a physical person account
                decimal minimumBalance = account.Product.MinimumAccountBalancePhysicalPerson;
                // Check if the account has enough funds considering the transfer amount, charges, and blocked amount
                return account.Balance - (amount + charges + account.BlockedAmount) >= minimumBalance;
            }
            else
            {
                // Calculate the minimum required balance for a moral person account
                decimal minimumBalance = account.Product.MinimumAccountBalanceMoralPerson;
                // Check if the account has enough funds considering the transfer amount, charges, and blocked amount
                return account.Balance - (amount + charges + account.BlockedAmount) >= minimumBalance;
            }
        }

        /// <summary>
        /// Calculates the total amount to transfer including charges.
        /// </summary>
        /// <param name="Charges">The charges associated with the transfer.</param>
        /// <param name="amount">The amount to transfer.</param>
        /// <returns>The total amount to transfer including charges.</returns>
        private decimal TotalAmountToTransferWithCharges(decimal Charges, decimal amount)
        {
            return amount + Charges;
        }


        /// <summary>
        /// Creates a TellerOperation object based on the provided parameters.
        /// </summary>
        /// <param name="amount">The amount associated with the operation.</param>
        /// <param name="operationType">The type of operation (e.g., Credit, Debit).</param>
        /// <param name="teller">The teller associated with the operation.</param>
        /// <param name="tellerAccount">The account of the teller.</param>
        /// <param name="transaction">The transaction associated with the operation.</param>
        /// <param name="eventType">The type of event (e.g., ChargeOfTransfer).</param>
        /// <param name="isInterBranch">A boolean value indicating whether it's an inter-branch operation.</param>
        /// <param name="sourceBranchId">The ID of the source branch.</param>
        /// <param name="destinationBranchId">The ID of the destination branch.</param>
        /// <returns>The created TellerOperation object.</returns>
        private TellerOperation CreateTellerOperation(decimal amount, string operationType, Teller teller, Account tellerAccount, Transaction transaction, string eventType, bool isInterBranch, string sourceBranchId, string destinationBranchId, string cusotmerName, string memberAccountNumber)
        {

            var data = new TellerOperation
            {
                Id = BaseUtilities.GenerateUniqueNumber(),
                AccountID = tellerAccount.Id,
                AccountNumber = tellerAccount.AccountNumber,
                Amount = amount,
                OpenOfDayReference = tellerAccount.OpenningOfDayReference == null ? $"GAV{DateTime.Now.ToString("yyyyMMdd")}" : tellerAccount.OpenningOfDayReference,
                OperationType = operationType.ToString(),
                BranchId = tellerAccount.BranchId,
                BankId = tellerAccount.BankId,
                CurrentBalance = tellerAccount.Balance,
                Date = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow),
                PreviousBalance = tellerAccount.PreviousBalance,
                TellerID = teller.Id, 
                Description = $"{transaction.TransactionType} of {BaseUtilities.FormatCurrency(amount)}, Account Number: {tellerAccount.AccountNumber}",
                TransactionReference = transaction.TransactionReference,
                TransactionType = transaction.TransactionType,
                UserID = _userInfoToken.FullName,
                EventName = eventType,
                MemberName = cusotmerName,
                AccountingDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow),
                IsCashOperation = false, 
                MemberAccountNumber = memberAccountNumber,
                CustomerId = transaction.CustomerId,
                ReferenceId = transaction.TransactionReference,
                DestinationBrachId = destinationBranchId,
                SourceBranchId = sourceBranchId,
                IsInterBranch = isInterBranch,
                DestinationBranchCommission = transaction.DestinationBranchCommission,
                SourceBranchCommission = transaction.SourceBranchCommission
            };
            return data;
        }
    }
}
