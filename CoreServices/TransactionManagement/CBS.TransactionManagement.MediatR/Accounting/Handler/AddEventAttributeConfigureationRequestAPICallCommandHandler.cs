using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Command;
using Newtonsoft.Json;
using CBS.TransactionManagement.Data;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to configure new event attributes through an API call.
    /// </summary>
    public class AddEventAttributeConfigureationRequestAPICallCommandHandler : IRequestHandler<AddEventAttributeConfigureationRequestAPICallCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<AddEventAttributeConfigureationRequestAPICallCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;

        /// <summary>
        /// Constructor for initializing the AddEventAttributeConfigureationRequestAPICallCommandHandler.
        /// </summary>
        public AddEventAttributeConfigureationRequestAPICallCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<AddEventAttributeConfigureationRequestAPICallCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            PathHelper pathHelper)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _pathHelper = pathHelper;
        }

        /// <summary>
        /// Handles the AddEventAttributeConfigureationRequestCommand to configure new event attributes via an API call.
        /// </summary>
        public async Task<ServiceResponse<bool>> Handle(AddEventAttributeConfigureationRequestAPICallCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Serialize request for API call
                string requestData = JsonConvert.SerializeObject(request);

                // Log and perform the API call
                var serviceResponse = await APICallHelper.AccountingAPICalls<ServiceResponse<bool>>(
                    _pathHelper,
                    _userInfoToken.Token,
                    requestData,
                    _pathHelper.AccountingEventAttributesCreateURL
                );

                // Log and audit success
                string successMessage = "Event attributes configured successfully via accounting API.";
                await LogAndAuditAsync(request, successMessage, HttpStatusCodeEnum.OK, LogLevelInfo.Information);

                // Return success response
                return ServiceResponse<bool>.ReturnResultWith200(serviceResponse.Data, successMessage);
            }
            catch (Exception ex)
            {
                // Log and audit failure
                string errorMessage = $"Error occurred while calling accounting API to configure event attributes: {ex.Message}";
                _logger.LogError(errorMessage);
                await LogAndAuditAsync(request, errorMessage, HttpStatusCodeEnum.InternalServerError, LogLevelInfo.Error);

                // Return error response
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }

        /// <summary>
        /// Logs and audits the operation.
        /// </summary>
        private async Task LogAndAuditAsync(AddEventAttributeConfigureationRequestAPICallCommand request, string message, HttpStatusCodeEnum statusCode, LogLevelInfo logLevel)
        {
            await BaseUtilities.LogAndAuditAsync(
                message,
                request,
                statusCode,
                LogAction.Create,
                logLevel,
                request.productAccountingBookId
            );
        }
    }

}
