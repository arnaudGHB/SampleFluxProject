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
    /// Handles the command to add a new AccountingEvent for loan approval.
    /// </summary>
    public class AdLoanApprovalCommandHandler : IRequestHandler<AdLoanApprovalCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<AdLoanApprovalCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for database transactions.
        private readonly UserInfoToken _userInfoToken; // Token for user information.
        private readonly PathHelper _pathHelper; // Helper for constructing API paths.
        private readonly IAPIUtilityServicesRepository _accountingPostingConfirmationRepository; // Repository for utility service logs.

        /// <summary>
        /// Constructor for initializing the AdLoanApprovalCommandHandler.
        /// </summary>
        public AdLoanApprovalCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<AdLoanApprovalCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            PathHelper pathHelper,
            IAPIUtilityServicesRepository accountingPostingConfirmationRepository)
        {
            _userInfoToken = userInfoToken; // Assigning the provided user info token.
            _logger = logger; // Assigning the logger instance.
            _uow = uow; // Assigning the unit of work.
            _pathHelper = pathHelper; // Assigning the path helper.
            _accountingPostingConfirmationRepository = accountingPostingConfirmationRepository; // Assigning the utility services repository.
        }

        /// <summary>
        /// Handles the AdLoanApprovalCommand to process loan approval postings.
        /// </summary>
        /// <param name="request">The command containing loan approval data.</param>
        /// <param name="cancellationToken">Token to signal task cancellation.</param>
        /// <returns>ServiceResponse indicating the result of the operation.</returns>
        public async Task<ServiceResponse<bool>> Handle(AdLoanApprovalCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Serialize the request into JSON for API submission
                string serializedData = JsonConvert.SerializeObject(request);

                // Construct the destination URL for the loan approval API call
                string destinationUrl = $"{_pathHelper.AccountingBaseURL}{_pathHelper.LoanApprovalPostingURL}";

                // Log the serialized request data to the utility services repository
                await _accountingPostingConfirmationRepository?.CreatAccountingRLog(
                    serializedData, // Serialized request data
                    CommandDataType.LoanApprovalPostingCommand, // Command data type (specific to loan approval)
                    request.TransactionReferenceId, // Transaction reference ID
                    request.TransactionDate, // Transaction date
                    destinationUrl); // Destination URL for the loan approval API

                // Make the API call to process the loan approval posting
                var response = await APICallHelper.AccountingAPICalls<ServiceResponse<bool>>(
                    _pathHelper,
                    _userInfoToken.Token,
                    serializedData,
                    _pathHelper.LoanApprovalPostingURL);

                // Log and audit the success of the operation
                string successMessage = $"Loan approval accounting posting successful. Transaction Reference: {request.TransactionReferenceId}.";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage, // Success message for audit
                    request, // Original request object
                    HttpStatusCodeEnum.OK, // HTTP status code for success
                    LogAction.LoanApprovalPostingCommand, // Log action specific to loan approval posting
                    LogLevelInfo.Information, // Log level for information
                    request.TransactionReferenceId); // Reference ID for tracking

                // Return success response with data from the API
                return ServiceResponse<bool>.ReturnResultWith200(response.Data, "Loan approval accounting posting was successful.");
            }
            catch (Exception ex)
            {
                // Serialize the request for error debugging
                string serializedData = JsonConvert.SerializeObject(request);

                // Log and audit the error that occurred during the operation
                string errorMessage = $"Error occurred during loan approval accounting posting. Transaction Reference: {request.TransactionReferenceId}. Error: {ex.Message}";
                _logger.LogError(ex, errorMessage); // Log the error with exception details
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage, // Error message for audit
                    request, // Original request object
                    HttpStatusCodeEnum.InternalServerError, // HTTP status code for failure
                    LogAction.LoanApprovalPostingCommand, // Log action specific to loan approval posting
                    LogLevelInfo.Error, // Log level for error
                    request.TransactionReferenceId); // Reference ID for tracking

                // Return a 500 Internal Server Error response with error details
                return ServiceResponse<bool>.Return500(ex, "Loan approval accounting returned an error.");
            }
        }
    }
}
