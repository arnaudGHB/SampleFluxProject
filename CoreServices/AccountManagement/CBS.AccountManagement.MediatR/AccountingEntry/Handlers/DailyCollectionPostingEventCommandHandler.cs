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
    public class DailyCollectionPostingEventCommandHandler : IRequestHandler<DailyCollectionPostingEventCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICashMovementTrackingConfigurationRepository _chartOfAccountRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DailyCollectionPostingEventCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ITransactionDataRepository _transactionDataRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;
        private readonly IMediator _mediator;


        public DailyCollectionPostingEventCommandHandler(
            ITransactionDataRepository transactionDataRepository,
             IAccountingEntryRepository accountingEntryRepository,
    IOperationEventAttributeRepository operationEventAttributeRepository,
    IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
             IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<DailyCollectionPostingEventCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken,
            ICashMovementTrackingConfigurationRepository chartOfAccountRepository, IAccountingEntriesServices accountingEntriesServices, IMediator mediator)
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

        public async Task<ServiceResponse<bool>> Handle(DailyCollectionPostingEventCommand command, CancellationToken cancellationToken)
        {
            List<Data.AccountingEntry> accountingEntries = new List<Data.AccountingEntry>();
            List<Data.AccountingEntryDto> accountingEntriesDto = new List<Data.AccountingEntryDto>();
            string errorMessage = "";

            try
            {
                // 🚀 Step 1: Check if transaction reference already exists
                if (await _accountingService.TransactionExists(command.TransactionReferenceId))
                {
                    errorMessage = $"An entry has already been posted with this transaction Ref: {command.TransactionReferenceId}.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.DailyCollectionPostingEventCommand, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }

                // 🚀 Step 2: Retrieve Teller Account
                var eventEntryRule = _accountingService.GetAccountEntryRuleByProductID(command.TellerSource);
                if (eventEntryRule == null)
                {
                    errorMessage = $"Entry rule with EventCode {command.TellerSource} not found.";
                    _logger.LogWarning(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.NotFound, LogAction.DailyCollectionPostingEventCommand, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                var tellerAccount = await _accountRepository.FindAsync(eventEntryRule.DeterminationAccountId);
                if (tellerAccount == null)
                {
                    errorMessage = $"Teller account with ID {eventEntryRule.DeterminationAccountId} not found.";
                    _logger.LogWarning(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.NotFound, LogAction.DailyCollectionPostingEventCommand, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                // 🚀 Step 3: Process Daily Amount Collection Entries
                foreach (var entry in command.DailyAmountCollection)
                {
                    Data.Account account = null;

                    // Process Principal Entry
                    if (entry.IsPrincipal)
                    {
                        var entryRule = _accountingService.GetAccountEntryRuleByProductID(entry.GetDailySavingAccountEventCode(command.ProductId));
                        if (entryRule == null)
                        {
                            errorMessage = "The event code for the principal amount cannot be found.";
                            _logger.LogWarning(errorMessage);
                            await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.NotFound, LogAction.DailyCollectionPostingEventCommand, LogLevelInfo.Warning);
                            return ServiceResponse<bool>.Return404(errorMessage);
                        }

                        account = _accountRepository.FindBy(x => x.ChartOfAccountManagementPositionId == entryRule.DeterminationAccountId
                                                              && x.AccountOwnerId == _userInfoToken.BranchId).FirstOrDefault();

                        if (account == null)
                        {
                            errorMessage = $"Account with ID {entryRule.DeterminationAccountId} not found.";
                            _logger.LogWarning(errorMessage);
                            await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.NotFound, LogAction.DailyCollectionPostingEventCommand, LogLevelInfo.Warning);
                            return ServiceResponse<bool>.Return404(errorMessage);
                        }

                        accountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(
                            entry.Naration,
                            command.MemberReference,
                            command.TransactionDate,
                            tellerAccount,
                            account,
                            entry.Amount,
                            "DailyCollectionPostingEventCommand",
                            command.TransactionReferenceId,
                            _userInfoToken.BranchId
                        ));
                    }
                    // Process Non-Principal Entry
                    else
                    {
                        var entryRule = _accountingService.GetAccountEntryRuleByProductID(command.ProductId + "@" + entry.EventAttributeName);
                        if (entryRule == null)
                        {
                            errorMessage = "The event code for this non-principal entry cannot be found.";
                            _logger.LogWarning(errorMessage);
                            await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.NotFound, LogAction.DailyCollectionPostingEventCommand, LogLevelInfo.Warning);
                            return ServiceResponse<bool>.Return404(errorMessage);
                        }

                        account = await _accountRepository.FindAsync(entryRule.DeterminationAccountId);
                        if (account == null)
                        {
                            errorMessage = $"Account with ID {entryRule.DeterminationAccountId} not found.";
                            _logger.LogWarning(errorMessage);
                            await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.NotFound, LogAction.DailyCollectionPostingEventCommand, LogLevelInfo.Warning);
                            return ServiceResponse<bool>.Return404(errorMessage);
                        }

                        accountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(
                            entry.Naration,
                            command.MemberReference,
                            command.TransactionDate,
                            tellerAccount,
                            account,
                            entry.Amount,
                            "DailyCollectionPostingEventCommand",
                            command.TransactionReferenceId,
                            _userInfoToken.BranchId
                        ));
                    }
                }

                // 🚀 Step 4: Process Accounting Entries
                accountingEntries = _mapper.Map(accountingEntriesDto, accountingEntries);
                _accountingEntryRepository.AddRange(accountingEntries);

                // 🚀 Step 5: Validate and Persist Double-Entry Rule
                if (_accountingService.EvaluateDoubleEntryRule(accountingEntries) && accountingEntries.Count > 1)
                {
                    await _uow.SaveAsync();
                }
                else
                {
                    errorMessage = "Accounting double-entry rule not validated. Contact the administrator.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Forbidden, LogAction.Create, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return422(errorMessage);
                }

                // ✅ Step 6: Return success response
                string successMessage = "Daily collection posting transaction completed successfully.";
                await BaseUtilities.LogAndAuditAsync(successMessage, command, HttpStatusCodeEnum.OK, LogAction.Create, LogLevelInfo.Information);
                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception ex)
            {
                // 🚨 Handle unexpected errors
                errorMessage = $"Error during daily collection posting transaction: {ex.Message}.";
                _logger.LogError(ex, errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.InternalServerError, LogAction.Create, LogLevelInfo.Critical);
                return ServiceResponse<bool>.Return500(ex, errorMessage);
            }
        }

        private bool CheckIfCommissionIsApplied(List<AmountCollection> amountCollection)
        {
            return amountCollection.Where(x => x.IsInterBankOperationPrincipalCommission == true).Any() && amountCollection.Count() > 0;
        }


        //private async Task<Data.Account> GetHomeLiaisonAccount(Data.ChartOfAccount accountChart, string BranchId,string BranchCodeX)
        //{
        //    var result = _accountRepository.FindBy(x => x.ChartOfAccountId == (accountChart.Id) && x.BranchId == (BranchId));
        //    if (result.Any())
        //    {
        //        return result.FirstOrDefault();
        //    }
        //    else
        //    {
        //        var branch = await _accountingService.GetBranchCodeAsync(BranchId);

        //        var model = new Commands.AddAccountCommand
        //        {
        //            AccountOwnerId = BranchId,
        //            BranchCodeX = BranchCodeX,
        //            AccountName = accountChart.LabelEn + " " + branch.name,
        //            AccountNumberManagementPosition = accountNumberManagementPosition,
        //            AccountNumber = accountChart.AccountNumber,
        //            AccountNumberNetwok = (accountChart.AccountNumber.PadRight(6, '0') + branch.bank.bankCode + BranchCodeX).PadRight(12, '0')+ accountNumberManagementPosition,
        //            AccountNumberCU = (accountChart.AccountNumber.PadRight(6, '0') + _userInfoToken.BranchCodeX).PadRight(9, '0') + accountNumberManagementPosition,
        //            ChartOfAccountId = accountChart.Id,
        //            AccountCategoryId = accountChart.AccountCartegoryId
        //        };
        //        await _mediator.Send(model);
        //        result = _accountRepository.FindBy(x => x.ChartOfAccountId == (accountChart.Id) && x.AccountOwnerId == (BranchId));
        //        return result.FirstOrDefault();
        //    }


        private OperationEventAttributeTypes GetOperationEventAttributeTypes(string operationType)
        {
            if (operationType.ToLower().Contains(OperationEventAttributeTypes.deposit.ToString()))
            {
                return OperationEventAttributeTypes.deposit;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.withdrawal.ToString()))
            {
                return OperationEventAttributeTypes.withdrawal;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.transfer.ToString()))
            {
                return OperationEventAttributeTypes.transfer;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.cashreplenishment.ToString()))
            {
                return OperationEventAttributeTypes.cashreplenishment;
            }
            else
            {
                return OperationEventAttributeTypes.none;
            }
        }

        private TransactionCode GetTransCode(string operationType)
        {
            if (operationType.ToLower().Contains(OperationEventAttributeTypes.deposit.ToString()))
            {
                return TransactionCode.CINT;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.withdrawal.ToString()))
            {
                return TransactionCode.COUT;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.transfer.ToString()))
            {
                return TransactionCode.TRANS;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.cashreplenishment.ToString()))
            {
                return TransactionCode.CRP;
            }
            else
            {
                return TransactionCode.None;
            }
        }
        private decimal GetPrincipal(List<AmountCollection> amountCollection)
        {
            return amountCollection.Where(x => x.IsPrincipal == true).FirstOrDefault().Amount;
        }

        private async Task LogError(string message, Exception ex, MakeAccountPostingCommand command)
        {
            // Common logic for logging errors and auditing
            _logger.LogError(ex, message);
            // Assuming APICallHelper.AuditLogger is a static utility method for auditing
            await APICallHelper.AuditLogger(_userInfoToken.Email, nameof(MakeAccountPostingCommand), command, message, "Error", 500, _userInfoToken.Token);
        }



    }


}