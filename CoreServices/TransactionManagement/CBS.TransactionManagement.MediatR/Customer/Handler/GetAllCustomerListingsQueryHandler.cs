using AutoMapper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR;
using CBS.TransactionManagement.Queries;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.TransactionManagement.Handler
{

    public class GetAllCustomerListingsQueryHandler : IRequestHandler<GetAllCustomerListingsQuery, ServiceResponse<List<CustomerDto>>>
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;
        private readonly ILogger<GetAllCustomerListingsQueryHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllCustomerListingsQueryHandler"/> class.
        /// </summary>
        /// <param name="userInfoToken">The user information token.</param>
        /// <param name="logger">Logger for logging actions and errors.</param>
        /// <param name="pathHelper">Helper for constructing API paths.</param>
        public GetAllCustomerListingsQueryHandler(
            UserInfoToken userInfoToken,
            ILogger<GetAllCustomerListingsQueryHandler> logger,
            PathHelper pathHelper)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _pathHelper = pathHelper;
        }

        /// <summary>
        /// Handles the query to retrieve all customer listings with filtering.
        /// </summary>
        /// <param name="request">The request containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CustomerDto>>> Handle(GetAllCustomerListingsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                string serializedData = JsonConvert.SerializeObject(request);
                // Call API to retrieve customer listings
                var customerData = await APICallHelper.PostData<ServiceResponse<List<CustomerDto>>>(_pathHelper.CustomerBaseUrl, _pathHelper.GetAllCustomerListings, serializedData, _userInfoToken.Token);
                // Log and audit successful operation
                await BaseUtilities.LogAndAuditAsync(
                    $"Successfully retrieved customer listings.",
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.GetAllCustomersByTransactionService,
                    LogLevelInfo.Information);

                return ServiceResponse<List<CustomerDto>>.ReturnResultWith200(customerData.Data);
            }
            catch (Exception e)
            {
                string errorMessage = $"Error occurred while retrieving customer listings. Error: {e.Message}";

                // Log and audit the error
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.GetAllCustomersByTransactionService,
                    LogLevelInfo.Error);

                return ServiceResponse<List<CustomerDto>>.Return500(e, errorMessage);
            }
        }
    }
}

