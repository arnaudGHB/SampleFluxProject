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
using CBS.TransactionManagement.Data.Entity;
using CBS.TransactionManagement.MediatR.Commands;
using Newtonsoft.Json;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to block a list of accounts with their balances.
    /// </summary>
    public class BlockListOfAccountBalanceCommandHandler : IRequestHandler<BlockListOfAccountBalanceCommand, ServiceResponse<bool>>
    {
        private readonly UserInfoToken _userInfoToken; // Holds the user's information for auditing/logging.
        private readonly IAccountRepository _AccountRepository; // Repository to manage accounts data.
        private readonly IMapper _mapper; // Automapper for object mapping (if needed for mapping DTOs).
        private readonly ILogger<BlockListOfAccountBalanceCommandHandler> _logger; // Logger to log information and errors.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of Work to manage transactions.
        private readonly IBlockedAccountRepository _blockedAccountRepository; // Repository to manage blocked account records.

        // Constructor to inject dependencies required for handling the command.
        public BlockListOfAccountBalanceCommandHandler(
            IAccountRepository AccountRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper,
            ILogger<BlockListOfAccountBalanceCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IBlockedAccountRepository blockedAccountRepository = null) // Optional repository injection for blocked accounts.
        {
            _AccountRepository = AccountRepository; // Assigning the injected account repository.
            _mapper = mapper; // Assigning the injected mapper.
            _logger = logger; // Assigning the injected logger.
            _userInfoToken = UserInfoToken; // Assigning the user's information.
            _uow = uow; // Assigning the unit of work for managing transactions.
            _blockedAccountRepository = blockedAccountRepository; // Assigning the blocked account repository.
        }

        /// <summary>
        /// Handles the BlockListOfAccountBalanceCommand to block or release amounts in accounts based on the request.
        /// </summary>
        public async Task<ServiceResponse<bool>> Handle(BlockListOfAccountBalanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Stringify the account details for logging and auditing
                string accountDetails = JsonConvert.SerializeObject(request.AccountToBlockeds);

                // Check if the operation is a block or release
                if (!request.IsRelease)
                {
                    // Process each account to be blocked
                    foreach (var accountTo in request.AccountToBlockeds)
                    {
                        // Retrieve the account details by account number
                        var account = await _AccountRepository.GetAccountByAccountNumber(accountTo.AccountNumber);

                        // Validate if the account exists
                        if (account == null)
                        {
                            string message = $"Account not found for blocking: {accountTo.AccountNumber}";
                            _logger.LogWarning(message);

                            // Log and audit the failure with account details
                            await BaseUtilities.LogAndAuditAsync(
                                $"{message}, Reason: {accountTo.Reason}, Accounts: {accountDetails}",
                                request,
                                HttpStatusCodeEnum.NotFound,
                                LogAction.BlockAccount,
                                LogLevelInfo.Warning);

                            return ServiceResponse<bool>.Return403(message);
                        }

                        // Check if the account has sufficient funds for blocking
                        decimal availableBalance = account.Balance - account.BlockedAmount;
                        if (availableBalance < accountTo.Amount)
                        {
                            string insufficientFundsMessage = $"Insufficient funds to block {BaseUtilities.FormatCurrency(accountTo.Amount)} in account: {accountTo.AccountNumber}. " +
                                                              $"Available balance: {BaseUtilities.FormatCurrency(availableBalance)}. Accounts: {accountDetails}";
                            _logger.LogWarning(insufficientFundsMessage);

                            // Log and audit the insufficient funds case
                            await BaseUtilities.LogAndAuditAsync(
                                insufficientFundsMessage,
                                request,
                                HttpStatusCodeEnum.BadRequest,
                                LogAction.BlockAccount,
                                LogLevelInfo.Warning);

                            return ServiceResponse<bool>.Return403(insufficientFundsMessage);
                        }

                        // Update account blocking details
                        account.DateBlocked = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                        account.BlockedId += request.LoanApplicationId;
                        account.ReasonOfBlocked += $"{accountTo.Reason} (Loan application Id: {request.LoanApplicationId}), ";
                        account.BlockedAmount += accountTo.Amount;
                        _AccountRepository.Update(account);

                        // Add the blocking entry in the BlockedAccount repository
                        _blockedAccountRepository.Add(new BlockedAccount
                        {
                            Id = BaseUtilities.GenerateUniqueNumber(10),
                            AccountId = account.Id,
                            Amount = accountTo.Amount,
                            Date = account.DateBlocked,
                            Reason = account.ReasonOfBlocked,
                            AccountBalance = account.Balance,
                            AccountNumber = account.AccountNumber, 
                            MemberName = account.CustomerName,
                            MemberReference = account.CustomerId,
                            Status = "Blocked",
                            LoanApplicationId = request.LoanApplicationId
                        });
                    }
                }
                else
                {
                    // Process each account to release
                    foreach (var accountTo in request.AccountToBlockeds)
                    {
                        // Retrieve the account details by account number
                        var account = await _AccountRepository.GetAccountByAccountNumber(accountTo.AccountNumber);

                        // Validate if the account exists
                        if (account == null)
                        {
                            string message = $"Account not found for releasing: {accountTo.AccountNumber}";
                            _logger.LogWarning(message);

                            // Log and audit the failure with account details
                            await BaseUtilities.LogAndAuditAsync(
                                $"{message}, Reason: {accountTo.Reason}, Accounts: {accountDetails}",
                                request,
                                HttpStatusCodeEnum.NotFound,
                                LogAction.BlockAccount,
                                LogLevelInfo.Warning);

                            return ServiceResponse<bool>.Return403(message);
                        }

                        // Update account release details
                        account.ReasonOfBlocked += $"{accountTo.Reason} (Loan application Id: {request.LoanApplicationId}), ";
                        account.BlockedAmount -= accountTo.Amount;
                        _AccountRepository.Update(account);

                        // Add the release entry in the BlockedAccount repository
                        _blockedAccountRepository.Add(new BlockedAccount
                        {
                            Id = BaseUtilities.GenerateUniqueNumber(10),
                            AccountId = account.Id,
                            Amount = accountTo.Amount,
                            Date = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow),
                            Reason = account.ReasonOfBlocked,
                            AccountBalance = account.Balance,
                            AccountNumber = account.AccountNumber,
                            MemberName = account.CustomerName,
                            MemberReference = account.CustomerId,
                            Status = "Released",
                            LoanApplicationId = request.LoanApplicationId
                        });
                    }
                }

                // Commit the changes in the database
                await _uow.SaveAsync();

                // Log and audit the success
                string successMessage = request.IsRelease
                    ? $"Successfully released blocked amounts. Accounts: {accountDetails}"
                    : $"Successfully blocked amounts. Accounts: {accountDetails}";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.BlockAccount,
                    LogLevelInfo.Information);

                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception e)
            {
                string errorMessage = $"Error in BlockListOfAccountBalanceCommand: {e.Message}. Accounts: {JsonConvert.SerializeObject(request.AccountToBlockeds)}";
                _logger.LogError(errorMessage, e);

                // Log and audit the error
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.BlockAccount,
                    LogLevelInfo.Error);

                return ServiceResponse<bool>.Return500(e, errorMessage);
            }
        }
    }

}
