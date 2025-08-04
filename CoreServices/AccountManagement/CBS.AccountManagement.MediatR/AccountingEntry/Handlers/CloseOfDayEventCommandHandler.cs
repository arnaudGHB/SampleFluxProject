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
    //public class CloseOfDayEventCommandHandler : IRequestHandler<CloseOfDayEventCommand, ServiceResponse<bool>>
    //{
    //    private readonly IAccountingRuleRepository _accountingRuleRepository;
    //    private readonly IAccountRepository _accountRepository;
    //    private readonly ITellerDailyProvisionRepository _tellerDailyProvisionRepository;
    //    private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
    //    private readonly IAccountingEntryRepository _accountingEntryRepository;
    //    private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
    //    private readonly IOperationEventRepository _operationEventRepository;
    //    private readonly IMapper _mapper;
    //    private readonly ILogger<CloseOfDayEventCommandHandler> _logger;
    //    private readonly IUnitOfWork<POSContext> _uow;
    //    private readonly IAccountTypeRepository _accountTypeRepository;
    //    private readonly UserInfoToken _userInfoToken;
    //    private readonly IConfiguration _configuration;
    //    private readonly IAccountingEntriesServices _accountingService;

    //    public CloseOfDayEventCommandHandler(
    //        IAccountRepository accountRepository,
    //        IAccountingEntryRepository accountingEntryRepository,
    //  IAccountTypeRepository accountTypeRepository,
    //        IAccountingRuleRepository accountingRuleRepository,
    //        IAccountingEntriesServices accountingService,
    //        IAccountingRuleEntryRepository accountingRuleEntryRepository,
    //       ITellerDailyProvisionRepository tellerDailyProvisionRepository,
    //        IMapper mapper,
    //        IConfiguration configuration,
    //        ILogger<CloseOfDayEventCommandHandler> logger,
    //        IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken, IChartOfAccountRepository chartOfAccountRepository, IOperationEventAttributeRepository? operationEventAttributeRepository, IOperationEventRepository? operationEventRepository)
    //    {
    //        _accountingEntryRepository = accountingEntryRepository;
    //        _accountRepository = accountRepository;
    //        _accountingRuleEntryRepository = accountingRuleEntryRepository;
    //        _accountingRuleRepository = accountingRuleRepository;
    //        _operationEventAttributeRepository = operationEventAttributeRepository;
    //        _operationEventRepository = operationEventRepository;
    //        _mapper = mapper;
    //        _logger = logger;
    //        _uow = uow;
    //        _accountTypeRepository = accountTypeRepository;
    //        _userInfoToken = userInfoToken;
    //        _configuration = configuration;
    //        _tellerDailyProvisionRepository = tellerDailyProvisionRepository;
    //        ILogger<AccountingEntriesServices> _serviceLogger = new LoggerFactory().CreateLogger<AccountingEntriesServices>();
    //        _accountingService = accountingService;// new AccountingEntriesServices(accountRepository, accountingRuleEntryRepository, accountingEntryRepository, configuration, uow, userInfoToken, operationEventAttributeRepository, operationEventRepository, _serviceLogger);
    //    }

    //    //public async Task<ServiceResponse<bool>> Handle(CloseOfDayEventCommand  command, CancellationToken cancellationToken)
    //    //{
         
    //    //    List<AccountAndOperationType> accountAndOperationTypes = new List<AccountAndOperationType>();
    //    //    string errorMessage = "";
    //    //    try
    //    //    {
    //    //        var modelStatus = await _tellerDailyProvisionRepository.FindAsync(command.TransactionReferenceId);
    //    //        if (modelStatus==null)
    //    //        {
    //    //            errorMessage = $" There is not daily provisioning has been done with the Id ={command.TransactionReferenceId} ";
    //    //            _logger.LogInformation(errorMessage);
    //    //            await APICallHelper.AuditLogger(_userInfoToken.Email, command.getEventDescription(),
    //    //                command, errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
    //    //            return ServiceResponse<bool>.Return422(errorMessage);

    //    //        }
    //    //        var ruleEntries = _accountingService.GetAccountingEntryRule(command.EventCode);
    //    //        if (ruleEntries?.Count == 0)
    //    //        {
    //    //            errorMessage = $"AccountingEntryRule with ID {command.EventCode} not found.";
    //    //            _logger.LogInformation(errorMessage);
    //    //            await APICallHelper.AuditLogger(_userInfoToken.Email, "OpeningAndClosingOfDayPostingEventCommand",
    //    //                command, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
    //    //            return ServiceResponse<bool>.Return404(errorMessage);
    //    //        }
    //    //        var listOfAccountAndOperationTypes = await _accountingService.GetListAccountAndOperationTypesAsync(ruleEntries, command.EventCode);
    //    //        Booking booking = new Booking(command.Amount, command.AmountDifference, command.TransactionReferenceId, GetPostingType(command), command.getEventDescription());
    //    //        foreach (var item in listOfAccountAndOperationTypes)
    //    //        {
    //    //            string debitAccountId = item.DebitAccount.Id;
    //    //            string creditAccountId = item.CreditAccount.Id;
    //    //            List<AccountingEntryDto> listResult = new List<AccountingEntryDto>();
                  
    //    //            var debitAccount = await _accountingService.UpdateAccountBalanceAsync(_userInfoToken.BranchId, debitAccountId, command.Amount, AccountOperationType.DEBIT, "OpeningOfDayPostingEventCommand");
    //    //            var creditAccount = await _accountingService.UpdateAccountBalanceAsync(_userInfoToken.BranchId, creditAccountId, command.Amount, AccountOperationType.CREDIT, "OpeningOfDayPostingEventCommand");
    //    //                  Booking crBooking = new Booking(command.TransactionReferenceId, AccountOperationType.CREDIT, creditAccount, debitAccount, Convert.ToDecimal(command.Amount));
    //    //            Booking drBooking = new Booking(command.TransactionReferenceId, AccountOperationType.DEBIT, creditAccount, debitAccount, Convert.ToDecimal(command.Amount));
                   
    //    //            AccountingEntryDto debitEntry = _accountingService.CreateDebitEntry(booking, creditAccount, debitAccount);
    //    //            AccountingEntryDto creditEntry = _accountingService.CreateCreditEntry(booking, creditAccount, debitAccount);
    //    //            listResult.Add(debitEntry);
    //    //            listResult.Add(creditEntry);
    //    //            List<Data.AccountingEntry> entris = new List<Data.AccountingEntry>();
    //    //            entris = _mapper.Map(listResult, entris);
    //    //            // Iterate through each accounting rule entry and perform the posting
    //    //            _accountingEntryRepository.AddRange(entris);
    //    //            modelStatus = modelStatus.UpdateTellerProvision(command.EventCode, command.Amount,command.AmountDifference);
    //    //            _tellerDailyProvisionRepository.Update(modelStatus);


    //    //        }
    //    //        await _uow.SaveAsync();
    //    //        _logger.LogInformation("Account postings completed successfully.");
    //    //        // Retrieve the accounting rule based on the provided AccountingRuleId
    //    //        await APICallHelper.AuditLogger(_userInfoToken.Email, "OpeningAndClosingOfDayPostingEventCommand",
    //    //            command, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
    //    //        return ServiceResponse<bool>.ReturnResultWith200(true);
    //    //        // Validate the existence of the accounting rule

    //    //    }
    //    //    catch (Exception e)
    //    //    {
    //    //        // Log error and return 500 Internal Server Error response with error message
    //    //        errorMessage = $"Error occurred while performing account postings: {e.Message}";
    //    //        _logger.LogError(errorMessage);
    //    //        return ServiceResponse<bool>.Return500(e, errorMessage);
    //    //    }
    //    //}




    //    private string GetPostingType(CloseOfDayEventCommand command)
    //    {
    //        if (command.EventCode.Equals(AccountingEvent.NORMAL_OOD))
    //        {
    //            return "OPENING OF DAY POSTING";
    //        }
    //        else if (command.EventCode.Equals(AccountingEvent.NEGATIVE_OOD))
    //        {
    //            return "OPENING OF DAY POSTING WITH NEGATIVE BALANCE";
    //        }
    //        else if (command.EventCode.Equals(AccountingEvent.POSITIVE_OOD))
    //        {
    //            return "OPENING OF DAY POSTING WITH POSITIVE BALANCE";
    //        }
    //        else if (command.EventCode.Equals(AccountingEvent.NORMAL_COD))
    //        {
    //            return "CLOSING OF DAY POSTING";
    //        }
    //        else if (command.EventCode.Equals(AccountingEvent.NEGATIVE_COD))
    //        {
    //            return "CLOSING OF DAY POSTING WITH NEGATIVE BALANCE";
    //        }
    //        else if (command.EventCode.Equals(AccountingEvent.POSITIVE_COD))
    //        {
    //            return "CLOSING OF DAY POSTING WITH POSITIVE BALANCE";
    //        }
    //        else
    //        {
    //            return "POSITING ERROR";
    //        }
    //    }

    //}


}

