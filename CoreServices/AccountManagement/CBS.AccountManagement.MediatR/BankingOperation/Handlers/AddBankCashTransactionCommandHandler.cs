using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Command;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.Handlers
{

    public class AddBankCashTransactionCommandHandler : IRequestHandler<AddBankCashOutTransactionCommand, ServiceResponse<bool>>
    {
        private readonly ICashReplenishmentRepository _cashReplenishmentRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IBankTransactionRepository _bankTranstionRepository;
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AddBankCashTransactionCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IUserNotificationRepository _userNotificationRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        private readonly IAccountingEntriesServices _accountingService;
        private const string OpeningOfDayEventCode = "OOD";
        private readonly PathHelper _pathHelper;
        public AddBankCashTransactionCommandHandler(
            IAccountRepository accountRepository,
            IMediator mediator,
            ICashReplenishmentRepository cashReplenishmentRepository,
            IAccountingEntryRepository accountingEntryRepository,
         
            IMapper mapper,
            ILogger<AddBankCashTransactionCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken, IConfiguration configuration,
            IAccountingEntriesServices accountingEntriesServices,
            ITellerDailyProvisionRepository tellerDailyProvisionRepository, IBankTransactionRepository? bankTranstionRepository, IUserNotificationRepository userNotificationRepository)
        {

            _bankTranstionRepository = bankTranstionRepository;
                       _accountingEntryRepository = accountingEntryRepository;
            _accountRepository = accountRepository;
            _cashReplenishmentRepository = cashReplenishmentRepository;
                       _mapper = mapper;
            _userNotificationRepository = userNotificationRepository;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _configuration = configuration;
            _pathHelper = new PathHelper(_configuration);
            _mediator = mediator;
                       _accountingService = accountingEntriesServices; //new AccountingEntriesServices(accountRepository, accountingRuleEntryRepository, configuration, uow, userInfoToken, operationEventAttributeRepository, operationEventRepository, _serviceLogger);


        }


        public async Task<ServiceResponse<bool>> Handle(AddBankCashOutTransactionCommand command, CancellationToken cancellationToken)
        {

            try
            {
       
                List<AccountingEntryDto> accountingEntrieses = new List<AccountingEntryDto>();
                string errorMessage = string.Empty;

                if (_userInfoToken.IsHeadOffice)
                {
                    errorMessage = $"This operation cannot be carried out at the level of the Head Office";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Unauthorized, LogAction.AddBankCashOutTransactionCommand, LogLevelInfo.Warning);

                    return ServiceResponse<bool>.Return401(errorMessage);
                }
 


                var CashModels =await  _cashReplenishmentRepository.FindAsync(command.ReferenceId);
                if (CashModels==null)
                {
                    errorMessage = $"This Cash Replenishment reference does not exist, kindly contact Admintrator";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.NotFound, LogAction.AddBankCashOutTransactionCommand, LogLevelInfo.Warning);

                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                var models =   _bankTranstionRepository.FindBy(pop=>pop.ReferenceId==command.ReferenceId&& pop.IsDeleted==false);
                if (models.Any())
                {
                    errorMessage = $"This transaction reference has already been used, kindly contact Admintrator";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.AddBankCashOutTransactionCommand, LogLevelInfo.Warning);

                    return ServiceResponse<bool>.Return409(errorMessage);
                }    
                var bankAccount = await _accountRepository.FindAsync(command.FromAccountId);// sourceAccount

                var cashAccount = await _accountRepository.FindAsync(command.ToAccountId);//destination
                if (cashAccount == null)
                {
                    errorMessage = $"This no vault account set for event {OpeningOfDayEventCode}, kindly contact Admintrator";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.AddBankCashOutTransactionCommand, LogLevelInfo.Warning);

                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                BankTransaction bankCashOutModel = command.SetBankTransaction(command, BaseUtilities.GenerateInsuranceUniqueNumber(15, "BCO"));
                // _accountingService.CreateDebitEntry(drBooking, CreditAccount, DebitAccount, Convert.ToDateTime(bankCashOutModel.ValueDate));

                List<AccountingEntryDto> DebitEntries = new List<AccountingEntryDto>();
                DebitEntries.AddRange(await _accountingService.CashMovementAsync(GenerateNaration(cashAccount, bankAccount, command), (_userInfoToken.FullName+" "+_userInfoToken.BankCode), BaseUtilities.UtcToLocal(), cashAccount , bankAccount, Convert.ToDecimal(command.Amount), "BankCashOutTransactionCommand", command.ReferenceId, _userInfoToken.BranchId));
 
                var entries = _mapper.Map<List<Data.AccountingEntry>>(DebitEntries);
                  var RequestModel=    CashModels;
                RequestModel.Status = CashReplishmentRequestStatus.Completed.ToString();
                var CashModel000 = await _cashReplenishmentRepository.FindAsync(RequestModel.Id);
                _cashReplenishmentRepository.Update(RequestModel);
                //CashModel000.Status = CashReplishmentRequestStatus.awaiting_corresponding_entry_posting.ToString();
                //CashModel000.ParentCashReplenishId = bankCashOutModel.ReferenceId;
                //_cashReplenishmentRepository.Update(CashModel000);
                _accountingEntryRepository.AddRange(entries);
                _bankTranstionRepository.Add(bankCashOutModel);

                var userModel = _userNotificationRepository.FindBy(xx => xx.ActionId.Equals(command.ReferenceId)).FirstOrDefault();
                if (userModel == null)
                {
                    errorMessage = $"This bank cashout opearations with referenceId:{command.ReferenceId} has already been done ";
                      await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.AddBankCashOutTransactionCommand, LogLevelInfo.Warning);
                   return ServiceResponse<bool>.Return409(errorMessage);
                 
                }
                _userNotificationRepository.Remove(userModel);

                if (_accountingService.EvaluateDoubleEntryRule(entries) && entries.Count() > 1)
                {
                    await _uow.SaveAsync();
                    var model = new CashMovementDenominationCommand
                    {
                        Reference= command.ReferenceId,
                        Amount = command.Amount,
                        CurrencyNotesRequest = command.CurrencyNotesRequest,
                        BranchId = _userInfoToken.BranchId,
                        OperationType= "CashIn"
                    };
                  var results=  await _mediator.Send(model);
                   
                }
                else
                {
                    errorMessage = "Accounting double entry rule not validated contact administrator";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.AddBankCashOutTransactionCommand, LogLevelInfo.Warning);

                    return ServiceResponse<bool>.Return422(errorMessage);
                }
                errorMessage = "AddBankCashOutTransactionCommand Transaction Completed Successfully";
                await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.AddBankCashOutTransactionCommand, LogLevelInfo.Warning);

                return ServiceResponse<bool>.ReturnResultWith200(true, errorMessage);

            }
            catch (Exception ex)
            {

                return ServiceResponse<bool>.Return500(ex.Message);
            }
        }
        private string GenerateNaration(Data.Account sourceAccount, Data.Account destinationAccount, AddBankCashOutTransactionCommand command)
        {
            return $"BANK CASH OUT of XAF {command.Amount} from {sourceAccount.AccountNumberCU}-{sourceAccount.AccountName} to " +
                $"{destinationAccount.AccountNumberCU}-{destinationAccount.AccountName} Ref:{command.ReferenceId}";

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



    public class AddBankCashInForTransactionCommandHandler : IRequestHandler<AddBankCashInForTransactionCommand, ServiceResponse<bool>>
    {
        private readonly IDepositNotifcationRepository _depositNotifcationRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IBankTransactionRepository _bankTranstionRepository;
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AddBankCashInForTransactionCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;

        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;

        private readonly IAccountingEntriesServices _accountingService;
        private const string OpeningOfDayEventCode = "OOD";
        private readonly PathHelper _pathHelper;
        public AddBankCashInForTransactionCommandHandler(
            IAccountRepository accountRepository,
            IMediator mediator,
            IDepositNotifcationRepository cashReplenishmentRepository,
            IAccountingEntryRepository accountingEntryRepository,

            IMapper mapper,
            ILogger<AddBankCashInForTransactionCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken, IConfiguration configuration,
            IAccountingEntriesServices accountingEntriesServices,
            ITellerDailyProvisionRepository tellerDailyProvisionRepository, IBankTransactionRepository? bankTranstionRepository)
        {

            _bankTranstionRepository = bankTranstionRepository;
            _accountingEntryRepository = accountingEntryRepository;
            _accountRepository = accountRepository;
            _depositNotifcationRepository = cashReplenishmentRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _configuration = configuration;
            _pathHelper = new PathHelper(_configuration);

            _accountingService = accountingEntriesServices; //new AccountingEntriesServices(accountRepository, accountingRuleEntryRepository, configuration, uow, userInfoToken, operationEventAttributeRepository, operationEventRepository, _serviceLogger);


        }


        public async Task<ServiceResponse<bool>> Handle(AddBankCashInForTransactionCommand command, CancellationToken cancellationToken)
        {

            try
            {

                List<AccountingEntryDto> accountingEntrieses = new List<AccountingEntryDto>();
                string errorMessage = string.Empty;

                if (_userInfoToken.IsHeadOffice)
                {
                    errorMessage = $"This operation cannot be carried out at the level of the Head Office";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "AddBankCashInForTransactionCommand",

                        JsonConvert.SerializeObject(command), errorMessage, LogLevelInfo.Information.ToString(), 403, _userInfoToken.Token);

                    return ServiceResponse<bool>.Return403(errorMessage);
                }
                var CashModels = await _depositNotifcationRepository.FindAsync(command.ReferenceId);
                if (CashModels == null)
                {
                    errorMessage = $"This depositnotification reference does not exist, kindly contact Admintrator";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "AddBankCashInForTransactionCommand",

                        JsonConvert.SerializeObject(command), errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);

                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                var models = _bankTranstionRepository.FindBy(pop => pop.ReferenceId == command.ReferenceId && pop.IsDeleted == false);
                if (models.Any())
                {
                    errorMessage = $"This transaction reference has already been used, kindly contact Admintrator";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "AddBankCashInForTransactionCommand",

                        JsonConvert.SerializeObject(command), errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);

                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                var bankAccount = await _accountRepository.FindAsync(command.FromAccountId);
                var cashAccount = await _accountRepository.FindAsync(command.ToAccountId);
                if (cashAccount == null)
                {
                    errorMessage = $"This no vault account set for event {OpeningOfDayEventCode}, kindly contact Admintrator";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "AddBankCashInForTransactionCommand",

                        JsonConvert.SerializeObject(command), errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);

                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                List<AccountingEntryDto> DebitEntries = null;
                DebitEntries.AddRange(await _accountingService.CashMovementAsync(GenerateNaration(cashAccount, bankAccount, command), (_userInfoToken.FullName + " " + _userInfoToken.BankCode), BaseUtilities.UtcToLocal(), bankAccount, cashAccount, Convert.ToDecimal(command.Amount), "BankCashOutTransactionCommand", command.ReferenceId, _userInfoToken.BranchId));
            
                var entries = _mapper.Map<List<Data.AccountingEntry>>(accountingEntrieses);
                BankTransaction bankCashOutModel = command.SetBankTransaction(command, BaseUtilities.GenerateInsuranceUniqueNumber(15, "BCO"));
                var RequestModel = CashModels;
                RequestModel.Status = CashReplishmentRequestStatus.Completed.ToString();
              
                _accountingEntryRepository.AddRange(entries);
                _bankTranstionRepository.Add(bankCashOutModel);

                if (_accountingService.EvaluateDoubleEntryRule(entries) && entries.Count() > 1)
                {

                    await _uow.SaveAsync();
                }
                else
                {
                    return ServiceResponse<bool>.Return422("Accounting double entry rule not validated contact administrator");
                }
                return ServiceResponse<bool>.ReturnResultWith200(true, "Transaction Completed Successfully");

            }
            catch (Exception ex)
            {

                return ServiceResponse<bool>.Return500(ex.Message);
            }
        }
        private string GenerateNaration(Data.Account sourceAccount, Data.Account destinationAccount, AddBankCashInForTransactionCommand command)
        {
            return $"BANK CASH IN of XAF {command.Amount} from {sourceAccount.AccountNumberCU}-{sourceAccount.AccountName} to " +
                $"{destinationAccount.AccountNumberCU}-{destinationAccount.AccountName} Ref:{command.ReferenceId}";

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
