using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using Newtonsoft.Json;
using CBS.TransactionManagement.Data.Dto.LoanRepayment;
using CBS.TransactionManagement.Data.LoanQueries;
using CBS.TransactionManagement.MediatR.Loans.Queries;

namespace CBS.TransactionManagement.Loans.Handlers
{
    /// <summary>
    /// Handles the request to retrieve open loans for a branch by consuming an external API.
    /// </summary>
    public class GetOpenLoansByBranchQueryHandler : IRequestHandler<GetOpenLoansByBranchQuery, ServiceResponse<List<LightLoanDto>>>
    {
        private readonly ILogger<GetOpenLoansByBranchQueryHandler> _logger; // Logger for tracking API calls and errors.
        private readonly UserInfoToken _userInfoToken; // Token for authenticating API requests.
        private readonly PathHelper _pathHelper; // Helper for constructing API endpoint paths.

        /// <summary>
        /// Initializes a new instance of the <see cref="GetOpenLoansByBranchQueryHandler"/> class.
        /// </summary>
        /// <param name="userInfoToken">Authentication token containing user details.</param>
        /// <param name="logger">Logger instance for logging API interactions.</param>
        /// <param name="pathHelper">Helper for managing API endpoint paths.</param>
        public GetOpenLoansByBranchQueryHandler(
            UserInfoToken userInfoToken,
            ILogger<GetOpenLoansByBranchQueryHandler> logger,
            PathHelper pathHelper)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _pathHelper = pathHelper;
        }

        /// <summary>
        /// Processes the request to retrieve all open loans for a branch by calling an external API.
        /// </summary>
        /// <param name="request">The request containing the branch ID.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>A service response containing a list of open loans.</returns>
        public async Task<ServiceResponse<List<LightLoanDto>>> Handle(GetOpenLoansByBranchQuery request, CancellationToken cancellationToken)
        {
            string operationDescription = $"Fetching open loans for branch [{request.BranchId}] via API";

            try
            {
                // Construct query string for API request
                var fullUrl = $"{string.Format(_pathHelper.GetOpenLoansByBranch,request.BranchId)}";

                // Log the API request details
                _logger.LogInformation($"Sending API request: {operationDescription}. Endpoint: {fullUrl}");

                // Make the API request to retrieve open loans
                var serviceResponse = await APICallHelper.GetAsynch<ServiceResponse<List<LightLoanDto>>>(
                    _pathHelper.LoanBaseURL, fullUrl, _userInfoToken.Token);

                // Validate API response
                if (serviceResponse == null || serviceResponse.Data == null)
                {
                    string noDataMessage = $"API returned no data for {operationDescription}.";
                    _logger.LogWarning(noDataMessage);
                    return ServiceResponse<List<LightLoanDto>>.Return404(noDataMessage);
                }

                // Log and audit the successful API call
                await BaseUtilities.LogAndAuditAsync(
                    $"{operationDescription} succeeded.",
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.GetBranchLoansForSalaryAnalysis,
                    LogLevelInfo.Information);

                return ServiceResponse<List<LightLoanDto>>.ReturnResultWith200(serviceResponse.Data);
            }
            catch (Exception e)
            {
                // Construct and log the error message
                string errorMessage = $"API call failed while {operationDescription}. Error: {e.Message}";
                _logger.LogError(errorMessage);

                // Audit the error
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.GetBranchLoansForSalaryAnalysis,
                    LogLevelInfo.Error);

                return ServiceResponse<List<LightLoanDto>>.Return500(e, errorMessage);
            }
        }
    }

}
