using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Command;
using Newtonsoft.Json;
using CBS.TransactionManagement.MediatR.UtilityServices;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to process Mobile Money Operations (Cashin & Cashout).
    /// </summary>
    public class RemittanceAccountingPostingCommandHandler : IRequestHandler<RemittanceAccountingPostingCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<RemittanceAccountingPostingCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for managing database transactions.
        private readonly UserInfoToken _userInfoToken; // Token containing user-specific information.
        private readonly PathHelper _pathHelper; // Helper for constructing API paths.
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository; // Repository for utility services operations.

        /// <summary>
        /// Constructor for initializing the RemittanceAccountingPostingCommandHandler.
        /// </summary>
        public RemittanceAccountingPostingCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<RemittanceAccountingPostingCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            PathHelper pathHelper,
            IAPIUtilityServicesRepository utilityServicesRepository)
        {
            _userInfoToken = userInfoToken; // Assign user info token
            _logger = logger; // Assign logger instance
            _uow = uow; // Assign unit of work
            _pathHelper = pathHelper; // Assign path helper
            _utilityServicesRepository = utilityServicesRepository; // Assign utility services repository
        }

        /// <summary>
        /// Handles the RemittanceAccountingPostingCommand to process Mobile Money operations.
        /// </summary>
        /// <param name="request">The RemittanceAccountingPostingCommand containing transaction data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(RemittanceAccountingPostingCommand request, CancellationToken cancellationToken)
        {

            try
            {
                // Serialize the filtered request for logging and API call
                string serializedData = JsonConvert.SerializeObject(request);

                // Construct the destination URL for the API call
                string destinationUrl = $"{_pathHelper.AccountingBaseURL}{_pathHelper.MakeRemittanceCommandURL}";

                // Log the serialized request data to the utility services repository
                await _utilityServicesRepository.CreatAccountingRLog(
                    serializedData,
                    CommandDataType.RemittanceAccountingPostingCommand,
                    request.TransactionReferenceId,
                    request.TransactionDate,
                    destinationUrl);

                // Make the API call to process the accounting entries
                var apiResponse = await APICallHelper.AccountingAPICalls<ServiceResponse<bool>>(
                    _pathHelper,
                    _userInfoToken.Token,
                    serializedData,
                    _pathHelper.MakeRemittanceCommandURL
                );

                // Log and audit the successful operation
                string successMessage = "Remittance operation [Cashin & Cashout] accounting posting was successful.";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Information,
                    request.TransactionReferenceId
                );

                return ServiceResponse<bool>.ReturnResultWith200(apiResponse.Data, successMessage);
            }
            catch (Exception ex)
            {
                // Handle and log exceptions
                string errorMessage = $"Error occurred during Remittance operation [Cashin & Cashout]: {ex.Message}";
                _logger.LogError(errorMessage);

                // Log the failed operation for audit purposes
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Error,
                    request.TransactionReferenceId
                );

                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }

    }

}
