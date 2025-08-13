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
    /// Handles the command to retrieve loans by multiple customers.
    /// </summary>
    public class GetLoansByMultipleCustomersQueryHandler : IRequestHandler<GetLoansByMultipleCustomersQuery, ServiceResponse<List<Loan>>>
    {
        private readonly ILogger<GetLoansByMultipleCustomersQueryHandler> _logger; // Logger for logging actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;

        /// <summary>
        /// Constructor for initializing the GetLoansByMultipleCustomersQueryHandler.
        /// </summary>
        /// <param name="userInfoToken">User information token for authentication.</param>
        /// <param name="logger">Logger for logging actions and errors.</param>
        /// <param name="pathHelper">Helper for constructing API paths.</param>
        public GetLoansByMultipleCustomersQueryHandler(
            UserInfoToken userInfoToken,
            ILogger<GetLoansByMultipleCustomersQueryHandler> logger,
            PathHelper pathHelper)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _pathHelper = pathHelper;
        }

        /// <summary>
        /// Handles the request to retrieve loans by multiple customers.
        /// </summary>
        /// <param name="request">The request containing customer IDs and operation type.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<Loan>>> Handle(GetLoansByMultipleCustomersQuery request, CancellationToken cancellationToken)
        {
            string operationDescription = $"Fetching loans for multiple customers: Operation Type [{request.OperationType}]";

            try
            {
                // Serialize request for logging
                var queryString = BaseUtilities.ToQueryString(request);
                var fullUrl = $"{_pathHelper.GetCustomersLoans}?{queryString}";

                // Log the request details
                _logger.LogInformation($"Request for {operationDescription}. URL: {fullUrl}");

                // Call the loan service API
                var serviceResponse = await APICallHelper.GetAsynch<ServiceResponse<List<Loan>>>(_pathHelper.LoanBaseURL, fullUrl, _userInfoToken.Token);

                // Audit and return successful response
                await BaseUtilities.LogAndAuditAsync(
                    $"{operationDescription} succeeded.",
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.GetMembersLoans,
                    LogLevelInfo.Information);

                return ServiceResponse<List<Loan>>.ReturnResultWith200(serviceResponse.Data);
            }
            catch (Exception e)
            {
                // Construct and log the error message
                string errorMessage = $"Error occurred while {operationDescription}. Error: {e.Message}";
                _logger.LogError(errorMessage);

                // Audit the error
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.GetMembersLoans,
                    LogLevelInfo.Error);

                return ServiceResponse<List<Loan>>.Return500(e, errorMessage);
            }
        }
    }

}
