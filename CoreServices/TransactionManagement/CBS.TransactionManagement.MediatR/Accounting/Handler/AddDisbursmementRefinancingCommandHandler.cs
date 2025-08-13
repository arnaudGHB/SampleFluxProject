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
    /// Handles the command to process loan disbursement refinancing.
    /// </summary>
    public class AddDisbursmementRefinancingCommandHandler : IRequestHandler<AddDisbursmementRefinancingCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<AddDisbursmementRefinancingCommandHandler> _logger; // Logger for logging actions and errors
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for managing database transactions
        private readonly UserInfoToken _userInfoToken; // Token containing user-specific information
        private readonly PathHelper _pathHelper; // Helper for constructing API paths
        private readonly IAPIUtilityServicesRepository _accountingPostingConfirmationRepository; // Repository for accounting posting confirmations.

        /// <summary>
        /// Initializes a new instance of the AddDisbursmementRefinancingCommandHandler class.
        /// </summary>
        /// <param name="userInfoToken">Token containing user information.</param>
        /// <param name="logger">Logger for recording operations.</param>
        /// <param name="uow">Unit of work for database transactions.</param>
        /// <param name="pathHelper">Helper for retrieving API paths.</param>
        public AddDisbursmementRefinancingCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<AddDisbursmementRefinancingCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            PathHelper pathHelper,
            IAPIUtilityServicesRepository accountingPostingConfirmationRepository)
        {
            _userInfoToken = userInfoToken; // Assign user info token
            _logger = logger; // Assign logger instance
            _uow = uow; // Assign unit of work
            _pathHelper = pathHelper; // Assign path helper
            _accountingPostingConfirmationRepository=accountingPostingConfirmationRepository;
        }

        /// <summary>
        /// Handles the loan disbursement refinancing command.
        /// </summary>
        /// <param name="request">The disbursement command containing transaction details.</param>
        /// <param name="cancellationToken">A token for cancelling the operation.</param>
        public async Task<ServiceResponse<bool>> Handle(AddDisbursmementRefinancingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Serialize the request to JSON format
                string requestData = JsonConvert.SerializeObject(request);

                // Construct the destination URL for the API call
                string destinationUrl = $"{_pathHelper.AccountingBaseURL}{_pathHelper.LoanRefinancingPostingURL}";
                // Log the serialized request data to the utility services repository
                await _accountingPostingConfirmationRepository?.CreatAccountingRLog(
                    requestData,
                    CommandDataType.LoanDisbursementRefinancingPostingCommand,
                    request.TransactionReferenceId,
                    request.TransactionDate,
                    destinationUrl);

                // Log the serialized request data to the utility services repository
                await BaseUtilities.LogAndAuditAsync(
                    $"Initiating Loan Refinancing Posting for Transaction Reference: {request.TransactionReferenceId}",
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Information,
                    request.TransactionReferenceId);

                // Make API call to the accounting service
                var apiResponse = await APICallHelper.AccountingAPICalls<ServiceResponse<bool>>(
                    _pathHelper,
                    _userInfoToken.Token,
                    requestData,
                    _pathHelper.LoanRefinancingPostingURL);

                // Log and audit the successful operation
                string successMessage = $"Loan Disbursement Refinancing successful for [{request.DisbursementType}], Transaction Reference: {request.TransactionReferenceId}";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Information,
                    request.TransactionReferenceId);

                // Return the success response
                return ServiceResponse<bool>.ReturnResultWith200(apiResponse.Data, "Disbursement was successful.");
            }
            catch (Exception ex)
            {
                // Serialize the request for debugging
                string requestData = JsonConvert.SerializeObject(request);

                // Handle exceptions and log detailed error
                string errorMessage = $"Loan Disbursement Refinancing failed for [{request.DisbursementType}], Transaction Reference: {request.TransactionReferenceId}. Error: {ex.Message}";

                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Error,
                    request.TransactionReferenceId);

                // Return error response
                return ServiceResponse<bool>.Return500(ex, "An error occurred while processing loan disbursement.");
            }
        }
    }

}
///api/v1/Account/Balance/Customer/{0}