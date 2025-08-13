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
    /// Handles the command to add a new AccountingEvent for member account postings.
    /// </summary>
    public class AddEventMemberAccountPostingCommandHandler : IRequestHandler<AddEventMemberAccountPostingCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<AddEventMemberAccountPostingCommandHandler> _logger; // Logger for logging actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for managing database transactions.
        private readonly UserInfoToken _userInfoToken; // Token containing user-specific information.
        private readonly PathHelper _pathHelper; // Helper for constructing API paths.
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository; // Repository for utility services operations.

        /// <summary>
        /// Constructor for initializing the handler.
        /// </summary>
        public AddEventMemberAccountPostingCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<AddEventMemberAccountPostingCommandHandler> logger,
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
        /// Handles the command to process accounting event postings for member accounts.
        /// </summary>
        public async Task<ServiceResponse<bool>> Handle(AddEventMemberAccountPostingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Filter out any zero or invalid amounts
                var filteredRequest = FilterZeros(request);

                // Serialize request for API call
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

                // Log success and audit the operation
                string successMessage = "Event posting for non-cash payments was successful.";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    filteredRequest,
                    HttpStatusCodeEnum.OK,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Information,
                    request.TransactionReferenceId
                );

                return ServiceResponse<bool>.ReturnResultWith200(serviceResponse.Data, successMessage);
            }
            catch (Exception ex)
            {
                // Handle errors
                var filteredRequest = FilterZeros(request);
                string requestData = JsonConvert.SerializeObject(filteredRequest);
                string errorMessage = $"Error occurred while posting event for non-cash payments: {ex.Message}";

                // Log and audit the error
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    filteredRequest,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Error,
                    request.TransactionReferenceId
                );

                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }

        /// <summary>
        /// Filters out zero or insignificant values from the event collections.
        /// </summary>
        private AddEventMemberAccountPostingCommand FilterZeros(AddEventMemberAccountPostingCommand request)
        {
            var filteredCollections = request.AmountEventCollections
                .Where(detail => Math.Abs(detail.Amount) > 0)
                .ToList();

            return new AddEventMemberAccountPostingCommand
            {
                AmountEventCollections = filteredCollections,
                TransactionReferenceId = request.TransactionReferenceId,
                Source = request.Source,
                ProductId = request.ProductId,
                TransactionDate = request.TransactionDate
            };
        }
    }

}
