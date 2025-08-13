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
    /// Handles the AddLoanRefundPostingCommand to post loan refund accounting entries.
    /// </summary>
    public class AddLoanRefundPostingCommandHandler : IRequestHandler<AddLoanRefundPostingCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<AddLoanRefundPostingCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for managing database transactions.
        private readonly UserInfoToken _userInfoToken; // Token containing user-specific information.
        private readonly PathHelper _pathHelper; // Helper for constructing API paths.
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository; // Repository for utility services operations.

        /// <summary>
        /// Constructor for initializing the AddLoanRefundPostingCommandHandler.
        /// </summary>
        public AddLoanRefundPostingCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<AddLoanRefundPostingCommandHandler> logger,
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
        /// Handles the AddLoanRefundPostingCommand to post loan refund accounting entries.
        /// </summary>
        /// <param name="request">The AddLoanRefundPostingCommand containing accounting event data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(AddLoanRefundPostingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Filter out any 0 amount from the request
                var filtered = FilterZeros(request);
                string serializedData = JsonConvert.SerializeObject(filtered);

                // Construct the destination URL for the API call
                string destinationUrl = $"{_pathHelper.AccountingBaseURL}{_pathHelper.AccountingPostingLoanRefundURL}";

                // Log the serialized request data to the utility services repository
                await _utilityServicesRepository.CreatAccountingRLog(
                    serializedData,
                    CommandDataType.MakeNonCashAccountAdjustmentCommand,
                    request.TransactionReferenceId,
                    request.TransactionDate,
                    destinationUrl);

                // Make the API call
                var customerData = await APICallHelper.AccountingAPICalls<ServiceResponse<bool>>(
                    _pathHelper,
                    _userInfoToken.Token,
                    serializedData,
                    _pathHelper.AccountingPostingLoanRefundURL);

                // Log and audit the success operation
                string successMessage = "Accounting posting for loan refund was successful.";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage + $" Data: {serializedData}",
                    filtered,
                    HttpStatusCodeEnum.OK,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Information,
                    request.TransactionReferenceId);

                return ServiceResponse<bool>.ReturnResultWith200(customerData.Data, successMessage);
            }
            catch (Exception e)
            {
                // Filter and log the error details
                var filtered = FilterZeros(request);
                string serializedData = JsonConvert.SerializeObject(filtered);
                var errorMessage = $"Error occurred while posting accounting entries: {e.Message}. Data: {serializedData}";

                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    filtered,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Error,
                    request.TransactionReferenceId);

                return ServiceResponse<bool>.Return500("Accounting service failed.");
            }
        }

        /// <summary>
        /// Filters out zero amounts from the AmountCollection in the request.
        /// </summary>
        /// <param name="request">The AddLoanRefundPostingCommand to filter.</param>
        /// <returns>A new AddLoanRefundPostingCommand with filtered data.</returns>
        public AddLoanRefundPostingCommand FilterZeros(AddLoanRefundPostingCommand request)
        {
            var amountCollections = request.AmountCollection
                .Where(detail => Math.Abs(detail.Amount) > 0)
                .ToList();

            return new AddLoanRefundPostingCommand
            {
                AmountCollection = amountCollections,
                AccountNumber = request.AccountNumber,
                Naration = request.Naration,
                Amount = request.Amount,
                BranchId = request.BranchId,
                MemberReference=request.MemberReference,
                IsOldSystemLoan = request.IsOldSystemLoan,
                LoanProductId = request.LoanProductId,
                TellerSource = request.TellerSource,
                TransactionDate = request.TransactionDate,
                LoanRefundCollectionAlpha = request.LoanRefundCollectionAlpha,
                TransactionReferenceId = request.TransactionReferenceId
            };
        }
    }

}
