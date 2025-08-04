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
    /// Handles the command to add a new AccountingEvent.
    /// </summary>
    public class AutoPostingEventCommandHandler : IRequestHandler<AutoPostingEventCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<AutoPostingEventCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for managing database transactions.
        private readonly IAccountingPostingConfirmationRepository _accountingPostingConfirmationRepository; // Repository for posting confirmations.
        private readonly UserInfoToken _userInfoToken; // Token containing user-specific information.
        private readonly PathHelper _pathHelper; // Helper for constructing API paths.
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository; // Repository for utility services operations.

        /// <summary>
        /// Constructor for initializing the AutoPostingEventCommandHandler.
        /// </summary>
        public AutoPostingEventCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<AutoPostingEventCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            PathHelper pathHelper,
            IAccountingPostingConfirmationRepository accountingPostingConfirmationRepository = null,
            IAPIUtilityServicesRepository utilityServicesRepository = null)
        {
            _userInfoToken = userInfoToken; // Assign user info token
            _logger = logger; // Assign logger instance
            _uow = uow; // Assign unit of work
            _pathHelper = pathHelper; // Assign path helper
            _accountingPostingConfirmationRepository = accountingPostingConfirmationRepository; // Assign posting confirmation repository
            _utilityServicesRepository = utilityServicesRepository; // Assign utility services repository
        }

        /// <summary>
        /// Handles the AutoPostingEventCommand to process accounting event postings.
        /// </summary>
        public async Task<ServiceResponse<bool>> Handle(AutoPostingEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Filter and clean up the request
                var filteredRequest = FilterZeros(request);

                // Serialize the request data
                string requestData = JsonConvert.SerializeObject(filteredRequest);

                // Construct the destination URL for the API call
                string destinationUrl = $"{_pathHelper.AccountingBaseURL}{_pathHelper.AccountingEventPostingURL}";

                // Log the serialized request data to the utility services repository
                await _utilityServicesRepository.CreatAccountingRLog(
                    requestData,
                    CommandDataType.AutoPostingEventCommand,
                    request.TransactionReferenceId,
                    request.TransactionDate,
                    destinationUrl);

                // Call the accounting API
                var serviceResponse = await APICallHelper.AccountingAPICalls<ServiceResponse<bool>>(
                    _pathHelper,
                    _userInfoToken.Token,
                    requestData,
                    _pathHelper.AccountingEventPostingURL
                );

                // Log success
                string successMessage = "Auto Event posting for cash payment was successful.";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    filteredRequest,
                    HttpStatusCodeEnum.OK,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Information,
                    request.TransactionReferenceId);

                // Return success response
                return ServiceResponse<bool>.ReturnResultWith200(serviceResponse.Data, successMessage);
            }
            catch (Exception ex)
            {
                // Handle errors
                var filteredRequest = FilterZeros(request);
                string requestData = JsonConvert.SerializeObject(filteredRequest);
                string errorMessage = $"Error occurred while posting event for cash payment: {ex.Message}";

                // Log error
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    filteredRequest,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Error,
                    request.TransactionReferenceId);

                // Return error response
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }

        /// <summary>
        /// Filters out zero or insignificant values from the event collections.
        /// </summary>
        private AutoPostingEventCommand FilterZeros(AutoPostingEventCommand request)
        {
            var filteredCollections = request.AmountEventCollections
                .Where(detail => Math.Abs(detail.Amount) > 0)
                .ToList();

            return new AutoPostingEventCommand
            {
                AmountEventCollections = filteredCollections,
                TransactionReferenceId = request.TransactionReferenceId,
                Source = request.Source,
                MemberReference=request.MemberReference,
                TransactionDate = request.TransactionDate
            };
        }
    }

}
