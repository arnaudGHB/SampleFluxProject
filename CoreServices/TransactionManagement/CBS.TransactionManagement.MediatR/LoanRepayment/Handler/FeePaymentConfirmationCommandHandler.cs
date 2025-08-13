using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Command;
using Newtonsoft.Json;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.LoanRepayment.Command;
using CBS.TransactionManagement.Data.Dto.LoanRepayment;
using CBS.TransactionManagement.MediatR.LoanRepayment.Command;

namespace CBS.TransactionManagement.LoanRepayment.Handlers
{
    /// <summary>
    /// Handles the command to confirm loan fee payments.
    /// </summary>
    public class FeePaymentConfirmationCommandHandler : IRequestHandler<FeePaymentConfirmationCommand, ServiceResponse<List<FeePaymentConfirmationDto>>>
    {
        private readonly ILogger<FeePaymentConfirmationCommandHandler> _logger; // Logger for tracking errors and events.
        private readonly UserInfoToken _userInfoToken; // User authentication token.
        private readonly PathHelper _pathHelper; // API endpoint helper.

        /// <summary>
        /// Constructor for initializing the Fee Payment Confirmation Handler.
        /// </summary>
        /// <param name="userInfoToken">User authentication token.</param>
        /// <param name="logger">Logger instance for monitoring operations.</param>
        /// <param name="pathHelper">Helper class for API endpoints.</param>
        public FeePaymentConfirmationCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<FeePaymentConfirmationCommandHandler> logger,
            PathHelper pathHelper)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _pathHelper = pathHelper;
        }

        /// <summary>
        /// Handles loan fee payment confirmation by calling an external Loan API.
        /// </summary>
        /// <param name="request">Loan fee payment confirmation request containing payment details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Service response containing payment confirmation details.</returns>
        public async Task<ServiceResponse<List<FeePaymentConfirmationDto>>> Handle(FeePaymentConfirmationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Convert request data to JSON format for logging and API transmission.
                string requestData = JsonConvert.SerializeObject(request);
                _logger.LogInformation($"[INFO] Initiating Loan Fee Payment Confirmation process. Request Data: {requestData}");

                // Step 2: Call the Loan Fee Payment API.
                var apiResponse = await APICallHelper.LoanRepayment<ServiceResponse<List<FeePaymentConfirmationDto>>>(
                    _pathHelper,
                    _userInfoToken.Token,
                    requestData,
                    _pathHelper.LoanApplicationFeeURL
                );

                // Step 3: Check API response status.
                if (apiResponse.StatusCode == 200 && apiResponse.Data != null)
                {
                    string successMessage = $"[SUCCESS] Loan fee payment confirmation successful. Transaction Reference: {request.TransactionReference}, Loan Application ID: {request.LoanApplicationId}.";
                    _logger.LogInformation(successMessage);
                    await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.LoanFeePayment, LogLevelInfo.Information);

                    return ServiceResponse<List<FeePaymentConfirmationDto>>.ReturnResultWith200(apiResponse.Data, successMessage);
                }
                else
                {
                    string failureMessage = $"[ERROR] Loan Fee Payment API returned an unsuccessful status. Status Code: {apiResponse.StatusCode}, Response Message: {apiResponse.Message}.";
                    _logger.LogError(failureMessage);
                    await BaseUtilities.LogAndAuditAsync(failureMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.LoanFeePayment, LogLevelInfo.Error);

                    return ServiceResponse<List<FeePaymentConfirmationDto>>.Return500(failureMessage);
                }
            }
            catch (Exception e)
            {
                // Step 4: Log and audit API failure.
                string errorMessage = $"[CRITICAL ERROR] Loan Fee Payment Confirmation failed for Transaction Reference: {request.TransactionReference}. Exception: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.LoanFeePayment, LogLevelInfo.Error);

                return ServiceResponse<List<FeePaymentConfirmationDto>>.Return500(e, errorMessage);
            }
        }
    }

}
