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

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to add a new Account.
    /// </summary>
    public class BlockAmountInAccountCommandHandler : IRequestHandler<BlockAmountInAccountCommand, ServiceResponse<bool>>
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<BlockAmountInAccountCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly IBlockedAccountRepository _blockedAccountRepository;
        /// <summary>
        /// Constructor for initializing the BlockAmountInAccountCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public BlockAmountInAccountCommandHandler(
            IAccountRepository AccountRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper,
            ILogger<BlockAmountInAccountCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IBlockedAccountRepository blockedAccountRepository = null)
        {

            _AccountRepository = AccountRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = UserInfoToken;
            _uow = uow;
            _blockedAccountRepository = blockedAccountRepository;
        }

        /// <summary>
        /// Handles the BlockAmountInAccountCommand to add a new Account.
        /// </summary>
        /// <param name="request">The BlockAmountInAccountCommand containing Account data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(BlockAmountInAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Input validation
                if (string.IsNullOrEmpty(request.AccountNumber) || request.Amount <= 0)
                {
                    string message = "Invalid account number or amount.";
                    return ServiceResponse<bool>.Return400(message);
                }

                // Fetch account details
                var account = await _AccountRepository.FindBy(x => x.AccountNumber == request.AccountNumber).FirstOrDefaultAsync();
                if (account == null)
                {
                    string message = $"Guarantor's account number {request.AccountNumber} does not exist.";
                    await APICallHelper.AuditLogger(_userInfoToken.FullName, LogAction.Create.ToString(), request, $"{message}, {request.Reason}", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                    return ServiceResponse<bool>.Return403(message);
                }

                // Check if there are sufficient unblocked funds
                if ((account.Balance - account.BlockedAmount) < request.Amount)
                {
                    string message = $"Insufficient funds in account number {request.AccountNumber}.";
                    return ServiceResponse<bool>.Return403(false, message);
                }

                // Generate unique ID and set timestamps
                var currentTime = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                var blockedId = BaseUtilities.GenerateUniqueNumber();

                if (request.Status == "UnBlocked")
                {
                    account.DateReleased = currentTime;
                    account.BlockedAmount = 0;
                }
                else
                {
                    account.DateBlocked = currentTime;
                    account.BlockedId = blockedId;
                    account.ReasonOfBlocked = $"{account.ReasonOfBlocked} {request.Reason} {request.LoanApplicationId}".Trim();
                    account.BlockedAmount += request.Amount;
                }

                _AccountRepository.Update(account);

                // Check for existing blocked account
                var blockedAccount = await _blockedAccountRepository
                    .FindBy(x => x.AccountNumber == request.AccountNumber && x.LoanApplicationId == request.LoanApplicationId)
                    .FirstOrDefaultAsync();

                if (blockedAccount != null)
                {
                    string message = $"Guarantor already guaranteed {BaseUtilities.FormatCurrency(request.Amount)} for this application.";
                    return ServiceResponse<bool>.Return403(false, message);
                }

                // Add new blocked account record
                _blockedAccountRepository.Add(new BlockedAccount
                {
                    Id = blockedId,
                    AccountId = account.Id,
                    Amount = request.Amount,
                    MemberReference = account.CustomerId,
                    Date = currentTime,
                    Reason = account.ReasonOfBlocked,
                    AccountBalance = account.Balance,
                    AccountNumber = account.AccountNumber,
                    Status = request.Status,
                    MemberName = account.CustomerName,
                    LoanApplicationId = request.LoanApplicationId
                });

                await _uow.SaveAsync();

                // Audit log
                await APICallHelper.AuditLogger(
                    _userInfoToken.FullName,
                    LogAction.Create.ToString(),
                    request,
                    $"Blocked account number {account.AccountNumber} for amount {request.Amount}. Reason: {request.Reason}",
                    LogLevelInfo.Information.ToString(),
                    200,
                    _userInfoToken.Token
                );

                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception ex)
            {
                // Log and return error response
                var errorMessage = $"Error in BlockAmountInAccountCommand: {ex.Message}";
                _logger.LogError(ex, errorMessage); // Log with exception stack trace
                await APICallHelper.AuditLogger(
                    _userInfoToken.Email,
                    LogAction.Create.ToString(),
                    request,
                    errorMessage,
                    LogLevelInfo.Error.ToString(),
                    500,
                    _userInfoToken.Token
                );
                return ServiceResponse<bool>.Return500(ex.Message);
            }
        }
    }

}
