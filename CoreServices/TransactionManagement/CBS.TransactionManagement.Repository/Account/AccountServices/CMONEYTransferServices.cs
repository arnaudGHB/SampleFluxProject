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
    public class CMONEYTransferServices : ICMONEYTransferServices
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

        public CMONEYTransferServices(ITellerOperationRepository tellerOperationRepository = null, IMediator mediator = null, ITransactionRepository transactionRepository = null, ILogger<AccountRepository> logger = null, IAccountRepository accountRepository = null, IUnitOfWork<TransactionContext> uow = null, UserInfoToken userInfoToken = null, IChargesWaivedRepository chargesWaivedRepository = null, IWithdrawalNotificationRepository withdrawalNotificationRepository = null, ISavingProductFeeRepository savingProductFeeRepository = null)
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
        public async Task<TransactionDto> TransferCMONEY(Teller teller, Account tellerAccount, TransferTTP request, string Reference, Account senderAccount, Account receiverAccount, CustomerDto senderCustomer, CustomerDto receiverCustomer, string sourceType, string TPPtransferType)
        {
            try
            {
                // Initialize variables
                bool isInterBranchOperation = false;
                string transferType = TransferType.Local.ToString();
                // Check if sender and receiver accounts are the same
                if (senderAccount.AccountNumber == receiverAccount.AccountNumber)
                {
                    string errorMessage = $"Transfer cannot be done to the same account number. Sender: {senderAccount.AccountNumber}, Receiver: {receiverAccount.AccountNumber}";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.TransferCMONEY, LogLevelInfo.Warning);
                    throw new InvalidOperationException(errorMessage);
                }
                // Check if the transfer is an inter-branch operation
                if (senderCustomer.BranchId != receiverCustomer.BranchId)
                {
                    isInterBranchOperation = true;
                    transferType = TransferType.Inter_Branch.ToString();
                }
                // Retrieve transfer parameters for the sender account
                var transferParameter = senderAccount.Product.TransferParameters.FirstOrDefault(x => x.TransferType == transferType);
                if (transferParameter == null)
                {
                    var errorMessage = $"Transfer parameters are not configured for {senderAccount.Product.Name}. Contact your administrator to configure these parameters on {senderAccount.Product.Name}.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.TransferCMONEY, LogLevelInfo.Warning);
                    throw new InvalidOperationException(errorMessage);
                }
                // Calculate transfer charges
                var transferCharges = await CalculateTransferCharges(request.Amount, senderAccount.ProductId, FeeOperationType.CMoney);
                // Check if sender has enough funds for the transfer
                if (!CheckIfSenderHasEnoughFundForTransfer(senderAccount, request.Amount, transferCharges.TotalCharges, senderCustomer.LegalForm))
                {
                    var errorMessage = $"{senderCustomer.FirstName} {senderCustomer.LastName}, your balance is insufficient to perform this transfer.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.TransferCMONEY, LogLevelInfo.Warning);
                    throw new InvalidOperationException(errorMessage);
                }
                // Calculate total amount to transfer with charges
                decimal totalAmountToTransfer = TotalAmountToTransferWithCharges(transferCharges.TotalCharges, request.Amount);
                // Create and process sender transaction
                var senderTransactionEntity = await CreateSenderTransactionEntity(request, senderAccount, transferParameter, receiverAccount, transferCharges, "XAF", teller.Id, isInterBranchOperation, senderCustomer.BranchId, receiverCustomer.BranchId, totalAmountToTransfer, Reference, TPPtransferType);
                // Update sender account properties
                UpdateSenderAccountProperties(senderAccount, totalAmountToTransfer);
                // Create and process receiver transaction
                var receiverTransactionEntity = await CreateReceiverTransactionEntity(request, receiverAccount, transferParameter, senderAccount, senderTransactionEntity);
                // Update receiver account properties
                UpdateReceiverAccountProperties(receiverAccount, request);
                // Map sender transaction to DTO
                var transactionDto = CurrencyNotesMapper.MapTransactionToDto(senderTransactionEntity, null, _userInfoToken);
                // Create teller operation
                var tellerOperation = CreateTellerOperation(request.Amount, OperationType.Transfer.ToString(), teller, tellerAccount, senderTransactionEntity, TransactionType.TTP_TRANSFER.ToString(), isInterBranchOperation, senderCustomer.BranchId, receiverCustomer.BranchId, senderCustomer.FirstName, senderAccount.AccountNumber);

                _tellerOperationRepository.Add(tellerOperation);
                return transactionDto;
            }
            catch (Exception ex)
            {
                // Log and handle exceptions
                _logger.LogError($"Error occurred while processing withdrawal: {ex.Message}");
                throw; // Rethrow the exception for handling at a higher level
            }
        }

        /// <summary>
        /// Updates the properties of the sender's account based on the total transfer amount including charges.
        /// </summary>
        /// <param name="senderAccount">The sender's account to update.</param>
        /// <param name="TotalAmountToTransferWithCharges">The total amount to transfer including charges.</param>
        private void UpdateSenderAccountProperties(Account senderAccount, decimal TotalAmountToTransferWithCharges)
        {
            // Update Account entity properties with values from the request
            // Store the previous balance before the update
            senderAccount.PreviousBalance = senderAccount.Balance;
            // Decrease the balance of the sender's account by the total amount to transfer with charges
            senderAccount.Balance -= TotalAmountToTransferWithCharges;
            // Encrypt the updated balance for security purposes
            senderAccount.EncryptedBalance = BalanceEncryption.Encrypt(senderAccount.Balance.ToString(), senderAccount.AccountNumber);
            // Set the Product property to null to avoid unintentional updates
            senderAccount.Product = null;
            // Use the repository to update the existing Account entity
            _accountRepository.Update(senderAccount);
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
            receiverTransactionEntity.Note ??= $"{receiverAccount.CustomerName}, you have successfully received a transfer of {BaseUtilities.FormatCurrency(request.Amount)} from account number {senderAccount.AccountNumber} into your account {receiverAccount.AccountNumber}. The reference for this transaction is {senderTransaction.TransactionReference}. Please review your account for confirmation.";
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
            // Update Account entity properties with values from the request
            // Store the previous balance before the update
            receiverAccount.PreviousBalance = receiverAccount.Balance;
            // Increase the balance of the receiver's account by the transfer amount
            receiverAccount.Balance += request.Amount;
            // Encrypt the updated balance for security purposes
            receiverAccount.EncryptedBalance = BalanceEncryption.Encrypt(receiverAccount.Balance.ToString(), receiverAccount.AccountNumber);
            // Set the Product property to null to avoid unintentional updates
            receiverAccount.Product = null;
            // Use the repository to update the existing Account entity
            _accountRepository.Update(receiverAccount);
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
            senderTransactionEntity.Note ??= $"{senderAccount.CustomerName}, a transfer of {BaseUtilities.FormatCurrency(request.Amount)} has been successfully initiated from your account ({senderAccount.AccountNumber}) to account number {receiverAccount.AccountNumber}. An additional charge of {BaseUtilities.FormatCurrency(transferCharges.TotalCharges)} has been applied for this transaction, making the total amount debited from your account {BaseUtilities.FormatCurrency(totalAmounToTransfer)}. For your reference, the transaction ID is {senderTransactionEntity.TransactionReference}. Please review your account balance to confirm.";

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
        /// Calculates the transfer charges for a given amount based on the transfer parameters and fee configuration.
        /// </summary>
        /// <param name="amount">The amount for which the transfer charges need to be calculated.</param>
        /// <param name="transferParameter">The transfer parameters containing the product and fee information.</param>
        /// <param name="transferType">The type of transfer (e.g., local transfer or inter-brnach transfer).</param>
        /// <returns>A TransferCharges object containing the calculated service charge and total charges.</returns>
        //private async Task<TransferCharges> CalculateTransferCharges(decimal amount, TransferParameter transferParameter, string transferType)
        //{
        //    // Initialize TransferCharges object to store calculated charges
        //    TransferCharges transferCharges = new TransferCharges();
        //    // Retrieve transfer fees based on the transfer parameters
        //    var fees = await _savingProductFeeRepository.FindBy(x => x.SavingProductId == transferParameter.ProductId && x.FeePolicyType == transferType && x.FeeType.ToLower() == "transfer" && !x.IsDeleted).Include(x => x.Fee).Include(x => x.Fee.FeePolicies).ToListAsync();
        //    // Check if transfer fees are configured for the product
        //    if (!fees.Any())
        //    {
        //        var errorMessage = $"Transfer charges are not configured for {transferParameter.Product.Name}. Contact your administrator to configure charges on {transferParameter.Product.Name}.";
        //        _logger.LogError(errorMessage);
        //        throw new InvalidOperationException(errorMessage);
        //    }

        //    // Extract fee types from the retrieved fees
        //    var rate = fees.FirstOrDefault(x => x.Fee.FeeType.ToLower() == "rate");
        //    var range = fees.FirstOrDefault(x => x.Fee.FeeType.ToLower() == "range");
        //    // Calculate service charge based on withdrawal fee rate
        //    decimal percentageChargedAmount = rate?.Fee?.FeePolicies?.FirstOrDefault()?.Value ?? 0;

        //    if (rate != null && rate.Fee != null && !rate.Fee.FeePolicies.Any())
        //    {
        //        percentageChargedAmount = XAFWallet.ConvertPercentageToCharge(rate.Fee.FeePolicies.FirstOrDefault().Value, amount);
        //    }
        //    else
        //    {
        //        // Handle the case where rate is null or any of its properties are null
        //        // For example:
        //        percentageChargedAmount = 0; // Assign a default value or handle accordingly
        //    }
        //    if (percentageChargedAmount > 0)
        //    {
        //        // Apply percentage-based charge if applicable
        //        transferCharges.ServiceCharge = percentageChargedAmount;
        //    }
        //    else
        //    {
        //        // Calculate range-based charge if applicable
        //        decimal rangeChargeCalculated = CurrencyNotesMapper.GetFeePolicyRange(range.Fee.FeePolicies.ToList(), amount).Charge;
        //        transferCharges.ServiceCharge = rangeChargeCalculated;
        //    }
        //    // Calculate total charges including additional fees
        //    var all_charges = XAFWallet.CalculateCustomerWithdrawalCharges(transferCharges.ServiceCharge, 0);
        //    transferCharges.TotalCharges = all_charges;
        //    return transferCharges;
        //}
        public async Task<TransferCharges> CalculateTransferCharges(decimal amount, string productid, FeeOperationType transferType)
        {
            // Initialize TransferCharges object to store calculated charges
            TransferCharges transferCharges = new TransferCharges();

            // Define the queryable collection for fees
            IQueryable<SavingProductFee> feeQuery = _savingProductFeeRepository
                .FindBy(x => x.SavingProductId == productid
                             && x.FeeType.ToLower() == "transfer"
                             && !x.IsDeleted)
                .Include(x => x.Fee)
                .ThenInclude(f => f.FeePolicies);

            // Use switch to filter fees based on the transfer type
            switch (transferType)
            {
                case FeeOperationType.MemberShip:
                    feeQuery = feeQuery.Where(x => x.Fee.OperationFeeType == FeeOperationType.MemberShip.ToString());
                    break;
                case FeeOperationType.Operation:
                    feeQuery = feeQuery.Where(x => x.Fee.OperationFeeType == FeeOperationType.Operation.ToString());
                    break;
                case FeeOperationType.CMoney:
                    feeQuery = feeQuery.Where(x => x.Fee.OperationFeeType == FeeOperationType.CMoney.ToString());
                    break;
                case FeeOperationType.Gav:
                    feeQuery = feeQuery.Where(x => x.Fee.OperationFeeType == FeeOperationType.Gav.ToString());
                    break;
                case FeeOperationType.MobileMoney:
                    feeQuery = feeQuery.Where(x => x.Fee.OperationFeeType == FeeOperationType.MobileMoney.ToString());
                    break;
                case FeeOperationType.OrangeMoney:
                    feeQuery = feeQuery.Where(x => x.Fee.OperationFeeType == FeeOperationType.OrangeMoney.ToString());
                    break;
                default:
                    throw new InvalidOperationException("Unsupported transfer type.");
            }

            // Execute the query and retrieve the list of fees
            var fees = await feeQuery.ToListAsync();

            // Check if transfer fees are configured for the product
            if (!fees.Any())
            {
                var errorMessage = $"Transfer charges for {transferType.ToString()} are not configured for the selected account. Contact your administrator to configure charges on selected account for {transferType.ToString()}.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            // Extract fee types from the retrieved fees
            var rate = fees.FirstOrDefault(x => x.Fee.FeeType.ToLower() == "rate");
            var range = fees.FirstOrDefault(x => x.Fee.FeeType.ToLower() == "range");

            // Calculate service charge based on withdrawal fee rate
            decimal percentageChargedAmount = rate?.Fee?.FeePolicies?.FirstOrDefault()?.Value ?? 0;

            if (rate != null && rate.Fee != null && rate.Fee.FeePolicies.Any())
            {
                percentageChargedAmount = XAFWallet.ConvertPercentageToCharge(rate.Fee.FeePolicies.FirstOrDefault().Value, amount);
            }
            else
            {
                // Assign a default value or handle accordingly if rate is null or has no policies
                percentageChargedAmount = 0;
            }

            if (percentageChargedAmount > 0)
            {
                // Apply percentage-based charge if applicable
                transferCharges.ServiceCharge = percentageChargedAmount;
            }
            else
            {
                // Calculate range-based charge if applicable
                decimal rangeChargeCalculated = CurrencyNotesMapper.GetFeePolicyRange(range.Fee.FeePolicies.ToList(), amount).Charge;
                transferCharges.ServiceCharge = rangeChargeCalculated;
            }

            // Calculate total charges including additional fees
            var all_charges = XAFWallet.CalculateCustomerWithdrawalCharges(transferCharges.ServiceCharge, 0);
            transferCharges.TotalCharges = all_charges;

            return transferCharges;
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
