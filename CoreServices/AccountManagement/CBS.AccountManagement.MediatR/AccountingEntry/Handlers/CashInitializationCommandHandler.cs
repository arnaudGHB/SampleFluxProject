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
using DocumentFormat.OpenXml.Spreadsheet;
using FluentValidation.Validators;

namespace CBS.AccountManagement.MediatR.Handlers
{
    public class CashInitializationCommandHandler : IRequestHandler<CashInitializationCommand, ServiceResponse<CashInitResponse>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CashInitializationCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ITransactionDataRepository _transactionDataRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;
        private readonly IMediator _mediator;
        private const  string EventCode = "Cash_To_Vault";
        private const string DAILY_CASH_DEFICIT = "DAILY_CASH_DEFICIT";
        private const string DAILY_CASH_SURPLUS = "DAILY_CASH_SURPLUS";
   

        public CashInitializationCommandHandler(
            ITransactionDataRepository transactionDataRepository,
             IAccountingEntryRepository accountingEntryRepository,
    IOperationEventAttributeRepository operationEventAttributeRepository,
    IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
             IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<CashInitializationCommandHandler> logger,
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
        public static void Swap(decimal   a, decimal b)
        {
            decimal temp = a;
            a = b;
            b = temp;
        }


        public async Task<ServiceResponse<CashInitResponse>> Handle(CashInitializationCommand command, CancellationToken cancellationToken)
        {
            CashInitResponse response = new CashInitResponse();
            try
            {
                var allEntries = new List<Data.AccountingEntryDto>();
                string branchId = _userInfoToken.BranchId;
                string branchCode = _userInfoToken.BranchCode;

                // Fetch accounting data for validation
                var accounts = await _accountingService.GetCashMovementAccountByEventCode(EventCode, branchId, branchCode);
                decimal balanceDifference = command.Amount - accounts.DeterminantAccount.CurrentBalance;

                // Ensure vault amount matches the recorded general ledger balance
                if (command.AmountInVault != accounts.BalancingAccount.CurrentBalance)
                {
                    string errorMsg = "The vault amount entered does not match the value recorded in the general ledger. " +
                                      "Please verify the actual vault amount and retry.";
                    await BaseUtilities.LogAndAuditAsync(errorMsg,
                  command, HttpStatusCodeEnum.Forbidden, LogAction.VaultInitilization, LogLevelInfo.Warning);
                    response = new CashInitResponse(false, errorMsg);
                    await LogAndReturnError(command, errorMsg);
                }

                // Validate cash balance differences and inform user accordingly
                if (balanceDifference < 0)
                {
                    response = new CashInitResponse(false,
                        $"There is a cash deficit of XAF {Math.Abs(balanceDifference):N}. " +
                        "Please verify the cash balance before proceeding.");
                }
                else if (balanceDifference > 0)
                {
                    response = new CashInitResponse(false,
                        $"There is a cash surplus of XAF {balanceDifference:N}. " +
                        "Ensure correct reconciliation before proceeding.");
                }
                else
                {
                    // If the cash amount is balanced, perform standard cash initialization
                    allEntries.AddRange(await ProcessCashMovement(command, accounts.BalancingAccount, accounts.DeterminantAccount));
                }

                // Handle scenarios where the user has explicitly agreed to proceed despite deficit/surplus
                if (balanceDifference != 0 && command.CanProceed == true)
                {
                    string eventCode = balanceDifference < 0 ? DAILY_CASH_DEFICIT : DAILY_CASH_SURPLUS;
                    var adjustmentAccount = await _accountingService.GetCashMovementAccountByEventCode(eventCode, branchId, branchCode);
                    decimal absDifference = Math.Abs(balanceDifference);

                    // Log adjustment processing
                    await BaseUtilities.LogAndAuditAsync($"Processing cash adjustment due to a {(balanceDifference < 0 ? "deficit" : "surplus")} of XAF {absDifference:N}.",
                        command, HttpStatusCodeEnum.Forbidden, LogAction.VaultInitilization, LogLevelInfo.Warning);

                    // Adjust cash movement entries accordingly
                    allEntries.AddRange(await ProcessCashMovement(command, adjustmentAccount.DeterminantAccount, accounts.DeterminantAccount, absDifference));
                    allEntries.AddRange(await ProcessCashMovement(command, accounts.BalancingAccount, accounts.DeterminantAccount));
                }

                // Final validation before committing to the database
                if (allEntries.Any())
                {
                    _accountingEntryRepository.AddRange(_mapper.Map<List<Data.AccountingEntry>>(allEntries));

                    if (_accountingService.EvaluateDoubleEntryRule(_mapper.Map<List<Data.AccountingEntry>>(allEntries)))
                    {
                        await _uow.SaveAsync();

                        // Log successful transaction
                        await BaseUtilities.LogAndAuditAsync("Cash initialization completed successfully.",
                            command, HttpStatusCodeEnum.OK, LogAction.VaultInitilization, LogLevelInfo.Information);

                        return ServiceResponse<CashInitResponse>.ReturnResultWith200(
                            new CashInitResponse(true, "Cash initialization was successfully processed."),
                            "Transaction completed successfully."
                        );
                    }
                }

                // If we reach here, something went wrong with the double-entry validation
                return ServiceResponse<CashInitResponse>.Return403(response, response.Naration);
            }
            catch (Exception ex)
            {
                // Log unexpected errors in the process
                string errorMessage = $"Unexpected error occurred during cash initialization: {ex.Message}. Please contact support.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.InternalServerError, LogAction.VaultInitilization, LogLevelInfo.Error);

                return ServiceResponse<CashInitResponse>.Return403(response,response.Naration);
            }
        }

        /// <summary>
        /// Logs an error message and throws an exception.
        /// This ensures error messages are standardized and user-friendly.
        /// </summary>
        private async Task LogAndReturnError(CashInitializationCommand command, string errorMessage)
        {
            await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.BadRequest, LogAction.VaultInitilization, LogLevelInfo.Warning);
            throw new Exception(errorMessage);
        }

        /// <summary>
        /// Processes cash movement transactions between accounts.
        /// Helps reduce code repetition and standardizes transaction logging.
        /// </summary>
        private async Task<List<Data.AccountingEntryDto>> ProcessCashMovement(CashInitializationCommand command, Data.Account accountFrom, Data.Account accountTo, decimal? amountOverride = null)
        {
            decimal amount = amountOverride ?? command.Amount;

            await BaseUtilities.LogAndAuditAsync($"Processing cash movement: {accountFrom.AccountName} → {accountTo.AccountName}, Amount: XAF {amount:N}.",
                command, HttpStatusCodeEnum.OK, LogAction.VaultInitilization, LogLevelInfo.Information);

            return await _accountingService.CashMovementAsync(
                command.Naration,
                $"{_userInfoToken.FullName} CASHINIT",
                command.AccountingDate,
                accountFrom,
                accountTo,
                amount,
                "CASH_INITIALIZATION_IN_VAULT",
                command.TransactionReference,
                _userInfoToken.BranchId
            );
        }




    }


}