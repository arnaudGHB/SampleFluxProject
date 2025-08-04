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
using CBS.TransactionManagement.MediatR;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Data.Entity.TransferLimits;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using DocumentFormat.OpenXml.Office2010.Drawing;
using CBS.TransactionManagement.Data.Entity.AccountingDayOpening;
using DocumentFormat.OpenXml.Features;
using CBS.TransactionManagement.Repository.VaultP;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to add a new Transfer Transaction.
    /// </summary>
    public class TransferConfirmationCommandHandler : IRequestHandler<TransferConfirmationCommand, ServiceResponse<TransactionDto>>
    {
        private readonly IAccountRepository _AccountRepository;
        private readonly ITransferRepository _transferRepository;
        private readonly ITransactionRepository _TransactionRepository; // Repository for accessing Transaction data.
        private readonly ILogger<TransferConfirmationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly ITellerRepository _tellerRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfigRepository _ConfigRepository;
        private readonly IAccountingDayRepository _accountingDayRepository;
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Constructor for initializing the TransferConfirmationCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Transaction data access.</param>
        /// <param name="TransactionRepository">Repository for Transaction data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public TransferConfirmationCommandHandler(
            IAccountRepository AccountRepository,
            ITransactionRepository TransactionRepository,
            ILogger<TransferConfirmationCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            ITellerOperationRepository tellerOperationRepository = null,
            ITellerRepository tellerRepository = null,
            UserInfoToken userInfoToken = null,
            IConfigRepository configRepository = null,
            IMediator mediator = null,
            ITransferRepository transferRepository = null,
            IAccountingDayRepository accountingDayRepository = null)
        {
            _AccountRepository = AccountRepository;
            _TransactionRepository = TransactionRepository;
            _logger = logger;
            _uow = uow;
            _tellerOperationRepository = tellerOperationRepository;
            _tellerRepository = tellerRepository;
            _userInfoToken = userInfoToken;
            _ConfigRepository = configRepository;
            _mediator = mediator;
            _transferRepository = transferRepository;
            _accountingDayRepository = accountingDayRepository;
        }

        /// <summary>
        /// Handles the TransferConfirmationCommand to add a new Transfer Transaction.
        /// </summary>
        /// <param name="request">The TransferConfirmationCommand containing Transaction data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>


        public async Task<ServiceResponse<TransactionDto>> Handle(TransferConfirmationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the current accounting date for the user's branch.
                string msg = string.Empty;
                var accountingDate = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);

                // Step 2: Retrieve transfer details using Transfer ID.
                var transfer = await _transferRepository.FindAsync(request.TransferId);
                if (transfer == null)
                {
                    msg = $"Transfer ID {request.TransferId} is invalid. Please verify and try again.";
                    await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.NotFound, LogAction.Transfer, LogLevelInfo.Warning);
                    return ServiceResponse<TransactionDto>.Return404(msg);
                }

                // Step 3: Validate if transfer is already approved.
                if (transfer.Status == Status.Approved.ToString())
                {
                    msg = $"Transfer with reference {transfer.TransactionRef} has already been approved.";
                    await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.Forbidden, LogAction.Transfer, LogLevelInfo.Warning);
                    return ServiceResponse<TransactionDto>.Return403(msg);
                }

                if (request.Status == Status.Approved.ToString())
                {
                    bool isInternalTransfer = false;
                    string transferType = transfer.TransactionType;

                    // Step 4: Retrieve teller details associated with the transfer.
                    var teller = await GetTeller(transfer.TellerId);
                    if (teller == null)
                    {
                        msg = $"Teller with ID {transfer.TellerId} does not exist.";
                        await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.NotFound, LogAction.Transfer, LogLevelInfo.Warning);
                        return ServiceResponse<TransactionDto>.Return404(msg);
                    }

                    // Step 5: Verify teller's authorization for the transfer amount.
                    if (teller.MaximumTransferAmount < transfer.Amount)
                    {
                        msg = $"Teller {teller.Name} is not authorized for transfers exceeding {BaseUtilities.FormatCurrency(teller.MaximumTransferAmount)}.";
                        await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.Forbidden, LogAction.Transfer, LogLevelInfo.Warning);
                        return ServiceResponse<TransactionDto>.Return403(msg);
                    }

                    // Step 6: Retrieve teller's account information.
                    var tellerAccount = await GetTellerAccount(transfer.TellerId);
                    if (tellerAccount == null)
                    {
                        msg = $"No account found for teller {teller.Name}.";
                        await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.NotFound, LogAction.Transfer, LogLevelInfo.Warning);
                        return ServiceResponse<TransactionDto>.Return404(msg);
                    }

                    // Step 7: Retrieve sender account and validate status.
                    var senderAccount = await GetSenderAccount(transfer.SourceAccountNumber);
                    if (senderAccount == null || !senderAccount.Status.Equals(AccountStatus.Active.ToString()))
                    {
                        msg = senderAccount == null
                            ? $"Sender account {transfer.SourceAccountNumber} is not found."
                            : $"Sender account {transfer.SourceAccountNumber} is inactive.";
                        await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.Forbidden, LogAction.Transfer, LogLevelInfo.Warning);
                        return ServiceResponse<TransactionDto>.Return403(msg);
                    }

                    // Step 8: Retrieve sender customer details and validate membership approval.
                    var getCustomerCommand = new GetCustomerCommandQuery { CustomerID = senderAccount.CustomerId };
                    var serviceResponse = await _mediator.Send(getCustomerCommand);
                    if (serviceResponse.StatusCode != 200 || serviceResponse.Data.MembershipApprovalStatus.ToLower() != AccountStatus.approved.ToString().ToLower())
                    {
                        msg = $"Sender customer membership is not approved or could not be retrieved. Current status: {serviceResponse.Data?.MembershipApprovalStatus ?? "Unknown"}.";
                        await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.Forbidden, LogAction.Transfer, LogLevelInfo.Warning);
                        return ServiceResponse<TransactionDto>.Return403(msg);
                    }
                    var senderCustomer = serviceResponse.Data;

                    // Step 9: Retrieve receiver account and customer details.
                    var receiverAccount = await _AccountRepository.GetAccountByAccountNumber(transfer.DestinationAccountNumber);
                    if (receiverAccount == null)
                    {
                        msg = $"Receiver account {transfer.DestinationAccountNumber} could not be located.";
                        await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.NotFound, LogAction.Transfer, LogLevelInfo.Warning);
                        return ServiceResponse<TransactionDto>.Return404(msg);
                    }

                    var getCustomerCommand1 = new GetCustomerCommandQuery { CustomerID = receiverAccount.CustomerId };
                    var receiverCustomerResponse = await _mediator.Send(getCustomerCommand1);
                    if (receiverCustomerResponse.StatusCode != 200)
                    {
                        msg = $"Error retrieving receiver customer details for account {transfer.DestinationAccountNumber}.";
                        await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.NotFound, LogAction.Transfer, LogLevelInfo.Warning);
                        return ServiceResponse<TransactionDto>.Return404(msg);
                    }
                    var receiverCustomer = receiverCustomerResponse.Data;

                    // Step 10: Prevent self-transfers by comparing sender and receiver accounts.
                    if (senderAccount.AccountNumber == receiverAccount.AccountNumber)
                    {
                        msg = "Transfer cannot be processed within the same account.";
                        await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.Forbidden, LogAction.Transfer, LogLevelInfo.Warning);
                        return ServiceResponse<TransactionDto>.Return403(msg);
                    }

                    // Step 11: Validate transfer limits.
                    isInternalTransfer = transfer.IsInterBranchOperation;
                    var transferLimit = GetTransferLimit(senderAccount.Product.TransferParameters, transferType);
                    if (transferLimit == null || !IsTransferAmountWithinLimits(transfer.Amount, transferLimit))
                    {
                        msg = $"Transfer limits not configured or exceeded for this transaction.";
                        await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.Forbidden, LogAction.Transfer, LogLevelInfo.Warning);
                        return ServiceResponse<TransactionDto>.Return403(msg);
                    }

                    // Step 12: Validate sender's account balance sufficiency.
                    if (!HasEnoughFundsForTransfer(senderAccount, transferLimit, transfer.Amount))
                    {
                        msg = $"Insufficient funds in account {senderAccount.AccountNumber} for transfer amount {transfer.Amount}.";
                        await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.Forbidden, LogAction.Transfer, LogLevelInfo.Warning);
                        return ServiceResponse<TransactionDto>.Return403(msg);
                    }

                    // Step 13: Process sender transaction and update account balances.
                    var senderTransactionEntity = await CreateSenderTransactionEntity(request, senderAccount, transferLimit, receiverAccount, TransferCharges(transferLimit, transfer.Amount), "", senderCustomer.BranchId, receiverCustomer.BranchId, isInternalTransfer, teller.Id, transfer, accountingDate);
                    UpdateSenderAccountProperties(senderAccount, senderTransactionEntity, transferLimit, transfer.Amount);

                    // Step 14: Process receiver transaction and update account balances.
                    var receiverTransactionEntity = await CreateReceiverTransactionEntity(request, receiverAccount, transferLimit, senderAccount, senderTransactionEntity, transfer);
                    UpdateReceiverAccountProperties(receiverAccount, receiverTransactionEntity, request, transfer.Amount);
                    CreateTellerOperation(transfer.Amount, OperationType.Credit, tellerAccount, senderTransactionEntity, OperationType.Transfer.ToString(), _userInfoToken.Id, accountingDate);
                    CreateTellerOperation(transfer.Amount, OperationType.Debit, tellerAccount, receiverTransactionEntity, OperationType.Transfer.ToString(), _userInfoToken.Id, accountingDate);
                    if (transfer.Charges>0)
                    {
                        CreateTellerOperation(transfer.Charges, OperationType.Credit, tellerAccount, receiverTransactionEntity, OperationType.Transfer.ToString(), _userInfoToken.Id, accountingDate);
                    }

                    // Step 15: Complete and save the transfer, log success, and send confirmation.
                    transfer.ApprovedByUserName = _userInfoToken.FullName;
                    transfer.DateOfApproval = BaseUtilities.UtcToDoualaTime(DateTime.Now);
                    transfer.ValidatorComment = request.Note;
                    transfer.Status = Status.Approved.ToString();
                    _transferRepository.Update(transfer);
                    await _uow.SaveAsync();

                    var transactionDto = CurrencyNotesMapper.MapTransactionToDto(senderTransactionEntity, null, _userInfoToken);
                    transactionDto.Teller = teller;
                    transactionDto.Account = senderAccount;

                    await SendTransferSMSToSenderAndReceiver(senderAccount, receiverAccount, senderTransactionEntity, senderCustomer, receiverCustomer);
                    var apiRequest = MakeAccountingPosting(transfer.Amount, receiverAccount, senderAccount, senderTransactionEntity, senderAccount.AccountName, receiverAccount.AccountName, isInternalTransfer,accountingDate);
                    var result = await _mediator.Send(apiRequest);

                    msg = result.StatusCode == 200 ? "Transfer completed successfully." : "Transfer completed with an error in accounting posting.";
                    await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.OK, LogAction.Transfer, LogLevelInfo.Information);
                    return ServiceResponse<TransactionDto>.ReturnResultWith200(transactionDto, msg);
                }
                else
                {
                    // Step 16: Process transfer rejection and update status.
                    msg = "Transfer rejected.";
                    transfer.ApprovedByUserName = _userInfoToken.FullName;
                    transfer.DateOfApproval = BaseUtilities.UtcToDoualaTime(DateTime.Now);
                    transfer.ValidatorComment = request.Note;
                    transfer.Status = request.Status.ToString();
                    _transferRepository.Update(transfer);
                    await _uow.SaveAsync();

                    await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.Forbidden, LogAction.Transfer, LogLevelInfo.Warning);
                    return ServiceResponse<TransactionDto>.ReturnResultWith200(new TransactionDto(), "Operation completed.");
                }
            }
            catch (Exception e)
            {
                // Step 17: Log the error and return an internal server error response.
                var errorMessage = $"An error occurred during transfer processing: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.Transfer, LogLevelInfo.Error);
                return ServiceResponse<TransactionDto>.Return500("Transfer processing failed. Please contact support.");
            }
        }
        private async Task SendTransferSMSToSenderAndReceiver(Account senderAccount, Account receiverAccount, Transaction transaction, CustomerDto senderCustomer, CustomerDto receiverCustomer)
        {
            // Prepare sender SMS
            string senderMsg = $"Dear {senderCustomer.FirstName} {senderCustomer.LastName}, Your transfer of {BaseUtilities.FormatCurrency(transaction.OriginalDepositAmount)} to account number {BaseUtilities.PartiallyEncryptAccountNumber(receiverAccount.AccountNumber)} was successfully processed. Transaction Reference: {transaction.TransactionReference}. Date and Time: {transaction.CreatedDate}. A charge of {BaseUtilities.FormatCurrency(transaction.Fee)} was deducted from your account {BaseUtilities.PartiallyEncryptAccountNumber(senderAccount.AccountNumber)}. Your current balance is now {BaseUtilities.FormatCurrency(senderAccount.Balance)}. Thank you for banking with us.";
            if (senderCustomer.Language.ToLower() == "french")
            {
                senderMsg = $"Cher {senderCustomer.FirstName} {senderCustomer.LastName}, Votre transfert de {BaseUtilities.FormatCurrency(transaction.OriginalDepositAmount)} sur le compte numéro {BaseUtilities.PartiallyEncryptAccountNumber(receiverAccount.AccountNumber)} a été traité avec succès. Référence de transaction : {transaction.TransactionReference}. Date et heure : {transaction.CreatedDate}. Des frais de {BaseUtilities.FormatCurrency(transaction.Fee)} ont été déduits de votre compte {BaseUtilities.PartiallyEncryptAccountNumber(senderAccount.AccountNumber)}. Votre solde actuel est maintenant de {BaseUtilities.FormatCurrency(senderAccount.Balance)}. Merci de votre confiance en notre banque.";

            }

            // Prepare receiver SMS
            string receiverMsg = $"Dear {receiverCustomer.FirstName} {receiverCustomer.LastName}, You have received a transfer of {BaseUtilities.FormatCurrency(transaction.OriginalDepositAmount)} in to you account {BaseUtilities.PartiallyEncryptAccountNumber(receiverAccount.AccountNumber)} from account number {BaseUtilities.PartiallyEncryptAccountNumber(senderAccount.AccountNumber)}. Transaction Reference: {transaction.TransactionReference}. Date and Time: {transaction.CreatedDate}. Your current balance is now {BaseUtilities.FormatCurrency(receiverAccount.Balance)}. Thank you for banking with us.";
            if (receiverCustomer.Language.ToLower() == "french")
            {
                receiverMsg = $"Cher {receiverCustomer.FirstName} {receiverCustomer.LastName}, Vous avez reçu un virement de {BaseUtilities.FormatCurrency(transaction.OriginalDepositAmount)} sur votre compte {BaseUtilities.PartiallyEncryptAccountNumber(receiverAccount.AccountNumber)} en provenance du compte numéro {BaseUtilities.PartiallyEncryptAccountNumber(senderAccount.AccountNumber)}. Référence de transaction : {transaction.TransactionReference}. Date et heure : {transaction.CreatedDate}. Votre solde actuel est maintenant de {BaseUtilities.FormatCurrency(receiverAccount.Balance)}. Merci de votre confiance en notre banque.";

            }
            // Send SMS to sender
            var senderSmsCommand = new SendSMSPICallCommand
            {
                messageBody = senderMsg,
                recipient = senderCustomer.Phone
            };
            await _mediator.Send(senderSmsCommand);

            // Send SMS to receiver
            var receiverSmsCommand = new SendSMSPICallCommand
            {
                messageBody = receiverMsg,
                recipient = receiverCustomer.Phone
            };
            await _mediator.Send(receiverSmsCommand);
        }

        private AddTransferPostingCommand MakeAccountingPosting(
    decimal amount,
    Account receiverAccount,
    Account senderAccount,
    Transaction senderTransaction,
    string senderProductName,
    string receiverProductName,
    bool isInterBranch,
    DateTime accountingDate)
        {
            var addAccountingPostingCommand = new AddTransferPostingCommand
            {
                FromProductId = senderAccount.ProductId,
                ToProductId = receiverAccount.ProductId,
                FromMemberReference = senderAccount.CustomerId,
                ExternalBranchCode = receiverAccount.BranchCode,
                TransactionDate = accountingDate,
                TransactionReferenceId = senderTransaction.TransactionReference,
                ExternalBranchId = receiverAccount.BranchId,
                IsInterBranchTransaction = isInterBranch,
                AmountCollection = new List<AmountCollectionItem>()
            };

            // Add principal amount
            AddPrincipalAmount(addAccountingPostingCommand.AmountCollection, amount, isInterBranch, senderAccount, senderTransaction);

            if (isInterBranch)
            {
                AddInterBranchCommissions(addAccountingPostingCommand.AmountCollection, senderTransaction, senderAccount);
            }
            else
            {
                AddIntraBranchCommissions(addAccountingPostingCommand.AmountCollection, senderTransaction, senderAccount);
            }

            return addAccountingPostingCommand;
        }

        /// <summary>
        /// Adds the principal amount to the transaction collection.
        /// </summary>
        /// <param name="amountCollection">The collection to which the principal amount will be added.</param>
        /// <param name="amount">The principal amount of the transaction.</param>
        /// <param name="isInterBranch">Indicates if the transaction is inter-branch.</param>
        /// <param name="senderAccount">The sender's account details.</param>
        /// <param name="senderTransaction">The transaction details.</param>
        private void AddPrincipalAmount(
            List<AmountCollectionItem> amountCollection,
            decimal amount,
            bool isInterBranch,
            Account senderAccount,
            Transaction senderTransaction)
        {
            amountCollection.Add(new AmountCollectionItem(false, true, false)
            {
                Amount = amount,
                IsPrincipal = true,
                EventAttributeName = OperationEventRubbriqueName.Principal_Saving_Account.ToString(),
                LiaisonEventName = isInterBranch ? OperationEventRubbriqueName.Liasson_Account.ToString() : null,
                Naration = $"Principal Transfer: [Member: {senderAccount.CustomerName}, Reference: {senderAccount.CustomerId}, Amount: {BaseUtilities.FormatCurrency(amount)}, Transaction REF: {senderTransaction.TransactionReference}]"
            });
        }

        /// <summary>
        /// Adds inter-branch specific commissions to the transaction collection.
        /// </summary>
        /// <param name="amountCollection">The collection to which the commissions will be added.</param>
        /// <param name="senderTransaction">The transaction details.</param>
        /// <param name="senderAccount">The sender's account details.</param>
        private void AddInterBranchCommissions(
            List<AmountCollectionItem> amountCollection,
            Transaction senderTransaction,
            Account senderAccount)
        {
            amountCollection.AddRange(new[]
            {
        CreateAmountCollectionItem(senderTransaction.SourceBranchCommission, OperationEventRubbriqueName.Transfer_Fee_Account.ToString(), senderTransaction, senderAccount, "Source Branch Commission"),
        CreateAmountCollectionItem(senderTransaction.DestinationBranchCommission, SharingWithPartner.DestinationBranchCommission_Account.ToString(), senderTransaction, senderAccount, "Destination Branch Commission"),
        CreateAmountCollectionItem(senderTransaction.HeadOfficeCommission, OperationEventRubbriqueName.HeadOfficeShareTransferCommission.ToString(), senderTransaction, senderAccount, "Head Office Commission"),
        CreateAmountCollectionItem(senderTransaction.FluxAndPTMCommission, OperationEventRubbriqueName.FluxAndPTMShareTransferCommission.ToString(), senderTransaction, senderAccount, "Flux and PTM Commission"),
        CreateAmountCollectionItem(senderTransaction.CamCCULCommission, OperationEventRubbriqueName.CamCCULShareTransferCommission.ToString(), senderTransaction, senderAccount, "CamCCUL Commission")
    });
        }

        /// <summary>
        /// Adds intra-branch specific commissions to the transaction collection.
        /// </summary>
        /// <param name="amountCollection">The collection to which the commissions will be added.</param>
        /// <param name="senderTransaction">The transaction details.</param>
        /// <param name="senderAccount">The sender's account details.</param>
        private void AddIntraBranchCommissions(
            List<AmountCollectionItem> amountCollection,
            Transaction senderTransaction,
            Account senderAccount)
        {
            amountCollection.AddRange(new[]
            {
        CreateAmountCollectionItem(senderTransaction.Fee, OperationEventRubbriqueName.Transfer_Fee_Account.ToString(), senderTransaction, senderAccount, "Transfer Fee"),
        CreateAmountCollectionItem(senderTransaction.HeadOfficeCommission, OperationEventRubbriqueName.HeadOfficeShareTransferCommission.ToString(), senderTransaction, senderAccount, "Head Office Commission"),
        CreateAmountCollectionItem(senderTransaction.FluxAndPTMCommission, OperationEventRubbriqueName.FluxAndPTMShareTransferCommission.ToString(), senderTransaction, senderAccount, "Flux and PTM Commission"),
        CreateAmountCollectionItem(senderTransaction.CamCCULCommission, OperationEventRubbriqueName.CamCCULShareTransferCommission.ToString(), senderTransaction, senderAccount, "CamCCUL Commission")
    });
        }

        /// <summary>
        /// Creates an AmountCollectionItem instance with detailed narration.
        /// </summary>
        /// <param name="amount">The amount for the collection item.</param>
        /// <param name="eventAttributeName">The event attribute name for the collection item.</param>
        /// <param name="senderTransaction">The transaction details.</param>
        /// <param name="senderAccount">The sender's account details.</param>
        /// <param name="description">The description for the narration.</param>
        /// <returns>An initialized AmountCollectionItem.</returns>
        private AmountCollectionItem CreateAmountCollectionItem(
            decimal amount,
            string eventAttributeName,
            Transaction senderTransaction,
            Account senderAccount,
            string description)
        {
            return new AmountCollectionItem(false, true, false)
            {
                Amount = amount,
                IsPrincipal = false,
                EventAttributeName = eventAttributeName,
                LiaisonEventName = OperationEventRubbriqueName.Liasson_Account.ToString(),
                Naration = $"{description}: [Member: {senderAccount.CustomerName}, Reference: {senderAccount.CustomerId}, Amount: {BaseUtilities.FormatCurrency(amount)}, Transaction REF: {senderTransaction.TransactionReference}]"
            };
        }


        // Helper method to retrieve teller information
        private async Task<Teller> GetTeller(string tellerId)
        {
            return await _tellerRepository
                .FindBy(t => t.Id == tellerId && !t.IsPrimary)
                .FirstOrDefaultAsync();
        }

        // Helper method to retrieve teller account
        private async Task<Account> GetTellerAccount(string tellerId)
        {
            return await _AccountRepository
                .FindBy(t => t.TellerId == tellerId)
                .FirstOrDefaultAsync();
        }

        // Helper method to check if year and day are open

        // Helper method to retrieve sender account details
        private async Task<Account> GetSenderAccount(string accountNumber)
        {
            return await _AccountRepository
                .FindBy(a => a.AccountNumber == accountNumber)
                .Include(a => a.Product)
                    .ThenInclude(t => t.TransferParameters).FirstOrDefaultAsync();
        }

        private async Task<Account> GetReceiverAccount(string accountNumber)
        {
            return await _AccountRepository
                .FindBy(a => a.AccountNumber == accountNumber)
                .Include(a => a.Product)
                    .ThenInclude(t => t.TransferParameters)
                .FirstOrDefaultAsync();
        }


        // Helper method to verify sender account balance integrity
        private bool VerifySenderBalanceIntegrity(Account senderAccount)
        {
            string senderAccountBalance = senderAccount.Balance.ToString();
            return BalanceEncryption.VerifyBalanceIntegrity(senderAccountBalance, senderAccount.EncryptedBalance, senderAccount.AccountNumber);
        }

        // Helper method to get transfer limit based on source details
        private TransferParameter GetTransferLimit(IEnumerable<TransferParameter> transferLimits, string transferType)
        {
            return transferLimits.FirstOrDefault(w => w.TransferType == transferType);
        }

        // Helper method to check if there are enough funds for transfer
        private bool HasEnoughFundsForTransfer(Account account, TransferParameter transferLimit, decimal amount)

        {
            var closeFee = XAFWallet.CalculateCustomerCharges(
                transferLimit.TransferFeeRate,
                0, 0,
                amount);

            decimal totalAmountToTransferWithCharges = amount + TransferCharges(transferLimit, amount);

            return account.Balance - totalAmountToTransferWithCharges >= closeFee;
        }

        private decimal TransferCharges(TransferParameter transferLimit, decimal amount)
        {
            return XAFWallet.CalculateCustomerCharges(
                transferLimit?.TransferFeeRate ?? 0,
                0, 0,
                amount);
        }

        private decimal TotalAmountToTransferWithCharges(TransferParameter transferLimit, decimal amount)
        {
            return amount + TransferCharges(transferLimit, amount);
        }


        // Helper method to verify receiver account balance integrity
        private bool VerifyReceiverBalanceIntegrity(Account receiverAccount)
        {
            string receiverAccountBalance = receiverAccount.Balance.ToString();
            return BalanceEncryption.VerifyBalanceIntegrity(receiverAccountBalance, receiverAccount.EncryptedBalance, receiverAccount.AccountNumber);
        }

        // Helper method to check if the transfer amount lies within limits
        private bool IsTransferAmountWithinLimits(decimal amount, TransferParameter transferLimit)
        {
            return DecimalComparer.IsGreaterThanOrEqual(amount, transferLimit.MinAmount) && DecimalComparer.IsLessThanOrEqual(amount, transferLimit.MaxAmount);
        }

        // Helper method to create sender transaction entity
        private async Task<Transaction> CreateSenderTransactionEntity(TransferConfirmationCommand request, Account senderAccount, TransferParameter transferLimit, Account receiverAccount, decimal Charges, string currencyid, string sourceBranchId, string destinationBranchId, bool isInternalTransfer, string tellerId, Transfer transfer, DateTime accountingDate)
        {

            decimal totalAmounToTransfer = TotalAmountToTransferWithCharges(transferLimit, transfer.Amount);
            var senderTransactionEntity = new Transaction();
            senderTransactionEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            senderTransactionEntity.Id = BaseUtilities.GenerateUniqueNumber();
            senderTransactionEntity.TransactionReference = transfer.TransactionRef;
            senderTransactionEntity.TransactionType = transferLimit.TransferType;
            senderTransactionEntity.Operation = TransactionType.TRANSFER.ToString();
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
            senderTransactionEntity.OriginalDepositAmount = transfer.Amount;
            senderTransactionEntity.Fee = Charges;
            senderTransactionEntity.Tax = 0;
            senderTransactionEntity.AccountingDate = accountingDate;
            senderTransactionEntity.Credit = 0;
            senderTransactionEntity.Debit = (transfer.Amount + Charges);
            senderTransactionEntity.Amount = -(transfer.Amount + Charges);
            senderTransactionEntity.CustomerId = senderAccount.CustomerId;
            senderTransactionEntity.ReceiptTitle = "Cash Transfer Receipt Reference: " + transfer.TransactionRef;
            senderTransactionEntity.OperationType = OperationType.Debit.ToString();
            senderTransactionEntity.FeeType = Events.ChargeOfTransfer.ToString();
            senderTransactionEntity.TellerId = tellerId;
            senderTransactionEntity.SourceType = "BackOffice";
            senderTransactionEntity.SourceBrachId = sourceBranchId;
            senderTransactionEntity.IsInterBrachOperation = isInternalTransfer;
            senderTransactionEntity.DestinationBrachId = destinationBranchId;
            senderTransactionEntity.DestinationBranchCommission = isInternalTransfer ? XAFWallet.CalculateCommission(transferLimit.DestinationBranchOfficeShare, Charges) : 0;
            senderTransactionEntity.SourceBranchCommission = isInternalTransfer ? XAFWallet.CalculateCommission(transferLimit.SourceBrachOfficeShare, Charges) : Charges;
            senderTransactionEntity.Note ??= $"Account number {senderAccount.AccountNumber} made a transfer of {BaseUtilities.FormatCurrency(transfer.Amount)} to account number {receiverAccount.AccountNumber} with charge of {BaseUtilities.FormatCurrency(Charges)} making total of {BaseUtilities.FormatCurrency(totalAmounToTransfer)}, Reference: {senderTransactionEntity.TransactionReference}";
            _TransactionRepository.Add(senderTransactionEntity);
            return senderTransactionEntity;
        }

        // Helper method to update sender account properties
        private void UpdateSenderAccountProperties(Account senderAccount, Transaction senderTransactionEntity, TransferParameter transferLimit, decimal amount)
        {
            // Update Account entity properties with values from the request
            senderAccount.PreviousBalance = senderAccount.Balance;
            senderAccount.Balance -= TotalAmountToTransferWithCharges(transferLimit, amount);
            senderAccount.EncryptedBalance = BalanceEncryption.Encrypt(senderAccount.Balance.ToString(), senderAccount.AccountNumber);
            senderAccount.Product = null;
            // Use the repository to update the existing Account entity
            _AccountRepository.Update(senderAccount);
        }

        // Helper method to create receiver transaction entity
        private async Task<Transaction> CreateReceiverTransactionEntity(TransferConfirmationCommand request, Account receiverAccount, TransferParameter transferLimit, Account senderAccount, Transaction senderTransaction, Transfer transfer)
        {
            var receiverTransactionEntity = new Transaction();
            // Convert UTC to local time and set it in the entity
            receiverTransactionEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            receiverTransactionEntity.Id = BaseUtilities.GenerateUniqueNumber();
            receiverTransactionEntity.TransactionReference = senderTransaction.TransactionReference;
            receiverTransactionEntity.TransactionType = transferLimit.TransferType;
            receiverTransactionEntity.Operation = TransactionType.TRANSFER.ToString();
            receiverTransactionEntity.PreviousBalance = receiverAccount.Balance;
            receiverTransactionEntity.Balance = receiverAccount.Balance + transfer.Amount;
            receiverTransactionEntity.AccountId = receiverAccount.Id;
            receiverTransactionEntity.SenderAccountId = senderAccount.Id;
            receiverTransactionEntity.AccountNumber = receiverAccount.AccountNumber;
            receiverTransactionEntity.ReceiverAccountId = receiverAccount.Id;
            receiverTransactionEntity.ProductId = receiverAccount.ProductId;
            receiverTransactionEntity.Status = "COMPLETED";
            receiverTransactionEntity.Note ??= $"Account number {receiverAccount.AccountNumber} recieves a transfer of {BaseUtilities.FormatCurrency(transfer.Amount)} from account number {senderAccount.AccountNumber}, Reference: {senderTransaction.TransactionReference}";
            receiverTransactionEntity.BankId = receiverAccount.BankId;
            receiverTransactionEntity.BranchId = receiverAccount.BranchId;
            receiverTransactionEntity.OriginalDepositAmount = transfer.Amount;
            receiverTransactionEntity.Fee = 0;
            receiverTransactionEntity.Tax = 0;
            receiverTransactionEntity.AccountingDate = senderTransaction.AccountingDate;
            receiverTransactionEntity.Amount = transfer.Amount;
            receiverTransactionEntity.CustomerId = receiverAccount.CustomerId;
            //receiverTransactionEntity.CurrencyNotesId = senderTransaction.CurrencyNotesId;
            receiverTransactionEntity.OperationType = OperationType.Credit.ToString();
            receiverTransactionEntity.FeeType = Events.None.ToString();
            receiverTransactionEntity.TellerId = senderTransaction.TellerId;
            receiverTransactionEntity.Debit = 0;
            receiverTransactionEntity.Credit = transfer.Amount;
            receiverTransactionEntity.SourceType = senderTransaction.SourceType;
            receiverTransactionEntity.IsInterBrachOperation = senderTransaction.IsInterBrachOperation;
            receiverTransactionEntity.SourceBrachId = senderTransaction.BranchId;
            receiverTransactionEntity.DestinationBrachId = senderTransaction.DestinationBrachId;
            receiverTransactionEntity.DestinationBranchCommission = senderTransaction.DestinationBranchCommission;
            receiverTransactionEntity.SourceBranchCommission = senderTransaction.SourceBranchCommission;
            receiverTransactionEntity.ReceiptTitle = senderTransaction.ReceiptTitle;

            _TransactionRepository.Add(receiverTransactionEntity);
            return receiverTransactionEntity;
        }

        // Helper method to update receiver account properties
        private void UpdateReceiverAccountProperties(Account receiverAccount, Transaction receiverTransactionEntity, TransferConfirmationCommand request, decimal Amount)
        {
            // Update Account entity properties with values from the request
            receiverAccount.PreviousBalance = receiverAccount.Balance;
            receiverAccount.Balance += Amount;
            receiverAccount.EncryptedBalance = BalanceEncryption.Encrypt(receiverAccount.Balance.ToString(), receiverAccount.AccountNumber);
            receiverAccount.Product = null;
            // Use the repository to update the existing Account entity
            _AccountRepository.Update(receiverAccount);
        }

        // Helper method to log error and audit
        private async Task LogErrorAndAudit(string userEmail, string logAction, TransferConfirmationCommand request, string errorMessage, string logLevel, int statusCode, string userToken)
        {
            _logger.LogError(errorMessage);
            await APICallHelper.AuditLogger(userEmail, logAction, request, errorMessage, logLevel, statusCode, userToken);
        }

        private void CreateTellerOperation(decimal amount, OperationType operationType, Account tellerAccount, Transaction transactionEntityEntryFee, string eventType, string userID, DateTime accountingDate)
        {
            var tellerOperation = new TellerOperation
            {
                Id = BaseUtilities.GenerateUniqueNumber(),
                AccountID = tellerAccount.Id,
                AccountNumber = tellerAccount.AccountNumber,
                Amount = amount,
                Description = $"{transactionEntityEntryFee.TransactionType} of {BaseUtilities.FormatCurrency(amount)}, Account Number: {tellerAccount.AccountNumber}",
                OpenOfDayReference = tellerAccount.OpenningOfDayReference,
                OperationType = operationType.ToString(),
                BranchId = tellerAccount.BranchId,
                BankId = tellerAccount.BankId,
                CurrentBalance = tellerAccount.Balance,
                Date = accountingDate,
                AccountingDate = accountingDate,
                CustomerId = transactionEntityEntryFee.CustomerId,
                MemberAccountNumber = transactionEntityEntryFee.AccountNumber,
                ReferenceId = transactionEntityEntryFee.TransactionReference,
                PreviousBalance = tellerAccount.PreviousBalance,
                TellerID = tellerAccount.TellerId,
                TransactionReference = transactionEntityEntryFee.TransactionReference,
                TransactionType = transactionEntityEntryFee.TransactionType,
                UserID = userID,
                EventName = eventType, IsCashOperation=false,
                DestinationBrachId = transactionEntityEntryFee.DestinationBrachId,
                SourceBranchId = transactionEntityEntryFee.SourceBrachId,
                IsInterBranch = transactionEntityEntryFee.IsInterBrachOperation,
                DestinationBranchCommission = transactionEntityEntryFee.DestinationBranchCommission,
                SourceBranchCommission = transactionEntityEntryFee.SourceBranchCommission
            };
            _tellerOperationRepository.Add(tellerOperation);

        }
    }
}