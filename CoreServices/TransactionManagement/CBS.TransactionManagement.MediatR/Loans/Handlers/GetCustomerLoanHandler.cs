using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using Newtonsoft.Json;
using CBS.TransactionManagement.Data.Dto.LoanRepayment;
using CBS.TransactionManagement.Data.LoanQueries;

namespace CBS.TransactionManagement.Loans.Handlers
{
    /// <summary>
    /// Handles the command to retrieve customer loans.
    /// </summary>
    public class GetCustomerLoanHandler : IRequestHandler<GetCustomerLoan, ServiceResponse<List<Loan>>>
    {
        private readonly ILogger<GetCustomerLoanHandler> _logger; // Logger for logging actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;

        /// <summary>
        /// Constructor for initializing the GetCustomerLoanHandler.
        /// </summary>
        /// <param name="userInfoToken">User information token for authentication.</param>
        /// <param name="logger">Logger for logging actions and errors.</param>
        /// <param name="pathHelper">Helper for constructing API paths.</param>
        public GetCustomerLoanHandler(
            UserInfoToken userInfoToken,
            ILogger<GetCustomerLoanHandler> logger,
            PathHelper pathHelper)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _pathHelper = pathHelper;
        }

        /// <summary>
        /// Handles the request to retrieve customer loans.
        /// </summary>
        /// <param name="request">The request containing loan filter parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<Loan>>> Handle(GetCustomerLoan request, CancellationToken cancellationToken)
        {
            string operationDescription = "Fetching loans for a specific customer.";
            try
            {
                // Serialize request for logging and API call
                string serializedRequest = JsonConvert.SerializeObject(request);
                _logger.LogInformation($"Request for {operationDescription}: {serializedRequest}");

                // Call the loan service API
                var serviceResponse = await APICallHelper.PostData<ServiceResponse<List<Loan>>>(
                    _pathHelper.LoanBaseURL,
                    _pathHelper.GetCustomerLoan,
                    serializedRequest,
                    _userInfoToken.Token);

                if (serviceResponse.StatusCode == 200)
                {
                    // Log and return successful response
                    await BaseUtilities.LogAndAuditAsync(
                        $"{operationDescription} succeeded.",
                        request,
                        HttpStatusCodeEnum.OK,
                        LogAction.GetCustomerLoans,
                        LogLevelInfo.Information);

                    return ServiceResponse<List<Loan>>.ReturnResultWith200(serviceResponse.Data);
                }

                string warningMessage = $"No loans found for the customer. Operation: {operationDescription}.";
                _logger.LogWarning(warningMessage);
                return ServiceResponse<List<Loan>>.Return404(warningMessage);
            }
            catch (Exception e)
            {
                string errorMessage = $"Error occurred while {operationDescription}. Error: {e.Message}";

                // Log and audit the error
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.GetCustomerLoans,
                    LogLevelInfo.Error);

                return ServiceResponse<List<Loan>>.Return500(errorMessage);
            }
        }
    }

}
