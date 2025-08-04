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
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Commands.WithdrawalNotificationP;
using CBS.TransactionManagement.Data.Dto.WithdrawalNotificationP;
using CBS.TransactionManagement.Repository.WithdrawalNotificationP;
using CBS.TransactionManagement.Data.Entity.WithdrawalNotificationP;
using Microsoft.Identity.Client;

namespace CBS.TransactionManagement.Handlers.WithdrawalNotificationP
{
    /// <summary>
    /// Handles the command to add a new WithdrawalLimits.
    /// </summary>
    public class AddWithdrawalNotificationRequestCommandHandler : IRequestHandler<AddWithdrawalNotificationRequestCommand, ServiceResponse<WithdrawalNotificationDto>>
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly IWithdrawalNotificationRepository _WithdrawalNotificationRepository;
        private readonly IAccountRepository _IAccountRepository;

        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddWithdrawalNotificationRequestCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Constructor for initializing the AddWithdrawalNotificationRequestCommandHandler.
        /// </summary>
        public AddWithdrawalNotificationRequestCommandHandler(
            IMapper mapper,
            UserInfoToken userInfoToken,
            IWithdrawalNotificationRepository withdrawalNotificationRepository,
            ILogger<AddWithdrawalNotificationRequestCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null,
            IAccountRepository iAccountRepository = null)
        {
            _WithdrawalNotificationRepository = withdrawalNotificationRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
            _IAccountRepository = iAccountRepository;
        }

        /// <summary>
        /// Handles the AddWithdrawalNotificationRequestCommand to add a new WithdrawalNotification.
        /// </summary>
        public async Task<ServiceResponse<WithdrawalNotificationDto>> Handle(AddWithdrawalNotificationRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if there is an existing withdrawal notification for the given customer and account
                var existingWithdrawalNotification = await GetWithdrawalNotification(request.CustomerId, request.AccountNumber);
                if (existingWithdrawalNotification != null)
                {
                    return HandleWithdrawalNotificationInProgress(request);
                }
                var account=await GetAccountBalance(request.AccountNumber, request.AmountRequired);
                // Map command to WithdrawalNotification entity
                var withdrawalNotificationEntity = MapToWithdrawalNotification(request, existingWithdrawalNotification);
                withdrawalNotificationEntity.ApprovalStatus = Status.Pending.ToString();
                withdrawalNotificationEntity.InitiatingBranchId = _userInfoToken.BranchID;
                withdrawalNotificationEntity.MemberBranchId = account.BranchId;
                withdrawalNotificationEntity.NotificationDate = BaseUtilities.UtcNowToDoualaTime();
                withdrawalNotificationEntity.ServiceClearName = _userInfoToken.FullName;
                withdrawalNotificationEntity.ApprovalDate = DateTime.MinValue;
                withdrawalNotificationEntity.AccountId = account.Id;
                withdrawalNotificationEntity.AccountBalance = account.Balance;
                // Add the withdrawal notification to the repository
                await AddWithdrawalNotificationAsync(withdrawalNotificationEntity);

                // Map entity back to DTO for response
                var withdrawalNotificationDto = _mapper.Map<WithdrawalNotificationDto>(withdrawalNotificationEntity);
                string message = "Withdrawal notification created successfully.";
                return ServiceResponse<WithdrawalNotificationDto>.ReturnResultWith200(withdrawalNotificationDto, message);
            }
            catch (Exception e)
            {
                return HandleErrorSavingWithdrawalNotification(e);
            }
        }

        // Other methods...

        // Method to handle case where a withdrawal notification is already in progress
        private ServiceResponse<WithdrawalNotificationDto> HandleWithdrawalNotificationInProgress(AddWithdrawalNotificationRequestCommand request)
        {
            var errorMessage = "Member has a pending withdrawal request.";
            _logger.LogError(errorMessage);
            LogAndAuditError(request, errorMessage, 409).Wait();
            return ServiceResponse<WithdrawalNotificationDto>.Return409(errorMessage);
        }

        // Method to add withdrawal notification to the repository
        private async Task AddWithdrawalNotificationAsync(WithdrawalNotification withdrawalNotification)
        {
            _WithdrawalNotificationRepository.Add(withdrawalNotification);
            await _uow.SaveAsync();
        }

        // Method to log and audit error during withdrawal notification handling
        private async Task LogAndAuditError(AddWithdrawalNotificationRequestCommand request, string errorMessage, int statusCode)
        {
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), statusCode, _userInfoToken.Token);
        }

        // Method to handle error while saving withdrawal notification
        private ServiceResponse<WithdrawalNotificationDto> HandleErrorSavingWithdrawalNotification(Exception e)
        {
            var errorMessage = $"Error occurred while saving withdrawal notification: {e.Message}";
            _logger.LogError(errorMessage);
            return ServiceResponse<WithdrawalNotificationDto>.Return500(e);
        }
        // Method to retrieve account by account number
        private async Task<Account> GetAccountBalance(string accountNumber, decimal amountRequested)
        {
            var account = await _IAccountRepository.FindBy(x => x.AccountNumber == accountNumber).FirstOrDefaultAsync();

            if (account == null)
            {
                var errorMessage = $"Account number {accountNumber} does not exist.";

                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
            if (account.Balance - (amountRequested + account.BlockedAmount) <= 0)
            {
                var errorMessage = $"Insufficient fund in account number: {accountNumber}. Current balance: {BaseUtilities.FormatCurrency(account.Balance)}. Amount requested: {BaseUtilities.FormatCurrency(amountRequested)}, Blocked Amount: {BaseUtilities.FormatCurrency(account.BlockedAmount)}, Total: {BaseUtilities.FormatCurrency(account.Balance + account.BlockedAmount)}";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
            return account;
        }
        // Method to retrieve withdrawal notification by customer ID and account number
        private async Task<WithdrawalNotification> GetWithdrawalNotification(string customerId, string accountNumber)
        {
            return await _WithdrawalNotificationRepository.FindBy(x => x.CustomerId == customerId && x.AccountNumber == accountNumber && x.GracePeriodDate.Date >= DateTime.Now.Date && x.IsDeleted==false).FirstOrDefaultAsync();
        }
        // Method to map the command to a WithdrawalNotification entity
        private WithdrawalNotification MapToWithdrawalNotification(AddWithdrawalNotificationRequestCommand request, WithdrawalNotification existingWithdrawalNotification)
        {
            // Map command properties to entity
            var withdrawalNotificationEntity = _mapper.Map<WithdrawalNotification>(request);
            withdrawalNotificationEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            withdrawalNotificationEntity.Id = BaseUtilities.GenerateUniqueNumber();
            string reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, TransactionType.WITHDRAWALREQUEST.ToString(),false);
            withdrawalNotificationEntity.TransactionReference = reference;
            return withdrawalNotificationEntity;
        }

    }

}
