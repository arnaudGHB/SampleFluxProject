using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
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
    public class OpeningOfDayPostingEventCommandHandler : IRequestHandler<OpeningOfDayEventCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingRuleRepository _accountingRuleRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITellerDailyProvisionRepository _tellerDailyProvisionRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly IOperationEventRepository _operationEventRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OpeningOfDayPostingEventCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IAccountTypeRepository _accountTypeRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;
        private readonly ICashMovementTrackingConfigurationRepository _chartOfAccountRepository;
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountManagementPositionRepository;
        private readonly IMediator _mediator;
        public OpeningOfDayPostingEventCommandHandler(
            IAccountRepository accountRepository,
            IAccountingEntryRepository accountingEntryRepository,
      IAccountTypeRepository accountTypeRepository,
            IAccountingRuleRepository accountingRuleRepository,
            IAccountingRuleEntryRepository accountingRuleEntryRepository,
           ITellerDailyProvisionRepository tellerDailyProvisionRepository,
           ICashMovementTrackingConfigurationRepository chartOfAccountRepository,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<OpeningOfDayPostingEventCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken, IAccountingEntriesServices accountingEntriesServices, IOperationEventAttributeRepository? operationEventAttributeRepository, IOperationEventRepository? operationEventRepository)
        {
            _accountingEntryRepository = accountingEntryRepository;
            _accountRepository = accountRepository;
            _accountingRuleEntryRepository = accountingRuleEntryRepository;
            _accountingRuleRepository = accountingRuleRepository;
            _operationEventAttributeRepository = operationEventAttributeRepository;
            _operationEventRepository = operationEventRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _accountTypeRepository = accountTypeRepository;
            _userInfoToken = userInfoToken;
            _configuration = configuration;
            _tellerDailyProvisionRepository = tellerDailyProvisionRepository;
            ILogger<AccountingEntriesServices> _serviceLogger = new LoggerFactory().CreateLogger<AccountingEntriesServices>();
            _accountingService = accountingEntriesServices;
            _chartOfAccountRepository = chartOfAccountRepository;
            _chartOfAccountManagementPositionRepository = chartOfAccountManagementPosition;
        }

        public IChartOfAccountManagementPositionRepository? chartOfAccountManagementPosition { get; }

        public async Task<ServiceResponse<bool>> Handle(OpeningOfDayEventCommand command, CancellationToken cancellationToken)
        {
            List<Data.Account> AccountLists = new List<Data.Account>();
            List<Data.AccountingEntry> entris = new List<Data.AccountingEntry>();
            List<CashMovementAccount> accountAndOperationTypes = new List<CashMovementAccount>();
            string errorMessage = "";
            try
            {

                if (await _accountingService.TransactionExists(command.TransactionReferenceId))
                {
                    errorMessage = $"An entry has already been posted with this transaction Ref:{command.TransactionReferenceId}.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.Create, LogLevelInfo.Information);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                else
                {
                    var ruleEntries = _accountingService.GetAccountingEntryRule(command.EventCode);
                    if (ruleEntries?.Count == 0)
                    {
                        errorMessage = $"AccountingEntryRule with ID {command.EventCode} not found.";
                        _logger.LogInformation(errorMessage);
                        await APICallHelper.AuditLogger(_userInfoToken.Email, "OpeningAndClosingOfDayPostingEventCommand",
                            command, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                        return ServiceResponse<bool>.Return404(errorMessage);
                    }
                    var collection = await _accountingService.GetListAccountAndOperationTypesAsync(ruleEntries, command.EventCode, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                    //Booking booking = new Booking(command.Amount, command.AmountDifference, command.TransactionReferenceId, GetPostingType(command), command.getEventDescription());

                    //foreach (var item in collection)
                    //{

                    //    entris.AddRange(await _accountingService.CashMovementAsync(item.Naration, (_userInfoToken.FullName + " " + _userInfoToken.BranchCodeX), command.TransactionDate, item.SourceAccount, item.DestinationAccount, command.Amount, "OpeningOfTheDayCommand", command.TransactionReferenceId));

                    //}
                    //listResult.Add(creditEntry);

                    //entris = _mapper.Map(listResult, entris);
                    // Iterate through each accounting rule entry and perform the posting
                    _accountingEntryRepository.AddRange(entris);



                    //TellerDailyProvision tellerDailyProvision = new TellerDailyProvision(command.EventCode, command.Amount, command.AmountDifference);

                    //tellerDailyProvision.Id = command.TransactionReferenceId;
                    //_tellerDailyProvisionRepository.Add(tellerDailyProvision);
                    await _uow.SaveAsync();
                    _logger.LogInformation("Account postings completed successfully.");
                    // Retrieve the accounting rule based on the provided AccountingRuleId
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "OpeningAndClosingOfDayPostingEventCommand",
                        command, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                    return ServiceResponse<bool>.ReturnResultWith200(true);
                }



            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                errorMessage = $"Error occurred while performing account postings: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "OpeningAndClosingOfDayPostingEventCommand",
              command, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<bool>.Return500(e, errorMessage);
            }
        }




        private string GetPostingType(OpeningOfDayEventCommand command)
        {
            if (command.EventCode.Equals(AccountingEvent.NORMAL_OOD))
            {
                return "OPENING OF DAY POSTING";
            }
            else if (command.EventCode.Equals(AccountingEvent.NEGATIVE_OOD))
            {
                return "OPENING OF DAY POSTING WITH NEGATIVE BALANCE";
            }
            else if (command.EventCode.Equals(AccountingEvent.POSITIVE_OOD))
            {
                return "OPENING OF DAY POSTING WITH POSITIVE BALANCE";
            }
            else if (command.EventCode.Equals(AccountingEvent.NORMAL_COD))
            {
                return "CLOSING OF DAY POSTING";
            }
            else if (command.EventCode.Equals(AccountingEvent.NEGATIVE_COD))
            {
                return "CLOSING OF DAY POSTING WITH NEGATIVE BALANCE";
            }
            else if (command.EventCode.Equals(AccountingEvent.POSITIVE_COD))
            {
                return "CLOSING OF DAY POSTING WITH POSITIVE BALANCE";
            }
            else
            {
                return "POSITING ERROR";
            }
        }

    }


}

