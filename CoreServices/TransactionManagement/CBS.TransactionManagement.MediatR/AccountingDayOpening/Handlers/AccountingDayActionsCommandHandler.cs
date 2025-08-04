using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Data.Dto.AccountingDayOpening;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.MediatR.AccountingDayOpening.Commands;

namespace CBS.TransactionManagement.Handlers.AccountingDayOpening
{
    /// <summary>
    /// Command handler for performing actions on an accounting day such as opening, closing, or reopening.
    /// </summary>
    public class AccountingDayActionsCommandHandler : IRequestHandler<AccountingDayActionsCommand, ServiceResponse<CloseOrOpenAccountingDayResultDto>>
    {
        private readonly ILogger<AccountingDayActionsCommandHandler> _logger; // Logger for actions and errors
        private readonly UserInfoToken _userInfoToken; // User information for context and auditing
        private readonly IAccountingDayRepository _accountingDayRepository; // Repository for accounting day data access

        /// <summary>
        /// Constructor to initialize the handler with dependencies.
        /// </summary>
        /// <param name="userInfoToken">User information token for auditing.</param>
        /// <param name="logger">Logger for logging actions and errors.</param>
        /// <param name="accountingDayRepository">Repository for accessing accounting day data.</param>
        public AccountingDayActionsCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<AccountingDayActionsCommandHandler> logger,
            IAccountingDayRepository accountingDayRepository)
        {
            _userInfoToken = userInfoToken ?? throw new ArgumentNullException(nameof(userInfoToken));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _accountingDayRepository = accountingDayRepository ?? throw new ArgumentNullException(nameof(accountingDayRepository));
        }

        /// <summary>
        /// Handles the AccountingDayActionsCommand to perform actions on the accounting day.
        /// </summary>
        /// <param name="request">The command containing action details.</param>
        /// <param name="cancellationToken">Cancellation token for request cancellation.</param>
        /// <returns>A service response indicating the success or failure of the operation.</returns>
        public async Task<ServiceResponse<CloseOrOpenAccountingDayResultDto>> Handle(AccountingDayActionsCommand request, CancellationToken cancellationToken)
        {
            // Ensure request data is valid
            if (request == null)
            {
                var invalidRequestMessage = "The AccountingDayActionsCommand request is null.";
                _logger.LogError(invalidRequestMessage);
                return ServiceResponse<CloseOrOpenAccountingDayResultDto>.Return400(invalidRequestMessage);
            }

            try
            {
                // Perform the action on the accounting day
                var result = await _accountingDayRepository.AccountingDayAction(request.Id, request.Option);

                // Log and audit success
                string successMessage = result.Message;
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.AccountingDayAction, LogLevelInfo.Information);

                // Return success response with details
                return ServiceResponse<CloseOrOpenAccountingDayResultDto>.ReturnResultWith200(result, successMessage);
            }
            catch (Exception ex)
            {
                // Construct error message with exception details
                string errorMessage = $"An error occurred while performing '{request.Option}' on accounting day with ID {request.Id}: {ex.Message}";

                // Log and audit the error
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.AccountingDayAction, LogLevelInfo.Error);

                // Return error response
                return ServiceResponse<CloseOrOpenAccountingDayResultDto>.Return500(errorMessage);
            }
        }
    }

}
