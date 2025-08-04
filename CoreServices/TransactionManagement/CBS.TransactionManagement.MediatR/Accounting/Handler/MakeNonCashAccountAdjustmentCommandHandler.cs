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
    /// Handles the MakeNonCashAccountAdjustmentCommand to post loan refund accounting entries.
    /// </summary>
    public class MakeNonCashAccountAdjustmentCommandHandler : IRequestHandler<MakeNonCashAccountAdjustmentCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<MakeNonCashAccountAdjustmentCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for managing database transactions.
        private readonly UserInfoToken _userInfoToken; // Token containing user-specific information.
        private readonly PathHelper _pathHelper; // Helper for constructing API paths.
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository; // Repository for utility services operations.

        /// <summary>
        /// Constructor for initializing the MakeNonCashAccountAdjustmentCommandHandler.
        /// </summary>
        public MakeNonCashAccountAdjustmentCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<MakeNonCashAccountAdjustmentCommandHandler> logger,
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
        /// Handles the MakeNonCashAccountAdjustmentCommand to post loan refund accounting entries.
        /// </summary>
        /// <param name="request">The MakeNonCashAccountAdjustmentCommand containing accounting event data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(MakeNonCashAccountAdjustmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string serializedData = JsonConvert.SerializeObject(request);

                // Construct the destination URL for the API call
                string destinationUrl = $"{_pathHelper.AccountingBaseURL}{_pathHelper.MakeNonCashAccountAdjustmentCommandURL}";

                // Log the serialized request data to the utility services repository
                await _utilityServicesRepository.CreatAccountingRLog(
                    serializedData,
                    CommandDataType.MakeNonCashAccountAdjustmentCommand,
                    request.TransactionReference,
                    request.TransactionDate,
                    destinationUrl);

                // Make the API call
                var customerData = await APICallHelper.AccountingAPICalls<ServiceResponse<bool>>(
                    _pathHelper,
                    _userInfoToken.Token,
                    serializedData,
                    _pathHelper.MakeNonCashAccountAdjustmentCommandURL);

                // Log and audit the success operation
                string successMessage = "Accounting posting for loan refund was successful.";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage + $" Data: {serializedData}",
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Information,
                    request.TransactionReference);

                return ServiceResponse<bool>.ReturnResultWith200(customerData.Data, successMessage);
            }
            catch (Exception e)
            {
                // Filter and log the error details
                string serializedData = JsonConvert.SerializeObject(request);
                var errorMessage = $"Error occurred while posting accounting entries: {e.Message}. Data: {serializedData}";

                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage, request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Error,
                    request.TransactionReference);

                return ServiceResponse<bool>.Return500("Accounting service failed.");
            }
        }


    }

}
