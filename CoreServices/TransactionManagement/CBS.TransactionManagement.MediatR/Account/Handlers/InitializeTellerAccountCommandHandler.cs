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
    /// Handles the command to update an Account based on InitializeTellerAccountCommand.
    /// </summary>
    public class InitializeTellerAccountCommandHandler : IRequestHandler<InitializeTellerAccountCommand, ServiceResponse<AccountDto>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly ILogger<InitializeTellerAccountCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the InitializeTellerAccountCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public InitializeTellerAccountCommandHandler(
            IAccountRepository AccountRepository,
            UserInfoToken userInfoToken,
            ILogger<InitializeTellerAccountCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null)
        {
            _AccountRepository = AccountRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the InitializeTellerAccountCommand to update an Account.
        /// </summary>
        /// <param name="request">The InitializeTellerAccountCommand containing updated Account data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountDto>> Handle(InitializeTellerAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Account entity to be updated from the repository
                var existingAccount = await _AccountRepository.FindAsync(request.Id);

                // Check if the Account entity exists
                if (existingAccount != null)
                {
                    if (!existingAccount.Status.Equals(AccountStatus.Closed.ToString()))
                    {
                        // Log and return a 400 Bad Request response with an error message
                        string errorMessage = $"{request.Id} has already been opened";
                        _logger.LogError(errorMessage);
                        await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                        return ServiceResponse<AccountDto>.Return409(errorMessage);
                    }

                    // Update Account entity properties with values from the request
                    existingAccount.Balance = request.Amount;
                    existingAccount.PreviousBalance = 0;
                    existingAccount.Status = request.Status;
                    existingAccount.EncryptedBalance = BalanceEncryption.Encrypt(existingAccount.Balance.ToString(), existingAccount.AccountNumber);

                    // Use the repository to update the existing Account entity
                    _AccountRepository.Update(existingAccount);

                    // Save changes using Unit of Work
                    if (await _uow.SaveAsync() <= 0)
                    {
                        await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, "An error occurred initializing this account", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                        return ServiceResponse<AccountDto>.Return500();
                    }

                    // Prepare the response and return a successful response with 200 Status code
                    var response = ServiceResponse<AccountDto>.ReturnResultWith200(_mapper.Map<AccountDto>(existingAccount));
                    _logger.LogInformation($"Account {request.Id} was successfully initialized.");
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, $"Account {request.Id} was successfully initialized.", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                    return response;
                }
                else
                {
                    // If the Account entity was not found, return a 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<AccountDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Account: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<AccountDto>.Return500(e);
            }
        }
    }
}
