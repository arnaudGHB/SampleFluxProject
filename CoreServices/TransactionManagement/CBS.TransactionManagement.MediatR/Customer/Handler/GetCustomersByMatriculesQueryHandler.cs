using AutoMapper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.Handler
{
    /// <summary>
    /// Handles the query to retrieve customers by matricules.
    /// </summary>
    public class GetCustomersByMatriculesQueryHandler : IRequestHandler<GetCustomersByMatriculesQuery, ServiceResponse<List<CustomerDto>>>
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;
        private readonly ILogger<GetCustomersByMatriculesQueryHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetCustomersByMatriculesQueryHandler"/> class.
        /// </summary>
        /// <param name="userInfoToken">The user information token.</param>
        /// <param name="logger">Logger for logging actions and errors.</param>
        /// <param name="pathHelper">Helper for constructing API paths.</param>
        public GetCustomersByMatriculesQueryHandler(
            UserInfoToken userInfoToken,
            ILogger<GetCustomersByMatriculesQueryHandler> logger,
            PathHelper pathHelper)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _pathHelper = pathHelper;
        }

        /// <summary>
        /// Handles the query to retrieve customers by matricules.
        /// </summary>
        /// <param name="request">The request containing operation type and matricules.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CustomerDto>>> Handle(GetCustomersByMatriculesQuery request, CancellationToken cancellationToken)
        {
            string operationDescription = $"Getting customers from member services by matricules for {request.OperationType}";
            try
            {
                // Construct query string and full URL
                string queryString = BaseUtilities.ToQueryString(request);
               
                var fullUrl = $"{_pathHelper.GetMembers}?{queryString}";
                var customerData = await APICallHelper.GetCustomers<ServiceResponse<List<CustomerDto>>>(_pathHelper.CustomerBaseUrl, fullUrl, _userInfoToken.Token);

                // Log and audit success
                await BaseUtilities.LogAndAuditAsync(
                    $"Successfully retrieved customers by matricules for operation: {request.OperationType}.",
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.GetMembersByMareicules,
                    LogLevelInfo.Information);

                return ServiceResponse<List<CustomerDto>>.ReturnResultWith200(customerData.Data);
            }
            catch (Exception e)
            {
                string errorMessage = $"Error occurred while retrieving customers for {request.OperationType}. Error: {e.Message}";

                // Log error and audit
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.GetMembersByMareicules,
                    LogLevelInfo.Error);

                return ServiceResponse<List<CustomerDto>>.Return500(e, errorMessage);
            }
        }
    }
}

