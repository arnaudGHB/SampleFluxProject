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
using CBS.TransactionManagement.MediatR.TellerP.Commands;

namespace CBS.TransactionManagement.MediatR.TellerP.Handlers
{

    /// <summary>
    /// Handles the command to update a Teller based on EndTellerDayCommand.
    /// </summary>
    public class ConfirmationEndOfDayForAllSubTellerCommandHandler : IRequestHandler<EndOfDayBySubTellerIDCommand, ServiceResponse<SubTellerProvioningHistoryDto>>
    {
        private readonly IAccountRepository _AccountRepository;
        private readonly IMediator _Mediator;
        private readonly ITransactionRepository _transactionRepository; // Repository for accessing Teller data.
        private readonly ITellerRepository _TellerRepository; // Repository for accessing Teller data.
        private readonly ILogger<ConfirmationEndOfDayForAllSubTellerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IDailyTellerRepository _tellerProvisioningAccountRepository;
        private readonly ISubTellerProvisioningHistoryRepository _SubTellerProvioningHistoryRepository;
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly ITellerRepository _tellerRepository;

        /// <summary>
        /// Constructor for initializing the EndOfDayAllSubTellerCommandHandler.
        /// </summary>
        /// <param name="TellerRepository">Repository for Teller data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public ConfirmationEndOfDayForAllSubTellerCommandHandler(
            ITellerRepository TellerRepository,
            IAccountRepository AccountRepository,
            IMediator mediator,
            ILogger<ConfirmationEndOfDayForAllSubTellerCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null,
            IDailyTellerRepository tellerProvisioningAccountRepository = null,
            ITellerOperationRepository tellerOperationRepository = null,
            ITellerRepository tellerRepository = null,
            ITransactionRepository transactionRepository = null,
            ISubTellerProvisioningHistoryRepository tellerHistoryRepository = null)
        {
            _TellerRepository = TellerRepository;
            _AccountRepository = AccountRepository;
            _logger = logger;
            _Mediator = mediator;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
            _tellerProvisioningAccountRepository = tellerProvisioningAccountRepository;
            _tellerOperationRepository = tellerOperationRepository;
            _tellerRepository = tellerRepository;
            _transactionRepository = transactionRepository;
            _SubTellerProvioningHistoryRepository = tellerHistoryRepository;
        }

        /// <summary>
        /// Handles the EndTellerDayCommand to update a Teller.
        /// </summary>
        /// <param name="request">The EndTellerDayCommand containing updated Teller data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// 


        public async Task<ServiceResponse<SubTellerProvioningHistoryDto>> Handle(EndOfDayBySubTellerIDCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var tellerHistory = await _SubTellerProvioningHistoryRepository.FindAsync(request.SubTellerProvioningHistoryID);

                string errorMessage = null;
                if (tellerHistory == null)
                {
                    errorMessage = $"Issue with the teller history registration. No record was found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                    return ServiceResponse<SubTellerProvioningHistoryDto>.Return500(errorMessage);
                }
                var teller = await _TellerRepository.FindAsync(tellerHistory.TellerId);
                tellerHistory.PrimaryTellerComment = request.Comment;
                tellerHistory.PrimaryTellerConfirmationStatus = request.PrimaryTellerConfirmationStatus;
                var tellerAccount = await _AccountRepository.FindBy(x => x.TellerId == teller.Id).FirstOrDefaultAsync();
                if (request.PrimaryTellerConfirmationStatus == CloseOfDayActions.Closed.ToString())
                {
                    tellerHistory.ClossedDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
                    tellerAccount.Balance = 0;
                    tellerAccount.PreviousBalance = 0;
                    _AccountRepository.Update(tellerAccount);
                    errorMessage = $"{teller.Name} was closed sucessfully by {_userInfoToken.FullName}";
                }
                else
                {
                    errorMessage = $"{teller.Name} is still not close. Status {request.PrimaryTellerConfirmationStatus}";
                }
                tellerHistory.ClossedStatus = CloseOfDayStatus.OK.ToString();
                _SubTellerProvioningHistoryRepository.Update(tellerHistory);
                if (await _uow.SaveAsync() <= 0)
                {
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, "An error occurred updating _tellerHistoryRepository", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                    return ServiceResponse<SubTellerProvioningHistoryDto>.Return500();
                }
                var response = ServiceResponse<SubTellerProvioningHistoryDto>.ReturnResultWith200(_mapper.Map<SubTellerProvioningHistoryDto>(tellerHistory));
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return response;
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while clossing Tellers day. Error: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<SubTellerProvioningHistoryDto>.Return500(e);
            }
        }



    }

}
