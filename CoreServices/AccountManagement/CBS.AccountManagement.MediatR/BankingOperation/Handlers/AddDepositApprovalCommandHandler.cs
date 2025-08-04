using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.BankingOperation.Commands;
using CBS.AccountManagement.MediatR.Command;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using MediatR;
using Microsoft.AspNetCore.RequestDecompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.Handlers
{

    public class AddDepositApprovalCommandHandler : IRequestHandler<AddDepositApprovalCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingRuleRepository _accountingRuleRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICashMovementTrackingConfigurationRepository _ChartOfaccountRepository;
        private readonly IChartOfAccountManagementPositionRepository _ChartOfAccountManagementPositionRepository;
        private readonly IDepositNotifcationRepository _depositNotifcationRepository;
        private readonly ITellerDailyProvisionRepository _tellerDailyProvisionRepository;
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<AddDepositApprovalCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IUserNotificationRepository _userNotificationRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;

        private readonly IAccountingEntriesServices _accountingService;
        private const string OpeningOfDayEventCode = "OOD";
        private readonly PathHelper _pathHelper;
        public AddDepositApprovalCommandHandler(
            IAccountRepository accountRepository,
            IMediator mediator,

            IAccountingEntryRepository accountingEntryRepository,
            IDepositNotifcationRepository cashReplenishmentRepository,
            IMapper mapper,
            ILogger<AddDepositApprovalCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken, IConfiguration configuration,
            IOperationEventAttributeRepository? operationEventAttributeRepository,
            IOperationEventRepository? operationEventRepository,
            IAccountingEntriesServices accountingEntriesServices,
            ITellerDailyProvisionRepository tellerDailyProvisionRepository, IUserNotificationRepository userNotificationRepository)
        {

            _tellerDailyProvisionRepository = tellerDailyProvisionRepository;
            _accountingEntryRepository = accountingEntryRepository;
            _accountRepository = accountRepository;
            _depositNotifcationRepository = cashReplenishmentRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _configuration = configuration;
            _pathHelper = new PathHelper(_configuration);
            _userNotificationRepository = userNotificationRepository;
            _operationEventAttributeRepository = operationEventAttributeRepository;
            _operationEventRepository = operationEventRepository;
            _mediator = mediator;
            _accountingService = accountingEntriesServices; //new AccountingEntriesServices(accountRepository, accountingRuleEntryRepository, configuration, uow, userInfoToken, operationEventAttributeRepository, operationEventRepository, _serviceLogger);


        }

        public IOperationEventAttributeRepository? _operationEventAttributeRepository { get; }
        public IOperationEventRepository? _operationEventRepository { get; }

        public async Task<ServiceResponse<bool>> Handle(AddDepositApprovalCommand command, CancellationToken cancellationToken)
        {
            var accountingEntrieses = new List<Data.AccountingEntryDto>();
            string errorMessage = string.Empty;
            try
            {
            

                if (_userInfoToken.IsHeadOffice == false)
                {
                    errorMessage = $"You cannot approve this request at the level of the branch office,Kindly contact administrator";
                    _logger.LogError(errorMessage);
                    //await APICallHelper.AuditLogger(_userInfoToken.Email, "AddDepositApprovalCommand",

                    //    JsonConvert.SerializeObject(command), errorMessage, LogLevelInfo.Information.ToString(), 403, _userInfoToken.Token);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Forbidden, LogAction.AddDepositApprovalCommand, LogLevelInfo.Warning);

                    return ServiceResponse<bool>.Return403(errorMessage);
                }

                var model = await _depositNotifcationRepository.FindAsync(command.Id);
            
                if (model == null)
                {
                    errorMessage = $"There no cash deposit notification requestId :{command.Id} kindly contact Admintrator";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Forbidden, LogAction.AddDepositApprovalCommand, LogLevelInfo.Warning);

                    return ServiceResponse<bool>.Return403(errorMessage);
                }
                if (model.Status.ToLower().Equals(CashReplishmentRequestStatus.Awaiting_uploaded_bank_deposit_receipt.ToString().ToLower()))
                {
                    errorMessage = $"The depositNotification requestId :{command.Id} has already been validated is awaiting corresponding entry posting.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Forbidden, LogAction.AddDepositApprovalCommand, LogLevelInfo.Warning);

                    return ServiceResponse<bool>.Return409(errorMessage);
                }

                model.ApprovedMessage = command.ApprovedMessage;
                model.ApprovedBy = _userInfoToken.Id;
                model.ApprovedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                 model.BankAccountId = command.BankAccountId;
                if (command.Status.ToLower().Equals(CashReplishmentRequestStatus.RedirectToBranchBD.ToString().ToLower()))
                {
                    model.Status = CashReplishmentRequestStatus.RedirectToBranchBD.ToString();
                    model.IsApproved = true;
                    model.CorrepondingBranchId = command.CorrepondingBranchId;
                }
                else if(command.Status.ToLower().Equals(CashReplishmentRequestStatus.RedirectToBranchBTB.ToString().ToLower()))
                {
                    var _branches = await APICallHelper.GetAllBranchInfos(_pathHelper, _userInfoToken);
                    var _branch = _branches.Find(x => x.id == command.CorrepondingBranchId);
                    var branch = _branches.Find(x => x.id == model.BranchId);
                    string reference = $"VTL-{branch.branchCode}-{command.Id.Substring(command.Id.Length-6)}";
                    var valretun = await _accountingService.TransactionExists(reference) ? throw new Exception($"The referenceId:{reference} Already exist contact admin for support") : "";
                    model.IsApproved = true;
                    model.Status = CashReplishmentRequestStatus.RedirectToBranchBTB.ToString();
                    model.CorrepondingBranchId = command.CorrepondingBranchId;
                    string EventCode = "Vault_To_Liaison";
                
                    var accounts = await _accountingService.GetCashMovementAccountByEventCode(EventCode, _branch.id, _branch.branchCode);
                    accountingEntrieses.AddRange(await _accountingService.CashMovementAsync(BuildNaration(accounts,model), _userInfoToken.FullName+"-"+_userInfoToken.BranchCode, BaseUtilities.UtcToLocal(), accounts.BalancingAccount, accounts.DeterminantAccount, Convert.ToDecimal(model.Amount), "Vault To Liaison", command.Id, model.BranchId,true,_branch.id));
                    _accountingEntryRepository.AddRange(_mapper.Map<List<Data.AccountingEntry>>(accountingEntrieses));
                }
                else if (command.Status.ToLower().Equals(CashReplishmentRequestStatus.Approved.ToString().ToLower())|| command.Status.ToLower().Equals(CashReplishmentRequestStatus.RedirectToBranchBD.ToString().ToLower()))
                {
                    var banks = await APICallHelper.GetAllBranchInfos(_pathHelper, _userInfoToken);
                    var branch = banks.Where(x => x.id == model.BranchId).FirstOrDefault();

                    var ToAccount = await _accountRepository.FindAsync(model.BankAccountId);
                    string reference = $"VTT-{branch.branchCode}-{command.Id.Substring(command.Id.Length-6)}";
                    model.IsApproved = true;
                    model.Status = CashReplishmentRequestStatus.Awaiting_uploaded_bank_deposit_receipt.ToString();
                    var valretun = await _accountingService.TransactionExists(reference) ? throw new Exception($"The referenceId:{reference} Already exist contact admin for support") : "";


                    var AccountModel = new GetAccountByEventCodeQuery
                    {
                        EventCode = "Vault_To_Transit",
                        ToBranchCode = branch.branchCode,
                        ToBranchId = model.BranchId
                    };
                    var result = await _mediator.Send(AccountModel);
                    if (result.StatusCode.Equals(200))
                    {
                        var SourceAccount = result.Data.Where(x => x.Type == "Source").FirstOrDefault();
                        var FromAccount = await _accountRepository.FindAsync(SourceAccount.Id);
                        string Naration = $"Cash movement of XFA {model.Amount} from {FromAccount.AccountNumberCU}-{FromAccount.AccountName} to {ToAccount.AccountNumberCU}-{ToAccount.AccountName} of|Reference:{reference}  | Event: [Vault_To_Transit]";

                        accountingEntrieses = await _accountingService.CashMovementAsync(Naration, "HeadOffice-" + _userInfoToken.FullName, BaseUtilities.UtcToDoualaTime(DateTime.Now), FromAccount, ToAccount, model.Amount,
                            "Vault_To_Transit", reference, model.BranchId, true, model.BranchId);

                        _accountingEntryRepository.AddRange(_mapper.Map<List<Data.AccountingEntry>>(accountingEntrieses));
                    }

                }
                else if (command.Status.ToLower().Equals(CashReplishmentRequestStatus.Rejected.ToString().ToLower()))
                {
                    model.IsApproved = true;
                    model.Status = CashReplishmentRequestStatus.Rejected.ToString();
                }
                _depositNotifcationRepository.Update(model);


                var userModel = _userNotificationRepository.FindBy(xx => xx.ActionId.Equals(model.Id)).FirstOrDefault();
   

                var userv = new UsersNotification("Upload Deposit Completion", "/Notification/BankDepositCompletion?KEY={0}", model.Id, BaseUtilities.GenerateInsuranceUniqueNumber(15, "NOTIF-"), _userInfoToken.Id, _userInfoToken.BranchId, model.Id);


             

                userModel.Action = userv.Action;
                userModel.ActionUrl = userv.ActionUrl;
                userModel.ActionId = userv.ActionId;
                userModel.BranchId = userv.BranchId;
                userModel.ModifiedBy = _userInfoToken.Id;
                userModel.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
                userModel.TempData = model.BranchId;
                _userNotificationRepository.Update(userModel);
                errorMessage = "AddDepositApprovalCommand was completed successfully";
                await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.OK, LogAction.AddDepositApprovalCommand, LogLevelInfo.Information);

                await _uow.SaveAsync();
            
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception ex)
            {
                errorMessage=$"Execption :{ex.Message}" ;
                await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.OK, LogAction.AddDepositApprovalCommand, LogLevelInfo.Information);

                return ServiceResponse<bool>.Return500(ex.Message);
            }
        }

        private string BuildNaration((Data.Account DeterminantAccount, Data.Account BalancingAccount) accounts, DepositNotification model)
        {
            return $"[Vault_to_Liaison] resulted after BankDepositRequest :{model.Id} of XAF {model.Amount} from {accounts.DeterminantAccount.AccountNumberCU}-{accounts.DeterminantAccount.AccountName} to {accounts.BalancingAccount.AccountNumberCU}-{accounts.BalancingAccount.AccountName} ";
        }

        private string SetRequestStatus(bool isApproved)
        {
            return isApproved ? CashReplishmentRequestStatus.Approved.ToString() : CashReplishmentRequestStatus.Rejected.ToString();
        }
    }

}
