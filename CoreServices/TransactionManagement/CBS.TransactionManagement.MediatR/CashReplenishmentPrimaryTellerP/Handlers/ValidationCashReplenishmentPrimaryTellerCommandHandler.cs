using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Commands;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data.Entity.AccountingDayOpening;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.Repository.DailyStatisticBoard;
using CBS.TransactionManagement.Repository.VaultP;

namespace CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Handlers
{

    /// <summary>
    /// Handles the command to update a Teller based on CashReplenishmentPrimaryTeller.
    /// </summary>
    public class ValidationCashReplenishmentPrimaryTellerCommandHandler : IRequestHandler<ValidationCashReplenishmentPrimaryTellerCommand, ServiceResponse<CashReplenishmentPrimaryTellerDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITellerRepository _tellerRepository;
        private readonly IMediator _mediator;
        private readonly ICashReplenishmentPrimaryTellerRepository _CashReplenishmentRepository;
        private readonly ILogger<ValidationCashReplenishmentPrimaryTellerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IPrimaryTellerProvisioningHistoryRepository _primaryTellerProvisioningHistoryRepository;
        private readonly IAccountingDayRepository _accountingDayRepository;
        private readonly IGeneralDailyDashboardRepository _generalDailyDashboardRepository; // Repository for accessing account data.
        private readonly IVaultRepository _vaultRepository; // Repository for accessing account data.

        /// <summary>
        /// Constructor for initializing the AddCashReplenishmentPrimaryTellerCommandHandler.
        /// </summary>
        /// <param name="TellerRepository">Repository for Teller data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public ValidationCashReplenishmentPrimaryTellerCommandHandler(


            ITellerRepository TellerRepository,
            IAccountRepository AccountRepository,
            ILogger<ValidationCashReplenishmentPrimaryTellerCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null,
            ICashReplenishmentPrimaryTellerRepository cashReplenishmentPrimaryTellerRepository = null,
            ITellerRepository tellerRepository = null,
            IMediator mediator = null,
            IPrimaryTellerProvisioningHistoryRepository primaryTellerProvisioningHistoryRepository = null,
            IAccountingDayRepository accountingDayRepository = null,
            IGeneralDailyDashboardRepository generalDailyDashboardRepository = null,
            IVaultRepository vaultRepository = null)
        {
            _accountRepository = AccountRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
            _CashReplenishmentRepository = cashReplenishmentPrimaryTellerRepository;
            _tellerRepository = tellerRepository;
            _mediator = mediator;
            _primaryTellerProvisioningHistoryRepository = primaryTellerProvisioningHistoryRepository;
            _accountingDayRepository = accountingDayRepository;
            _generalDailyDashboardRepository = generalDailyDashboardRepository;
            _vaultRepository=vaultRepository;
        }

        /// <summary>
        /// Handles the CashReplenishmentPrimaryTeller to update a Teller.
        /// </summary>
        /// <param name="request">The CashReplenishmentPrimaryTeller containing updated Teller data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>

        /// <summary>
        /// Handles the validation and processing of cash requisitions for primary tellers.
        /// This class validates the requisition, manages account balances, logs actions, 
        /// and ensures proper cash outflow.
        /// </summary>
        public async Task<ServiceResponse<CashReplenishmentPrimaryTellerDto>> Handle(ValidationCashReplenishmentPrimaryTellerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the current accounting date for the branch.
                var accountingDate = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);

                // Retrieve the existing cash replenishment request from the repository.
                var existingCashReplenishment = await _CashReplenishmentRepository.FindAsync(request.Id);

                // Check if the cash replenishment request exists.
                if (existingCashReplenishment == null)
                {
                    string errorMessage = $"Cash replenishment with ID {request.Id} was not found.";
                    _logger.LogError(errorMessage);

                    // Log the error and return a 404 response.
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.CashReplenishment_57, LogLevelInfo.Warning, request.Id);
                    return ServiceResponse<CashReplenishmentPrimaryTellerDto>.Return404(errorMessage);
                }

                // Check if the cash replenishment request is already approved.
                if (existingCashReplenishment.ApprovedStatus == Status.Approved.ToString())
                {
                    string errorMessage = $"Request {existingCashReplenishment.TransactionReference} has already been validated.";
                    _logger.LogError(errorMessage);

                    // Log the error and return a 403 response.
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.CashReplenishment_57, LogLevelInfo.Warning, request.Id);
                    return ServiceResponse<CashReplenishmentPrimaryTellerDto>.Return403(errorMessage);
                }

                // Retrieve the primary teller for the branch.
                var primaryTeller = await _tellerRepository.GetPrimaryTeller(existingCashReplenishment.BranchId);

                // Retrieve the teller associated with the cash replenishment request.
                var teller = await _tellerRepository.GetTeller(existingCashReplenishment.TellerId);

                // Map the request properties to the existing cash replenishment entity.
                _mapper.Map(request, existingCashReplenishment);
                existingCashReplenishment.ApprovedBy = _userInfoToken.FullName ?? _userInfoToken.Email;
                existingCashReplenishment.ApprovedDate = BaseUtilities.UtcNowToDoualaTime();
                existingCashReplenishment.ApprovedByUserId = _userInfoToken.Id;

                // Check if the cash replenishment request has been approved.
                if (existingCashReplenishment.ApprovedStatus == Status.Approved.ToString())
                {
                    // Retrieve the currency notes details from the request.
                    var notes = request.CurrencyNote;
                    string reference = existingCashReplenishment.TransactionReference;

                    // Retrieve the current currency notes from the system.
                    var currencyNote = await RetrieveCurrencyNotes(reference, request.CurrencyNote);
                    var currencyNotesList = currencyNote.Data;

                    // Retrieve the account balance for the sub-teller.
                    var accountBalance = await _accountRepository.RetrieveTellerAccount(teller);

                    // Check if the requested amount exceeds the teller's maximum amount to manage.
                    if ((accountBalance.Balance + request.ConfirmedAmount) > teller.MaximumAmountToManage)
                    {
                        string errorMessage = "The requested amount exceeds the teller's maximum allowable balance.";
                        _logger.LogError(errorMessage);

                        // Log the error and return a 403 response.
                        await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.CashReplenishment_57, LogLevelInfo.Warning, existingCashReplenishment.TransactionReference);
                        return ServiceResponse<CashReplenishmentPrimaryTellerDto>.Return403(errorMessage);
                    }

                    // Ensure the account balance integrity for the sub-teller.
                    _accountRepository.VerifyBalanceIntegrity(accountBalance);

                    // Perform cash-in operations for the primary teller.
                    _primaryTellerProvisioningHistoryRepository.CashInByDinomination(request.ConfirmedAmount, notes, primaryTeller.Id, accountingDate, reference);
                    // Perform cash-out operations from the vault.
                    await _vaultRepository.CashOutByDenominationAsync(request.ConfirmedAmount, notes, existingCashReplenishment.BranchId, reference,EventCode.Vault_To_Cash.ToString().ToUpper(),true);
                    _accountRepository.CreditAccount(accountBalance, existingCashReplenishment.ConfirmedAmount);
                }

                // Call accounting posting for the provisioning.
                var accountingPosting = await AccountinPosting(existingCashReplenishment.TransactionReference, request.ConfirmedAmount);
                
                // Update the cash replenishment record in the repository.
                _CashReplenishmentRepository.Update(existingCashReplenishment);

                //Any Bank.

                // Save all changes to the database.
                await _uow.SaveAsync();
                //var accountingPosting = await AccountinPosting(existingCashReplenishment.TransactionReference, request.ConfirmedAmount);

                // Log the successful validation and return a success response.
                string successMessage = $"Cash replenishment for teller {teller.Name} at branch {_userInfoToken.BranchName} successfully validated by {_userInfoToken.FullName}.";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.CashReplenishment_57, LogLevelInfo.Information, existingCashReplenishment.TransactionReference);

                return ServiceResponse<CashReplenishmentPrimaryTellerDto>.ReturnResultWith200(_mapper.Map<CashReplenishmentPrimaryTellerDto>(existingCashReplenishment), successMessage);
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions by logging the error and returning a 500 response.
                string errorMessage = $"An error occurred while validating cash replenishment for {_userInfoToken.BranchName}. Error: {ex.Message}";
                _logger.LogError(ex, errorMessage);

                // Log the error and return a 500 Internal Server Error response.
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.CashReplenishment_57, LogLevelInfo.Error, null);
                return ServiceResponse<CashReplenishmentPrimaryTellerDto>.Return500(ex);
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
        private async Task<ServiceResponse<bool>> AccountinPosting(string reference, decimal amount)
        {
            // Construct a detailed narration for the cash replenishment posting
            string narration = $"Cash Replenishment 57 for {_userInfoToken.BranchName} | Reference: {reference} | Amount: {BaseUtilities.FormatCurrency(amount)} | Event: Vault to Cash";

            // Create command to perform the cash replenishment posting
            var addOpenAndCloseOfDay = new InternalAutoPostingEventCommand(
                EventCode.Vault_To_Cash.ToString(),
                amount,
                reference,
                narration
            );

            // Send command to _mediator
            var addOpenAndCloseOfDayResponse = await _mediator.Send(addOpenAndCloseOfDay);

            // Handle failure case with detailed logging and exception
            if (addOpenAndCloseOfDayResponse.StatusCode != 200)
            {
                var errorMessage = $"Cash Replenishment 57 for {_userInfoToken.BranchName} failed for Reference: {reference} | Amount: {BaseUtilities.FormatCurrency(amount)} | StatusCode: {addOpenAndCloseOfDayResponse.StatusCode}.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            // Log success
            _logger.LogInformation($"Cash Replenishment 57 for {_userInfoToken.BranchName} succeeded | Reference: {reference} | Amount: {BaseUtilities.FormatCurrency(amount)}.");

            // Return response
            return addOpenAndCloseOfDayResponse;
        }


    }

}
