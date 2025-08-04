using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;

using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.BankingOperation;
using CBS.AccountManagement.MediatR.BankingOperation.Commands;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CBS.AccountManagement.MediatR.Handlers
{

    public class TransactionReversalRequestApprovalCommandHandler : IRequestHandler<TransactionReversalRequestApprovalCommand, ServiceResponse<TransactionReversalRequestDataDto>>
    {
  
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionReversalRequestRepository _transactionReversalRequestRepository;
      
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TransactionReversalRequestApprovalCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;

        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;

        private readonly IAccountingEntriesServices _accountingService;
        private const string OpeningOfDayEventCode = "OOD";
        private readonly PathHelper _pathHelper;
        private readonly IMediator _mediator;

        public TransactionReversalRequestApprovalCommandHandler(
            IAccountRepository accountRepository,

            IMediator mediator,
            IAccountingEntryRepository accountingEntryRepository,

            ITransactionReversalRequestRepository transactionReversalRequestRepository,
            IMapper mapper,
            ILogger<TransactionReversalRequestApprovalCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken, IConfiguration configuration,

            IAccountingEntriesServices accountingEntriesServices,
            ITellerDailyProvisionRepository tellerDailyProvisionRepository)
        {

 
            _accountingEntryRepository = accountingEntryRepository;
            _accountRepository = accountRepository;
            _transactionReversalRequestRepository = transactionReversalRequestRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _configuration = configuration;
            _pathHelper = new PathHelper(_configuration);

            _mediator = mediator;

            _accountingService = accountingEntriesServices; //new AccountingEntriesServices(accountRepository, accountingRuleEntryRepository, configuration, uow, userInfoToken, operationEventAttributeRepository, operationEventRepository, _serviceLogger);


        }


        public async Task<ServiceResponse<TransactionReversalRequestDataDto>> Handle(TransactionReversalRequestApprovalCommand command, CancellationToken cancellationToken)
        {
            TransactionReversalRequestDataDto response = new TransactionReversalRequestDataDto();
            try
            {
                string errorMessage = string.Empty;
                var model = await _transactionReversalRequestRepository.FindAsync(command.Id);
                if (model == null)
                {
                    errorMessage = $"There no is TransactionReversalRequest requestId :{command.Id} kindly contact Admintrator";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "TransactionReversalRequestApprovalCommand",
                        command, errorMessage, LogLevelInfo.Information.ToString(), 403, _userInfoToken.Token);
                    return ServiceResponse<TransactionReversalRequestDataDto>.Return403(errorMessage);
                }
                model.ApprovedMessage = command.ApprovedMessage;
                model.ApprovedBy = _userInfoToken.Id;
                model.ApprovedDate = DateTime.Now.ToUtc();
                model.IsApproved = command.IsApproved;
                model.Status = SetRequestStatus(command.IsApproved);
                var listOfAccountingEntry = _accountingEntryRepository.FindBy(x => x.ReferenceID == model.ReferenceId).ToList();
                if (listOfAccountingEntry.Count()>0)
                {
                    var Ids = from entry in listOfAccountingEntry
                              select entry.Id;
                    var reversalCommand = new ReverseAccountingEntryCommand
                    {
                        ReferenceId = listOfAccountingEntry.ToArray()[0].ReferenceID,
                    };
                    var ResultStatus = await _mediator.Send(reversalCommand);
                     
                    response.ReversalRequest = JsonConvert.SerializeObject(model);
                    response.DataBeforeReversal = JsonConvert.SerializeObject(listOfAccountingEntry);

                    if (_userInfoToken.Id != model.IssuedBy)
                    {
                        if (ResultStatus.StatusCode == 200)
                        {
                            _transactionReversalRequestRepository.Update(model);
                            await _uow.SaveAsync();
                            response.DataAfterReversal = JsonConvert.SerializeObject(ResultStatus.Data);
                        }
                        return ServiceResponse<TransactionReversalRequestDataDto>.ReturnResultWith200(response);
                    }
                    else
                    {
                        errorMessage = $"The transaction reversal request issuer:{model.IssuedBy} cannot be the same to approved ";
                        _logger.LogError(errorMessage);
                        await APICallHelper.AuditLogger(_userInfoToken.Email, "TransactionReversalRequestApprovalCommand",
                            command, errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                        return ServiceResponse<TransactionReversalRequestDataDto>.Return422(errorMessage);
                    }


                }
                else
                {
                    errorMessage = $"There no accounting entry with reference:{command.Id} kindly contact Admintrator";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "TransactionReversalRequestApprovalCommand",
                        command, errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                    return ServiceResponse<TransactionReversalRequestDataDto>.Return422(errorMessage);
                }
            }
            catch (Exception ex)
            {
                return ServiceResponse<TransactionReversalRequestDataDto>.Return500(ex.Message);
            }
        }

        private async Task _SendTellerProvisioningCallBack(CashReplenishmentCallBackModel cashReplenishmentCallBackModel)
        {

            await APICallHelper.SendPrimaryTellerCallBackInfos(cashReplenishmentCallBackModel, _pathHelper, _userInfoToken);
        }

        private string SetRequestStatus(bool isApproved)
        {
            return isApproved ? "Approved" : "Rejected";
        }
    }

}
