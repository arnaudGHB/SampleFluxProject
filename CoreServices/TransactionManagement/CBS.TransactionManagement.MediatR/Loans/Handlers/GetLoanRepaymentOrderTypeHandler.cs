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
    /// Handles the command to retrieve Loan Repayment Order.
    /// </summary>
    public class GetLoanRepaymentOrderTypeHandler : IRequestHandler<GetLoanRepaymentOrderQuery, ServiceResponse<LoanRepaymentOrderDto>>
    {
        private readonly ILogger<GetLoanRepaymentOrderTypeHandler> _logger; // Logger for logging actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;

        /// <summary>
        /// Constructor for initializing the GetLoanRepaymentOrderTypeHandler.
        /// </summary>
        /// <param name="userInfoToken">User information token for authentication.</param>
        /// <param name="logger">Logger for logging actions and errors.</param>
        /// <param name="pathHelper">Helper for constructing API paths.</param>
        public GetLoanRepaymentOrderTypeHandler(
            UserInfoToken userInfoToken,
            ILogger<GetLoanRepaymentOrderTypeHandler> logger,
            PathHelper pathHelper)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _pathHelper = pathHelper;
        }

        /// <summary>
        /// Handles the request to retrieve Loan Repayment Order .
        /// </summary>
        /// <param name="request">The request containing loan filter parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanRepaymentOrderDto>> Handle(GetLoanRepaymentOrderQuery request, CancellationToken cancellationToken)
        {
            string operationDescription = "Fetching loans for a specific customer.";
            try
            {
                // Serialize request for logging and API call
                string serializedRequest = JsonConvert.SerializeObject(request);
                _logger.LogInformation($"Request for {operationDescription}: {serializedRequest}");

                // Call the loan service API
                var serviceResponse = await APICallHelper.GetData<ServiceResponse<LoanRepaymentOrderDto>>(
                    _pathHelper.LoanBaseURL,
                    $"{_pathHelper.GetLoanProductRepaymentOrder}{request.LoanRepaymentOrderType}",
                    _userInfoToken.Token);

                if (serviceResponse.StatusCode == 200)
                {
                    // Log and return successful response
                    await BaseUtilities.LogAndAuditAsync(
                        $"{operationDescription} succeeded.",
                        request,
                        HttpStatusCodeEnum.OK,
                        LogAction.GetLoanRepaymentOrderTypes,
                        LogLevelInfo.Information);

                   

                    return ServiceResponse<LoanRepaymentOrderDto>.ReturnResultWith200(serviceResponse.Data);
                }

                string warningMessage = $"No loans found for the customer. Operation: {operationDescription}.";
                _logger.LogWarning(warningMessage);
                return ServiceResponse<LoanRepaymentOrderDto>.Return404(warningMessage);
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
                    LogAction.GetLoanRepaymentOrderTypes,
                    LogLevelInfo.Error);

                return ServiceResponse<LoanRepaymentOrderDto>.Return500(errorMessage);
            }
        }
    }

}
