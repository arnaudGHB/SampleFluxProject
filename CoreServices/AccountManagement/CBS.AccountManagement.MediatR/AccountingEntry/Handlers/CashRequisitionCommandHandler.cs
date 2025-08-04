using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;
using CBS.AccountManagement.Data.Dto.AccountingEntryDto;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.ExceptionHandler;
using Microsoft.Extensions.Configuration;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Data;
using CBS.AccountManagement.MediatR.AccountingEntry.Commands;

namespace CBS.AccountManagement.MediatR.Handlers
{
    public class CashRequisitionCommandHandler : IRequestHandler<CashRequisitionCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CashRequisitionCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ITransactionDataRepository _transactionDataRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;
        private readonly IMediator _mediator;


        public CashRequisitionCommandHandler(
            ITransactionDataRepository transactionDataRepository,
             IAccountingEntryRepository accountingEntryRepository,
    IOperationEventAttributeRepository operationEventAttributeRepository,
    IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
             IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<CashRequisitionCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken,
            IChartOfAccountManagementPositionRepository chartOfAccountRepository, IAccountingEntriesServices accountingEntriesServices, IMediator mediator)
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
            _mediator = mediator;
            _accountingService = accountingEntriesServices;
        }

        public async Task<ServiceResponse<bool>> Handle(CashRequisitionCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // List to store accounting entries
                List<Data.AccountingEntry> AccountingEntries = new List<Data.AccountingEntry>();
                List<Data.AccountingEntryDto> AccountingEntriesDto = new List<Data.AccountingEntryDto>();

                // Variables for accounts (to be populated later)
                Data.Account LiaisonAccount = new Data.Account();
                Data.Account TellerAccount = new Data.Account();
                Data.Account SupplierAccount = new Data.Account();
                Data.Account CommissionAccount = new Data.Account();

                // Check if the transaction reference already exists to prevent duplicate postings
                if (await _accountingService.TransactionExists(command.TransactionReferenceId))
                {
                    string duplicateTransactionMessage = $"A transaction with reference {command.TransactionReferenceId} has already been processed. " +
                                                         "Please verify the transaction details before retrying.";

                    await BaseUtilities.LogAndAuditAsync(duplicateTransactionMessage, command, HttpStatusCodeEnum.Conflict, LogAction.CashRequisition, LogLevelInfo.Information);
                    return ServiceResponse<bool>.Return409(duplicateTransactionMessage);
                }

                // Fetch cash movement accounts for the given event code
                var accounts = await _accountingService.GetCashMovementAccountByEventCode(command.EventCode, _userInfoToken.BranchId, _userInfoToken.BranchCode);

                // Validate if accounts were retrieved successfully
                if ( accounts.DeterminantAccount == null || accounts.BalancingAccount == null)
                {
                    string accountFetchError = $"Failed to retrieve the necessary accounts for event code: {command.EventCode}. " +
                                               "Please contact support if the issue persists.";

                    await BaseUtilities.LogAndAuditAsync(accountFetchError, command, HttpStatusCodeEnum.BadRequest, LogAction.CashRequisition, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return422(accountFetchError);
                }

                // Perform cash movement transactions
                AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(
                    command.Naration,
                    $"{_userInfoToken.FullName} {_userInfoToken.BankCode}[Cash_To_Vault]",
                    BaseUtilities.UtcNowToDoualaTime(),
                    accounts.DeterminantAccount,
                    accounts.BalancingAccount,
                    Convert.ToDecimal(command.Amount),
                    "CashRequisitionCommand",
                    command.TransactionReferenceId,
                    _userInfoToken.BranchId
                ));

                // Map DTOs to actual Accounting Entries
                AccountingEntries = _mapper.Map(AccountingEntriesDto, AccountingEntries);

                // Store accounting entries in the database
                _accountingEntryRepository.AddRange(AccountingEntries);

                // Validate double-entry accounting rule
                if (!_accountingService.EvaluateDoubleEntryRule(AccountingEntries))
                {
                    string doubleEntryError = "Accounting double-entry rule validation failed. " +
                                              "This indicates an imbalance in the ledger. Contact an administrator for assistance.";

                    await BaseUtilities.LogAndAuditAsync(doubleEntryError, command, HttpStatusCodeEnum.Forbidden, LogAction.CashRequisition, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return422(doubleEntryError);
                }

                // Save changes without affecting the branch ID
                await _uow.SaveAsyncWithOutAffectingBranchId();

                // Log and return success response
                await BaseUtilities.LogAndAuditAsync("Cash requisition transaction completed successfully.",
                    command, HttpStatusCodeEnum.OK, LogAction.CashRequisition, LogLevelInfo.Information);

                return ServiceResponse<bool>.ReturnResultWith200(true, "Transaction Completed Successfully");
            }
            catch (Exception ex)
            {
                // Handle unexpected errors and log them for debugging
                string unexpectedErrorMessage = $"An unexpected error occurred during cash requisition: {ex.Message}. " +
                                                "Please try again or contact support if the issue persists.";

                await BaseUtilities.LogAndAuditAsync(unexpectedErrorMessage, command, HttpStatusCodeEnum.InternalServerError, LogAction.CashRequisition, LogLevelInfo.Error);
                return ServiceResponse<bool>.Return403(false, unexpectedErrorMessage);
            }
        }

 
    }


}