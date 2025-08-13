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
    public class AddTransferPostingCommandHandler : IRequestHandler<AddTransferPostingCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<AddTransferPostingCommandHandler> _logger; // Logger for logging actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for managing database transactions.
        private readonly UserInfoToken _userInfoToken; // Token containing user-specific information.
        private readonly PathHelper _pathHelper; // Helper for constructing API paths.
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository; // Repository for utility services operations.

        /// <summary>
        /// Constructor for initializing the AddTransferPostingCommandHandler.
        /// </summary>
        public AddTransferPostingCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<AddTransferPostingCommandHandler> logger,
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
        /// Handles the AddTransferPostingCommand to process a transfer posting.
        /// </summary>
        /// <param name="request">The AddTransferPostingCommand containing transfer posting data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(AddTransferPostingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Filter out zero amounts from the request
                var filtered = FilterZeros(request);
                string serializedData = JsonConvert.SerializeObject(filtered);

                // Construct the destination URL for the API call
                string destinationUrl = $"{_pathHelper.AccountingBaseURL}{_pathHelper.AccountingTransferPostingURL}";

                // Log the serialized request data to the utility services repository
                await _utilityServicesRepository.CreatAccountingRLog(
                    serializedData,
                    CommandDataType.MakeTransferPosting,
                    request.TransactionReferenceId,
                    request.TransactionDate,
                    destinationUrl);

                // Call the Accounting API
                var apiResponse = await APICallHelper.AccountingAPICalls<ServiceResponse<bool>>(
                    _pathHelper,
                    _userInfoToken.Token,
                    serializedData,
                    _pathHelper.AccountingTransferPostingURL);

                // Log and audit the successful operation
                string successMessage = "Accounting posting for transfer was successful.";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage + $" Data: {serializedData}",
                    filtered,
                    HttpStatusCodeEnum.OK,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Information,
                    request.TransactionReferenceId);

                return ServiceResponse<bool>.ReturnResultWith200(apiResponse.Data, successMessage);
            }
            catch (Exception e)
            {
                // Filter and log the error details
                var filtered = FilterZeros(request);
                string data = JsonConvert.SerializeObject(filtered);
                var errorMessage = $"Error occurred while posting accounting for transfer: {e.Message}. Data: {data}";

                _logger.LogError(errorMessage);
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
        /// Filters out zero amounts from the AmountCollection in the request.
        /// </summary>
        /// <param name="request">The AddTransferPostingCommand to filter.</param>
        /// <returns>A new AddTransferPostingCommand with filtered data.</returns>
        public AddTransferPostingCommand FilterZeros(AddTransferPostingCommand request)
        {
            var amountCollections = request.AmountCollection
                .Where(detail => detail.Amount > 0)
                .ToList();

            return new AddTransferPostingCommand
            {
                AmountCollection = amountCollections,
                ExternalBranchId = request.ExternalBranchId,
                IsInterBranchTransaction = request.IsInterBranchTransaction,
                TransactionReferenceId = request.TransactionReferenceId,
                FromProductId = request.FromProductId,
                ToProductId = request.ToProductId,
                ExternalBranchCode = request.ExternalBranchCode,
                FromMemberReference = request.FromMemberReference,
                TransactionDate = request.TransactionDate
            };
        }
    }

}

