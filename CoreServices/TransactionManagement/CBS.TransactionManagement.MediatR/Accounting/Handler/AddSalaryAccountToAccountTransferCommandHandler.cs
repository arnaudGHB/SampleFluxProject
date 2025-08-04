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
using CBS.TransactionManagement.MediatR.Accounting.Command;

namespace CBS.TransactionManagement.Handlers
{

    /// <summary>
    /// Handles the command to process a transfer from a salary account to another account via the Accounting API.
    /// </summary>
    public class AddSalaryAccountToAccountTransferCommandHandler : IRequestHandler<AddSalaryAccountToAccountTransferCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<AddSalaryAccountToAccountTransferCommandHandler> _logger; // Logger for logging actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for managing database transactions.
        private readonly UserInfoToken _userInfoToken; // Token containing user-specific information.
        private readonly PathHelper _pathHelper; // Helper for constructing API paths.
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository; // Repository for utility services operations.

        /// <summary>
        /// Constructor for initializing the AddSalaryAccountToAccountTransferCommandHandler.
        /// </summary>
        /// <param name="userInfoToken">User information token for authentication and logging.</param>
        /// <param name="logger">Logger instance for tracking execution flow and errors.</param>
        /// <param name="uow">Unit of work for managing database transactions.</param>
        /// <param name="pathHelper">Path helper for API endpoint management.</param>
        /// <param name="utilityServicesRepository">Repository for logging accounting-related requests.</param>
        public AddSalaryAccountToAccountTransferCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<AddSalaryAccountToAccountTransferCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            PathHelper pathHelper,
            IAPIUtilityServicesRepository utilityServicesRepository)
        {
            _userInfoToken = userInfoToken; // Assign user info token.
            _logger = logger; // Assign logger instance.
            _uow = uow; // Assign unit of work.
            _pathHelper = pathHelper; // Assign path helper.
            _utilityServicesRepository = utilityServicesRepository; // Assign utility services repository.
        }

        /// <summary>
        /// Handles the AddSalaryAccountToAccountTransferCommand to process a transfer posting to the Accounting API.
        /// </summary>
        /// <param name="request">The command containing transfer posting data.</param>
        /// <param name="cancellationToken">A cancellation token to handle request cancellation.</param>
        /// <returns>A service response indicating success or failure of the transfer posting.</returns>
        public async Task<ServiceResponse<bool>> Handle(AddSalaryAccountToAccountTransferCommand request, CancellationToken cancellationToken)
        {
            // Step 1: Serialize the request data for logging
            string serializedData = JsonConvert.SerializeObject(request);

            try
            {
                // Step 2: Construct the API endpoint for the transfer
                string destinationUrl = $"{_pathHelper.AccountingBaseURL}{_pathHelper.AccountToAccountTransferByTransctionCodeURL}";

                // Step 3: Log the request in the accounting request repository before API call
                await _utilityServicesRepository.CreatAccountingRLogBG(
                    serializedData,
                    CommandDataType.AddSalaryAccountToAccountTransferCommand,
                    request.ReferenceCode,
                    request.AccountingDate,
                    destinationUrl, request.UserInfoToken);

                // Step 4: Call the Accounting API to process the transfer
                var apiResponse = await APICallHelper.GetObjectById<ServiceResponse<bool>>(
                    _pathHelper.AccountingBaseURL,
                    _pathHelper.AccountToAccountTransferByTransctionCodeURL,
                    request.UserInfoToken.Token,
                    request.ReferenceCode
                );

                // Step 5: Log and audit the successful accounting posting
                string successMessage = $"[SUCCESS] Accounting posting for transfer successfully completed. Reference Code: {request.ReferenceCode}.";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage + $" Data: {serializedData}", request, HttpStatusCodeEnum.OK, LogAction.AccountingPosting, LogLevelInfo.Information, request.UserInfoToken.FullName, request.UserInfoToken.Token, request.ReferenceCode);

 

                // Step 6: Return success response
                return ServiceResponse<bool>.ReturnResultWith200(apiResponse.Data, successMessage);
            }
            catch (Exception e)
            {
                // Step 7: Handle and log any exceptions
                string errorMessage = $"[ERROR] Accounting transfer posting failed. Reference Code: {request.ReferenceCode}. Error: {e.Message}. Data: {serializedData}";
                _logger.LogError(errorMessage);

                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.AccountingPosting, LogLevelInfo.Error, request.UserInfoToken.FullName, request.UserInfoToken.Token, request.ReferenceCode);
                // Step 8: Return error response
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }
    }

}

