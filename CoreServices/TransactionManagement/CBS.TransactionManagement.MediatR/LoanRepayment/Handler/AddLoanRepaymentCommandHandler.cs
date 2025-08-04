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
    /// Handles the command to process a Loan Repayment event.
    /// </summary>
    public class AddLoanRepaymentCommandHandler : IRequestHandler<AddRepaymentCommand, ServiceResponse<RefundDto>>
    {
        private readonly ILogger<AddLoanRepaymentCommandHandler> _logger; // Logger for tracking and auditing.
        private readonly UserInfoToken _userInfoToken; // User authentication token.
        private readonly PathHelper _pathHelper; // Helper class for API endpoint references.

        /// <summary>
        /// Constructor for initializing the Loan Repayment Handler.
        /// </summary>
        /// <param name="userInfoToken">User information token for authorization.</param>
        /// <param name="logger">Logger instance for monitoring operations.</param>
        /// <param name="pathHelper">Helper class for API endpoints.</param>
        public AddLoanRepaymentCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<AddLoanRepaymentCommandHandler> logger,
            PathHelper pathHelper)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _pathHelper = pathHelper;
        }

        /// <summary>
        /// Handles the Loan Repayment process by calling the Loan Repayment API.
        /// </summary>
        /// <param name="request">Loan repayment request containing payment details.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>Service response containing refund details if successful, or an error response.</returns>
        public async Task<ServiceResponse<RefundDto>> Handle(AddRepaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Convert request data to JSON format for logging and API transmission.
                string requestData = JsonConvert.SerializeObject(request);
                _logger.LogInformation($"[INFO] Initiating Loan Repayment process. Request Data: {requestData}");

                // Step 2: Call the Loan Repayment API
                var apiResponse = await APICallHelper.LoanRepayment<ServiceResponse<RefundDto>>(
                    _pathHelper,
                    _userInfoToken.Token,
                    requestData,
                    _pathHelper.RepaymentURL
                );

                // Step 3: Check API response status
                if (apiResponse.StatusCode == 200)
                {
                    string successMessage = $"[SUCCESS] Loan repayment successfully processed. Loan ID: {request.LoanId}, Amount: {request.Amount}.";
                    _logger.LogInformation(successMessage);
                    await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.LoanRepayment, LogLevelInfo.Information);

                    return ServiceResponse<RefundDto>.ReturnResultWith200(apiResponse.Data, successMessage);
                }
                else
                {
                    string failureMessage = $"[ERROR] Loan Repayment API call failed. Status Code: {apiResponse.StatusCode}, Response Message: {apiResponse.Message}.";
                    _logger.LogError(failureMessage);
                    await BaseUtilities.LogAndAuditAsync(failureMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.LoanRepayment, LogLevelInfo.Error);

                    return ServiceResponse<RefundDto>.Return500(failureMessage);
                }
            }
            catch (Exception e)
            {
                // Step 4: Log and audit API failure
                string errorMessage = $"[CRITICAL ERROR] Loan repayment process failed for Loan ID: {request.LoanId}. Exception: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.LoanRepayment, LogLevelInfo.Error);

                return ServiceResponse<RefundDto>.Return500(e, errorMessage);
            }
        }
    }

}
