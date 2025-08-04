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
    /// Handles the MakeNonCashAccountAdjustmentCommand by managing accounting entries, validating accounts, and processing fund transfers.
    /// </summary>
    public class MakeNonCashAccountAdjustmentCommandHandler : IRequestHandler<MakeNonCashAccountAdjustmentCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountRepository;
  
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<MakeNonCashAccountAdjustmentCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ITransactionDataRepository _transactionDataRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;

        public MakeNonCashAccountAdjustmentCommandHandler(
            ITransactionDataRepository transactionDataRepository,
            IAccountingEntryRepository accountingEntryRepository,
            IOperationEventAttributeRepository operationEventAttributeRepository,
            IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
            IMediator mediator,
            IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<MakeNonCashAccountAdjustmentCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            UserInfoToken userInfoToken,
            IChartOfAccountManagementPositionRepository chartOfAccountRepository,
            IAccountingEntriesServices accountingEntriesServices)
        {
            _accountingEntryRepository = accountingEntryRepository;
            _mapper = mapper;
            _mediator = mediator;
            _accountRepository = accountRepository;
            _configuration = configuration;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
 
            _accountingService = accountingEntriesServices;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> Handle(MakeNonCashAccountAdjustmentCommand command, CancellationToken cancellationToken)
        {
            string errorMessage = "";
           Data.Account fromAccount = new Data.Account();
            Data.Account toAccount = new Data.Account();
            try
            {
                if (await _accountingService.TransactionExists(command.TransactionReference))
                {
                      errorMessage = $"An entry has already been posted with this transaction Ref:{command.TransactionReference}.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Forbidden, LogAction.MakeNonCashAccountAdjustmentCommand, LogLevelInfo.Warning);

                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                if (command.Source.ToLower()=="memberaccount")
                {
                    if (command.BookingDirection.ToLower() == "debit")
                    {
                        fromAccount = await ValidateAccountAsync(command.ProductId, command, true);
                        toAccount = await _accountingService.GetMFI_ChartOfAccount(command.ChartOfAccountId, _userInfoToken.BranchId, _userInfoToken.BranchCode);

                    }
                    else
                    {
                        toAccount = await ValidateAccountAsync(command.ProductId, command, true);
                        fromAccount = await _accountingService.GetMFI_ChartOfAccount(command.ChartOfAccountId, _userInfoToken.BranchId, _userInfoToken.BranchCode);


                    }
                }
                else
                {
                    if (command.BookingDirection.ToLower() == "debit")
                    {
                        fromAccount = await _accountingService.GetMFI_ChartOfAccount(command.ChartOfAccountId, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                        toAccount = await ValidateAccountAsync(command.ProductId, command, true);

                    }
                    else
                    {
                        toAccount = await ValidateAccountAsync(command.ProductId, command, true);
                         fromAccount = await _accountingService.GetMFI_ChartOfAccount(command.ChartOfAccountId, _userInfoToken.BranchId, _userInfoToken.BranchCode);


                    }
          
                }
                var accountingEntries = await _accountingService.CashMovementAsync(command.Narration, command.MemberReference, command.TransactionDate, fromAccount, toAccount, command.Amount, "MakeNonCashAccountAdjustmentCommand", command.TransactionReference, _userInfoToken.BranchId, false, _userInfoToken.BranchId);




                if (!await SaveAccountingEntries(accountingEntries))
                {
                      errorMessage = $"Accounting double entry rule not validated. Contact administrator.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Forbidden, LogAction.MakeNonCashAccountAdjustmentCommand, LogLevelInfo.Warning);

                    return ServiceResponse<bool>.Return403("Accounting double entry rule not validated. Contact administrator.");
                }

                return ServiceResponse<bool>.ReturnResultWith200(true, "Transaction Completed Successfully");
            }
            catch (Exception ex)
            {
                  errorMessage = ex.Message;
                await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.InternalServerError, LogAction.MakeNonCashAccountAdjustmentCommand, LogLevelInfo.Error);


                _logger.LogError(ex, errorMessage);
                return ServiceResponse<bool>.Return500(ex, errorMessage);
            }
        }

    

        private async Task<Data.Account> ValidateAccountAsync(string productId, MakeNonCashAccountAdjustmentCommand command, bool isSourceAccount)
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

        private async Task<Exception> CreateValidationException(MakeNonCashAccountAdjustmentCommand command, string message, int statusCode)
        {
            // Log the validation error or handle it as needed.
            await LogAudit(command, message, LogLevelInfo.Warning, statusCode);

            // Return a custom exception or a standard one with the message and status code.
            return new Exception($"Validation Error: {message} (Status Code: {statusCode})");
        }
    
        private async Task LogAudit(MakeNonCashAccountAdjustmentCommand command, string message, LogLevelInfo warning, int statusCode)
        {
            await APICallHelper.AuditLogger(_userInfoToken.Email, "MakeNonCashAccountAdjustmentCommand", command, message, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);

        }

 
        private async Task<bool> SaveAccountingEntries(List<AccountingEntryDto> accountingEntries)
        {
            var entries = _mapper.Map<List<Data.AccountingEntry>>(accountingEntries);
            _accountingEntryRepository.AddRange(entries);

            if (_accountingService.EvaluateDoubleEntryRule(entries) && entries.Count > 1)
            {
                await _uow.SaveAsync();
                return true;
            }

            return false;
        }
    }
}
