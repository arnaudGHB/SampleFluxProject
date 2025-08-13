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
    public class MobileMoneyOperationCommandHandler : IRequestHandler<MobileMoneyOperationCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<MobileMoneyOperationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for managing database transactions.
        private readonly UserInfoToken _userInfoToken; // Token containing user-specific information.
        private readonly PathHelper _pathHelper; // Helper for constructing API paths.
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository; // Repository for utility services operations.

        /// <summary>
        /// Constructor for initializing the MobileMoneyOperationCommandHandler.
        /// </summary>
        public MobileMoneyOperationCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<MobileMoneyOperationCommandHandler> logger,
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
        /// Handles the MobileMoneyOperationCommand to process Mobile Money operations.
        /// </summary>
        /// <param name="request">The MobileMoneyOperationCommand containing transaction data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(MobileMoneyOperationCommand request, CancellationToken cancellationToken)
        {
            // Ensure that only valid amounts are processed
            var filteredRequest = FilterZeros(request);

            try
            {
                // Serialize the filtered request for logging and API call
                string serializedData = JsonConvert.SerializeObject(filteredRequest);

                // Construct the destination URL for the API call
                string destinationUrl = $"{_pathHelper.AccountingBaseURL}{_pathHelper.MomoMobileMoneyURL}";

                // Log the serialized request data to the utility services repository
                await _utilityServicesRepository.CreatAccountingRLog(
                    serializedData,
                    CommandDataType.MobileMoneyOperationCommand,
                    request.TransactionReference,
                    request.TransactionDate,
                    destinationUrl);

                // Make the API call to process the accounting entries
                var apiResponse = await APICallHelper.AccountingAPICalls<ServiceResponse<bool>>(
                    _pathHelper,
                    _userInfoToken.Token,
                    serializedData,
                    _pathHelper.MomoMobileMoneyURL
                );

                // Log and audit the successful operation
                string successMessage = "Mobile Money operation [Cashin & Cashout] accounting posting was successful.";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    filteredRequest,
                    HttpStatusCodeEnum.OK,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Information,
                    request.TransactionReference
                );

                return ServiceResponse<bool>.ReturnResultWith200(apiResponse.Data, successMessage);
            }
            catch (Exception ex)
            {
                // Handle and log exceptions
                string errorMessage = $"Error occurred during Mobile Money operation [Cashin & Cashout]: {ex.Message}";
                _logger.LogError(errorMessage);

                // Log the failed operation for audit purposes
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    filteredRequest,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Error,
                    request.TransactionReference
                );

                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }

        /// <summary>
        /// Filters out invalid data from the request, such as zero amounts.
        /// </summary>
        /// <param name="request">The original command request.</param>
        /// <returns>A new MobileMoneyOperationCommand with filtered data.</returns>
        public MobileMoneyOperationCommand FilterZeros(MobileMoneyOperationCommand request)
        {
            if (Math.Abs(request.Amount) <= 0)
            {
                throw new ArgumentException("Transaction amount must be greater than zero.", nameof(request.Amount));
            }

            return new MobileMoneyOperationCommand
            {
                Amount = request.Amount,
                TransactionDate = request.TransactionDate,
                OperationType = request.OperationType,
                TellerSources = request.TellerSources,
                TransactionReference = request.TransactionReference
            };
        }
    }

}
