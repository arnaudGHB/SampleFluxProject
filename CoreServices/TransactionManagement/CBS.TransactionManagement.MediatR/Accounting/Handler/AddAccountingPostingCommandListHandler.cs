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
using CBS.TransactionManagement.MediatR.Accounting.Command;
using CBS.TransactionManagement.MediatR.UtilityServices;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to add a new AccountingEvent in bulk.
    /// </summary>
    public class AddAccountingPostingCommandListHandler : IRequestHandler<AddAccountingPostingCommandList, ServiceResponse<bool>>
    {
        // Dependency for logging information and errors
        private readonly ILogger<AddAccountingPostingCommandListHandler> _logger;

        // Dependency for working with transactions in a unit-of-work pattern
        private readonly IUnitOfWork<TransactionContext> _uow;

        // Token containing user-specific information
        private readonly UserInfoToken _userInfoToken;

        // Helper class for managing URL paths
        private readonly PathHelper _pathHelper;

        // Repository for interacting with utility services
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository;

        /// <summary>
        /// Initializes a new instance of the handler with the required dependencies.
        /// </summary>
        public AddAccountingPostingCommandListHandler(
            UserInfoToken userInfoToken,
            ILogger<AddAccountingPostingCommandListHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            PathHelper pathHelper,
            IAPIUtilityServicesRepository utilityServicesRepository)
        {
            _userInfoToken = userInfoToken; // Assign user info token
            _logger = logger; // Assign logger instance
            _uow = uow; // Assign unit of work instance
            _pathHelper = pathHelper; // Assign path helper instance
            _utilityServicesRepository = utilityServicesRepository; // Assign utility services repository
        }

        /// <summary>
        /// Handles the bulk accounting posting command.
        /// </summary>
        public async Task<ServiceResponse<bool>> Handle(AddAccountingPostingCommandList request, CancellationToken cancellationToken)
        {
            try
            {
                // Filter out zero-value amounts from the request
                var filtered = FilterZeros(request);

                // Serialize the filtered request data for logging and API purposes
                string serializedData = JsonConvert.SerializeObject(filtered);

                // Construct the destination URL for the API call
                string destinationUrl = $"{_pathHelper.AccountingBaseURL}{_pathHelper.MakeBulkAccountPostingURL}";

                // Log the serialized request data to the utility services repository
                await _utilityServicesRepository.CreatAccountingRLog(
                    serializedData,
                    CommandDataType.MakeBulkAccountPostingCommand,
                    request.MakeAccountPostingCommands.FirstOrDefault().TransactionReferenceId,
                    request.MakeAccountPostingCommands.FirstOrDefault().TransactionDate,
                    destinationUrl);

                // Make the API call to the accounting service
                var response = await APICallHelper.AccountingAPICalls<ServiceResponse<bool>>(
                    _pathHelper,
                    _userInfoToken.Token,
                    serializedData,
                    _pathHelper.MakeBulkAccountPostingURL);

                // Log and audit successful processing of the command
                await BaseUtilities.LogAndAuditAsync(
                    $"Successfully processed bulk accounting posting for OperationType: {request.OperationType}.",
                    filtered,
                    HttpStatusCodeEnum.OK,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Information,
                    request.MakeAccountPostingCommands.FirstOrDefault()?.TransactionReferenceId);

                // Return a successful service response
                return ServiceResponse<bool>.ReturnResultWith200(response.Data, "Accounting posting was successful.");
            }
            catch (Exception e)
            {
                // Filter out zero-value amounts for logging in case of an error
                var filtered = FilterZeros(request);

                // Serialize the filtered request data for logging purposes
                string serializedData = JsonConvert.SerializeObject(filtered);

                // Construct an error message with details about the failure
                string errorMessage = $"Failed to process bulk accounting posting for OperationType: {request.OperationType}. Error: {e.Message}";

                // Log the error message
                _logger.LogError(errorMessage);

                // Log and audit the error details
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    filtered,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Error,
                    request.MakeAccountPostingCommands.FirstOrDefault()?.TransactionReferenceId);

                // Return a service response with an error status
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }

        /// <summary>
        /// Filters out zero-value amounts from the request data.
        /// </summary>
        private AddAccountingPostingCommandList FilterZeros(AddAccountingPostingCommandList requests)
        {
            // Create a new filtered request object
            var filteredRequest = new AddAccountingPostingCommandList
            {
                OperationType = requests.OperationType, // Copy operation type

                // Filter MakeAccountPostingCommands to remove entries with zero amounts
                MakeAccountPostingCommands = requests.MakeAccountPostingCommands
                    .Select(request =>
                    {
                        // Filter out zero-value amount collections
                        var filteredAmountCollections = request.AmountCollection
                            .Where(detail => Math.Abs(detail.Amount) > 0)
                            .ToList();

                        // Filter out zero-value event collections
                        var filteredEventCollections = request.AmountEventCollections
                            .Where(detail => Math.Abs(detail.Amount) > 0)
                            .ToList();

                        // Return a new command object with filtered data
                        return new AddAccountingPostingCommand
                        {
                            AmountCollection = filteredAmountCollections,
                            AmountEventCollections = filteredEventCollections,
                            Source = request.Source,
                            ExternalBranchCode = request.ExternalBranchCode,
                            ProductId = request.ProductId,
                            AccountNumber = request.AccountNumber,
                            AccountHolder = request.AccountHolder,
                            ExternalBranchId = request.ExternalBranchId, 
                            IsInterBranchTransaction = request.IsInterBranchTransaction,
                            Naration = request.Naration,
                            OperationType = request.OperationType,
                            ProductName = request.ProductName, MemberReference=request.MemberReference,
                            TransactionDate = request.TransactionDate,
                            TransactionReferenceId = request.TransactionReferenceId
                        };
                    })
                    .ToList()
            };

            return filteredRequest; // Return the filtered request
        }
    }

}
