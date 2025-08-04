using AutoMapper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Queries;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.TransactionManagement.Handler
{
    /// <summary>
    /// Handles the command to validate a customer's PIN and generate OTP if required.
    /// </summary>
    public class GenerateOTPCommandHandler : IRequestHandler<CustomerPinValidationCommand, ServiceResponse<CustomerPinValidationDto>>
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;
        private readonly ILogger<GenerateOTPCommandHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateOTPCommandHandler"/> class.
        /// </summary>
        public GenerateOTPCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<GenerateOTPCommandHandler> logger,
            PathHelper pathHelper = null)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _pathHelper = pathHelper;
        }

        /// <summary>
        /// Handles the CustomerPinValidationCommand.
        /// </summary>
        /// <param name="request">The command containing the customer PIN validation data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CustomerPinValidationDto>> Handle(CustomerPinValidationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Serialize request for API call
                string requestData = JsonConvert.SerializeObject(request);

                // Call the API to validate customer PIN
                var customerData = await APICallHelper.ValidateCustomerPin<ServiceResponse<CustomerPinValidationDto>>(_pathHelper, requestData, _userInfoToken.Token);

                // Log successful validation
                await BaseUtilities.LogAndAuditAsync(
                    "Customer PIN validated successfully.",
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.MemberPinValidation,
                    LogLevelInfo.Information
                    );

                return customerData;
            }
            catch (Exception e)
            {
                // Handle and log errors
                string errorMessage = $"Error occurred while verifying customer PIN: {e.Message}";
                _logger.LogError(errorMessage);

                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.MemberPinValidation,
                    LogLevelInfo.Error,
                    null);

                return ServiceResponse<CustomerPinValidationDto>.Return500(errorMessage);
            }
        }
    }
}

