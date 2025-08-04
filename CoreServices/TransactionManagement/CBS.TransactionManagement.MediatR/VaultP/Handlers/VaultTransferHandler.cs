using CBS.TransactionManagement.Data.Dto.OTP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.OTP.Commands;
using CBS.TransactionManagement.Repository.VaultP;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.TransactionManagement.MediatR.VaultP
{
    /// <summary>
    /// Handles Vault Transfer operations between two vaults.
    /// </summary>
    public class VaultTransferHandler : IRequestHandler<VaultTranferCommand, ServiceResponse<bool>>
    {
        private readonly IVaultRepository _vaultRepository; // Repository for performing vault operations.
        private readonly ILogger<VaultTransferHandler> _logger; // Logger for recording operational details.

        /// <summary>
        /// Initializes a new instance of the <see cref="VaultTransferHandler"/> class.
        /// </summary>
        /// <param name="vaultRepository">The vault repository for performing operations.</param>
        /// <param name="logger">Logger for recording operation details.</param>
        public VaultTransferHandler(IVaultRepository vaultRepository, ILogger<VaultTransferHandler> logger)
        {
            _vaultRepository = vaultRepository; // Dependency injection of the vault repository.
            _logger = logger; // Dependency injection of the logger.
        }

        /// <summary>
        /// Handles the VaultTransferCommand by transferring cash between two vaults.
        /// </summary>
        /// <param name="request">The command containing the transfer details.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A response indicating the success or failure of the operation.</returns>
        public async Task<ServiceResponse<bool>> Handle(VaultTranferCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate input parameters
                if (string.IsNullOrWhiteSpace(request.FromBranchId) || string.IsNullOrWhiteSpace(request.ToBranchId))
                {
                    // Check if the source or destination branch IDs are missing.
                    const string error = "Both FromBranchId and ToBranchId must be specified.";
                    _logger.LogError(error); // Log the error.

                    // Log and audit the error.
                    await BaseUtilities.LogAndAuditAsync(
                        error,
                        request,
                        HttpStatusCodeEnum.BadRequest,
                        LogAction.VaultTransfer,
                        LogLevelInfo.Warning
                    );

                    // Return a 400 Bad Request response.
                    return ServiceResponse<bool>.Return400(error);
                }

                if (request.Amount <= 0)
                {
                    // Check if the transfer amount is valid (greater than zero).
                    const string error = "Transfer amount must be greater than zero.";
                    _logger.LogError(error); // Log the error.

                    // Log and audit the error.
                    await BaseUtilities.LogAndAuditAsync(
                        error,
                        request,
                        HttpStatusCodeEnum.BadRequest,
                        LogAction.VaultTransfer,
                        LogLevelInfo.Warning
                    );

                    // Return a 400 Bad Request response.
                    return ServiceResponse<bool>.Return403(error);
                }

                if (request.CurrencyNotesRequest == null)
                {
                    // Check if the currency notes details are provided.
                    const string error = "CurrencyNotesRequest must be provided.";
                    _logger.LogError(error); // Log the error.

                    // Log and audit the error.
                    await BaseUtilities.LogAndAuditAsync(
                        error,
                        request,
                        HttpStatusCodeEnum.BadRequest,
                        LogAction.VaultTransfer,
                        LogLevelInfo.Warning
                    );

                    // Return a 400 Bad Request response.
                    return ServiceResponse<bool>.Return403(error);
                }

                // Log the initiation of the transfer
                string initiationMessage = $"Vault transfer initiated from Branch {request.FromBranchId} to Branch {request.ToBranchId} with Reference {request.Reference}." +
                                           $"Amount: {BaseUtilities.FormatCurrency(request.Amount)}.";
                _logger.LogInformation(initiationMessage); // Log the transfer initiation.

                // Log and audit the initiation message.
                await BaseUtilities.LogAndAuditAsync(
                    initiationMessage,
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.VaultTransfer,
                    LogLevelInfo.Information
                );

                // Perform the transfer operation
                _vaultRepository.TransferCash(
                    request.FromBranchId, // Source branch ID.
                    request.ToBranchId,   // Destination branch ID.
                    request.Amount,       // Transfer amount.
                    request.CurrencyNotesRequest, // Currency notes details.
                    request.Reference, TransactionType.TRANSFER.ToString()    // Unique reference for the transfer.
                , false);

                // Log the successful completion of the transfer
                string successMessage = $"Vault transfer successfully completed from Branch {request.FromBranchId} to Branch {request.ToBranchId}. " +
                                        $"Amount: {BaseUtilities.FormatCurrency(request.Amount)}, Reference: {request.Reference}.";
                _logger.LogInformation(successMessage); // Log the success message.

                // Log and audit the success message.
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.VaultTransfer,
                    LogLevelInfo.Information
                );

                // Return a 200 OK response with the success message.
                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception ex)
            {
                // Handle and log unexpected errors
                string errorMessage = $"An unexpected error occurred during the vault transfer. " +
                                      $"From Branch: {request.FromBranchId}, To Branch: {request.ToBranchId}, Reference: {request.Reference}. Error: {ex.Message}.";
                _logger.LogError(ex, errorMessage); // Log the exception and error details.

                // Log and audit the error message.
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.VaultTransfer,
                    LogLevelInfo.Error
                );

                // Return a 500 Internal Server Error response.
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }
    }
}
