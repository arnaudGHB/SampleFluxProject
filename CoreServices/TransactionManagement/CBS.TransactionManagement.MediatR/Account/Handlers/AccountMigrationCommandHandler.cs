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
using CBS.TransactionManagement.Helper.Helper;
using Microsoft.Extensions.DependencyInjection;
using CBS.TransactionManagement.MediatR.AccountMigrationBGService;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the account migration process by validating input, retrieving necessary data, and queuing the migration request.
    /// </summary>
    public class AccountMigrationCommandHandler : IRequestHandler<AccountMigrationCommand, ServiceResponse<bool>>
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly ISavingProductRepository _savingProductRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountMigrationCommandHandler> _logger;
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITellerRepository _tellerRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly AccountMigrationQueue _accountMigrationQueue;

        public AccountMigrationCommandHandler(
            ISavingProductRepository savingProductRepository,
            IAccountRepository accountRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<AccountMigrationCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IServiceProvider serviceProvider,
            ITellerRepository tellerRepository,
            ITransactionRepository transactionRepository,
            IServiceScopeFactory serviceScopeFactory,
            AccountMigrationQueue accountMigrationQueue)
        {
            _savingProductRepository = savingProductRepository ?? throw new ArgumentNullException(nameof(savingProductRepository));
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _userInfoToken = userInfoToken ?? throw new ArgumentNullException(nameof(userInfoToken));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _tellerRepository = tellerRepository ?? throw new ArgumentNullException(nameof(tellerRepository));
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _accountMigrationQueue = accountMigrationQueue ?? throw new ArgumentNullException(nameof(accountMigrationQueue));
        }

        /// <summary>
        /// Processes the account migration command by validating input, retrieving relevant data, and queuing the request.
        /// </summary>
        /// <param name="request">The account migration command.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns a service response indicating success or failure.</returns>
        public async Task<ServiceResponse<bool>> Handle(AccountMigrationCommand request, CancellationToken cancellationToken)
        {
            request.UserInfoToken = _userInfoToken;

            try
            {
                // ✅ Validate Input
                if (string.IsNullOrEmpty(request.BankId) || string.IsNullOrEmpty(request.BranchId))
                {
                    string validationMessage = "Validation failed: BankId or BranchId is missing.";
                    await BaseUtilities.LogAndAuditAsync(validationMessage, "AccountMigration", HttpStatusCodeEnum.BadRequest, LogAction.AccountBalanceMigrationProcessing, LogLevelInfo.Warning, request.UserInfoToken.FullName, request.UserInfoToken.Token, request.CorrelationId);
                    return ServiceResponse<bool>.Return409("Please set the BankId and BranchId fields.");
                }

                // ✅ Retrieve Saving Product
                var product = await _savingProductRepository.FindAsync(request.ProductId);
                if (product == null)
                {
                    string productNotFoundMessage = $"Product not found: {request.ProductId}";
                    await BaseUtilities.LogAndAuditAsync(productNotFoundMessage, "AccountMigration", HttpStatusCodeEnum.NotFound, LogAction.AccountBalanceMigrationProcessing, LogLevelInfo.Warning, request.UserInfoToken.FullName, request.UserInfoToken.Token, request.CorrelationId);
                    return ServiceResponse<bool>.Return409("Sorry, this product does not exist.");
                }

                // ✅ Retrieve Teller Information
                var teller = await _tellerRepository.GetTellerByOperationType(OperationType.NoneCash.ToString(), request.BranchId);
                if (teller == null)
                {
                    string tellerNotConfiguredMessage = $"None Cash Teller is not configured for BranchId: {request.BranchId}";
                    await BaseUtilities.LogAndAuditAsync(tellerNotConfiguredMessage, "AccountMigration", HttpStatusCodeEnum.Forbidden, LogAction.AccountBalanceMigrationProcessing, LogLevelInfo.Warning, request.UserInfoToken.FullName, request.UserInfoToken.Token, request.CorrelationId);
                    return ServiceResponse<bool>.Return403(false, "None Cash Teller is not configured for the selected branch.");
                }

                // ✅ Enqueue Migration Request
                string queueMessage = $"Queuing migration request for {request.Accounts.Count} accounts.";
                await BaseUtilities.LogAndAuditAsync(queueMessage, "AccountMigration", HttpStatusCodeEnum.OK, LogAction.AccountBalanceMigrationQueueProcessing, LogLevelInfo.Information, request.UserInfoToken.FullName, request.UserInfoToken.Token, request.CorrelationId);
                await _accountMigrationQueue.EnqueueAsync(request);

                return ServiceResponse<bool>.ReturnResultWith200(true, "Your migration request has been queued to be executed in background.");
            }
            catch (Exception e)
            {
                string errorMessage = $"Error occurred while processing the account migration request: {e.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, "AccountMigration", HttpStatusCodeEnum.InternalServerError, LogAction.AccountBalanceMigrationProcessing, LogLevelInfo.Error, request.UserInfoToken.FullName, request.UserInfoToken.Token, request.CorrelationId);
                _logger.LogError(e, errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}
