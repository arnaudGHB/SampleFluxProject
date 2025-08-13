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
    /// Handles the query to retrieve all loans.
    /// </summary>
    public class GetAllLoanQueriesHandler : IRequestHandler<GetAllLoanQueries, ServiceResponse<List<Loan>>>
    {
        private readonly ILogger<GetAllLoanQueriesHandler> _logger; // Logger for logging actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllLoanQueriesHandler"/> class.
        /// </summary>
        /// <param name="userInfoToken">User information token for authentication.</param>
        /// <param name="logger">Logger for logging actions and errors.</param>
        /// <param name="uow">Unit of Work for database transactions.</param>
        /// <param name="pathHelper">Helper for constructing API paths.</param>
        public GetAllLoanQueriesHandler(
            UserInfoToken userInfoToken,
            ILogger<GetAllLoanQueriesHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            PathHelper pathHelper)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _pathHelper = pathHelper;
        }

        /// <summary>
        /// Handles the query to retrieve all loans.
        /// </summary>
        /// <param name="request">The request containing loan filter parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<Loan>>> Handle(GetAllLoanQueries request, CancellationToken cancellationToken)
        {
            string operationDescription = "Fetching all loans from the loan service.";
            try
            {
                // Serialize request for logging and API call
                string serializedRequest = JsonConvert.SerializeObject(request);
                _logger.LogInformation($"Request for {operationDescription}: {serializedRequest}");

                // Call the loan API
                var serviceResponse = await APICallHelper.PostData<ServiceResponse<List<Loan>>>(
                    _pathHelper.LoanBaseURL,
                    _pathHelper.AllLoansURL,
                    serializedRequest,
                    _userInfoToken.Token);

                // Log and return successful response
                await BaseUtilities.LogAndAuditAsync(
                    $"{operationDescription} succeeded.",
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.GetAllLoans,
                    LogLevelInfo.Information);

                return ServiceResponse<List<Loan>>.ReturnResultWith200(serviceResponse.Data);
            }
            catch (Exception e)
            {
                string errorMessage = $"Error occurred while {operationDescription}. Error: {e.Message}";

                // Log and audit error
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.GetAllLoans,
                    LogLevelInfo.Error);

                return ServiceResponse<List<Loan>>.Return500(errorMessage);
            }
        }
    }

}
