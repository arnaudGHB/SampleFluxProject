using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the AddTransferEventCommand by managing accounting entries, validating accounts, and processing fund transfers.
    /// </summary>
    public class AddTransferEventCommandHandler : IRequestHandler<AddTransferEventCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICashMovementTrackingConfigurationRepository _chartOfAccountRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<AddTransferEventCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ITransactionDataRepository _transactionDataRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;

        public AddTransferEventCommandHandler(
            ITransactionDataRepository transactionDataRepository,
            IAccountingEntryRepository accountingEntryRepository,
            IOperationEventAttributeRepository operationEventAttributeRepository,
            IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
            IMediator mediator,
            IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<AddTransferEventCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            UserInfoToken userInfoToken,
            ICashMovementTrackingConfigurationRepository chartOfAccountRepository,
            IAccountingEntriesServices accountingEntriesServices)
        {
            _transactionDataRepository = transactionDataRepository;
            _accountingEntryRepository = accountingEntryRepository;
            _operationEventAttributeRepository = operationEventAttributeRepository;
            _accountingRuleEntryRepository = accountingRuleEntryRepository;
            _mapper = mapper;
            _mediator = mediator;
            _accountRepository = accountRepository;
            _configuration = configuration;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _chartOfAccountRepository = chartOfAccountRepository;
            _accountingService = accountingEntriesServices;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> Handle(AddTransferEventCommand command, CancellationToken cancellationToken)
        {
            string errorMessage = "";
            try
            {
                if (await _accountingService.TransactionExists(command.TransactionReferenceId))
                {
                      errorMessage = $"An entry has already been posted with this transaction Ref:{command.TransactionReferenceId}.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage,
                    command, HttpStatusCodeEnum.Conflict, LogAction.AddTransferEventCommand, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }

                var fromAccount = await ValidateAccountAsync(command.FromProductId, command, true);
                var toAccount = await ValidateAccountAsync(command.ToProductId, command, false);
                var accountingEntries = new List<AccountingEntryDto>();

                if (command.IsInterBranchTransaction)
                {
                    await HandleInterBranchTransaction(command, fromAccount, toAccount, accountingEntries);
                }
                else
                {
                    await HandleIntraBranchTransaction(command, fromAccount, toAccount, accountingEntries);
                }

                if (!await SaveAccountingEntries(accountingEntries))
                {
                    errorMessage = "Accounting double entry rule not validated. Contact administrator.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage,
                    command, HttpStatusCodeEnum.Forbidden, LogAction.AddTransferEventCommand, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return422(errorMessage);
                }
                errorMessage = "Transaction Completed Successfully";
                await BaseUtilities.LogAndAuditAsync(errorMessage,
                 command, HttpStatusCodeEnum.OK, LogAction.AddTransferEventCommand, LogLevelInfo.Information);

                return ServiceResponse<bool>.ReturnResultWith200(true, errorMessage);
            }
            catch (Exception ex)
            {
                  errorMessage = ex.Message;
                await BaseUtilities.LogAndAuditAsync(errorMessage,
                      command, HttpStatusCodeEnum.InternalServerError, LogAction.AddTransferEventCommand, LogLevelInfo.Error);
                _logger.LogError(ex, errorMessage);
                return ServiceResponse<bool>.Return500(ex, errorMessage);
            }
        }

    

        private async Task<Data.Account> ValidateAccountAsync(string productId, AddTransferEventCommand command, bool isSourceAccount)
        {
            var entryRuleId = command.GetOperationEventCode(productId);
            if (string.IsNullOrEmpty(entryRuleId))
            {
                throw await CreateValidationException(command, "No accounting rule for this product.", 404);
            }

            var entryRule = _accountingService.GetAccountEntryRuleByProductID(entryRuleId);
            if (entryRule?.DeterminationAccountId == null)
            {
                throw await CreateValidationException(command, "No active account associated with this rule.", 422);
            }

            return await _accountingService.GetAccountForProcessing(entryRule.DeterminationAccountId, command);
        }

        private async Task<Exception> CreateValidationException(AddTransferEventCommand command, string message, int statusCode)
        {
            // Log the validation error or handle it as needed.
            await LogAudit(command, message, LogLevelInfo.Warning, statusCode);

            // Return a custom exception or a standard one with the message and status code.
            return new Exception($"Validation Error: {message} (Status Code: {statusCode})");
        }
    
        private async Task LogAudit(AddTransferEventCommand command, string message, LogLevelInfo warning, int statusCode)
        {
            await APICallHelper.AuditLogger(_userInfoToken.Email, "AutoPostingEventCommand", command, message, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);

        }

        private async Task HandleInterBranchTransaction(AddTransferEventCommand command, Data.Account fromAccount, Data.Account toAccount, List<AccountingEntryDto> accountingEntries)
        {
            if (command.FromProductId == command.ToProductId)
            {
                await ProcessAmountCollection(command, fromAccount, accountingEntries);
            }
            else
            {
                var liaisonAccount = await _accountingService.GetLiaisonAccount(command);
                var principalTransferEntries = await _accountingService.TransferFundBetweenBranch( "",
                    command.FromMemberReference,
                    command.TransactionDate,
                    fromAccount,
                    toAccount,
                    liaisonAccount,
                    command.GetPrincipalAmount(),
                    command);
                accountingEntries.AddRange(principalTransferEntries);

                if (command.AmountCollection.Count > 1)
                {
                    await ProcessAmountCollection(command, fromAccount, accountingEntries);
                }
            }
        }

        private async Task HandleIntraBranchTransaction(AddTransferEventCommand command, Data.Account fromAccount, Data.Account toAccount, List<AccountingEntryDto> accountingEntries)
        {
            if (command.FromProductId == command.ToProductId)
            {
                await ProcessAmountCollection(command, fromAccount, accountingEntries);
            }
            else
            {
                var principalTransferEntries = await _accountingService.CashMovementAsync( "",
                    command.FromMemberReference,
                    command.TransactionDate,
                    fromAccount,
                    toAccount,
                    command.GetPrincipalAmount(),
                    "AddTransferEventCommand", command.TransactionReferenceId,_userInfoToken.BranchId);
                accountingEntries.AddRange(principalTransferEntries);

                if (command.AmountCollection.Count > 1)
                {
                    await ProcessAmountCollection(command, fromAccount, accountingEntries);
                }
            }
        }

        private async Task ProcessAmountCollection(AddTransferEventCommand command, Data.Account fromAccount, List<AccountingEntryDto> accountingEntries)
        {
            foreach (var item in command.AmountCollection.Where(item => item.Amount > 0))
            {
                var commissionAccount = await _accountingService.GetCommissionAccount(
                    item,
                    command.FromProductId,
                    command.ExternalBranchId,
                    command.ExternalBranchCode);

                var entries = await _accountingService.CashMovementAsync(
                    item.Naration,
                    command.FromMemberReference,
                    command.TransactionDate,
                    fromAccount,
                    commissionAccount,
                    item.Amount,
                    "AddTransferEventCommand",
                    command.TransactionReferenceId, _userInfoToken.BranchId);

                accountingEntries.AddRange(entries);
            }
        }

        private async Task<bool> SaveAccountingEntries(List<AccountingEntryDto> accountingEntries)
        {
            var entries = _mapper.Map<List<Data.AccountingEntry>>(accountingEntries);
            _accountingEntryRepository.AddRange(entries);

            if (_accountingService.EvaluateDoubleEntryRule(entries) && entries.Count > 1)
            {
                await _uow.SaveAsyncWithOutAffectingBranchId();
                return true;
            }

            return false;
        }
    }
}
