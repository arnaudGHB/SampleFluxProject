using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Command;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Handlers
{
    public class BranchToBranchTransferCommandHandler : IRequestHandler<BranchToBranchTransferCommand, ServiceResponse<bool>>
    {
        // Repositories and dependencies
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICashMovementTrackingConfigurationRepository _chartOfAccountRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly ICashReplenishmentRepository _cashReplenishmentRepository;
        private readonly IUserNotificationRepository _userNotificationRepository;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ITransactionDataRepository _transactionDataRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<BranchToBranchTransferCommandHandler> _logger;
        private readonly IAccountingEntriesServices _accountingService;
        private readonly UserInfoToken _userInfoToken;
        private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work (not used in the current implementation)

        // Constructor to inject dependencies
        public BranchToBranchTransferCommandHandler(
            ITransactionDataRepository transactionDataRepository,
            IAccountingEntryRepository accountingEntryRepository,
            IOperationEventAttributeRepository operationEventAttributeRepository,
            IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
            IMediator mediator,
            IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<BranchToBranchTransferCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            UserInfoToken userInfoToken,
            ICashMovementTrackingConfigurationRepository chartOfAccountRepository,
            IAccountingEntriesServices accountingEntriesServices,
            ICashReplenishmentRepository? cashReplenishmentRepository,
            IUserNotificationRepository? userNotificationRepository)
        {
            _userNotificationRepository = userNotificationRepository;
            _accountRepository = accountRepository;
            _operationEventAttributeRepository = operationEventAttributeRepository;
            _accountingEntryRepository = accountingEntryRepository;
            _mapper = mapper;
            _logger = logger;
            _cashReplenishmentRepository = cashReplenishmentRepository;
            _uow = uow;
            _transactionDataRepository = transactionDataRepository;
            _userInfoToken = userInfoToken;
            _configuration = configuration;
            _chartOfAccountRepository = chartOfAccountRepository;
            _accountingRuleEntryRepository = accountingRuleEntryRepository;
            _accountingService = accountingEntriesServices;
            _mediator = mediator;
        }

        // Handle method to process BranchToBranchTransferCommand
        public async Task<ServiceResponse<bool>> Handle(BranchToBranchTransferCommand command, CancellationToken cancellationToken)
        {
            // Variables for accounts and accounting entries
            Data.Account destinationAccount;
            Data.Account sourceAccount;
            List<AccountingEntryDto> accountingEntries = new List<AccountingEntryDto>();
            string errorMessage = "";

            try
            {
                // 🚨 Step 1: Prevent duplicate transactions
                if (await _accountingService.TransactionExists(command.ReferenceId))
                {
                    errorMessage = $"A transaction with reference {command.ReferenceId} has already been processed. Please verify before retrying.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.BranchToBranchTransferCashOut, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }

                // 🚨 Step 2: Validate that a corresponding cash replenishment request exists
                var cashReplenishmentRequest = await _cashReplenishmentRepository.FindAsync(command.ReferenceId);
                if (cashReplenishmentRequest == null)
                {
                    errorMessage = $"No pending cash replenishment request found for reference {command.ReferenceId}. Please check and try again.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.BranchToBranchTransferCashOut, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }

                // 🚨 Step 3: Fetch source and destination accounts
                sourceAccount = await _accountRepository.FindAsync(command.FromAccountId);
                destinationAccount = await _accountRepository.FindAsync(command.ToAccountId);

                if (sourceAccount == null || destinationAccount == null)
                {
                    errorMessage = $"Invalid account details provided. Please verify source and destination account IDs.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.BadRequest, LogAction.BranchToBranchTransferCashOut, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return422(errorMessage);
                }

                // 🚀 Step 4: Process the branch-to-branch cash movement
                var entriesDto = await _accountingService.CashMovementAsync(
                    GenerateNaration(destinationAccount, sourceAccount, command),
                    $"{_userInfoToken.FullName} {_userInfoToken.BranchCode}",
                    BaseUtilities.UtcToLocal(),
                    destinationAccount,
                    sourceAccount,
                    command.Amount,
                    "BranchToBranchTransferCommand",
                    command.ReferenceId,
                    _userInfoToken.BranchId);

                accountingEntries.AddRange(entriesDto);

                // 🚀 Step 5: Save accounting entries
                var entries = _mapper.Map<List<Data.AccountingEntry>>(accountingEntries);
                _accountingEntryRepository.AddRange(entries);

                // 🚀 Step 6: Update the status of the cash replenishment request
                cashReplenishmentRequest.Status = CashReplishmentRequestStatus.Awaiting_Branch_CashClearing.ToString();
                _cashReplenishmentRepository.Update(cashReplenishmentRequest);

                // 🚀 Step 7: Update user notifications
                var userNotification = _userNotificationRepository.FindBy(xx => xx.ActionId.Equals(cashReplenishmentRequest.Id)).FirstOrDefault();
                if (userNotification != null)
                {
                    var notification = new UsersNotification(
                        "Cash Clearing Request",
                        "/Notification/BranchCashClearingRequest?KEY={0}&BranchId=" + cashReplenishmentRequest.BranchId,
                        cashReplenishmentRequest.Id,
                        BaseUtilities.GenerateInsuranceUniqueNumber(15, "NOTIF-"),
                        _userInfoToken.Id,
                        _userInfoToken.BranchId,
                        cashReplenishmentRequest.BranchId);

                    userNotification.Action = notification.Action;
                    userNotification.ActionId = notification.ActionId;
                    userNotification.ModifiedBy = notification.ModifiedBy;
                    userNotification.ModifiedDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
                    userNotification.ActionUrl = notification.ActionUrl;
                    userNotification.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
                    userNotification.TempData = notification.TempData;
                    _userNotificationRepository.Update(userNotification);
                }

                // 🚀 Step 8: Validate double-entry rule and save transaction
                if (_accountingService.EvaluateDoubleEntryRule(entries) && entries.Count > 1)
                {
                    await _uow.SaveAsync();

                    // 🚀 Step 9: Trigger branch-to-branch transfer event for further processing
                    var branchTransferDto = new BranchToBranchTransferDto
                    {
                        Id = cashReplenishmentRequest.ReferenceId,
                        ReferenceId = cashReplenishmentRequest.ReferenceId,
                        CurrencyNotesRequest = command.CurrencyNotesRequest,
                        Amount = command.Amount
                    };
                    await _mediator.Send(branchTransferDto);

                    // ✅ Transaction successful
                    return ServiceResponse<bool>.ReturnResultWith200(true, "Branch-to-branch transfer completed successfully.");
                }
                else
                {
                    errorMessage = "Accounting double-entry validation failed. Possible ledger imbalance detected. Contact an administrator.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Forbidden, LogAction.BranchToBranchTransferCashOut, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return422(errorMessage);
                }
            }
            catch (Exception ex)
            {
                // 🚨 Handle unexpected errors
                errorMessage = $"An unexpected error occurred while processing the branch transfer: {ex.Message}. Please try again or contact support.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.InternalServerError, LogAction.BranchToBranchTransferCashOut, LogLevelInfo.Critical);
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(ex, errorMessage);
            }
        }

        // Helper method to generate narration for accounting entries
        private string GenerateNaration(Data.Account destinationAccount, Data.Account sourceAccount, BranchToBranchTransferCommand command)
        {
            // Implement your narration logic here based on destinationAccount, sourceAccount, and command
            return $"Transfer from {sourceAccount.AccountName} to {destinationAccount.AccountName} for {command.Amount} FCFA";
        }
        private async Task LogError(string message, Exception ex, AddTransferEventCommand command)
        {
            // Common logic for logging errors and auditing
            _logger.LogError(ex, message);
            // Assuming APICallHelper.AuditLogger is a static utility method for auditing
            await APICallHelper.AuditLogger(_userInfoToken.Email, nameof(AddTransferEventCommand), command, message, "Error", 500, _userInfoToken.Token);
        }


    }
}


