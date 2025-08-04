using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.CMoneyNotifications.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.TransactionManagement.MediatR.CMoneyNotifications.Handlers
{
    /// <summary>
    /// Handles the command to send a push notification for CMoney.
    /// </summary>
    public class CMoneyNotificationCommandHandler : IRequestHandler<CMoneyNotificationCommand, ServiceResponse<CMoneyNotificationDto>>
    {
        private readonly ILogger<CMoneyNotificationCommandHandler> _logger; // Logger for logging actions and errors.
        private readonly UserInfoToken _userInfoToken; // Information about the current user.
        private readonly PathHelper _pathHelper; // Helper for service URL paths.
        public IMediator _mediator { get; set; } // Mediator for handling internal commands or queries.

        /// <summary>
        /// Constructor for initializing the CMoneyNotificationCommandHandler.
        /// </summary>
        /// <param name="userInfoToken">Current user info for authentication and context.</param>
        /// <param name="logger">Logger for capturing logs and errors.</param>
        /// <param name="pathHelper">Helper for constructing service URLs.</param>
        /// <param name="mediator">Mediator for handling internal requests.</param>
        public CMoneyNotificationCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<CMoneyNotificationCommandHandler> logger,
            PathHelper pathHelper,
            IMediator mediator)
        {
            _userInfoToken = userInfoToken ?? throw new ArgumentNullException(nameof(userInfoToken));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pathHelper = pathHelper ?? throw new ArgumentNullException(nameof(pathHelper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Handles the CMoneyNotificationCommand to send a push notification.
        /// </summary>
        /// <param name="request">The CMoneyNotificationCommand containing notification details.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        public async Task<ServiceResponse<CMoneyNotificationDto>> Handle(CMoneyNotificationCommand request, CancellationToken cancellationToken)
        {
            // Generate a unique reference for tracking the operation.
            string logReference = BaseUtilities.GenerateUniqueNumber();

            try
            {
                // Serialize the request to JSON for logging and auditing.
                string serializedRequest = JsonConvert.SerializeObject(request);

                // Log the initiation of the push notification request.
                string initMessage = "Initiating push notification request.";
                await BaseUtilities.LogAndAuditAsync(initMessage, request, HttpStatusCodeEnum.OK, LogAction.PushNotificationInitiated, LogLevelInfo.Information, logReference);

                // Make the API call to send the notification.
                var serviceResponse = await APICallHelper.PostData<ServiceResponse<CMoneyNotificationDto>>(
                    _pathHelper.SMSBaseURL,
                    _pathHelper.PushNotification,
                    serializedRequest,
                    _userInfoToken.Token);

                // Check the response status.
                if (serviceResponse.StatusCode != 200)
                {
                    string errorMessage = $"Generating push notification failed: {serviceResponse.Message}";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.PushNotificationFailed, LogLevelInfo.Error, logReference);
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CMoneyNotificationDto>.Return500(errorMessage);
                }

                // Log the success message.
                string successMessage = "Push notification generated successfully.";
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.PushNotificationCompleted, LogLevelInfo.Information, logReference);
                _logger.LogInformation(successMessage);

                // Return the successful service response.
                return serviceResponse;
            }
            catch (Exception e)
            {
                // Log and audit any unexpected exception.
                string errorMessage = $"An error occurred while sending push notification: {e.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.PushNotificationFailed, LogLevelInfo.Error, logReference);
                _logger.LogError(errorMessage);

                // Return a 500 response with the error message.
                return ServiceResponse<CMoneyNotificationDto>.Return500(errorMessage);
            }
        }
    }
}
