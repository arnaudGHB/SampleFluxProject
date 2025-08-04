using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Command;
using Newtonsoft.Json;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.MediatR.UtilityServices;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to process internal auto-posting events.
    /// </summary>
    public class InternalAutoPostingEventCommandHandler : IRequestHandler<InternalAutoPostingEventCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<InternalAutoPostingEventCommandHandler> _logger; // Logger for logging actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for managing database transactions.
        private readonly UserInfoToken _userInfoToken; // Token containing user-specific information.
        private readonly PathHelper _pathHelper; // Helper for constructing API paths.
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository; // Repository for utility services operations.

        /// <summary>
        /// Constructor for initializing the handler.
        /// </summary>
        public InternalAutoPostingEventCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<InternalAutoPostingEventCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            PathHelper pathHelper,
            IAPIUtilityServicesRepository utilityServicesRepository
        )
        {
            _userInfoToken = userInfoToken; // Assign user info token
            _logger = logger; // Assign logger instance
            _uow = uow; // Assign unit of work
            _pathHelper = pathHelper; // Assign path helper
            _utilityServicesRepository = utilityServicesRepository; // Assign utility services repository
        }

        /// <summary>
        /// Handles the command to process internal auto-posting events.
        /// </summary>
        public async Task<ServiceResponse<bool>> Handle(InternalAutoPostingEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Serialize request for API call
                string requestData = JsonConvert.SerializeObject(request);

                // Construct the destination URL for the API call
                string destinationUrl = $"{_pathHelper.AccountingBaseURL}{_pathHelper.InternalAutoPostingEventURL}";

                // Log the serialized request data to the utility services repository
                await _utilityServicesRepository.CreatAccountingRLog(
                    requestData,
                    CommandDataType.InternalAutoPostingCommand,
                    request.EventCode,
                    DateTime.UtcNow,
                    destinationUrl);

                // Call the accounting API
                var serviceResponse = await APICallHelper.AccountingAPICalls<ServiceResponse<bool>>(
                    _pathHelper,
                    _userInfoToken.Token,
                    requestData,
                    _pathHelper.InternalAutoPostingEventURL
                );

                // Log success and audit the operation
                string successMessage = $"Cash Replenishment 57 for {_userInfoToken.BranchName}. Internal auto-posting event for EventCode {request.EventCode} was successful.";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Information,
                    request.EventCode
                );

                return ServiceResponse<bool>.ReturnResultWith200(serviceResponse.Data, successMessage);
            }
            catch (Exception ex)
            {
                // Handle errors
                string requestData = JsonConvert.SerializeObject(request);
                string errorMessage = $"Cash Replenishment 57 for {_userInfoToken.BranchName}. Error occurred while processing internal auto-posting event for EventCode {request.EventCode}: {ex.Message}";

                // Log and audit the error
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Error,
                    request.EventCode
                );

                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }
    }

}
