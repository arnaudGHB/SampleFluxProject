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
    /// Handles the query to get a customer by their telephone number.
    /// </summary>
    public class GetCustomerByTelephoneQueryHandler : IRequestHandler<GetCustomerByTelephoneQuery, ServiceResponse<CustomerDto>>
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;
        private readonly ILogger<GetCustomerByTelephoneQueryHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetCustomerByTelephoneQueryHandler"/> class.
        /// </summary>
        public GetCustomerByTelephoneQueryHandler(
            UserInfoToken userInfoToken,
            ILogger<GetCustomerByTelephoneQueryHandler> logger,
            PathHelper pathHelper = null)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _pathHelper = pathHelper;
        }

        /// <summary>
        /// Handles the GetCustomerByTelephoneQuery request.
        /// </summary>
        /// <param name="request">The query containing the telephone number.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CustomerDto>> Handle(GetCustomerByTelephoneQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Call API to retrieve customer data
                var customerData = await APICallHelper.GetCustomerMapperAsync(
                    _pathHelper.CustomerBaseUrl,
                    _pathHelper.CustomerByTelephoneBaseUrl,
                    request.TelephoneNumber,
                    _userInfoToken.Token);

                if (customerData != null)
                {
                    var customerDto = Mapper.MapToDto(customerData);

                    // Log successful retrieval
                    await BaseUtilities.LogAndAuditAsync(
                        $"Successfully retrieved customer by telephone number: {request.TelephoneNumber}",
                        request,
                        HttpStatusCodeEnum.OK,
                        LogAction.GetMembersByTelephonNumber,
                        LogLevelInfo.Information,
                        request.TelephoneNumber);

                    return ServiceResponse<CustomerDto>.ReturnResultWith200(customerDto);
                }

                // Log when no customer is found
                var notFoundMessage = $"No customer found with telephone number: {request.TelephoneNumber}";
                await BaseUtilities.LogAndAuditAsync(
                    notFoundMessage,
                    request,
                    HttpStatusCodeEnum.NotFound,
                    LogAction.GetMembersByTelephonNumber,
                    LogLevelInfo.Warning,
                    request.TelephoneNumber);

                return ServiceResponse<CustomerDto>.Return404(notFoundMessage);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error occurred while retrieving customer by telephone number: {request.TelephoneNumber}. Error: {ex.Message}";

                // Log error and audit the failure
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.GetMembersByTelephonNumber,
                    LogLevelInfo.Error,
                    request.TelephoneNumber);

                return ServiceResponse<CustomerDto>.Return500(ex, errorMessage);
            }
        }
    }
}

