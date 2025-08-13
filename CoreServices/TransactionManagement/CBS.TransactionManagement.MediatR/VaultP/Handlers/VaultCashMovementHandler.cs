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
    /// Handles Vault Cash Movement operations such as CashIn, CashOut, and Transfers.
    /// </summary>
    public class VaultCashMovementHandler : IRequestHandler<VaultCashMovementCommand, ServiceResponse<bool>>
    {
        private readonly IVaultRepository _vaultRepository;
        private readonly ILogger<VaultCashMovementHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="VaultCashMovementHandler"/> class.
        /// </summary>
        /// <param name="vaultRepository">The vault repository for performing operations.</param>
        /// <param name="logger">Logger for recording operation details.</param>
        public VaultCashMovementHandler(IVaultRepository vaultRepository, ILogger<VaultCashMovementHandler> logger)
        {
            _vaultRepository = vaultRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handles the VaultCashMovementCommand.
        /// </summary>
        /// <param name="request">The command containing the cash movement details.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A response indicating the success or failure of the operation.</returns>
        public async Task<ServiceResponse<bool>> Handle(VaultCashMovementCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate the operation type
                if (string.IsNullOrEmpty(request.OperationType))
                {
                    const string error = "OperationType is required and must be specified as either 'CashIn' or 'CashOut'.";
                    _logger.LogError(error);

                    await BaseUtilities.LogAndAuditAsync(
                        error,
                        request,
                        HttpStatusCodeEnum.BadRequest,
                        LogAction.VaultCashMovement,
                        LogLevelInfo.Warning
                    );

                    return ServiceResponse<bool>.Return400(error);
                }

                // Handle CashIn operation
                if (request.OperationType.Equals("CashIn", StringComparison.OrdinalIgnoreCase))
                {
                    _vaultRepository.CashInByDenomination(request.Amount, request.CurrencyNotesRequest, request.BranchId,request.Reference,TransactionType.CASH_IN.ToString());

                    string successMessage = $"Successfully performed CashIn operation for Branch ID {request.BranchId}. " +
                                            $"Amount: {BaseUtilities.FormatCurrency(request.Amount)}.";
                    _logger.LogInformation(successMessage);

                    await BaseUtilities.LogAndAuditAsync(
                        successMessage,
                        request,
                        HttpStatusCodeEnum.OK,
                        LogAction.VaultCashMovement,
                        LogLevelInfo.Information
                    );

                    return ServiceResponse<bool>.ReturnResultWith200(true, "CashIn operation completed successfully.");
                }

                // Handle CashOut operation
                if (request.OperationType.Equals("CashOut", StringComparison.OrdinalIgnoreCase))
                {
                    bool result = await _vaultRepository.CashOutByDenominationAsync(request.Amount, request.CurrencyNotesRequest, request.BranchId, request.Reference, TransactionType.CASH_WITHDRAWAL.ToString(),false);
                    if (result)
                    {
                        string successMessage = $"Successfully performed CashOut operation for Branch ID {request.BranchId}. " +
                                                $"Amount: {BaseUtilities.FormatCurrency(request.Amount)}.";
                        _logger.LogInformation(successMessage);

                        await BaseUtilities.LogAndAuditAsync(
                            successMessage,
                            request,
                            HttpStatusCodeEnum.OK,
                            LogAction.VaultCashMovement,
                            LogLevelInfo.Information
                        );

                        return ServiceResponse<bool>.ReturnResultWith200(true, "CashOut operation completed successfully.");
                    }
                    else
                    {
                        string insufficientDenominationError = "Failed to complete CashOut operation due to insufficient denominations. " +
                                                               $"Branch ID: {request.BranchId}, Amount Requested: {BaseUtilities.FormatCurrency(request.Amount)}.";
                        _logger.LogError(insufficientDenominationError);

                        await BaseUtilities.LogAndAuditAsync(
                            insufficientDenominationError,
                            request,
                            HttpStatusCodeEnum.BadRequest,
                            LogAction.VaultCashMovement,
                            LogLevelInfo.Warning
                        );

                        return ServiceResponse<bool>.Return400(insufficientDenominationError);
                    }
                }

                // Handle invalid operation type
                string invalidOperationError = $"Invalid OperationType '{request.OperationType}'. Must be either 'CashIn' or 'CashOut'.";
                _logger.LogError(invalidOperationError);

                await BaseUtilities.LogAndAuditAsync(
                    invalidOperationError,
                    request,
                    HttpStatusCodeEnum.BadRequest,
                    LogAction.VaultCashMovement,
                    LogLevelInfo.Warning
                );

                return ServiceResponse<bool>.Return400(invalidOperationError);
            }
            catch (Exception ex)
            {
                // Log and handle unexpected errors
                string unexpectedErrorMessage = $"An unexpected error occurred during VaultCashMovement operation. Branch ID: {request.BranchId}. Error: {ex.Message}.";
                _logger.LogError(ex, unexpectedErrorMessage);

                await BaseUtilities.LogAndAuditAsync(
                    unexpectedErrorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.VaultCashMovement,
                    LogLevelInfo.Error
                );

                return ServiceResponse<bool>.Return500(unexpectedErrorMessage);
            }
        }
    }
}
