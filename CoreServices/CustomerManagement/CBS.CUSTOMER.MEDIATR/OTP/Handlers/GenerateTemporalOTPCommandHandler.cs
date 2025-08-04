using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Dto.OTP;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.OTP.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.CUSTOMER.MEDIATR.OTP.Handlers
{
    /// <summary>
    /// Handles the generation of a temporal OTP (One-Time Password).
    /// </summary>
    public class GenerateTemporalOTPCommandHandler : IRequestHandler<GenerateTemporalOTPCommand, ServiceResponse<TempOTPDto>>
    {
        private readonly ILogger<GenerateTemporalOTPCommandHandler> _logger; // Logger for recording actions and errors.
        private readonly UserInfoToken _userInfoToken; // Token containing user information.
        private readonly PathHelper _pathHelper; // Helper for constructing API paths.
        public IMediator _mediator { get; set; } // Mediator for sending commands.

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateTemporalOTPCommandHandler"/> class.
        /// </summary>
        /// <param name="userInfoToken">Token containing user information.</param>
        /// <param name="logger">Logger for recording actions and errors.</param>
        /// <param name="pathHelper">Helper for constructing API paths.</param>
        /// <param name="mediator">Mediator for sending commands.</param>
        public GenerateTemporalOTPCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<GenerateTemporalOTPCommandHandler> logger,
            PathHelper pathHelper,
            IMediator mediator)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _pathHelper = pathHelper;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the command to generate a temporal OTP.
        /// </summary>
        /// <param name="request">The command containing OTP generation details.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A response containing the generated OTP.</returns>
        public async Task<ServiceResponse<TempOTPDto>> Handle(GenerateTemporalOTPCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Serialize the request for logging and auditing purposes.
                string serializedRequest = JsonConvert.SerializeObject(request);

                // Step 2: Call the external API to generate the temporal OTP.
                var serviceResponse = await APICallHelper.PostData<ServiceResponse<TempOTPDto>>(
                    _pathHelper.IdentityServerBaseUrl,
                    _pathHelper.GenerateTemporalOTPCodeUrl,
                    serializedRequest,
                    _userInfoToken.Token);

                // Step 3: Check the API response status.
                if (serviceResponse.StatusCode != 200)
                {
                    // Log and audit the failure.
                    string errorMessage = $"Generating Temporal OTP Failed: {serviceResponse.Message}";
                    _logger.LogError(errorMessage);

                    await BaseUtilities.LogAndAuditAsync(
                        errorMessage,
                        request,
                        HttpStatusCodeEnum.InternalServerError,
                        LogAction.GenerateTemporalOTP,
                        LogLevelInfo.Error);

                    // Return the failure response.
                    return ServiceResponse<TempOTPDto>.Return500(errorMessage);
                }

                // Step 4: Log and audit the success.
                string successMessage = "Temporal OTP generated successfully.";
                _logger.LogInformation(successMessage);

                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    serviceResponse.Data,
                    HttpStatusCodeEnum.OK,
                    LogAction.GenerateTemporalOTP,
                    LogLevelInfo.Information);

                // Step 5: Return the successful response from the API.
                return serviceResponse;
            }
            catch (Exception e)
            {
                // Step 6: Handle unexpected errors.
                string errorMessage = $"An error occurred while generating Temporal OTP: {e.Message}";
                _logger.LogError(e, errorMessage);

                // Step 6a: Log and audit the error.
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.GenerateTemporalOTP,
                    LogLevelInfo.Error);

                // Step 6b: Return a 500 Internal Server Error response.
                return ServiceResponse<TempOTPDto>.Return500(errorMessage);
            }
        }
    }
}
