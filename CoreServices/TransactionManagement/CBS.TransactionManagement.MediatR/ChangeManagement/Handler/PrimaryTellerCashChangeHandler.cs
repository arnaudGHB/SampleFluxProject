using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.MediatR.ChangeManagement.Command;
using CBS.TransactionManagement.Data.Dto;
using CBS.TransactionManagement.Repository.VaultP;
using CBS.TransactionManagement.Repository.AccountingDayOpening;

namespace CBS.TransactionManagement.MediatR.ChangeManagement.Handler
{


  
    /// <summary>
    /// Handler for managing primary teller cash changes.
    /// </summary>
    public class PrimaryTellerCashChangeHandler : IRequestHandler<PrimaryTellerCashChangeCommand, ServiceResponse<CashChangeHistoryDto>>
    {
        private readonly ITellerRepository _tellerRepository;
        private readonly ILogger<PrimaryTellerCashChangeHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IAccountingDayRepository _accountingDayRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IDailyTellerRepository _dailyTellerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IPrimaryTellerProvisioningHistoryRepository _primaryTellerProvisioningHistoryRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimaryTellerCashChangeHandler"/> class.
        /// </summary>
        /// <param name="tellerRepository">Repository for teller operations.</param>
        /// <param name="logger">Logger for logging events and errors.</param>
        /// <param name="mapper">AutoMapper instance for object mapping.</param>
        /// <param name="accountingDayRepository">Repository for fetching accounting day information.</param>
        /// <param name="userInfoToken">Current user info token for context.</param>
        /// <param name="dailyTellerRepository">Repository for managing daily teller operations.</param>
        /// <param name="accountRepository">Repository for account management.</param>
        /// <param name="primaryTellerProvisioningHistoryRepository">Repository for managing primary teller provisioning history.</param>
        public PrimaryTellerCashChangeHandler(
            ITellerRepository tellerRepository,
            ILogger<PrimaryTellerCashChangeHandler> logger,
            IMapper mapper,
            IAccountingDayRepository accountingDayRepository,
            UserInfoToken userInfoToken,
            IDailyTellerRepository dailyTellerRepository,
            IAccountRepository accountRepository,
            IPrimaryTellerProvisioningHistoryRepository primaryTellerProvisioningHistoryRepository)
        {
            _tellerRepository = tellerRepository;
            _logger = logger;
            _mapper = mapper;
            _accountingDayRepository = accountingDayRepository;
            _userInfoToken = userInfoToken;
            _dailyTellerRepository = dailyTellerRepository;
            _accountRepository = accountRepository;
            _primaryTellerProvisioningHistoryRepository = primaryTellerProvisioningHistoryRepository;
        }

        /// <summary>
        /// Handles the command for managing primary teller cash changes.
        /// </summary>
        /// <param name="request">The command containing change details.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A response containing the cash change history.</returns>
        public async Task<ServiceResponse<CashChangeHistoryDto>> Handle(PrimaryTellerCashChangeCommand request, CancellationToken cancellationToken)
        {
            string reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, LogAction.CashChangeOperationPrimaryTill.ToString(), false);
            try
            {
                // Log the initiation of the change management process
                string startMessage = "Starting primary teller cash change management.";
                _logger.LogInformation(startMessage);

                // Fetch the current accounting date for the branch
                var accountingDate = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);

                // Retrieve the active teller for the current day
                var dailyTeller = await _dailyTellerRepository.GetAnActivePrimaryTellerForTheDate();

                // Retrieve the teller and their associated account
                var teller = await _tellerRepository.GetTeller(dailyTeller.TellerId);
                var tellerAccount = await _accountRepository.RetrieveTellerAccount(teller);

                // Create the data carrier object for the cash change operation
                var changeManagement = new CashChangeDataCarrier
                {
                    denominationsGiven = request.DenominationsGiven,
                    denominationsReceived = request.DenominationsReceived,
                    changeReason = request.ChangeReason,
                    tellerId = _userInfoToken.Id,
                    accountingDate = accountingDate,
                    reference = reference, SystemName=teller.Name,
                    OpenningOfDayReference = tellerAccount?.OpenningOfDayReference
                };

                // Perform the cash change operation via the primary teller repository
                var cashChangeHistory = await _primaryTellerProvisioningHistoryRepository.ManageCashChangeAsync(changeManagement);

                // Map the result to a DTO for the response
                var cashChangeHistoryDto = _mapper.Map<CashChangeHistoryDto>(cashChangeHistory);

                // Log and audit the successful operation
                string successMessage = "Primary teller cash change management completed successfully.";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    cashChangeHistory,
                    HttpStatusCodeEnum.OK,
                    LogAction.TellerOperationChange,
                    LogLevelInfo.Information,
                    reference);

                // Return a successful service response with the result
                return ServiceResponse<CashChangeHistoryDto>.ReturnResultWith200(cashChangeHistoryDto, successMessage);
            }
            catch (Exception ex)
            {
                // Log and audit unexpected errors
                string errorMessage = $"An error occurred while managing primary teller cash change: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.TellerOperationChange,
                    LogLevelInfo.Error,
                    reference);

                // Return a failure response
                return ServiceResponse<CashChangeHistoryDto>.Return500(errorMessage);
            }
        }
    }
}
