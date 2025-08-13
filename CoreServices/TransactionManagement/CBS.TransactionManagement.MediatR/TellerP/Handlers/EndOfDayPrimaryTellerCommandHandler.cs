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
using CBS.TransactionManagement.Data;
using Microsoft.IdentityModel.Tokens;
using CBS.TransactionManagement.MediatR.TellerP.Commands;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Repository.DailyStatisticBoard;

namespace CBS.TransactionManagement.MediatR.TellerP.Handlers
{

    /// <summary>
    /// Handles the command to update a Teller based on EndTellerDayCommand.
    /// </summary>
    public class EndOfDayPrimaryTellerCommandHandler : IRequestHandler<EndOfDayPrimaryTellerCommand, ServiceResponse<PrimaryTellerProvisioningHistoryDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMediator _mediator;
        private readonly IPrimaryTellerProvisioningHistoryRepository _primaryTellerProvisioningHistoryRepository;
        private readonly ITransactionRepository _transactionRepository; // Repository for accessing Teller data.
        private readonly IDailyTellerRepository _dailyTellerRepository;
        private readonly ITellerRepository _TellerRepository; // Repository for accessing Teller data.
        private readonly ILogger<EndOfDayPrimaryTellerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _unitOfWork;
        private readonly UserInfoToken _userInfoToken;
        private readonly IDailyTellerRepository _tellerProvisioningAccountRepository;
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvisioningHistoryRepository;
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly IAccountingDayRepository _accountingDayRepository;
        private readonly IGeneralDailyDashboardRepository _generalDailyDashboardRepository; // Repository for accessing account data.

        /// <summary>
        /// Constructor for initializing the EndOfDayPrimaryTellerCommandHandler.
        /// </summary>
        /// <param name="TellerRepository">Repository for Teller data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public EndOfDayPrimaryTellerCommandHandler(
            ITellerRepository TellerRepository,
            IAccountRepository AccountRepository,
            IMediator mediator,
            ILogger<EndOfDayPrimaryTellerCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null,
            IDailyTellerRepository tellerProvisioningAccountRepository = null,
            ITellerOperationRepository tellerOperationRepository = null,
            ITransactionRepository transactionRepository = null,
            ISubTellerProvisioningHistoryRepository tellerHistoryRepository = null,
            IPrimaryTellerProvisioningHistoryRepository primaryTellerProvisioningHistoryRepository = null,
            IDailyTellerRepository dailyTellerRepository = null,
            IAccountingDayRepository accountingDayRepository = null,
            IGeneralDailyDashboardRepository generalDailyDashboardRepository = null)
        {
            _TellerRepository = TellerRepository;
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
            _dailyTellerRepository = dailyTellerRepository;
            _accountingDayRepository = accountingDayRepository;
            _generalDailyDashboardRepository = generalDailyDashboardRepository;
        }

        /// <summary>
        /// Handles the EndTellerDayCommand to update a Teller.
        /// </summary>
        /// <param name="request">The EndTellerDayCommand containing updated Teller data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// 


        public async Task<ServiceResponse<PrimaryTellerProvisioningHistoryDto>> Handle(EndOfDayPrimaryTellerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the current accounting day for the branch.
                var accountingDate = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);

                // Step 2: Get the active primary teller for the current day.
                var dailyTeller = await _dailyTellerRepository.GetAnActivePrimaryTellerForTheDate();

                // Step 3: Validate the existence of the teller by retrieving their details using the Teller ID.
                var teller = await _TellerRepository.GetTeller(dailyTeller.TellerId);

                // Step 4: Retrieve the primary teller's account details.
                var primaryTellerAccount = await _accountRepository.RetrieveTellerAccount(teller);

                // Step 5: Check if the day's operations have already been closed for this teller.
                if (primaryTellerAccount.OpenningOfDayStatus == CloseOfDayStatus.CLOSED.ToString())
                {
                    var errorMessage = $"End-of-day operation already completed for accounting day {primaryTellerAccount.OpenningOfDayDate} with reference {primaryTellerAccount.OpenningOfDayReference}.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.ClosedOfTills, LogLevelInfo.Warning);
                    return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return404(errorMessage);
                }

                // Step 6: Fetch the provisioning history for the primary teller using their reference ID.
                var tellerProvision = await _primaryTellerProvisioningHistoryRepository.FindBy(x => x.ReferenceId == primaryTellerAccount.OpenningOfDayReference).FirstOrDefaultAsync();

                // Step 7: Handle the case where no provisioning history is found.
                if (tellerProvision == null)
                {
                    var errorMessage = $"No provisioning history found for primary teller {teller.Name} for accounting day {accountingDate}.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.ClosedOfTills, LogLevelInfo.Warning);
                    return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return404(errorMessage);
                }

                // Step 8: Retrieve sub-tellers associated with the primary teller for cash data.
                var subTellers = await _subTellerProvisioningHistoryRepository.FindBy(x => x.ReferenceId == primaryTellerAccount.OpenningOfDayReference && x.BranchId==primaryTellerAccount.BranchId).Include(x=>x.Teller).ToListAsync();

                // Step 9: Check if all sub-tellers are closed.
                var openSubTellers = subTellers.Where(st => st.ClossedStatus != CloseOfDayStatus.CLOSED.ToString()).ToList();
                if (openSubTellers.Any())
                {
                    var errorMessage = $"The following sub-tellers are not closed: {string.Join(", ", openSubTellers.Select(x => x.Teller.Name))}. Please close all sub-tellers before proceeding.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.ClosedOfTills, LogLevelInfo.Warning);
                    return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return404(errorMessage);
                }

                // Step 10: Calculate the closing balance by summing sub-tellers' cash-at-hand balances and the primary teller's balance.
                decimal closingBalance = subTellers.Sum(x => x.CashAtHand) + primaryTellerAccount.Balance;

              
                // Step 12: Sum cash balances of all sub-tellers for further financial processing.
                decimal tellersBalance = subTellers.Sum(x => x.CashAtHand);

                // Step 13: Generate a unique reference number for the close-of-day operation.
                string Reference = BaseUtilities.GenerateInsuranceUniqueNumber(8, $"COD-PT{_userInfoToken.BranchCode}-");

                // Step 14: Prepare to add currency notes for the closing operation.
                var notes = request.CurrencyNotes;
                var addCurrencyNotesCommand = new AddCurrencyNotesCommand { CurrencyNote = notes, Reference = Reference, ServiceType = ServiceTypes.CLOSE_OF_DAY_PRIMARY_TELLER.ToString() };

                // Step 15: Execute the command to add currency notes and handle potential errors.
                var currencyNoteResponse = await _mediator.Send(addCurrencyNotesCommand);
                if (currencyNoteResponse.StatusCode != 200)
                {
                    var errorMessage = $"Failed to add currency notes: {currencyNoteResponse.Message}.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.ClosedOfTills, LogLevelInfo.Warning);
                    return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return403(errorMessage);
                }

                // Step 16: Update the primary teller's provisioning history with request data and generated reference.
                var currencyNote = currencyNoteResponse.Data;
                tellerProvision.PrimaryTellerComment = request.Comment;
                tellerProvision.ClossedStatus = CloseOfDayStatus.CLOSED.ToString();
                tellerProvision.CashAtHand = request.CashAtHand;
                tellerProvision.CloseOfDayReferenceId = Reference;
                tellerProvision.EndOfDayAmount = request.CashAtHand;
                tellerProvision.ClossedDate = BaseUtilities.UtcNowToDoualaTime();

                // Step 17: Update provisioning history in the repository to reflect the day's closure.
                var primaryTellerProvisioningHistory = _primaryTellerProvisioningHistoryRepository.CloseDay(notes, tellerProvision);

                // Step 18: Reset balances and close the accounts for all sub-tellers.
                foreach (var subTeller in subTellers)
                {
                    var teller1 = await _TellerRepository.GetTeller(subTeller.TellerId);
                    var tellerAccount = await _accountRepository.RetrieveTellerAccount(teller1);
                    _accountRepository.ResetAccountBalance(tellerAccount);
                }

                // Step 19: Reset primary teller's account balance and close the account for the day.
                _accountRepository.ResetAccountBalance(primaryTellerAccount, request.CashAtHand);
                await _accountRepository.ClosseDay(primaryTellerAccount);

                //var TellerBranch = await GetBranch(teller.BranchId);
                //var cashOperation = new CashOperation(teller.BranchId, request.CashAtHand, 0, TellerBranch.name, TellerBranch.branchCode, CashOperationType.CloseOfDayPrimaryTill, LogAction.ClosedOfTills, null);
                //await _generalDailyDashboardRepository.UpdateOrCreateDashboardAsync(cashOperation);

                // Step 20: Save changes to the database and commit the transaction.
                await _unitOfWork.SaveAsync();

                // Step 21: Prepare a success message indicating the end-of-day completion.
                var message = "End of Day (EOD) successfully completed for the primary till.";
                primaryTellerProvisioningHistory.Teller = teller;

                // Step 22: Log the success message for auditing.

                await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.OK, LogAction.ClosedOfTills, LogLevelInfo.Information);

                // Step 23: Return the result with updated provisioning history and status code 200.
                return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.ReturnResultWith200(_mapper.Map<PrimaryTellerProvisioningHistoryDto>(primaryTellerProvisioningHistory), message);
            }
            catch (InvalidOperationException ex)
            {
                string messageError = $"Validation error occurred while processing end day for the primary till: {ex.Message}";
                _logger.LogError(messageError);
                await BaseUtilities.LogAndAuditAsync(messageError, request, HttpStatusCodeEnum.InternalServerError, LogAction.ClosedOfTills, LogLevelInfo.Error);
                return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return400(ex.Message);
            }
            catch (Exception e)
            {
                // Step 24: Catch and handle exceptions, logging the error and returning a 500 response.
                var errorMessage = $"An error occurred while processing the end-of-day operation for the primary till: {e.Message}. StackTrace: {e.StackTrace}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.ClosedOfTills, LogLevelInfo.Error);
                return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return500(e);
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


    }

}
