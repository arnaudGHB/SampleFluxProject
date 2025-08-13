using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Data;
using Microsoft.IdentityModel.Tokens;
using CBS.TransactionManagement.Helper.Helper;
using CBS.TransactionManagement.MediatR.TellerP.Commands;

namespace CBS.TransactionManagement.MediatR.TellerP.Handlers
{

    /// <summary>
    /// Handles the command to update a Teller based on EndTellerDayCommand.
    /// </summary>
    public class EndOfDayAccountantCommandHandler : IRequestHandler<EndOfDayAccountantCommand, ServiceResponse<PrimaryTellerProvisioningHistoryDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMediator _mediator;
        private readonly IPrimaryTellerProvisioningHistoryRepository _primaryTellerProvisioningHistoryRepository;
        private readonly ITransactionRepository _transactionRepository; // Repository for accessing Teller data.

        private readonly ITellerRepository _tellerRepository; // Repository for accessing Teller data.
        private readonly ILogger<EndOfDayAccountantCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _unitOfWork;
        private readonly UserInfoToken _userInfoToken;
        private readonly IDailyTellerRepository _tellerProvisioningAccountRepository;
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvisioningHistoryRepository;
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly IConfigRepository _configRepository;

        /// <summary>
        /// Constructor for initializing the EndOfDayAccountantCommandHandler.
        /// </summary>
        /// <param name="TellerRepository">Repository for Teller data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public EndOfDayAccountantCommandHandler(
            ITellerRepository TellerRepository,
            IAccountRepository AccountRepository,
            IMediator mediator,
            ILogger<EndOfDayAccountantCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null,
            IDailyTellerRepository tellerProvisioningAccountRepository = null,
            ITellerOperationRepository tellerOperationRepository = null,
            ITellerRepository tellerRepository = null,
            ITransactionRepository transactionRepository = null,
            ISubTellerProvisioningHistoryRepository tellerHistoryRepository = null,
            IPrimaryTellerProvisioningHistoryRepository primaryTellerProvisioningHistoryRepository = null,
            IConfigRepository configRepository = null)
        {
            _tellerRepository = TellerRepository;
            _accountRepository = AccountRepository;
            _logger = logger;
            _mediator = mediator;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _unitOfWork = uow;
            _tellerProvisioningAccountRepository = tellerProvisioningAccountRepository;
            _tellerOperationRepository = tellerOperationRepository;
            _transactionRepository = transactionRepository;
            _subTellerProvisioningHistoryRepository = tellerHistoryRepository;
            _primaryTellerProvisioningHistoryRepository = primaryTellerProvisioningHistoryRepository;
            _configRepository = configRepository;
        }

        /// <summary>
        /// Handles the EndTellerDayCommand to update a Teller.
        /// </summary>
        /// <param name="request">The EndTellerDayCommand containing updated Teller data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// 


        public async Task<ServiceResponse<PrimaryTellerProvisioningHistoryDto>> Handle(EndOfDayAccountantCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Find the teller provisioning history by ID
                var tellerProvision = await _primaryTellerProvisioningHistoryRepository.FindAsync(request.primaryTellerProvioningHistoryID);

                // Check if teller provisioning history exists
                if (tellerProvision == null)
                {
                    var errorMessage = $"User has no provision.";
                    _logger.LogError(errorMessage);
                    await LogAuditError(request, errorMessage, LogLevelInfo.Error.ToString(), 404);
                    return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return404(errorMessage);
                }

                // Find the teller by ID
                var teller = await _tellerRepository.FindAsync(tellerProvision.TellerId);

                // Check if teller exists
                if (teller == null)
                {
                    var errorMessage = $"{tellerProvision.TellerId} was not found.";
                    _logger.LogError(errorMessage);
                    await LogAuditError(request, errorMessage, LogLevelInfo.Information.ToString(), 404);
                    return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return404(errorMessage);
                }

                // Find the teller's account
                var tellerAccount = await _accountRepository.FindBy(a => a.TellerId == teller.Id).FirstOrDefaultAsync();

                // Check if teller account exists
                if (tellerAccount == null)
                {
                    var errorMessage = $"Teller {teller.Name} does not have an operation account.";
                    _logger.LogError(errorMessage);
                    await LogAuditError(request, errorMessage, LogLevelInfo.Error.ToString(), 404);
                    return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return404();
                }

                // Validate amount received against cash at hand
                if (tellerProvision.CashAtHand != request.AmountRecieved)
                {
                    var errorMessage = $"The amount entered does not match the amount at hand entered by the teller.";
                    _logger.LogError(errorMessage);
                    await LogAuditError(request, errorMessage, LogLevelInfo.Error.ToString(), 409);
                    return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return409();
                }

                // Update teller provisioning history with accountant's details
                //tellerProvision.AccountantAmountCollected = request.AmountRecieved;
                //tellerProvision.AccountantComment = request.Comment;
                //tellerProvision.ClossedStatus = request.EODClosedStatus;
                //tellerProvision.AccountantConfirmationStatus = request.AccountantConfirmationStatus;

                // Update teller account balance
                tellerAccount.PreviousBalance = tellerAccount.Balance;
                tellerAccount.Balance += tellerProvision.CashAtHand;
                tellerAccount.EncryptedBalance = BalanceEncryption.Encrypt(tellerAccount.Balance.ToString(), tellerAccount.AccountNumber); // Encrypt balance
                _accountRepository.Update(tellerAccount);


                var conf = await _configRepository.GetConfigAsync(OperationSourceType.Web_Portal.ToString());
                await _configRepository.CloseDay(conf);
                // Update teller provisioning history and save changes
                _primaryTellerProvisioningHistoryRepository.Update(tellerProvision);
                await _unitOfWork.SaveAsync();

                // Log successful completion of end-of-day for accountant
                var message = "EOD Completed for accountant";
                _logger.LogInformation(message);
                await LogAuditError(request, message, LogLevelInfo.Information.ToString(), 200);

                // Return response with mapped DTO
                return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.ReturnResultWith200(_mapper.Map<PrimaryTellerProvisioningHistoryDto>(tellerProvision));
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while updating Teller: {e.Message}";
                _logger.LogError(errorMessage);
                await LogAuditError(request, errorMessage, LogLevelInfo.Error.ToString(), 500);
                return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return500(e);
            }
        }

        private async Task LogAuditError(EndOfDayAccountantCommand request, string errorMessage, string logLevel, int statusCode)
        {
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, logLevel, statusCode, _userInfoToken.Token);
        }



    }

}
