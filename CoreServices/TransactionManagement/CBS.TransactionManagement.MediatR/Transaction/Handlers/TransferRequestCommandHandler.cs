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
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.InteropServices;
using CBS.TransactionManagement.MediatR;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Queries;
using System.Diagnostics;
using CBS.TransactionManagement.Data.Dto.TransferLimits;
using CBS.TransactionManagement.Data.Entity.TransferLimits;
using System.Globalization;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using DocumentFormat.OpenXml.Bibliography;
using CBS.TransactionManagement.Repository.VaultP;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to add a new Transfer Transaction.
    /// </summary>
    public class TransferRequestCommandHandler : IRequestHandler<TransferRequestCommand, ServiceResponse<TransferDto>>
    {
        private readonly IAccountRepository _AccountRepository;
        private readonly ITransferRepository _transferRepository;

        private readonly ICurrencyNotesRepository _CurrencyNotesRepository;
        private readonly ITransactionRepository _TransactionRepository; // Repository for accessing Transaction data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<TransferRequestCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly IDailyTellerRepository _tellerProvisioningAccountRepository;
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly ITellerRepository _tellerRepository;
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvioningHistoryRepository;
        private readonly IAccountingDayRepository _accountingDayRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfigRepository _configRepository;
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Constructor for initializing the TransferRequestCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Transaction data access.</param>
        /// <param name="TransactionRepository">Repository for Transaction data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public TransferRequestCommandHandler(
            IAccountRepository AccountRepository,
            ITransactionRepository TransactionRepository,
            IMapper mapper,

            ILogger<TransferRequestCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IDailyTellerRepository tellerProvisioningAccountRepository = null,
            ITellerOperationRepository tellerOperationRepository = null,
            ITellerRepository tellerRepository = null,
            UserInfoToken userInfoToken = null,
            IConfigRepository configRepository = null,
            IMediator mediator = null,
            ICurrencyNotesRepository currencyNotesRepository = null,
            ITransferRepository transferRepository = null,
            ISubTellerProvisioningHistoryRepository subTellerProvioningHistoryRepository = null,
            IAccountingDayRepository accountingDayRepository = null)
        {
            _AccountRepository = AccountRepository;
            _TransactionRepository = TransactionRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _tellerProvisioningAccountRepository = tellerProvisioningAccountRepository;
            _tellerOperationRepository = tellerOperationRepository;
            _tellerRepository = tellerRepository;
            _userInfoToken = userInfoToken;
            _configRepository = configRepository;
            _mediator = mediator;
            _CurrencyNotesRepository = currencyNotesRepository;
            _transferRepository = transferRepository;
            _subTellerProvioningHistoryRepository = subTellerProvioningHistoryRepository;
            _accountingDayRepository = accountingDayRepository;
        }

        /// <summary>
        /// Handles the TransferRequestCommand to add a new Transfer Transaction.
        /// </summary>
        /// <param name="request">The TransferRequestCommand containing Transaction data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>


        public async Task<ServiceResponse<TransferDto>> Handle(TransferRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Initialize variables and retrieve the current accounting date for the branch.
                string msg = string.Empty;
                var accountingDate = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);

                // 2. Define transfer type and check if the transfer is internal or inter-branch. Initialize it as a local transfer.
                bool isInternalTransfer = false;
                string transferType = TransferType.Local.ToString();

                // 3. Retrieve the teller information based on the specified operation type for validation.
                var teller = await _tellerRepository.GetTellerByType(TellerType.NoneCashTeller.ToString());

                // 4. Validate teller's rights for performing a transfer operation, ensuring operational permissions.
                await _tellerRepository.CheckTellerOperationalRights(teller, OperationType.Transfer.ToString(), false);

                // 5. Validate teller's transfer limits for the requested amount to ensure it does not exceed their authorized threshold.
                await _tellerRepository.ValidateTellerLimites(request.Amount, request.Amount, teller);

                // 6. Retrieve the sender's account details and verify the account's status.
                var senderAccount = await _AccountRepository.GetAccountByAccountNumber(request.SenderAccountNumber);

                // 7. Check if sender's account exists and is active; log a warning if either condition fails.
                if (senderAccount == null || !senderAccount.Status.Equals(AccountStatus.Active.ToString()))
                {
                    var errorMessage = senderAccount == null
                        ? $"Transaction initiation error: The customer account {request.SenderAccountNumber} was not found."
                        : $"Transfer initiation error: The account {request.SenderAccountNumber} is currently inactive.";

                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.Transfer, LogLevelInfo.Warning);
                    return ServiceResponse<TransferDto>.Return400(errorMessage);
                }

                // 8. Retrieve sender customer details by invoking a query command to get customer data.
                var getCustomerCommand = new GetCustomerCommandQuery { CustomerID = senderAccount.CustomerId };
                var serviceResponse = await _mediator.Send(getCustomerCommand);

                // 9. Confirm the sender's customer membership approval status to verify eligibility for transfers.
                if (serviceResponse.StatusCode == 200)
                {
                    var senderCustomer = serviceResponse.Data;
                    if (senderCustomer.MembershipApprovalStatus.ToLower() == AccountStatus.Approved.ToString().ToLower())
                    {
                        // 10. Validate sender account balance integrity for security and accuracy.
                        _AccountRepository.VerifyBalanceIntegrity(senderAccount);

                        // 11. Retrieve the receiver's account details to validate the recipient information.
                        var receiverAccount = await _AccountRepository.GetAccountByAccountNumber(request.ReceiverAccountNumber);

                        // 12. Check if receiver's account exists and retrieve customer details.
                        var getCustomerCommand1 = new GetCustomerCommandQuery { CustomerID = receiverAccount.CustomerId };
                        var receiverCustomerx = await _mediator.Send(getCustomerCommand1);

                        if (receiverCustomerx.StatusCode != 200)
                        {
                            msg = "Error retrieving receiver's customer details. Please verify customer information and try again.";
                            await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.Forbidden, LogAction.Transfer, LogLevelInfo.Warning);
                            return ServiceResponse<TransferDto>.Return403(msg);
                        }

                        var receiverCustomer = receiverCustomerx.Data;

                        // 13. Verify that the transfer is not between identical accounts, as this is not permitted.
                        if (senderAccount.AccountNumber == receiverAccount.AccountNumber)
                        {
                            msg = $"Transfer error: Transfers between the same account number (Sender: {senderAccount.AccountNumber}, Receiver: {receiverAccount.AccountNumber}) are not allowed.";
                            await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.Forbidden, LogAction.Transfer, LogLevelInfo.Warning);
                            return ServiceResponse<TransferDto>.Return403(msg);
                        }
                        var sourceBranch = new BranchDto();
                        var destinationBranch = new BranchDto();
                        sourceBranch = await GetBranch(senderAccount.BranchId);
                        // 14. Determine if the transfer is inter-branch, requiring specific handling and limits.
                        if (senderCustomer.BranchId != receiverCustomer.BranchId)
                        {
                            destinationBranch = await GetBranch(receiverAccount.BranchId); ;
                            isInternalTransfer = true;
                            transferType = TransferType.Inter_Branch.ToString();
                        }
                        else
                        {
                            destinationBranch = sourceBranch;
                        }
                        // 15. Retrieve applicable transfer limits based on the product type and transfer type.
                        var transferLimit = GetTransferLimit(senderAccount.Product.TransferParameters, transferType);

                        // 16. Generate a unique transaction reference number based on the branch type and transaction type.
                        string reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, TransactionType.TRANSFER.ToString(), isInternalTransfer);

                        // 17. Ensure that the transfer limits are properly configured; log and halt if missing.
                        if (transferLimit == null)
                        {
                            msg = "Transfer limit configuration is missing. Please configure limits before proceeding.";
                            await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.Forbidden, LogAction.Transfer, LogLevelInfo.Warning);
                            return ServiceResponse<TransferDto>.Return403(msg);
                        }

                        // 18. Check if the sender has sufficient funds for the transfer; log a warning if funds are insufficient.
                        if (!HasEnoughFundsForTransfer(senderAccount, transferLimit, request.Amount))
                        {
                            msg = $"Insufficient funds: The account does not have enough balance to cover the transfer amount of {request.Amount}.";
                            await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.Forbidden, LogAction.Transfer, LogLevelInfo.Warning);
                            return ServiceResponse<TransferDto>.Return403(msg);
                        }

                        // 19. Re-validate the balance integrity of the receiver account.
                        _AccountRepository.VerifyBalanceIntegrity(receiverAccount);

                        // 20. Validate that the transfer amount falls within the configured transfer limits.
                        if (!IsTransferAmountWithinLimits(request.Amount, transferLimit))
                        {
                            msg = $"Transfer amount validation error: The requested amount {request.Amount} is outside the permissible range ({transferLimit.MinAmount} to {transferLimit.MaxAmount}).";
                            await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.Forbidden, LogAction.Transfer, LogLevelInfo.Warning);
                            return ServiceResponse<TransferDto>.Return403(msg);
                        }

                        // 21. Calculate any applicable transfer charges based on the transfer amount and limit configurations.
                        var charges = TransferCharges(transferLimit, request.Amount);

                        // 22. Process and create the sender transaction entity with all relevant details.
                        var senderTransactionEntity = await CreateSenderTransactionEntity(request, senderAccount, transferLimit, receiverAccount, charges, string.Empty, senderCustomer.BranchId, receiverCustomer.BranchId, isInternalTransfer, teller.Id, reference, accountingDate);

                        // 23. Process and create the receiver transaction entity and persist the transfer.
                        var transfer = await CreateTransfer(request, receiverAccount, transferLimit, senderAccount, senderTransactionEntity, accountingDate, sourceBranch, destinationBranch);

                        // 24. Map the transfer data to the TransferDto object to return as the response.
                        var transfterDto = _mapper.Map<TransferDto>(transfer);

                        // 25. Commit changes to the database to finalize the transfer.
                        await _uow.SaveAsync();

                        // 26. Send SMS notifications to both sender and receiver upon successful transaction.
                        await SendTransferSMSToSenderAndReceiver(senderAccount, receiverAccount, senderTransactionEntity, senderCustomer, receiverCustomer);

                        msg = "Transfer successfully initiated.";
                        await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.OK, LogAction.Transfer, LogLevelInfo.Information);

                        // 27. Return a 200 OK response with the transfer details.
                        return ServiceResponse<TransferDto>.ReturnResultWith200(transfterDto, msg);
                    }

                    // 28. Log and respond if the sender's customer membership status is not approved.
                    msg = $"Membership approval error: Customer's membership is not approved. Current status: {senderCustomer.MembershipApprovalStatus}.";
                    await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.Forbidden, LogAction.Transfer, LogLevelInfo.Warning);
                    return ServiceResponse<TransferDto>.Return403(msg);
                }

                // 29. Log and return an error if unable to retrieve customer details.
                return ServiceResponse<TransferDto>.Return403(serviceResponse.Message);

            }
            catch (Exception e)
            {
                // 30. Log an error with details of the exception and return a 500 Internal Server Error response.
                var errorMessage = $"An error occurred during transfer processing: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.Transfer, LogLevelInfo.Error);
                return ServiceResponse<TransferDto>.Return500(e);
            }
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
        private async Task SendTransferSMSToSenderAndReceiver(Account senderAccount, Account receiverAccount, Transaction transaction, CustomerDto senderCustomer, CustomerDto receiverCustomer)
        {
            // Prepare sender SMS
            string senderMsg = $"Dear {senderCustomer.FirstName} {senderCustomer.LastName}, You have initiated a transfer of {BaseUtilities.FormatCurrency(transaction.OriginalDepositAmount)} to account number {BaseUtilities.PartiallyEncryptAccountNumber(receiverAccount.AccountNumber)}. This operation will be completed 24HRS. Transaction Reference: {transaction.TransactionReference}. Date and Time: {transaction.CreatedDate}. This operation will incure a charge of {BaseUtilities.FormatCurrency(transaction.Fee)} which shall be deducted from your account {BaseUtilities.PartiallyEncryptAccountNumber(senderAccount.AccountNumber)}\nThank you for banking with us.";
            if (senderCustomer.Language.ToLower() == "french")
            {
                senderMsg = $"Cher {senderCustomer.FirstName} {senderCustomer.LastName}, Vous avez initié un transfert de {BaseUtilities.FormatCurrency(transaction.OriginalDepositAmount)} vers le compte numéro {BaseUtilities.PartiallyEncryptAccountNumber(receiverAccount.AccountNumber)}. Cette opération sera complétée dans les 24 heures. Référence de transaction : {transaction.TransactionReference}. Date et heure : {transaction.CreatedDate}. Cette opération entraînera des frais de {BaseUtilities.FormatCurrency(transaction.Fee)} qui seront déduits de votre compte {BaseUtilities.PartiallyEncryptAccountNumber(senderAccount.AccountNumber)}\nMerci de votre confiance en notre banque.";

            }


            // Send SMS to sender
            var senderSmsCommand = new SendSMSPICallCommand
            {
                messageBody = senderMsg,
                recipient = senderCustomer.Phone
            };
            await _mediator.Send(senderSmsCommand);


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
0, 0, amount);
        }

        private decimal TotalAmountToTransferWithCharges(TransferParameter transferLimit, decimal amount)
        {
            return amount + TransferCharges(transferLimit, amount);
        }


       

        // Helper method to check if the transfer amount lies within limits
        private bool IsTransferAmountWithinLimits(decimal amount, TransferParameter transferLimit)
        {
            return DecimalComparer.IsGreaterThanOrEqual(amount, transferLimit.MinAmount) && DecimalComparer.IsLessThanOrEqual(amount, transferLimit.MaxAmount);
        }

        // Helper method to create sender transaction entity
        private async Task<Transaction> CreateSenderTransactionEntity(TransferRequestCommand request, Account senderAccount, TransferParameter transferLimit, Account receiverAccount, decimal Charges, string currencyid, string sourceBranchId, string destinationBranchId, bool isInternalTransfer, string tellerId, string TransactionReference, DateTime accountingDate)
        {
            decimal totalAmounToTransfer = TotalAmountToTransferWithCharges(transferLimit, request.Amount);
            var senderTransactionEntity = new Transaction();
            senderTransactionEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            senderTransactionEntity.Id = BaseUtilities.GenerateUniqueNumber();
            senderTransactionEntity.TransactionReference = TransactionReference;
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
            senderTransactionEntity.OriginalDepositAmount = request.Amount;
            senderTransactionEntity.Fee = Charges;
            senderTransactionEntity.Tax = 0;
            senderTransactionEntity.Credit = 0;
            senderTransactionEntity.AccountingDate = accountingDate;
            senderTransactionEntity.Debit = (request.Amount + Charges);
            senderTransactionEntity.Amount = -(request.Amount + Charges);
            //senderTransactionEntity.CurrencyNotesId = currencyid;
            senderTransactionEntity.OperationType = OperationType.Debit.ToString();
            senderTransactionEntity.FeeType = Events.ChargeOfTransfer.ToString();
            senderTransactionEntity.TellerId = tellerId;
            senderTransactionEntity.SourceType = "BackOffice";
            senderTransactionEntity.SourceBrachId = sourceBranchId;
            senderTransactionEntity.IsInterBrachOperation = isInternalTransfer;
            senderTransactionEntity.DestinationBrachId = destinationBranchId;
            senderTransactionEntity.DestinationBranchCommission = isInternalTransfer ? XAFWallet.CalculateCommission(transferLimit.DestinationBranchOfficeShare, Charges) : 0;
            senderTransactionEntity.SourceBranchCommission = isInternalTransfer ? XAFWallet.CalculateCommission(transferLimit.SourceBrachOfficeShare, Charges) : Charges;
            senderTransactionEntity.Note ??= $"{TransactionType.TRANSFER.ToString()} the sum of {BaseUtilities.FormatCurrency(request.Amount)} to {receiverAccount.AccountNumber}";
            senderTransactionEntity.ReceiptTitle = "Cash Transfer Receipt Reference: " + TransactionReference;

            //_TransactionRepository.Add(senderTransactionEntity);
            return senderTransactionEntity;
        }

       
        private async Task<Transfer> CreateTransfer(TransferRequestCommand request, Account receiverAccount, TransferParameter transferLimit, Account senderAccount, Transaction senderTransaction, DateTime accountingDate, BranchDto SourceBranch, BranchDto DestinationBranch)
        {
            var transfer = new Transfer();
            transfer.DateOfInitiation = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            transfer.Id = BaseUtilities.GenerateUniqueNumber();
            transfer.TransactionRef = senderTransaction.TransactionReference;
            transfer.TransactionType = transferLimit.TransferType;
            transfer.AccountId = receiverAccount.Id;
            transfer.SourceAccountNumber = senderAccount.AccountNumber;
            transfer.DestinationAccountNumber = receiverAccount.AccountNumber;
            transfer.SourceAccountType = senderAccount.AccountType;
            transfer.DestinationAccountType = receiverAccount.AccountType;
            transfer.Status = Status.Pending.ToString();
            transfer.InitiatorComment = $"Request to {TransactionType.TRANSFER.ToString()} the sum of {BaseUtilities.FormatCurrency(request.Amount)} from my account number {senderAccount.AccountNumber} to {receiverAccount.AccountNumber}";
            transfer.BranchId = _userInfoToken.BranchID;
            transfer.Charges = senderTransaction.Fee;
            transfer.Tax = senderTransaction.Tax;
            transfer.AccountingDate = accountingDate;
            transfer.Amount = request.Amount;
            transfer.InitiatedByUSerName = _userInfoToken.FullName;
            transfer.TellerId = senderTransaction.TellerId;
            transfer.SourceType = senderTransaction.SourceType;
            transfer.IsInterBranchOperation = senderTransaction.IsInterBrachOperation;
            transfer.SourceBrachId = senderAccount.BranchId;
            transfer.DestinationBrachId = receiverAccount.BranchId;
            transfer.DestinationCommision = senderTransaction.DestinationBranchCommission;
            transfer.SourceCommision = senderTransaction.SourceBranchCommission;
            transfer.SourceAccountName = senderAccount.AccountName;
            transfer.SenderName = senderAccount.CustomerName;
            transfer.SourceBranchName = SourceBranch.name;
            transfer.DestinationAccountName = receiverAccount.AccountName;
            transfer.RecieverName = receiverAccount.CustomerName;
            transfer.DestinationBranchName = DestinationBranch.name;
            _transferRepository.Add(transfer);
            return transfer;
        }

        // Helper method to create receiver transaction entity
        private async Task<Transaction> CreateReceiverTransactionEntity(TransferRequestCommand request, Account receiverAccount, TransferParameter transferLimit, Account senderAccount, Transaction senderTransaction)
        {
            var receiverTransactionEntity = new Transaction();
            // Convert UTC to local time and set it in the entity
            receiverTransactionEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            receiverTransactionEntity.Id = BaseUtilities.GenerateUniqueNumber();
            receiverTransactionEntity.TransactionReference = senderTransaction.TransactionReference;
            receiverTransactionEntity.TransactionType = transferLimit.TransferType;
            receiverTransactionEntity.Operation = TransactionType.TRANSFER.ToString();
            receiverTransactionEntity.PreviousBalance = receiverAccount.Balance;
            receiverTransactionEntity.Balance = receiverAccount.Balance + request.Amount;
            receiverTransactionEntity.AccountId = receiverAccount.Id;
            receiverTransactionEntity.SenderAccountId = senderAccount.Id;
            receiverTransactionEntity.AccountNumber = receiverAccount.AccountNumber;
            receiverTransactionEntity.ReceiverAccountId = receiverAccount.Id;
            receiverTransactionEntity.ProductId = receiverAccount.ProductId;
            receiverTransactionEntity.Status = "COMPLETED";
            receiverTransactionEntity.Note ??= $"Recieves a transfer of {BaseUtilities.FormatCurrency(request.Amount)} from Account No: {senderAccount.AccountNumber}";
            receiverTransactionEntity.BankId = receiverAccount.BankId;
            receiverTransactionEntity.BranchId = receiverAccount.BranchId;
            receiverTransactionEntity.OriginalDepositAmount = request.Amount;
            receiverTransactionEntity.Fee = 0;
            receiverTransactionEntity.Tax = 0;
            receiverTransactionEntity.Amount = request.Amount;
            //receiverTransactionEntity.CurrencyNotesId = senderTransaction.CurrencyNotesId;
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
            //_TransactionRepository.Add(receiverTransactionEntity);
            return receiverTransactionEntity;
        }

       

    }
}