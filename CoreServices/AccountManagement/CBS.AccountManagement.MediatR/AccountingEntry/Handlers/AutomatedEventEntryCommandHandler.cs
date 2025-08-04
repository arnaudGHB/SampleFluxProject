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


    public class AutomatedEventEntryCommandHandler : IRequestHandler<AutomatedEventEntryCommand, ServiceResponse<EventEntryResponse>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly ICashReplenishmentRepository _cashReplenishmentRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<AutomatedEventEntryCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ITransactionDataRepository _transactionDataRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;
 

        public AutomatedEventEntryCommandHandler(
            ITransactionDataRepository transactionDataRepository,
             IAccountingEntryRepository accountingEntryRepository,
    IOperationEventAttributeRepository operationEventAttributeRepository,
    IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
            IMediator mediator,
             IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<AutomatedEventEntryCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken,
            IChartOfAccountManagementPositionRepository chartOfAccountRepository, IAccountingEntriesServices accountingEntriesServices, ICashReplenishmentRepository? cashReplenishmentRepository)
        {
            _accountRepository = accountRepository;
            _operationEventAttributeRepository = operationEventAttributeRepository;
            _accountingEntryRepository = accountingEntryRepository;
            _mapper = mapper;
            _logger = logger;
 
            _uow = uow;
 
            _userInfoToken = userInfoToken;
            _configuration = configuration;
            _chartOfAccountRepository = chartOfAccountRepository;
            _accountingRuleEntryRepository = accountingRuleEntryRepository;
            ILogger<AccountingEntriesServices> _serviceLogger = new LoggerFactory().CreateLogger<AccountingEntriesServices>();
            _accountingService = accountingEntriesServices;
            _mediator = mediator;
        }

        public async Task<ServiceResponse<EventEntryResponse>> Handle(AutomatedEventEntryCommand command, CancellationToken cancellationToken)
        {
     
            string errorMessage = "";
            try
            {
                if (await _accountingService.IsBranchEligibility(command.Entries[0].System_Id,_userInfoToken.BranchId))
                {
                    errorMessage = "You are not authorized to execute this journal entry in your branch. please contact your system administrator";
                    await BaseUtilities.LogAndAuditAsync(errorMessage,
               command, HttpStatusCodeEnum.Unauthorized, LogAction.AutomatedEventEntryCommand, LogLevelInfo.Information);
                    return ServiceResponse<EventEntryResponse>.Return401( errorMessage);

                }

                if (await _accountingService.CheckIfDoubbleVaidationIsRequired(command.Entries[0].System_Id))
                {
                    var AddEntryTempDataListCommand = new AddEntryTempDataListCommand
                    {
                        EntryTempDatas = await CreateEntryTempDataListCommandAsync(command)
                    };
                    AddEntryTempDataListCommand.IsSystem = true;
                     var result = await _mediator.Send(AddEntryTempDataListCommand);
                    if (result.StatusCode.Equals(200))
                    {
                        errorMessage = "Transaction Completed Successfully";
                        await BaseUtilities.LogAndAuditAsync(errorMessage,
                   command, HttpStatusCodeEnum.OK, LogAction.AutomatedEventEntryCommand, LogLevelInfo.Information);
                        return ServiceResponse<EventEntryResponse>.ReturnResultWith200(new EventEntryResponse { Status = true, ResponseMessge = errorMessage }, errorMessage);

                    }
                    else
                    {
                        errorMessage = "Contact administrator system administrator error occured while creating the temporal entry command handler\n" + result.Message;
                        await BaseUtilities.LogAndAuditAsync(errorMessage,
                   command, HttpStatusCodeEnum.InternalServerError, LogAction.AutomatedEventEntryCommand, LogLevelInfo.Error);
                        throw new ArgumentException(errorMessage);
                    }
                }
                else
                {
                    var model = new AddEntryTempDataListCommand
                    {
                        EntryTempDatas = await SetTempDataEntry(command),
                    };
                    var result = await _mediator.Send(model);
                    if (result.StatusCode.Equals(200))
                    {
                        errorMessage = "Transaction Completed Successfully";
                        await BaseUtilities.LogAndAuditAsync(errorMessage,
                   command, HttpStatusCodeEnum.OK, LogAction.AutoPostingEventCommand, LogLevelInfo.Information);

                        return ServiceResponse<EventEntryResponse>.ReturnResultWith200(new EventEntryResponse { Status = true, ResponseMessge = errorMessage }, errorMessage);

                    }
                    else
                    {
                        errorMessage = "Contact administrator system administrator error occured while creating the temporal entry command handler\n" + result.Message;
                        await BaseUtilities.LogAndAuditAsync(errorMessage,
                   command, HttpStatusCodeEnum.InternalServerError, LogAction.AutomatedEventEntryCommand, LogLevelInfo.Error);
                        throw new ArgumentException(errorMessage);
                    }
                }
             

            }
            catch (Exception ex)
            {

                // Log error and return 500 Internal Server Error response with error message
                errorMessage = $" {ex.Message}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AutomatedEventEntryCommand",
                    command, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                _logger.LogError(errorMessage);
                return ServiceResponse<EventEntryResponse>.Return500(ex, errorMessage);
            }
        }

        private async Task<List<EntryTempDataCommand>> CreateEntryTempDataListCommandAsync(AutomatedEventEntryCommand Collection)
        {
            List<EntryTempDataCommand> entryList = new List<EntryTempDataCommand>();
            foreach (var item in Collection.Entries)
            {

                var model = await _accountingService.CreateAccountForBranchByChartOfAccountIdAsync(item.MFI_ChartOfAccountId, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                var entry = new EntryTempDataCommand
                {
                    Id = "012",
                    AccountId = model.Id,
                    AccountBalance = model.CurrentBalance.ToString(),
                    AccountName = $"{model.AccountNumberCU}-{model.AccountName}",
                    Description = Collection.Description,
                    Reference = Collection.ReferenceId,
                    BookingDirection = item.BookingDirection,
                    AccountNumber = $"{model.AccountNumberCU}",
                    Amount = item.Amount,
                    AccountingEventId = item.System_Id,
                  BranchId= Collection.BranchId
                    
                };
                entryList.Add(entry);
            }
            return entryList;
        }

        private async Task< List<EntryTempDataCommand> >SetTempDataEntry(AutomatedEventEntryCommand command)
        {
            List<EntryTempDataCommand> entryList = new List<EntryTempDataCommand>();
            foreach (var item in command.Entries)
            {
             
                var model = await _accountingService.CreateAccountForBranchByChartOfAccountIdAsync(item.MFI_ChartOfAccountId, _userInfoToken.BranchCode, _userInfoToken.BranchId);
                var entry = new EntryTempDataCommand
                {
                    Id = "012",
                    AccountId = model.Id,
                    AccountBalance = model.CurrentBalance.ToString(),
                    AccountName = $"{model.AccountNumberCU}-{model.AccountName}",
                    Description = command.Description,
                    Reference = command.ReferenceId,
                    BookingDirection = item.BookingDirection,
                    AccountNumber = $"{model.AccountNumberCU}",
                    Amount = item.Amount
                };
                entryList.Add(entry);
            }
            return entryList;
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
