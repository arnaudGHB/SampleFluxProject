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
using CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Commands;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.MediatR.TellerP.Commands;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Repository.DailyStatisticBoard;

namespace CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Handlers
{

    /// <summary>
    /// Handles the command to update a CashReplenishment based on UpdateCashReplenishmentCommand.
    /// </summary>
    public class ValidateCashReplenishmentSubTellerCommandHandler : IRequestHandler<ValidateCashReplenishmentSubTellerCommand, ServiceResponse<SubTellerCashReplenishmentDto>>
    {
        private readonly ICashReplenishmentRepository _CashReplenishmentRepository; // Repository for accessing CashReplenishment data.
        private readonly ILogger<ValidateCashReplenishmentSubTellerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IAccountRepository _accountRepository;
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvioningHistoryRepository;
        private readonly ITellerRepository _tellerRepository;
        private readonly IMediator _mediator;
        private readonly IDailyTellerRepository _dailyTellerRepository;
        private readonly IPrimaryTellerProvisioningHistoryRepository _primaryTellerProvisioningHistoryRepository;
        private readonly IAccountingDayRepository _accountingDayRepository;
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly IGeneralDailyDashboardRepository _generalDailyDashboardRepository; // Repository for accessing account data.

        //_dailyTellerRepository
        /// <summary>
        /// Constructor for initializing the UpdateCashReplenishmentCommandHandler.
        /// </summary>
        /// <param name="CashReplenishmentRepository">Repository for CashReplenishment data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public ValidateCashReplenishmentSubTellerCommandHandler(
            ICashReplenishmentRepository CashReplenishmentRepository,
            ILogger<ValidateCashReplenishmentSubTellerCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null
,
            IAccountRepository accountRepository = null,
            ISubTellerProvisioningHistoryRepository subTellerProvioningHistoryRepository = null,
            ITellerRepository tellerRepository = null,
            IMediator mediator = null,
            IDailyTellerRepository dailyTellerRepository = null,
            IPrimaryTellerProvisioningHistoryRepository primaryTellerProvisioningHistoryRepository = null,
            IAccountingDayRepository accountingDayRepository = null,
            ITellerOperationRepository tellerOperationRepository = null,
            IGeneralDailyDashboardRepository generalDailyDashboardRepository = null)
        {
            _CashReplenishmentRepository = CashReplenishmentRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
            _accountRepository = accountRepository;
            _subTellerProvioningHistoryRepository = subTellerProvioningHistoryRepository;
            _tellerRepository = tellerRepository;
            _mediator = mediator;
            _dailyTellerRepository = dailyTellerRepository;
            _primaryTellerProvisioningHistoryRepository = primaryTellerProvisioningHistoryRepository;
            _accountingDayRepository = accountingDayRepository;
            _tellerOperationRepository = tellerOperationRepository;
            _generalDailyDashboardRepository = generalDailyDashboardRepository;
        }

        /// <summary>
        /// Handles the UpdateCashReplenishmentCommand to update a CashReplenishment.
        /// </summary>
        /// <param name="request">The UpdateCashReplenishmentCommand containing updated CashReplenishment data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<SubTellerCashReplenishmentDto>> Handle(ValidateCashReplenishmentSubTellerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var accountingDay = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);
                // Retrieve existing CashReplenishment entity from the repository
                var existingCashReplenishment = await _CashReplenishmentRepository.FindAsync(request.Id);

                // Check if the CashReplenishment entity exists
                if (existingCashReplenishment == null)
                {
                    // Log and return a 404 Not Found response if the entity does not exist
                    string errorMessage = $"{request.Id} was not found.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.CashReplenishment_57, LogLevelInfo.Warning, existingCashReplenishment.TransactionReference);
                    return ServiceResponse<SubTellerCashReplenishmentDto>.Return403(errorMessage);
                }

                if (existingCashReplenishment.ApprovedStatus == Status.Approved.ToString())
                {
                    // Log and return a 404 Not Found response if the entity does not exist
                    string errorMessage = $"Request {existingCashReplenishment.TransactionReference} already validated.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.CashReplenishment_57, LogLevelInfo.Warning, existingCashReplenishment.TransactionReference);
                    return ServiceResponse<SubTellerCashReplenishmentDto>.Return403(errorMessage);
                }
                // Validate teller provisioning account
                //await _primaryTellerProvisioningHistoryRepository.CheckIfPrimaryTellerIsOpened();

                //Get Primary teller
                var primaryTeller = await _tellerRepository.GetPrimaryTeller(existingCashReplenishment.BranchId);

                //Get Teller
                var teller = await _tellerRepository.GetTeller(existingCashReplenishment.TellerId);

                //Validate daily teller
                var dailyTeller = await _dailyTellerRepository.GetAnActiveTellerForTheDate(teller.Id);

                // Map the command to the existing CashReplenishment entity
                _mapper.Map(request, existingCashReplenishment);
                existingCashReplenishment.ApprovedBy = _userInfoToken.FullName == null ? _userInfoToken.Email : _userInfoToken.FullName;
                existingCashReplenishment.ApprovedDate = DateTime.Now.ToUtc();
                existingCashReplenishment.ApprovedByUserId = _userInfoToken.Id;
                var subTellerProvioning = new SubTellerProvioningHistory();
                // Check if the CashReplenishment is approved
                if (existingCashReplenishment.ApprovedStatus == Status.Approved.ToString())
                {
                    // Map currency notes to entity
                    var notes = request.CurrencyNotes;
                    string Reference = existingCashReplenishment.TransactionReference;
                    var currencyNote = await RetrieveCurrencyNotes(Reference, request.CurrencyNotes);
                    var currencyNotesList = currencyNote.Data;

                    // Retrieve currency notes.
                    string transactionCode = Reference;

                    //Get sub-teller account balance information
                    var subTellerAccount = await _accountRepository.RetrieveTellerAccount(teller);

                    // Check if the requested amount exceeds the teller's maximum amount to manage
                    if ((subTellerAccount.Balance + request.ConfirmedAmount) > teller.MaximumAmountToManage)
                    {
                        string errorMessage = $"The amount you requested has exceeded your teller's maximum amount.";
                        _logger.LogError(errorMessage);
                        await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.CashReplenishment_57, LogLevelInfo.Warning, existingCashReplenishment.TransactionReference);
                        return ServiceResponse<SubTellerCashReplenishmentDto>.Return403(errorMessage);
                    }

                    //Get primary-teller account balance information
                    var primaryTellerAccountBalance = await _accountRepository.RetrieveTellerAccount(primaryTeller);

                    // Check if the primary-teller account has sufficient funds
                    if ((primaryTellerAccountBalance.Balance + request.ConfirmedAmount) < 0 || primaryTellerAccountBalance.Balance == 0)
                    {
                        string errorMessage = $"Insufficient funds. Account balance is {BaseUtilities.FormatCurrency(primaryTellerAccountBalance.Balance)}. Reduce the requested amount or make a cash requisition from the accountant.";
                        _logger.LogError(errorMessage);
                        await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.CashReplenishment_57, LogLevelInfo.Warning, existingCashReplenishment.TransactionReference);
                        return ServiceResponse<SubTellerCashReplenishmentDto>.Return404(errorMessage);
                    }

                    // Check if the sub-teller account balance exceeds the primary-teller account balance
                    //if ((accountBalance.Balance + request.ConfirmedAmount) > primaryTellerAccountBalance.Balance)
                    //{
                    //    string errorMessage = $"Overflow of cash. Sub-teller account balance is: {BaseUtilities.FormatCurrency(accountBalance.Balance)} while Primary teller account balance is: {BaseUtilities.FormatCurrency(primaryTellerAccountBalance.Balance)}. Reduce the requested amount.";
                    //    _logger.LogError(errorMessage);
                    //    await LogAndUpdateAuditLog(LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404);
                    //    return ServiceResponse<SubTellerCashReplenishmentDto>.Return404(errorMessage);
                    //}

                    decimal previouseBalance = subTellerAccount.PreviousBalance;

                    // Check balance integrity of sub-teller
                    _accountRepository.VerifyBalanceIntegrity(subTellerAccount);

                    await _primaryTellerProvisioningHistoryRepository.CashOutByDinomination(request.ConfirmedAmount, notes, primaryTeller.Id, accountingDay, subTellerAccount.OpenningOfDayReference);
                    subTellerProvioning = _subTellerProvioningHistoryRepository.CashInByDinominationReplenisment(request.ConfirmedAmount, notes, teller.Id, accountingDay, subTellerAccount.OpenningOfDayReference);


                    //Update sub-teller account balance 
                    _accountRepository.CreditAccount(subTellerAccount, request.ConfirmedAmount);

                    // Check balance integrity of primary-teller
                    _accountRepository.VerifyBalanceIntegrity(primaryTellerAccountBalance);
                    //Update primary-teller account balance
                    _accountRepository.DebitAccount(primaryTellerAccountBalance, request.ConfirmedAmount);
                    CreateTellerOperation(request.ConfirmedAmount, OperationType.Credit, dailyTeller, subTellerAccount, Reference, TransactionType.CASH_REPLENISHMENT.ToString(), TransactionType.CASH_REPLENISHMENT.ToString(), accountingDay);
                }
                //var TellerBranch = await GetBranch(teller.BranchId);
                //var cashOperation = new CashOperation(teller.BranchId, request.ConfirmedAmount, 0, TellerBranch.name, TellerBranch.branchCode, CashOperationType.CashReplenishmentSubTill, LogAction.CashReplenishment_57, subTellerProvioning);
                //await _generalDailyDashboardRepository.UpdateOrCreateDashboardAsync(cashOperation);

                // Use the repository to update the existing CashReplenishment entity
                //_remittanceRepository.Update(existingCashReplenishment);
                await _uow.SaveAsync();

                // Log success and return response
                string msg = $"CashReplenishment for till {teller.Name} was successfully validated.";
                var response = ServiceResponse<SubTellerCashReplenishmentDto>.ReturnResultWith200(_mapper.Map<SubTellerCashReplenishmentDto>(existingCashReplenishment), msg);
                _logger.LogInformation(msg);
                await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.OK, LogAction.CashReplenishment_57, LogLevelInfo.Information, existingCashReplenishment.TransactionReference);
                return response;
            }
            catch (InvalidOperationException ex)
            {
                string messageError = $"Validation error occurred while processing cash requisition: {ex.Message}";
                _logger.LogError(messageError);
                await BaseUtilities.LogAndAuditAsync(messageError, request, HttpStatusCodeEnum.InternalServerError, LogAction.CashReplenishment_57, LogLevelInfo.Error,null);
                return ServiceResponse<SubTellerCashReplenishmentDto>.Return400(ex.Message);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while validating CashReplenishment_57: {e.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.CashReplenishment_57, LogLevelInfo.Error,null);
                return ServiceResponse<SubTellerCashReplenishmentDto>.Return500(e, errorMessage);
            }
        }
      
        private void CreateTellerOperation(decimal amount, OperationType operationType, DailyTeller teller, Account tellerAccount,
    string TransactionReference, string TransactionType,
    string eventType, DateTime accountingDate)
        {
            var tellerOperation = new TellerOperation
            {
                Id = BaseUtilities.GenerateUniqueNumber(),
                AccountID = tellerAccount.Id,
                AccountNumber = tellerAccount.AccountNumber,
                Amount = amount,
                OperationType = operationType.ToString(),
                BranchId = tellerAccount.BranchId,
                BankId = tellerAccount.BankId,
                OpenOfDayReference = tellerAccount.OpenningOfDayReference,
                CurrentBalance = tellerAccount.Balance,
                Date = accountingDate,
                AccountingDate = accountingDate,
                PreviousBalance = tellerAccount.PreviousBalance,
                TellerID = teller.TellerId,
                TransactionReference = TransactionReference,
                TransactionType = TransactionType,
                UserID = teller.UserName,
                Description = $"{TransactionType} of {BaseUtilities.FormatCurrency(amount)}, Account Number: {tellerAccount.AccountNumber}",
                ReferenceId = ReferenceNumberGenerator.GenerateReferenceNumber(_userInfoToken.BranchCode),
                CustomerId = "N/A",
                MemberAccountNumber = "N/A",
                EventName = eventType,
                DestinationBrachId = teller.BranchId,
                SourceBranchId = teller.BranchId,
                IsInterBranch = false,
                DestinationBranchCommission = 0,
                SourceBranchCommission = 0,
                IsCashOperation = true,
                MemberName = teller.UserName

            };
            _tellerOperationRepository.Add(tellerOperation);
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
        private async Task LogAndUpdateAuditLog(string action, ValidateCashReplenishmentSubTellerCommand request, string message, string logLevel, int statusCode)
        {
            await APICallHelper.AuditLogger(_userInfoToken.Email, action, request, message, logLevel, statusCode, _userInfoToken.Token);
        }


    }

}
