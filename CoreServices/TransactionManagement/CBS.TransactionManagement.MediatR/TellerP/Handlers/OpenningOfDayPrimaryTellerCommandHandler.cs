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
using Microsoft.IdentityModel.Tokens;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Helper.Helper;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.MediatR.TellerP.Commands;
using CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Commands;
using DocumentFormat.OpenXml.Vml.Office;
using CBS.TransactionManagement.Data.Entity.AccountingDayOpening;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using Newtonsoft.Json;
using CBS.TransactionManagement.Repository.DailyStatisticBoard;

namespace CBS.TransactionManagement.MediatR.TellerP.Handlers
{

    /// <summary>
    /// Handles the command to update a Teller based on PrimaryTellerOpenOfDayCommand.
    /// </summary>
    public class OpenningOfDayPrimaryTellerCommandHandler : IRequestHandler<OpenningOfDayPrimaryTellerCommand, ServiceResponse<PrimaryTellerProvisioningHistoryDto>>
    {
        private readonly IAccountRepository _AccountRepository;
        private readonly IConfigRepository _configRepository;
        private readonly IPrimaryTellerProvisioningHistoryRepository _primaryTellerProvisioningHistoryRepository;
        private readonly IDailyTellerRepository _dailyTellerRepository;
        private readonly ITellerRepository _TellerRepository; // Repository for accessing Teller data.
        private readonly ILogger<OpenningOfDayPrimaryTellerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly IAccountingDayRepository _accountingDayRepository;
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvisioningHistoryRepository;
        private readonly IGeneralDailyDashboardRepository _generalDailyDashboardRepository; // Repository for accessing account data.

        private readonly UserInfoToken _userInfoToken;
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Constructor for initializing the PrimaryTellerOpenOfDayCommandHandler.
        /// </summary>
        /// <param name="TellerRepository">Repository for Teller data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public OpenningOfDayPrimaryTellerCommandHandler(

            IPrimaryTellerProvisioningHistoryRepository TellerHistoryRepository,
            ITellerRepository TellerRepository,
            IAccountRepository AccountRepository,
            ILogger<OpenningOfDayPrimaryTellerCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null,
            IDailyTellerRepository tellerProvisioningAccountRepository = null,
            IMediator mediator = null
,
            IConfigRepository configRepository = null,
            IAccountingDayRepository accountingDayRepository = null,
            ISubTellerProvisioningHistoryRepository subTellerProvisioningHistoryRepository = null,
            IGeneralDailyDashboardRepository generalDailyDashboardRepository = null)
        {
            _primaryTellerProvisioningHistoryRepository = TellerHistoryRepository;
            _TellerRepository = TellerRepository;
            _AccountRepository = AccountRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
            _dailyTellerRepository = tellerProvisioningAccountRepository;
            _mediator = mediator;
            _configRepository = configRepository;
            _accountingDayRepository = accountingDayRepository;
            _subTellerProvisioningHistoryRepository = subTellerProvisioningHistoryRepository;
            _generalDailyDashboardRepository = generalDailyDashboardRepository;
        }

        /// <summary>
        /// Handles the PrimaryTellerOpenOfDayCommand to update a Teller.
        /// </summary>
        /// <param name="request">The PrimaryTellerOpenOfDayCommand containing updated Teller data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>


        public async Task<ServiceResponse<PrimaryTellerProvisioningHistoryDto>> Handle(OpenningOfDayPrimaryTellerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Get the current accounting date
                var accountingDate = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);

                // Step 2: Check if the system is open for transactions
                await _configRepository.CheckIfSystemIsOpen();

                // Step 3: Retrieve the active primary teller for today
                var dailyTeller = await _dailyTellerRepository.GetAnActivePrimaryTellerForTheDate();
                var teller = dailyTeller.Teller;

                // Step 4: Retrieve the teller's account details
                var primaryTellerAccount = await _AccountRepository.RetrieveTellerAccount(teller);

                // Step 5: Check if the primary teller's day is already opened
                if (primaryTellerAccount.OpenningOfDayStatus == CloseOfDayStatus.OOD.ToString())
                {
                    var errorMessage = $"Error: The primary teller's day for {accountingDate} is already opened. Teller: {teller.Name}";
                    _logger.LogWarning(errorMessage); // Log the error for tracking.
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.OpenOfTills, LogLevelInfo.Warning);

                    return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return403(errorMessage);
                }

                // Step 6: Generate a unique reference for the opening transaction
                string reference = BaseUtilities.GenerateInsuranceUniqueNumber(8, $"OOD-PT{_userInfoToken.BranchCode}-");

                // Step 7: Initialize the total amount from currency notes
                var notes = request.CurrencyNotes;
                var currencyNotesList = await CreateCurrencyDenomination(reference, notes);
                decimal total = primaryTellerAccount.Balance;

                // Step 8: Retrieve all sub-tellers for this primary teller and accounting date
                var subTellers = await _subTellerProvisioningHistoryRepository
                                        .FindBy(x => x.ReferenceId == primaryTellerAccount.OpenningOfDayReference
                                                     && x.BranchId == primaryTellerAccount.BranchId)
                                        .Include(x => x.Teller)
                                        .ToListAsync();

                // Step 9: Check if all sub-tellers are closed
                var openSubTellers = subTellers.Where(x => x.ClossedStatus != CloseOfDayStatus.CLOSED.ToString()).ToList();
                if (openSubTellers.Any())
                {
                    // List all open sub-tellers with their accounting days
                    var openTellerDetails = string.Join(", ", openSubTellers.Select(st => $"Teller: {st.Teller.Name}, Date: {st.OpenedDate}"));
                    var errorMessage = $"Cannot proceed with opening the primary teller for accounting day {accountingDate}. The following sub-tills are not yet closed: {openTellerDetails}. Please ensure all sub-tills are closed before proceeding.";
                    _logger.LogWarning(errorMessage); // Log the error for tracking.
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.OpenOfTills, LogLevelInfo.Warning);
                    throw new InvalidOperationException(errorMessage);
                }

                // Step 10: Retrieve primary teller's provisioning history
                var tellerProvisioningHistory = await _primaryTellerProvisioningHistoryRepository.FindAsync(primaryTellerAccount.OpenningOfDayReference);
                if (tellerProvisioningHistory == null)
                {
                    tellerProvisioningHistory = new PrimaryTellerProvisioningHistory();
                    total = 0; // Reset total if no previous history found
                    _logger.LogInformation($"No previous provisioning history found for reference {primaryTellerAccount.OpenningOfDayReference}. Total set to 0.");
                }
                else if (tellerProvisioningHistory.CashAtHand != total)
                {
                    total = tellerProvisioningHistory.CashAtHand; // Update total if different from historical cash at hand
                    _logger.LogInformation($"Updated total amount from historical records: {total}");
                }

                // Step 11: Validate teller limits
                await _TellerRepository.ValidateTellerLimites(request.InitialAmount, total, teller);
                _logger.LogInformation($"Teller limits validated for {teller.Name}. Initial amount: {request.InitialAmount}, Total available: {total}");

                // Step 12: Create a history record for the provisioning of the teller
                var tellerHistory = CreateTellerProvisioningHistory(dailyTeller, total, reference, accountingDate, primaryTellerAccount);
                _logger.LogInformation($"Created provisioning history for teller {teller.Name}: {tellerHistory}");

                // Step 13: Open the teller's account for transactions
                await _AccountRepository.OpenDay(reference, primaryTellerAccount, accountingDate);
                _logger.LogInformation($"Teller account for {teller.Name} opened for transactions with reference: {reference}");

                // Step 14: Record the history entry for the primary teller provisioning
                var primaryTellerProvisioningHistory = _primaryTellerProvisioningHistoryRepository.OpenDay(notes, tellerHistory);
                _logger.LogInformation($"Recorded primary teller provisioning history: {primaryTellerProvisioningHistory}");

                // Map the history entry to a DTO for return to the client.
                var primaryTellerProvisioningHistoryDto = _mapper.Map<PrimaryTellerProvisioningHistoryDto>(primaryTellerProvisioningHistory);
                primaryTellerProvisioningHistoryDto.Teller = teller; // Attach teller information to the DTO for clarity.

                //var TellerBranch = await GetBranch(teller.BranchId);
                //var cashOperation = new CashOperation(teller.BranchId, request.InitialAmount, 0, TellerBranch.name, TellerBranch.branchCode, CashOperationType.OpenOfDayPrimaryTill, LogAction.OpenOfTills,null);
                //await _generalDailyDashboardRepository.UpdateOrCreateDashboardAsync(cashOperation);


                // Step 15: Persist all changes to the database
                await _uow.SaveAsync();
                _logger.LogInformation($"Changes for primary teller {teller.Name} successfully saved to the database.");

                // Step 16: Return a success response
                var successMessage = $"Primary teller has successfully opened the till for accounting day of {accountingDate} with the total sum of {BaseUtilities.FormatCurrency(request.InitialAmount)}.";
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.OpenOfTills, LogLevelInfo.Information);

                return ServiceResponse<PrimaryTellerProvisioningHistoryDto>
                    .ReturnResultWith200(primaryTellerProvisioningHistoryDto, successMessage);
            }
            catch (InvalidOperationException ex)
            {
                string messageError = $"Validation error occurred while processing opening of the primary till: {ex.Message}";
                _logger.LogError(messageError);
                await BaseUtilities.LogAndAuditAsync(messageError, request, HttpStatusCodeEnum.InternalServerError, LogAction.OpenOfTills, LogLevelInfo.Error);
                return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return400(ex.Message);
            }
            catch (Exception e)
            {
                // Handle unexpected errors and log the exception
                string errorMessage = $"Open Of Day Error: {e.Message}. Request Details: {JsonConvert.SerializeObject(request)}. User: {_userInfoToken.FullName}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.OpenOfTills, LogLevelInfo.Error);

                // Return a 500 Internal Server Error response with the error message.
                return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return500(errorMessage);
            }
        }
        private async Task<BranchDto> GetBranch(string branchid)
        {
            var branchCommandQuery = new GetBranchByIdCommand { BranchId = branchid }; // Create command to get branch.
            var branchResponse = await _mediator.Send(branchCommandQuery); // Send command to _mediator.

            // Check if branch information retrieval was successful
            if (branchResponse.StatusCode != 200)
            {
                var errorMessage = $"Failed to retrieve branch information: {branchResponse.Message}";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            return branchResponse.Data; // Return branch data.
        }
        private async Task<List<CurrencyNotesDto>> CreateCurrencyDenomination(string reference, CurrencyNotesRequest currencyNotesRequest)
        {
            var addCurrencyNotesCommand = new AddCurrencyNotesCommand { CurrencyNote = currencyNotesRequest, Reference = reference }; // Create command to add currency notes.
            var currencyNoteResponse = await _mediator.Send(addCurrencyNotesCommand); // Send command to _mediator.
            if (currencyNoteResponse.StatusCode != 200)
            {
                var errorMessage = $"Failed to create denominations with reference {reference} for OOD.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
            return currencyNoteResponse.Data; // Return currency notes data.
        }


        private async Task CheckAccountBalanceAgainstDenominations(decimal DenominationAmount, decimal balance)
        {
            if (balance != DenominationAmount)
            {
                var errorMessage = $"Opening balance do not matched total denomination amount.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);

            }
        }



        private async Task CheckPreviousDayTransaction(Teller teller)
        {
            // Get the latest provisioning history for the primary teller
            var previousDayResult = await _primaryTellerProvisioningHistoryRepository.FindBy(h => h.TellerId == teller.Id).OrderByDescending(h => h.CreatedDate).FirstOrDefaultAsync();
            if (previousDayResult != null)
            {
                if (previousDayResult.ClossedStatus == CloseOfDayStatus.OOD.ToString())
                {
                    // The previous day was closed with a negative balance
                    var errorMessage = $"The operation day: {previousDayResult.OpenedDate} for the primary teller {teller.Code}-{teller.Name} is still opened. You have to close previouse day before opening a new day.";
                    _logger.LogInformation(errorMessage);
                    throw new InvalidOperationException(errorMessage);

                }
                else if (previousDayResult.ClossedDate == null || previousDayResult.ClossedDate == DateTime.MinValue)
                {
                    // The previous day was not closed, throw an exception or handle accordingly
                    var errorMessage = $"The previous day for the primary teller {teller.Code}-{teller.Name} was not closed. To continue with this operation, previous operations must be closed.";
                    _logger.LogInformation(errorMessage);
                    throw new InvalidOperationException(errorMessage);

                }
                else if (previousDayResult.CashAtHand < 0)
                {
                    // The previous day was closed with a positive balance
                    var errorMessage = $"The previous day for the primary teller {teller.Code}-{teller.Name} was closed with a positive balance.";
                    _logger.LogInformation(errorMessage);
                    throw new InvalidOperationException(errorMessage);

                }


            }


        }


        private PrimaryTellerProvisioningHistory CreateTellerProvisioningHistory(DailyTeller dailyTeller, decimal startOfDayAmount, string Reference, DateTime accountingDate, Account account)
        {
            decimal AmountT = startOfDayAmount;
            var tellerHistory = new PrimaryTellerProvisioningHistory
            {
                Id = Reference,
                TellerId = dailyTeller.TellerId,
                UserIdInChargeOfThisTeller = dailyTeller.UserName,
                BankId = dailyTeller.Teller.BankId,
                PrimaryTellerId = dailyTeller.TellerId,
                BranchId = dailyTeller.BranchId,
                ReferenceId = Reference,
                OpenOfDayAmount = AmountT,
                PrimaryTellerComment = $"Open of day {accountingDate} for the primary teller {dailyTeller.Teller.Code}-{dailyTeller.Teller.Name} with amount {AmountT}",
                AccountBalance = account.Balance,
                CashAtHand = AmountT,
                ClossedDate = DateTime.MinValue,
                EndOfDayAmount = 0,
                LastOperationType = "Open of day",
                OpenedDate = accountingDate,
                LastUserID = _userInfoToken.FullName,
                ProvisionedBy = "N/A",
                PreviouseBalance = account.Balance,
                ClossedStatus = CloseOfDayStatus.OOD.ToString(),
                DailyTellerId = dailyTeller.Id,
            };
            return tellerHistory;


        }

        private ServiceResponse<PrimaryTellerProvisioningHistoryDto> HandleError(string errorMessage, Exception exception = null, int statusCode = 400)
        {
            _logger.LogError(errorMessage);

            if (exception != null)
                _logger.LogError(exception.ToString());

            switch (statusCode)
            {
                case 400:
                    return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return400(errorMessage);
                case 404:
                    return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return404(errorMessage);
                case 500:
                    return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return500(exception);
                case 403:
                    return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return403(errorMessage);
                // Add more cases if needed
                default:
                    return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return409(errorMessage);
            }
        }



    }

}
