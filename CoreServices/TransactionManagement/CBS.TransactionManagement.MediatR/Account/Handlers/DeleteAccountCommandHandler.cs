using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the deletion of a user account. This marks the account as deleted instead of permanent removal.
    /// </summary>
    public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, ServiceResponse<bool>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly ILogger<DeleteAccountCommandHandler> _logger;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Initializes the DeleteAccountCommandHandler with required dependencies.
        /// </summary>
        /// <param name="accountRepository">Repository for account operations.</param>
        /// <param name="uow">Unit of work for managing transactions.</param>
        /// <param name="logger">Logger instance for error tracking and auditing.</param>
        /// <param name="userInfoToken">User information for authorization and logging.</param>
        public DeleteAccountCommandHandler(
            IAccountRepository accountRepository,
            IUnitOfWork<TransactionContext> uow,
            ILogger<DeleteAccountCommandHandler> logger,
            UserInfoToken userInfoToken)
        {
            _accountRepository = accountRepository;
            _uow = uow;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the account deletion request by marking it as deleted in the system.
        /// </summary>
        /// <param name="request">Contains the AccountId to be deleted.</param>
        /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
        /// <returns>A response indicating success or failure.</returns>
        public async Task<ServiceResponse<bool>> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Validate the Account ID
                if (string.IsNullOrEmpty(request.AccountId))
                {
                    var errorMessage = "[ERROR] Account deletion failed: Invalid or missing Account ID.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.Delete, LogLevelInfo.Error);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }

                // Step 2: Retrieve the account details
                var account = await _accountRepository.FindBy(a => a.Id == request.AccountId && !a.IsDeleted)
                    .FirstOrDefaultAsync();

                // Step 3: Check if the account exists
                if (account == null)
                {
                    var errorMessage = $"[ERROR] Account deletion failed: Account with ID {request.AccountId} not found or already deleted.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.Delete, LogLevelInfo.Error);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                // Step 4: Mark account as deleted (Soft Delete)
                account.IsDeleted = true;
                _accountRepository.Update(account);
                await _uow.SaveAsync();

                // Step 5: Log the successful deletion
                string successMessage = $"[SUCCESS] Account successfully deleted: [Account ID: {request.AccountId}, Name: {account.AccountName}, Type: {account.AccountType}].";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.Delete, LogLevelInfo.Information);

                // Step 6: Return success response
                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception ex)
            {
                // Step 7: Log and audit the exception
                var errorMessage = $"[CRITICAL ERROR] Account deletion failed for ID {request.AccountId}. Exception: {ex.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.Delete, LogLevelInfo.Error);

                // Step 8: Return error response
                return ServiceResponse<bool>.Return500(ex);
            }
        }
    }

}
