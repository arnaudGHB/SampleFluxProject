using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.ThirtPartyPayment.Queries;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Repository.AccountServices;
using CBS.TransactionManagement.Repository.WithdrawalNotificationP;

namespace CBS.TransactionManagement.ThirtPartyPayment.Handlers
{
    /// <summary>
    /// Handles the retrieval of transfer charges based on the GetTransferChargeQuery.
    /// </summary>
    public class GetTransferChargeQueryHandler : IRequestHandler<GetTransferChargeQuery, ServiceResponse<TransferChargesDto>>
    {
        // Dependencies for database access, logging, and account/transaction operations.
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTransferChargeQueryHandler> _logger;
        private readonly IAccountRepository _accountRepository;
        private readonly IWithdrawalNotificationRepository _withdrawalNotificationRepository;
        private readonly ISavingProductRepository _savingProductRepository;

        /// <summary>
        /// Constructor for initializing the GetTransferChargeQueryHandler with dependencies.
        /// </summary>
        public GetTransferChargeQueryHandler(
            ITransactionRepository transactionRepository,
            IMapper mapper,
            ILogger<GetTransferChargeQueryHandler> logger,
            IAccountRepository accountRepository = null,
            IWithdrawalNotificationRepository withdrawalNotificationRepository = null,
            ISavingProductRepository savingProductRepository = null)
        {
            _transactionRepository = transactionRepository; // Handles transaction-related operations.
            _mapper = mapper; // For mapping objects to DTOs.
            _logger = logger; // Logs actions and errors.
            _accountRepository = accountRepository; // Handles account-related operations.
            _withdrawalNotificationRepository = withdrawalNotificationRepository; // Retrieves withdrawal notifications.
            _savingProductRepository = savingProductRepository; // Manages saving product details.
        }

        /// <summary>
        /// Handles the GetTransferChargeQuery to retrieve transfer charges.
        /// </summary>
        public async Task<ServiceResponse<TransferChargesDto>> Handle(GetTransferChargeQuery request, CancellationToken cancellationToken)
        {
            var logAction = LogAction.CalculateTransferCharges; // Define the log action type.
            try
            {
                // Log the start of the operation with request details.
                _logger.LogInformation("Calculating transfer charges for Sender: {SenderAccountNumber}, Receiver: {ReceiverAccountNumber}, Amount: {Amount}",
                    request.SenderAccountNumber, request.ReceiverAccountNumber, request.Amount);

                // Step 1: Validate and retrieve the sender account details.
                var sourceAccount = await _accountRepository.GetAccountByAccountNumber(request.SenderAccountNumber);
                if (sourceAccount == null)
                {
                    var message = $"Account not found for Account Number: {request.SenderAccountNumber}";
                    _logger.LogWarning(message);
                    return ServiceResponse<TransferChargesDto>.Return404(message);
                }

                // Step 2: Ensure the sender account has valid transfer parameters configured.
                var sourceTransferParameters = sourceAccount.Product?.TransferParameters?.FirstOrDefault();
                if (sourceTransferParameters == null)
                {
                    var message = $"Transfer parameters not configured for Account Number: {request.SenderAccountNumber}";
                    _logger.LogWarning(message);
                    return ServiceResponse<TransferChargesDto>.Return404(message);
                }

                // Step 3: Validate and retrieve the receiver account details.
                var destinationAccount = await _accountRepository.GetAccountByAccountNumber(request.ReceiverAccountNumber);
                if (destinationAccount == null)
                {
                    var message = $"Account not found for Account Number: {request.ReceiverAccountNumber}";
                    _logger.LogWarning(message);
                    return ServiceResponse<TransferChargesDto>.Return404(message);
                }

                // Step 4: Ensure the receiver account has valid transfer parameters configured.
                var destinationTransferParameters = destinationAccount.Product?.TransferParameters?.FirstOrDefault();
                if (destinationTransferParameters == null)
                {
                    var message = $"Transfer parameters not configured for Account Number: {request.ReceiverAccountNumber}";
                    _logger.LogWarning(message);
                    return ServiceResponse<TransferChargesDto>.Return404(message);
                }

                // Step 5: Handle charges for saving accounts with potential withdrawal notifications.
                if (sourceAccount.AccountType == AccountType.Saving.ToString())
                {
                    bool hasWithdrawalNotification = false;

                    // Check for existing withdrawal notifications.
                    var withdrawalNotification = await _withdrawalNotificationRepository.GetWithdrawalNotification(
                        sourceAccount.CustomerId, sourceAccount.AccountNumber, request.Amount);
                    if (withdrawalNotification != null)
                    {
                        hasWithdrawalNotification = true;
                    }

                    // Fetch withdrawal form charges specific to the product.
                    var withdrawalFormCharge = sourceAccount.Product.WithdrawalFormSavingFormFeeFor3PP;

                    // Determine the transfer type and calculate the charges.
                    var transferType = _accountRepository.DetermineTransferType(sourceAccount, destinationAccount);
                    var operationFeeType = GetFeeOperationType(request.FeeOperationType);
                    var charges = await _accountRepository.CalculateTransferCharges(
                        request.Amount, sourceAccount.ProductId, operationFeeType, sourceAccount.BranchId, transferType.TransferType,
                        true, hasWithdrawalNotification, withdrawalFormCharge);

                    // Construct the transfer charges DTO.
                    var transferCharges = new TransferChargesDto
                    {
                        ServiceCharge = charges.ServiceCharge,
                        TotalCharges = charges.TotalCharges + request.Amount,
                        FormChargeCharges = charges.TransfeFormCharge
                    };

                    // Log and audit the successful charge calculation.
                    var successMessage = $"Transfer charges calculated successfully for Sender: {request.SenderAccountNumber}, Receiver: {request.ReceiverAccountNumber}.";
                    await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, logAction, LogLevelInfo.Information);

                    return ServiceResponse<TransferChargesDto>.ReturnResultWith200(transferCharges);
                }
                else
                {
                    // For non-saving accounts, calculate charges similarly.
                    var transferType = _accountRepository.DetermineTransferType(sourceAccount, destinationAccount);
                    var operationFeeType = GetFeeOperationType(request.FeeOperationType);
                    var charges = await _accountRepository.CalculateTransferCharges(
                        request.Amount, sourceAccount.ProductId, operationFeeType, sourceAccount.BranchId, transferType.TransferType);

                    // Construct the transfer charges DTO.
                    var transferCharges = new TransferChargesDto
                    {
                        ServiceCharge = charges.ServiceCharge,
                        TotalCharges = charges.TotalCharges + request.Amount + charges.TransfeFormCharge,
                        FormChargeCharges = charges.TransfeFormCharge
                    };

                    // Log and audit the successful charge calculation.
                    var successMessage = $"Transfer charges calculated successfully for Sender: {request.SenderAccountNumber}, Receiver: {request.ReceiverAccountNumber}.";
                    await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, logAction, LogLevelInfo.Information);

                    return ServiceResponse<TransferChargesDto>.ReturnResultWith200(transferCharges);
                }
            }
            catch (Exception e)
            {
                // Log and audit errors during the calculation process.
                var errorMessage = $"Failed to calculate transfer charges for Sender: {request.SenderAccountNumber}. Error: {e.Message}";
                _logger.LogError(e, errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, logAction, LogLevelInfo.Error);
                return ServiceResponse<TransferChargesDto>.Return500(e, errorMessage);
            }
        }

        /// <summary>
        /// Validates and retrieves the FeeOperationType from a string.
        /// </summary>
        public FeeOperationType GetFeeOperationType(string feeOperationTypeStr)
        {
            if (string.IsNullOrEmpty(feeOperationTypeStr))
                throw new ArgumentException("The fee operation type cannot be null or empty.");

            if (Enum.TryParse(feeOperationTypeStr, true, out FeeOperationType result))
                return result;

            throw new ArgumentException($"Invalid FeeOperationType: {feeOperationTypeStr}");
        }
    }
}
