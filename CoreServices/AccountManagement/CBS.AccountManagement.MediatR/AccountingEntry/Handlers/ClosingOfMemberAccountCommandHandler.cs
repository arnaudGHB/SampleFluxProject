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

using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CBS.AccountManagement.MediatR.Handlers
{


    public class ClosingOfMemberAccountCommandHandler : IRequestHandler<ClosingOfMemberAccountCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICashMovementTrackingConfigurationRepository _chartOfAccountRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<ClosingOfMemberAccountCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ITransactionDataRepository _transactionDataRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;


        public ClosingOfMemberAccountCommandHandler(
            ITransactionDataRepository transactionDataRepository,
             IAccountingEntryRepository accountingEntryRepository,
    IOperationEventAttributeRepository operationEventAttributeRepository,
    IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
            IMediator mediator,
             IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<ClosingOfMemberAccountCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken,
            ICashMovementTrackingConfigurationRepository chartOfAccountRepository, IAccountingEntriesServices accountingEntriesServices)
        {
            _accountRepository = accountRepository;
            _operationEventAttributeRepository = operationEventAttributeRepository;
            _accountingEntryRepository = accountingEntryRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _transactionDataRepository = transactionDataRepository;
            _userInfoToken = userInfoToken;
            _configuration = configuration;
            _chartOfAccountRepository = chartOfAccountRepository;
            _accountingRuleEntryRepository = accountingRuleEntryRepository;
            
          _accountingService = accountingEntriesServices;
            _mediator = mediator;
        }

        public async Task<ServiceResponse<bool>> Handle(ClosingOfMemberAccountCommand command, CancellationToken cancellationToken)
        {
            List<AccountingEntryDto> accountingEntries = new List<AccountingEntryDto>();
            string errorMessage = "";

            try
            {
                // 🚀 Step 1: Check if transaction reference already exists
                if (await _accountingService.TransactionExists(command.TransactionReference))
                {
                    errorMessage = $"An entry has already been posted with this transaction Ref: {command.TransactionReference}.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.ClosingOfMemberAccountCommand, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }

                // 🚀 Step 2: Retrieve accounting rule for the event code
                var ruleEntry = _accountingService.GetAccountEntryRuleByEventCode(command.EventCode);
                if (ruleEntry == null)
                {
                    errorMessage = $"Accounting entry rule not found for EventCode: {command.EventCode}.";
                    _logger.LogInformation(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.NotFound, LogAction.ClosingOfMemberAccountCommand, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                string RuleId = ruleEntry.Id; // ✅ Assign RuleId from ruleEntry

                // 🚀 Step 3: Get the destination account
                var destinationAccount = await _accountingService.GetAccountBasedOnChartOfAccountID(RuleId, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                if (destinationAccount == null)
                {
                    errorMessage = $"Destination account not found for RuleId: {RuleId}.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.NotFound, LogAction.ClosingOfMemberAccountCommand, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                // 🚀 Step 4: Process each member account
                foreach (var item in command.MemberAccountDetails)
                {
                    var sourceAccount = await GetProductAccount(item);
                    if (sourceAccount == null)
                    {
                        errorMessage = $"Source account for product {item.ProductName} not found.";
                        _logger.LogInformation(errorMessage);
                        await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.NotFound, LogAction.ClosingOfMemberAccountCommand, LogLevelInfo.Warning);
                        return ServiceResponse<bool>.Return404(errorMessage);
                    }

                    var entries = await _accountingService.CashMovementAsync(
                        GenerateNaration(item.Amount.ToString(), sourceAccount, destinationAccount, command),
                        command.MemberReference,
                        command.TransactionDate,
                        sourceAccount,
                        destinationAccount,
                        item.Amount,
                        "ClosingOfMembersAccount",
                        command.TransactionReference,
                        _userInfoToken.BranchId
                    );

                    accountingEntries.AddRange(entries);
                }

                // 🚀 Step 5: Map and store accounting entries
                var mappedEntries = _mapper.Map<List<Data.AccountingEntry>>(accountingEntries);
                _accountingEntryRepository.AddRange(mappedEntries);

                // 🚀 Step 6: Validate and persist double-entry rule
                if (_accountingService.EvaluateDoubleEntryRule(mappedEntries) && mappedEntries.Count > 1)
                {
                    await _uow.SaveAsync();
                }
                else
                {
                    errorMessage = "Accounting double entry rule not validated. Contact the administrator.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Forbidden, LogAction.Create, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return422(errorMessage);
                }

                // ✅ Step 7: Return success response
                string successMessage = "Member account closing transaction completed successfully.";
                await BaseUtilities.LogAndAuditAsync(successMessage, command, HttpStatusCodeEnum.OK, LogAction.Create, LogLevelInfo.Information);
                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception ex)
            {
                // 🚨 Handle unexpected errors
                errorMessage = $"Error during member account closing: {ex.Message}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.InternalServerError, LogAction.Create, LogLevelInfo.Critical);
                _logger.LogError(ex, errorMessage);
                return ServiceResponse<bool>.Return500(ex, errorMessage);
            }
        }

        private string GenerateNaration(string amount, Data.Account sourceAccount, Data.Account destinationAccount, ClosingOfMemberAccountCommand command)
        {
            return $"CashOut of XAF {amount} from {sourceAccount.AccountNumberCU}-{sourceAccount.AccountName} to " +
                $"{destinationAccount.AccountNumberCU}-{destinationAccount.AccountName} Ref:{command.TransactionReference}";

        }


        private async Task<Data.Account> GetProductAccount(MemberAccountDetail command)
        {
            string errorMessage = string.Empty;
            var FromEntryRuleId = command.GetOperationEventCode(command.ProductId);
            if (String.IsNullOrEmpty(FromEntryRuleId))
            {
                errorMessage = $"There is no accounting rule for this product : {command.ProductId} please contact system administrator";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddTransferEventCommand",
                    command, errorMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);

                _logger.LogError(errorMessage);
                throw new Exception(errorMessage);
            }
            var FromEntryRule = _accountingService.GetAccountEntryRuleByProductID(FromEntryRuleId);
            if (FromEntryRule == null)
            {
                errorMessage = $"There is no accounting rule for Id : {FromEntryRuleId} please contact system administrator";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddTransferEventCommand",
                    command, errorMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);

                _logger.LogError(errorMessage);
                throw new Exception(errorMessage);
            }
            if (FromEntryRule.DeterminationAccountId == null)
            {
                errorMessage = $"There is currently no active account associated with this AccountingEntryRuleID:{FromEntryRuleId} investment portfolio to deposit generated income. Please contact your system administrator to create an account to receive portfolio distributions.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddTransferEventCommand",
                   command, errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }
            var FromAccount = await _accountingService.GetAccountBasedOnChartOfAccountID(FromEntryRule.DeterminationAccountId, _userInfoToken.BranchId, _userInfoToken.BranchCode);


            return FromAccount;
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
