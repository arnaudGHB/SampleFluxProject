using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.AccountingEntry.Commands;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.ExceptionHandler;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.AccountingEntry.Handlers
{
    public class AutoPostingEventCommandHandler : IRequestHandler<AutoPostingEventCommand, ServiceResponse<bool>>
    {


        private readonly IAccountingEntryRepository _accountingEntryRepository;


        private readonly IMapper _mapper;
        private readonly ILogger<AutoPostingEventCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IAccountTypeRepository _accountTypeRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;
        private readonly ICashMovementTrackingConfigurationRepository _chartOfAccountRepository;
        private readonly IMediator _mediator;
        public AutoPostingEventCommandHandler(


            IAccountingEntryRepository accountingEntryRepository,




            IMapper mapper,
            IConfiguration configuration,
            ILogger<AutoPostingEventCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken,
            IAccountingEntriesServices accountingEntriesServices

            )
        {
            _accountingEntryRepository = accountingEntryRepository;


            _mapper = mapper;
            _logger = logger;
            _uow = uow;


            _userInfoToken = userInfoToken;
            _configuration = configuration;


            _accountingService = accountingEntriesServices;


        }

        public async Task<ServiceResponse<bool>> Handle(AutoPostingEventCommand command, CancellationToken cancellationToken)
        {
            List<Data.AccountingEntry> entris = new List<Data.AccountingEntry>();
            AccountingRuleEntry ruleEntry = null;
            List<Data.Account> AccountLists = new List<Data.Account>();
            List<CashMovementAccount> accountAndOperationTypes = new List<CashMovementAccount>();
            string errorMessage = "";
            try
            {
                if (await _accountingService.TransactionExists(command.TransactionReferenceId))
                {
                    errorMessage = $"An entry has already been posted with this transaction Ref:{command.TransactionReferenceId}.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.AutoPostingEventCommand, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                if (command.IsmemberAccount())
                {
                    ruleEntry = _accountingService.GetAccountingEntryRuleByEventCode(command.GetAccountingEventCode());
                }
                else
                {
                    ruleEntry = _accountingService.GetAccountingEntryRuleByEventCode(command.Source);
                    if (ruleEntry == null)
                    {
                        errorMessage = $"AccountingEntryRule with ID {command.Source} not found.";
                        _logger.LogInformation(errorMessage);
                        await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.NotFound, LogAction.AutoPostingEventCommand, LogLevelInfo.Warning);

                        return ServiceResponse<bool>.Return404(errorMessage);
                    }
                }

                var sourceAccount = await _accountingService.GetAccountBasedOnChartOfAccountID(ruleEntry.DeterminationAccountId, _userInfoToken.BranchId, _userInfoToken.BranchCode);

                foreach (var item in command.AmountEventCollections)
                {
                    var ruleEn = _accountingService.GetAccountingEntryRuleByEventCode(item.EventCode);
                    if (ruleEn == null)
                    {
                        errorMessage = $"AccountingEntryRule with ID {item.EventCode} not found.";
                        _logger.LogInformation(errorMessage);

                        await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.NotFound, LogAction.AutoPostingEventCommand, LogLevelInfo.Warning);

                        return ServiceResponse<bool>.Return404(errorMessage);
                    }
                    var destinationAccount = await _accountingService.GetAccountBasedOnChartOfAccountID(ruleEn.DeterminationAccountId, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                    Booking booking = new Booking(item.Amount, 0, command.TransactionReferenceId, item.EventCode, item.Naration, _userInfoToken.BranchId);

                    Data.Account debitAccount = new Data.Account();
                    Data.Account creditAccount = new Data.Account();
                    List<AccountingEntryDto> listResult = new List<AccountingEntryDto>();

                    if (item.EventCode.ToLower().Contains("expense"))
                    {
                        listResult = await _accountingService.CashMovementAsync(item.Naration, command.MemberReference, command.TransactionDate, destinationAccount, sourceAccount, item.Amount, "AutoPostingEventCommand", command.TransactionReferenceId, _userInfoToken.BranchId);

                    }
                    else
                    {
                        listResult = await _accountingService.CashMovementAsync(item.Naration, command.MemberReference, command.TransactionDate, sourceAccount, destinationAccount, item.Amount, "AutoPostingEventCommand", command.TransactionReferenceId, _userInfoToken.BranchId);

                    }


                    entris = _mapper.Map(listResult, entris);
                    // Iterate through each accounting rule entry and perform the posting
                    _accountingEntryRepository.AddRange(entris);

                }



                if (!await ValidateAndSaveEntries(entris))
                {
                    errorMessage = "Accounting double entry rule not validated. Contact administrator.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage,
                    command, HttpStatusCodeEnum.Forbidden, LogAction.AutoPostingEventCommand, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return403(errorMessage);
                }
                errorMessage = "Account postings completed successfully.";
                _logger.LogInformation("Account postings completed successfully.");
                // Retrieve the accounting rule based on the provided AccountingRuleId
                await BaseUtilities.LogAndAuditAsync(errorMessage,
                    command, HttpStatusCodeEnum.OK, LogAction.AutoPostingEventCommand, LogLevelInfo.Information);


                return ServiceResponse<bool>.ReturnResultWith200(true, errorMessage);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                errorMessage = $"Error occurred while performing account postings: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage,
                          command, HttpStatusCodeEnum.Forbidden, LogAction.AddTransferEventCommand, LogLevelInfo.Error);
                return ServiceResponse<bool>.Return500(e, errorMessage);
            }
        }

        private async Task<bool> ValidateAndSaveEntries(List<Data.AccountingEntry> entries)
        {
            if (!_accountingService.EvaluateDoubleEntryRule(entries))
                return false;

            await _uow.SaveAsyncWithOutAffectingBranchId();
            return true;
        }

    }
}

