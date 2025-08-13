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
    /// Handles the command to process loan disbursement for normal or new loans.
    /// </summary>
    public class AddDisbursmementPostingCommandHandler : IRequestHandler<AddDisbursmementPostingCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<AddDisbursmementPostingCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for managing transactions.
        private readonly IAPIUtilityServicesRepository _accountingPostingConfirmationRepository; // Repository for accounting posting confirmations.
        private readonly UserInfoToken _userInfoToken; // Token for user-specific information.
        private readonly PathHelper _pathHelper; // Helper for constructing API paths.

        /// <summary>
        /// Initializes a new instance of the AddDisbursmementPostingCommandHandler class.
        /// </summary>
        /// <param name="userInfoToken">Token containing user information.</param>
        /// <param name="logger">Logger for recording operations.</param>
        /// <param name="uow">Unit of work for database transactions.</param>
        /// <param name="pathHelper">Helper for retrieving API paths.</param>
        /// <param name="accountingPostingConfirmationRepository">Repository for accounting confirmation records.</param>
        public AddDisbursmementPostingCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<AddDisbursmementPostingCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            PathHelper pathHelper,
            IAPIUtilityServicesRepository accountingPostingConfirmationRepository = null)
        {
            _userInfoToken = userInfoToken; // Assign user info token
            _logger = logger; // Assign logger instance
            _uow = uow; // Assign unit of work
            _pathHelper = pathHelper; // Assign path helper
            _accountingPostingConfirmationRepository = accountingPostingConfirmationRepository; // Assign repository
        }

        /// <summary>
        /// Handles the loan disbursement posting process for normal or new loans.
        /// </summary>
        /// <param name="request">The command containing loan disbursement details.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A response indicating the success or failure of the operation.</returns>
        public async Task<ServiceResponse<bool>> Handle(AddDisbursmementPostingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Serialize the request to JSON format for the API call
                string requestData = JsonConvert.SerializeObject(request);

                // Construct the destination URL for the API call
                string destinationUrl = $"{_pathHelper.AccountingBaseURL}{_pathHelper.MakeLoanDisbursementPostingURL}";

                // Log the serialized request data to the utility services repository
                await _accountingPostingConfirmationRepository?.CreatAccountingRLog(
                    requestData,
                    CommandDataType.LoanDisbursementPostingCommand,
                    request.TransactionReferenceId,
                    request.TransactionDate,
                    destinationUrl);

                // Call the accounting service API for loan disbursement
                var apiResponse = await APICallHelper.AccountingAPICalls<ServiceResponse<bool>>(
                    _pathHelper,
                    _userInfoToken.Token,
                    requestData,
                    _pathHelper.MakeLoanDisbursementPostingURL
                );

                // Log and audit the successful operation
                string successMessage = $"Loan disbursement processed successfully. Transaction Reference: {request.TransactionReferenceId}";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Information,
                    request.TransactionReferenceId
                );

                // Return the success response
                return ServiceResponse<bool>.ReturnResultWith200(apiResponse.Data, "Disbursement was successful.");
            }
            catch (Exception ex)
            {
                // Serialize the request for debugging
                string requestData = JsonConvert.SerializeObject(request);

                // Log and audit the error with full context
                string errorMessage = $"Loan disbursement failed. Transaction Reference: {request.TransactionReferenceId}. Error: {ex.Message}";
                _logger.LogError(errorMessage);

                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Error,
                    request.TransactionReferenceId
                );

                // Return an error response with detailed context
                return ServiceResponse<bool>.Return500(ex, "An error occurred while processing the loan disbursement.");
            }
        }
    }

}
///api/v1/Account/Balance/Customer/{0}