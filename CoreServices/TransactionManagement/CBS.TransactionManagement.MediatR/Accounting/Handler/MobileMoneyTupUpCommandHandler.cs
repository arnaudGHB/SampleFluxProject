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
    /// Handles the command to process Mobile Money Top-Up transactions.
    /// </summary>
    public class MobileMoneyTupUpCommandHandler : IRequestHandler<MobileMoneyTupUpCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<MobileMoneyTupUpCommandHandler> _logger; // Logger for logging actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for managing database transactions.
        private readonly UserInfoToken _userInfoToken; // Token containing user-specific information.
        private readonly PathHelper _pathHelper; // Helper for constructing API paths.
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository; // Repository for utility services operations.

        /// <summary>
        /// Constructor for initializing the MobileMoneyTupUpCommandHandler.
        /// </summary>
        public MobileMoneyTupUpCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<MobileMoneyTupUpCommandHandler> logger,
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
        /// Handles the MobileMoneyTupUpCommand to process Mobile Money Top-Up transactions.
        /// </summary>
        /// <param name="request">The MobileMoneyTupUpCommand containing transaction data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(MobileMoneyTupUpCommand request, CancellationToken cancellationToken)
        {
            // Ensure the command contains valid data
            var filteredRequest = FilterZeros(request);

            try
            {
                // Serialize the filtered request
                string serializedData = JsonConvert.SerializeObject(filteredRequest);

                // Construct the destination URL for the API call
                string destinationUrl = $"{_pathHelper.AccountingBaseURL}{_pathHelper.MobileMoneyManagementURL}";

                // Log the serialized request data to the utility services repository
                await _utilityServicesRepository.CreatAccountingRLog(
                    serializedData,
                    CommandDataType.MobileMoneyTopUpCommand,
                    request.TransactionReferenceId,
                    request.TransactionDate,
                    destinationUrl);

                // Call the accounting API to process the transaction
                var apiResponse = await APICallHelper.AccountingAPICalls<ServiceResponse<bool>>(
                    _pathHelper,
                    _userInfoToken.Token,
                    serializedData,
                    _pathHelper.MobileMoneyManagementURL
                );

                // Log and audit the successful operation
                string successMessage = $"Mobile Money Top-Up operation succeeded for Transaction Reference: {request.TransactionReferenceId}.";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    filteredRequest,
                    HttpStatusCodeEnum.OK,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Information,
                    request.TransactionReferenceId
                );

                return ServiceResponse<bool>.ReturnResultWith200(apiResponse.Data, "Mobile Money Top-Up Accounting posting was successful.");
            }
            catch (Exception ex)
            {
                // Log and handle the exception
                string errorMessage = $"Error occurred during Mobile Money Top-Up operation for Transaction Reference: {request.TransactionReferenceId}. Error: {ex.Message}";
                _logger.LogError(errorMessage);

                // Audit the failed operation
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
        /// Filters invalid data, such as zero amounts, from the request.
        /// </summary>
        /// <param name="request">The original command request.</param>
        /// <returns>A new MobileMoneyTupUpCommand with valid data.</returns>
        public MobileMoneyTupUpCommand FilterZeros(MobileMoneyTupUpCommand request)
        {
            if (Math.Abs(request.Amount) <= 0)
            {
                throw new ArgumentException("Transaction amount must be greater than zero.", nameof(request.Amount));
            }

            return new MobileMoneyTupUpCommand
            {
                Amount = request.Amount,
                TransactionDate = request.TransactionDate,
                ChartOfAccountIdFrom = request.ChartOfAccountIdFrom,
                ChartOfAccountIdTo = request.ChartOfAccountIdTo,
                Direction = request.Direction,
                TransactionReferenceId = request.TransactionReferenceId
            };
        }
    }
}
