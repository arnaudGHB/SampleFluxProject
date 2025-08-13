using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Helper.Helper;

namespace CBS.AccountManagement.Handlers
{
    /// <summary>
    /// Handles the command to update an Account based on UpdateAccountBalanceCommand.
    /// </summary>
    public class UpdateAccountBalanceCommandHandler : IRequestHandler<UpdateAccountBalanceCommand, ServiceResponse<bool>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly ILogger<UpdateAccountBalanceCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the UpdateAccountBalanceCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateAccountBalanceCommandHandler(
            IAccountRepository AccountRepository,
            ILogger<UpdateAccountBalanceCommandHandler> logger,
            UserInfoToken userInfoToken,
            IUnitOfWork<TransactionContext> uow = null)
        {
            _AccountRepository = AccountRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateAccountBalanceCommand to update an Account.
        /// </summary>
        /// <param name="request">The UpdateAccountBalanceCommand containing updated Account data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(UpdateAccountBalanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the Account entity to be updated from the repository
                var existingAccount = await _AccountRepository.FindAsync(request.Id);

                // Step 2: Check if the Account entity exists
                if (existingAccount != null)
                {
                    // Step 3: Update Account entity properties with values from the request
                    existingAccount.PreviousBalance = existingAccount.Balance; // Save the current balance as the previous balance
                    if (request.IsDebit)
                    {
                        existingAccount.Balance -= request.Amount; // Update the balance by adding the requested amount
                    }
                    else
                    {
                        existingAccount.Balance += request.Amount; // Update the balance by adding the requested amount
                    }
                    existingAccount.EncryptedBalance = BalanceEncryption.Encrypt(existingAccount.Balance.ToString(), existingAccount.AccountNumber); // Encrypt the updated balance
                    existingAccount.LastOperation = request.LoastOperation;
                    existingAccount.LastInterestCalculatedDate = BaseUtilities.UtcNowToDoualaTime();
                    existingAccount.DateOfLastOperation = BaseUtilities.UtcNowToDoualaTime();
                    existingAccount.LastOperationAmount = request.Amount;
                    
                    // Step 4: Use the repository to update the existing Account entity
                    _AccountRepository.Update(existingAccount);

                    // Step 5: Save changes using Unit of Work
                    await _uow.SaveAsync();

                    // Step 6: Log success message
                    string msg = $"Account balance for customer {existingAccount.CustomerId} updated successfully. New balance {request.Amount}. Existing balance: {existingAccount.PreviousBalance}";
                    var logTask = Task.Run(() => _logger.LogInformation(msg));

                    // Step 7: Audit log the update action
                    var auditTask = APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, msg, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    // Step 8: Await parallel tasks
                    await Task.WhenAll(logTask, auditTask);

                    // Step 9: Prepare the response and return a successful response with 200 Status code
                    var response = ServiceResponse<bool>.ReturnResultWith200(true, msg);
                    return response;
                }
                else
                {
                    // Step 10: If the Account entity was not found, log an error and return a 404 Not Found response with an error message
                    string errorMessage = $"Account was not found to be updated.";
                    var logTask = Task.Run(() => _logger.LogError(errorMessage));

                    // Step 11: Audit log the failed update attempt
                    var auditTask = APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    // Step 12: Await parallel tasks
                    await Task.WhenAll(logTask, auditTask);

                    return ServiceResponse<bool>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Step 13: Log error and return a 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Account: {e.Message}";
                var logTask = Task.Run(() => _logger.LogError(errorMessage));

                // Step 14: Audit log the error
                var auditTask = APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);

                // Step 15: Await parallel tasks
                await Task.WhenAll(logTask, auditTask);

                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}
