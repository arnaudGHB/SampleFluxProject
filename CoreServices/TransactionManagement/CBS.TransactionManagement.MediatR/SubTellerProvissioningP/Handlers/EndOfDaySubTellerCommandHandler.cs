using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.MediatR.TellerP.Commands;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Repository.DailyStatisticBoard;

namespace CBS.TransactionManagement.MediatR.SubTellerProvissioningP.Handlers
{

    /// <summary>
    /// Handles the command to update a Teller based on EndTellerDayCommand.
    /// </summary>
    public class EndOfDaySubTellerCommandHandler : IRequestHandler<EndOfDaySubTellerCommand, ServiceResponse<SubTellerProvioningHistoryDto>>
    {
        private readonly IAccountRepository _AccountRepository;
        private readonly IMediator _Mediator;
        private readonly ITransactionRepository _transactionRepository; // Repository for accessing Teller data.
        private readonly IDailyTellerRepository _dailyTellerRepository;
        private readonly ITellerRepository _TellerRepository; // Repository for accessing Teller data.
        private readonly ILogger<EndOfDaySubTellerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly ISubTellerProvisioningHistoryRepository _SubTellerProvioningHistoryRepository;
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly ITellerRepository _tellerRepository;
        private readonly IGeneralDailyDashboardRepository _generalDailyDashboardRepository; // Repository for accessing account data.

        /// <summary>
        /// Constructor for initializing the EndOfDaySubTellerCommandHandler.
        /// </summary>
        /// <param name="TellerRepository">Repository for Teller data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public EndOfDaySubTellerCommandHandler(
            ITellerRepository TellerRepository,
            IAccountRepository AccountRepository,
            IMediator mediator,
            ILogger<EndOfDaySubTellerCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null,
            ITellerOperationRepository tellerOperationRepository = null,
            ITellerRepository tellerRepository = null,
            ITransactionRepository transactionRepository = null,
            ISubTellerProvisioningHistoryRepository tellerHistoryRepository = null,
            IDailyTellerRepository dailyTellerRepository = null,
            IGeneralDailyDashboardRepository generalDailyDashboardRepository = null)
        {
            _TellerRepository = TellerRepository;
            _AccountRepository = AccountRepository;
            _logger = logger;
            _Mediator = mediator;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
            _tellerOperationRepository = tellerOperationRepository;
            _tellerRepository = tellerRepository;
            _transactionRepository = transactionRepository;
            _SubTellerProvioningHistoryRepository = tellerHistoryRepository;
            _dailyTellerRepository = dailyTellerRepository;
            _generalDailyDashboardRepository = generalDailyDashboardRepository;
        }

        /// <summary>
        /// Handles the EndTellerDayCommand to update a Teller.
        /// </summary>
        /// <param name="request">The EndTellerDayCommand containing updated Teller data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// 


        public async Task<ServiceResponse<SubTellerProvioningHistoryDto>> Handle(EndOfDaySubTellerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve active sub teller for the current day from the repository
                var dailyTeller = await _dailyTellerRepository.GetAnActiveSubTellerForTheDate();

                // Step 2: Validate the retrieved teller and fetch its details from the repository
                var teller = await _TellerRepository.GetTeller(dailyTeller.TellerId);

                // Step 3: Get the account information for the sub teller
                var account = await _AccountRepository.RetrieveTellerAccount(teller);

                // Step 4: Retrieve the provisioning history for the teller for the day
                var tellerProvision = await _SubTellerProvioningHistoryRepository
                    .FindBy(t => t.TellerId == account.TellerId && t.ReferenceId == account.OpenningOfDayReference)
                    .FirstOrDefaultAsync();

                // Step 5: If no provision history exists, log the error and return 404
                if (tellerProvision == null)
                {
                    var errorMessage = $"No provision was found for till {teller.Name} for the accounting day {account.OpenningOfDayDate} with reference {account.OpenningOfDayReference}.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.OpenOfTills, LogLevelInfo.Warning);

                    return ServiceResponse<SubTellerProvioningHistoryDto>.Return404(errorMessage);
                }

                // Step 6: Check if the teller’s day has already been closed, log the error, and return 403
                if (tellerProvision.ClossedStatus == CloseOfDayStatus.CLOSED.ToString())
                {
                    var errorMessage = $"Your Till is Already Closed for the accounting day {account.OpenningOfDayDate} with Reference: {account.OpenningOfDayReference}.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.OpenOfTills, LogLevelInfo.Warning);
                    return ServiceResponse<SubTellerProvioningHistoryDto>.Return404(errorMessage);
                }

                // Step 7: Generate a unique close of day reference ID for the sub teller operation
                string Reference = BaseUtilities.GenerateInsuranceUniqueNumber(8, $"COD-ST{_userInfoToken.BranchCode}-");

                // Step 8: Prepare the currency notes command to record the closing notes
                var notes = request.CurrencyNotes;
                var addCurrencyNotesCommand = new AddCurrencyNotesCommand
                {
                    CurrencyNote = notes,
                    Reference = Reference,
                    ServiceType = ServiceTypes.CLOSE_OF_DAY_SUB_TELLER.ToString()
                };

                // Step 9: Send the currency notes command and validate the response
                var currencyNoteResponse = await _Mediator.Send(addCurrencyNotesCommand);
                if (currencyNoteResponse.StatusCode != 200)
                {
                    return ServiceResponse<SubTellerProvioningHistoryDto>.Return403(currencyNoteResponse.Message);
                }

              
                // Step 11: Update the teller provisioning history with the closing details
                var currencyNote = currencyNoteResponse.Data;
                tellerProvision.AccountBalance = account.Balance;
                tellerProvision.CloseOfReferenceId = Reference;
                tellerProvision.SubTellerComment = request.Comment;  // Comment regarding the closing
                tellerProvision.CashAtHand = request.CashAtHand;
                tellerProvision.ClossedStatus = CloseOfDayStatus.CLOSED.ToString();
                tellerProvision.ClossedDate = BaseUtilities.UtcNowToDoualaTime();

                // Step 12: Close the sub teller’s day in the repository
                var subTellerProvioningHistory = _SubTellerProvioningHistoryRepository.CloseDay(notes, tellerProvision);

                // Step 13: Close the account for the day
                await _AccountRepository.ClosseDay(account);
                //var TellerBranch = await GetBranch(teller.BranchId);
                //var cashOperation = new CashOperation(teller.BranchId, request.CashAtHand, 0, TellerBranch.name, TellerBranch.branchCode, CashOperationType.CloseOfDaySubTill, LogAction.ClosedOfTills);
                //await _generalDailyDashboardRepository.UpdateOrCreateDashboardAsync(cashOperation);

                // Step 14: Save all changes to the database
                await _uow.SaveAsync();

                // Step 15: Map the sub teller provisioning history to a DTO object for the response
                var subTellerProvioningHistoryDto = _mapper.Map<SubTellerProvioningHistoryDto>(subTellerProvioningHistory);
                subTellerProvioningHistoryDto.Teller = teller;

                // Step 16: Log the successful closure and return the response
                string message = $"Teller {teller.Name}'s day has been successfully closed with a total cash amount of {BaseUtilities.FormatCurrency(request.CashAtHand)}.";
                var response = ServiceResponse<SubTellerProvioningHistoryDto>.ReturnResultWith200(subTellerProvioningHistoryDto, message);

                await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.OK, LogAction.OpenOfTills, LogLevelInfo.Information);


                return response;
            }
            catch (InvalidOperationException ex)
            {
                string messageError = $"Validation error occurred while processing closing the sub till: {ex.Message}";
                _logger.LogError(messageError);
                await BaseUtilities.LogAndAuditAsync(messageError, request, HttpStatusCodeEnum.InternalServerError, LogAction.OpenOfTills, LogLevelInfo.Error);
                return ServiceResponse<SubTellerProvioningHistoryDto>.Return400(ex.Message);
            }
            catch (Exception e)
            {
                // Step 17: Handle any exceptions, log the error, and return a 500 Internal Server Error response
                string errorMessage = $"Error occurred while closing the sub till: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.OpenOfTills, LogLevelInfo.Error);
                return ServiceResponse<SubTellerProvioningHistoryDto>.Return500(e);
            }
        }

        private async Task<BranchDto> GetBranch(string branchid)
        {
            var branchCommandQuery = new GetBranchByIdCommand { BranchId = branchid }; // Create command to get branch.
            var branchResponse = await _Mediator.Send(branchCommandQuery); // Send command to _mediator.

            // Check if branch information retrieval was successful
            if (branchResponse.StatusCode != 200)
            {
                var errorMessage = $"Failed to retrieve branch information: {branchResponse.Message}";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            return branchResponse.Data; // Return branch data.
        }

    }

}
