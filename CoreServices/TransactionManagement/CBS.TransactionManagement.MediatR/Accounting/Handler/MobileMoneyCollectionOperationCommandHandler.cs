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
    public class MobileMoneyCollectionOperationCommandHandler : IRequestHandler<MobileMoneyCollectionOperationCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<MobileMoneyCollectionOperationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for managing database transactions.
        private readonly UserInfoToken _userInfoToken; // Token containing user-specific information.
        private readonly PathHelper _pathHelper; // Helper for constructing API paths.
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository; // Repository for utility services operations.

        /// <summary>
        /// Constructor for initializing the MobileMoneyCollectionOperationCommandHandler.
        /// </summary>
        public MobileMoneyCollectionOperationCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<MobileMoneyCollectionOperationCommandHandler> logger,
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
        /// Handles the MobileMoneyCollectionOperationCommand to process Momo cash collections.
        /// </summary>
        /// <param name="request">The MobileMoneyCollectionOperationCommand containing transaction data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(MobileMoneyCollectionOperationCommand request, CancellationToken cancellationToken)
        {
            // Filter out any 0 amount from the request
            var filtered = FilterZeros(request);

            try
            {
                // Serialize the filtered data for API call
                string serializedData = JsonConvert.SerializeObject(filtered);

                // Construct the destination URL for the API call
                string destinationUrl = $"{_pathHelper.AccountingBaseURL}{_pathHelper.AccountingPostingMomoCollectionURL}";

                // Log the serialized request data to the utility services repository
                await _utilityServicesRepository.CreatAccountingRLog(
                    serializedData,
                    CommandDataType.MobileMoneyCollectionOperationCommand,
                    request.TransactionReferenceId,
                    request.TransactionDate,
                    destinationUrl);

                // Call the accounting API
                var customerData = await APICallHelper.AccountingAPICalls<ServiceResponse<bool>>(
                    _pathHelper,
                    _userInfoToken.Token,
                    serializedData,
                    _pathHelper.AccountingPostingMomoCollectionURL);

                // Log and audit the successful operation
                string successMessage = "Deposit, Momo cash collection processed successfully.";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    filtered,
                    HttpStatusCodeEnum.OK,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Information,
                    request.TransactionReferenceId);

                return ServiceResponse<bool>.ReturnResultWith200(customerData.Data, "Accounting posting was successful.");
            }
            catch (Exception e)
            {
                // Log and handle the exception
                var errorMessage = $"Error occurred while posting accounting entries for Deposit, Momo cash collection: {e.Message}";
                _logger.LogError(errorMessage);

                // Audit the failed operation
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    filtered,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Error,
                    request.TransactionReferenceId);

                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }

        /// <summary>
        /// Filters out PaymentCollection details with zero amounts.
        /// </summary>
        /// <param name="request">The original command request.</param>
        /// <returns>A new MobileMoneyCollectionOperationCommand with filtered data.</returns>
        public MobileMoneyCollectionOperationCommand FilterZeros(MobileMoneyCollectionOperationCommand request)
        {
            // Filter out PaymentCollection details with zero amounts
            var amountCollections = request.PaymentCollection?
                .Where(detail => Math.Abs(detail.Amount) > 0)
                .ToList();

            // Create a new command with the filtered data
            var filteredCommand = new MobileMoneyCollectionOperationCommand
            {
                TransactionDate = request.TransactionDate,
                TransactionReferenceId = request.TransactionReferenceId,
                PaymentCollection = amountCollections,
                TellerSources = request.TellerSources
            };

            return filteredCommand;
        }
    }

}
