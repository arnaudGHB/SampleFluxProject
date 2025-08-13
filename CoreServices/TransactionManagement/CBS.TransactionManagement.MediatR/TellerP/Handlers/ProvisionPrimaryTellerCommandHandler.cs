using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Helper.Helper;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.MediatR.TellerP.Commands;
using CBS.TransactionManagement.Commands;

namespace CBS.TransactionManagement.MediatR.TellerP.Handlers
{

    /// <summary>
    /// Handles the command to update a Teller based on ProvisionPrimaryTellerCommand.
    /// </summary>
    public class ProvisionPrimaryTellerCommandHandler : IRequestHandler<ProvisionPrimaryTellerCommand, ServiceResponse<TellerDto>>
    {
        private readonly IAccountRepository _AccountRepository;

        private readonly ICashReplenishmentPrimaryTellerRepository _cashReplenishmentPrimaryTellerRepository;
        private readonly IPrimaryTellerProvisioningHistoryRepository _TellerHistoryRepository;
        private readonly IDailyTellerRepository _dailyTellerRepository;
        private readonly ICurrencyNotesRepository _CurrencyNotesRepository;
        private readonly ITellerRepository _TellerRepository; // Repository for accessing Teller data.
        private readonly ILogger<ProvisionPrimaryTellerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Constructor for initializing the ProvisionPrimaryTellerCommandHandler.
        /// </summary>
        /// <param name="TellerRepository">Repository for Teller data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public ProvisionPrimaryTellerCommandHandler(

            IPrimaryTellerProvisioningHistoryRepository TellerHistoryRepository,
            ICurrencyNotesRepository CurrencyNotesRepository,
            ITellerRepository TellerRepository,
            IAccountRepository AccountRepository,
            ILogger<ProvisionPrimaryTellerCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null,
            IDailyTellerRepository tellerProvisioningAccountRepository = null,
            IMediator mediator = null,
            ICashReplenishmentPrimaryTellerRepository cashReplenishmentPrimaryTellerRepository = null)
        {
            _TellerHistoryRepository = TellerHistoryRepository;
            _CurrencyNotesRepository = CurrencyNotesRepository;
            _TellerRepository = TellerRepository;
            _AccountRepository = AccountRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
            _dailyTellerRepository = tellerProvisioningAccountRepository;
            _mediator = mediator;
            _cashReplenishmentPrimaryTellerRepository = cashReplenishmentPrimaryTellerRepository;
        }

        /// <summary>
        /// Handles the ProvisionPrimaryTellerCommand to update a Teller.
        /// </summary>
        /// <param name="request">The ProvisionPrimaryTellerCommand containing updated Teller data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>


        public async Task<ServiceResponse<TellerDto>> Handle(ProvisionPrimaryTellerCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var cashReplenishement = await _cashReplenishmentPrimaryTellerRepository.FindAsync(request.ReplenishmentId);

                if (cashReplenishement == null)
                {
                    return HandleError($"Cash replenishment {request.ReplenishmentId} is not found.", null, 404);

                }

                if (cashReplenishement.Status)
                {
                    return HandleError($"Cash replenishment {request.ReplenishmentId} is already integrated into operation.", null, 403);

                }
                // Retrieve primary teller
                var teller = await _TellerRepository.GetPrimaryTeller(cashReplenishement.BranchId);
                var dailyTeller = await _dailyTellerRepository.GetThePrimaryTellerOfTheDay(teller.Id);

                // Map currency notes to entity
                var notes = request.CurrencyNotes;
                string Reference = cashReplenishement.TransactionReference;
                var currencyNote = await RetrieveCurrencyNotes(Reference, request.CurrencyNotes);

                var currencyNotesList = currencyNote.Data;
                // Retrieve currency notes.
                string transactionCode = Reference;

                var accountBalance = await GetPrimaryTellerAccount(teller);

                decimal previouseBalance = accountBalance.PreviousBalance;
                // Create teller history
                var tellerHistory = CreateTellerHistory(teller, request, currencyNotesList.FirstOrDefault().Total, transactionCode, Reference, previouseBalance, cashReplenishement.ConfirmedAmount, dailyTeller);
                // Update the teller entity
                cashReplenishement.Status = true;
                _cashReplenishmentPrimaryTellerRepository.Update(cashReplenishement);

                await _uow.SaveAsync();
                return ServiceResponse<TellerDto>.ReturnResultWith200(_mapper.Map<TellerDto>(teller), "Provision was successfull.");
            }
            catch (Exception e)
            {
                // Log and return a 500 Internal Server Error response with an error message
                return HandleError($"Error occurred: {e.Message}", e, 500);
            }
        }

        // Method to retrieve currency notes
        private async Task<ServiceResponse<List<CurrencyNotesDto>>> RetrieveCurrencyNotes(string reference, CurrencyNotesRequest currencyNotesRequest)
        {
            var addCurrencyNotesCommand = new AddCurrencyNotesCommand { CurrencyNote = currencyNotesRequest, Reference = reference }; // Create command to add currency notes.
            var currencyNoteResponse = await _mediator.Send(addCurrencyNotesCommand); // Send command to _mediator.

            if (currencyNoteResponse.StatusCode != 200)
            {
                return ServiceResponse<List<CurrencyNotesDto>>.Return403(""); // Return error response if currency notes retrieval fails.
            }
            return currencyNoteResponse; // Return currency notes data.
        }


        private async Task<Account> GetPrimaryTellerAccount(Teller teller)
        {
            return await _AccountRepository.FindBy(a => a.TellerId == teller.Id).FirstOrDefaultAsync();
        }

      
        private PrimaryTellerProvisioningHistory CreateTellerHistory(Teller teller, ProvisionPrimaryTellerCommand request, decimal startOfDayAmount, string transactionCode, string Reference, decimal previousebalance, decimal cashreplenishmentAmount, DailyTeller dailyTeller)
        {
            decimal AmountT = previousebalance + startOfDayAmount;
            var tellerHistory = new PrimaryTellerProvisioningHistory
            {
                Id = transactionCode,
                TellerId = teller.Id,
                UserIdInChargeOfThisTeller = dailyTeller.UserId,
                BankId = teller.BankId,
                PrimaryTellerId = teller.Id,
                BranchId = teller.BranchId,
                ReferenceId = Reference,
                OpenOfDayAmount = startOfDayAmount,
                PrimaryTellerComment = request.Note == string.Empty ? request.Note : $"Cash replenishment for {teller.Code}-{teller.Name} on date: {BaseUtilities.UtcNowToDoualaTime()}",
                AccountBalance = AmountT,
                CashAtHand = 0,
                ClossedDate = DateTime.MinValue,
                EndOfDayAmount = 0,
                OpenedDate = BaseUtilities.UtcToDoualaTime(DateTime.Now),
                LastUserID = string.Empty,
                ProvisionedBy = _userInfoToken.FullName,
                PreviouseBalance = previousebalance,
                ClossedStatus = CloseOfDayStatus.OOD.ToString(),
                CashReplenishmentAmount = cashreplenishmentAmount,
                ReplenishmentReferenceNumber = transactionCode,
                IsCashReplenishment = true,
                DailyTellerId = dailyTeller.Id
            };
            _TellerHistoryRepository.Add(tellerHistory);
            return tellerHistory;


        }

        private ServiceResponse<TellerDto> HandleError(string errorMessage, Exception exception = null, int statusCode = 400)
        {
            _logger.LogError(errorMessage);

            if (exception != null)
                _logger.LogError(exception.ToString());

            switch (statusCode)
            {
                case 400:
                    return ServiceResponse<TellerDto>.Return400(errorMessage);
                case 404:
                    return ServiceResponse<TellerDto>.Return404(errorMessage);
                case 500:
                    return ServiceResponse<TellerDto>.Return500(exception);
                case 403:
                    return ServiceResponse<TellerDto>.Return403(errorMessage);
                // Add more cases if needed
                default:
                    return ServiceResponse<TellerDto>.Return409(errorMessage);
            }
        }

    }

}
