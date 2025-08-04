using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.BankingOperation.Commands;
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

    public class AddApprovalCommandHandler : IRequestHandler<AddCashReplenishmentApprovalCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingRuleRepository _accountingRuleRepository;
        private readonly IUserNotificationRepository _userNotificationRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICashMovementTrackingConfigurationRepository _ChartOfaccountRepository;
        private readonly IChartOfAccountManagementPositionRepository _ChartOfAccountManagementPositionRepository;
        private readonly ICashReplenishmentRepository _cashReplenishmentRepository;
        private readonly ITellerDailyProvisionRepository _tellerDailyProvisionRepository;
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AddApprovalCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IMediator _mediator;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;

        private readonly IAccountingEntriesServices _accountingService;
        private const string OpeningOfDayEventCode = "OOD";
        private readonly PathHelper _pathHelper;
        public AddApprovalCommandHandler(
            IAccountRepository accountRepository,
            IMediator mediator,

            IAccountingEntryRepository accountingEntryRepository,
            ICashReplenishmentRepository cashReplenishmentRepository,
            IMapper mapper,
            ILogger<AddApprovalCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken, IConfiguration configuration,
            IOperationEventAttributeRepository? operationEventAttributeRepository,
            IOperationEventRepository? operationEventRepository,
            IAccountingEntriesServices accountingEntriesServices,
            ITellerDailyProvisionRepository tellerDailyProvisionRepository, IUserNotificationRepository userNotificationRepository)
        {

            _tellerDailyProvisionRepository = tellerDailyProvisionRepository;
            _accountingEntryRepository = accountingEntryRepository;
            _accountRepository = accountRepository;
            _cashReplenishmentRepository = cashReplenishmentRepository;
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


        private async Task<Branch> GetBranchInfo(string branchId)
        {
            List<Branch> branches = await APICallHelper.GetAllBranchInfos(_pathHelper, _userInfoToken);
            return branches.FirstOrDefault(x => x.id == branchId);
        }

        public async Task<ServiceResponse<bool>> Handle(AddCashReplenishmentApprovalCommand command, CancellationToken cancellationToken)
        {
            string errorMessage = "";
            List<AccountingEntryDto> accountingEntrieses = new List<AccountingEntryDto>();
            try
            {
                 // Validate Head Office access
                if (!_userInfoToken.IsHeadOffice)
                {
                    errorMessage = "You cannot approve this request at the branch office level. Kindly contact the administrator.";

                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Unauthorized, LogAction.AddCashReplenishmentApprovalCommand, LogLevelInfo.Warning);
                    return await LogAndReturnError(
                        errorMessage,
                        command,
                        403
                    );
                }

                // Fetch the cash replenishment request
                var model = await _cashReplenishmentRepository.FindAsync(command.Id);
                if (model == null)
                {
                    errorMessage = $"No cash replenishment request found with ID: {command.Id}.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.NotFound, LogAction.AddCashReplenishmentApprovalCommand, LogLevelInfo.Warning);

                    return await LogAndReturnError(errorMessage, command, 403);
                }
                
               // Business logic validations
                if (await _accountingService.TransactionExists(command.Id))
                {
                    errorMessage = "The transaction has already been process, Posting entry exist already";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.AddCashReplenishmentApprovalCommand, LogLevelInfo.Warning);

                    return await LogAndReturnError(errorMessage, command, 403);
                }

                if (command.ApprovedAmount > model.AmountRequested)
                {
                    errorMessage = "The approved amount cannot exceed the requested amount.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.AddCashReplenishmentApprovalCommand, LogLevelInfo.Warning);

                    return await LogAndReturnError(errorMessage, command, 403);
                }

                if (model.CashReplishmentRequestStatus.ToLower().Equals(CashReplishmentRequestStatus.Approved.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    errorMessage = $"Request ID {command.Id} has already been validated and is awaiting entry posting.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.AddCashReplenishmentApprovalCommand, LogLevelInfo.Warning);

                    return await LogAndReturnError(errorMessage, command, 409);
                }

                if (model.BranchId == command.CorrespondingBranchId && (command.Status == CashReplishmentRequestStatus.RedirectToBranchBTB.ToString()))
                {
                    errorMessage = "Self-redirection of cash requests is not permitted.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Forbidden, LogAction.AddCashReplenishmentApprovalCommand, LogLevelInfo.Warning);

                    return await LogAndReturnError(errorMessage, command, 403);
                }
                if (model.BranchId == command.CorrespondingBranchId && (command.Status == CashReplishmentRequestStatus.RedirectToBranchBCO.ToString()))
                {
                    errorMessage = "Self-redirection of cash requests is not permitted.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Forbidden, LogAction.AddCashReplenishmentApprovalCommand, LogLevelInfo.Warning);

                    return await LogAndReturnError(errorMessage, command, 403);
                }


                // Update model with approved details
                UpdateCashReplenishmentModel(model, command);

                // Handle different cash requisition types
                await HandleCashRequisitionType(command, model);
                if ((command.Status == CashReplishmentRequestStatus.RedirectToBranchBCO.ToString())|| (command.Status == CashReplishmentRequestStatus.Approved.ToString()))
                {
                    var banks = await APICallHelper.GetAllBranchInfos(_pathHelper,_userInfoToken);
                    var branch = banks.Where(x => x.id == model.BranchId).FirstOrDefault();
                    var toAccount = await _accountRepository.FindAsync(command.AccountId);
                    string reference = $"{command.Id}";                 
                    var AccountModel = new GetAccountByEventCodeQuery
                    {
                        EventCode = "Bank_To_Transit",
                        ToBranchCode = branch.branchCode,
                        ToBranchId = model.BranchId
                    };
                    var result =  await _mediator.Send(AccountModel);
                    if (result.StatusCode.Equals(200))
                    {
                        var destinationAccount = result.Data.Where(x => x.Type == "Destination").FirstOrDefault();
                        var fromAccount = await _accountRepository.FindAsync(destinationAccount.Id);
                        string Naration = $"Bank Cash Transit of XFA {command.ApprovedAmount} from {toAccount.AccountNumberCU}-{toAccount.AccountName} to {fromAccount.AccountNumberCU}-{fromAccount.AccountName} | Reference:{reference} | Event: [Bank_To_Transit]";

                        accountingEntrieses = await _accountingService.CashMovementAsync(Naration, "HeadOffice-" + _userInfoToken.FullName, BaseUtilities.UtcToDoualaTime(DateTime.Now), toAccount, fromAccount, command.ApprovedAmount,
                            "Bank_To_Transit", reference, _userInfoToken.BranchId, true, model.BranchId);

                        _accountingEntryRepository.AddRange(_mapper.Map<List<Data.AccountingEntry>> (accountingEntrieses));
                    }
                }
                errorMessage = "AddCashReplenishmentApprovalCommand transaction executed successfully";
                     // Save changes
                     await _uow.SaveAsync();
                await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.OK, LogAction.AddCashReplenishmentApprovalCommand, LogLevelInfo.Information);

                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing AddCashReplenishmentApprovalCommand.");
                errorMessage = $"An error occurred: {ex.Message}";
                // Save changes
                await _uow.SaveAsync();
                await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.InternalServerError, LogAction.AddCashReplenishmentApprovalCommand, LogLevelInfo.Error);

                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }

        private async Task<ServiceResponse<bool>> LogAndReturnError(string errorMessage, AddCashReplenishmentApprovalCommand command, int statusCode)
        {
            _logger.LogError(errorMessage);
            //await APICallHelper.AuditLogger(_userInfoToken.Email, nameof(AddCashReplenishmentApprovalCommand), JsonConvert.SerializeObject(command), errorMessage, LogLevelInfo.Information.ToString(), statusCode, _userInfoToken.Token);
            return ServiceResponse<bool>.Return403(errorMessage);
        }

        private void UpdateCashReplenishmentModel(CashReplenishment model, AddCashReplenishmentApprovalCommand command)
        {
            model.ApprovedMessage = command.ApprovedMessage;
            model.ApprovedBy = _userInfoToken.Id;
            model.ApprovedDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
            model.IsApproved = command.IsApproved;
            model.AmountApproved = command.ApprovedAmount;
            model.Status = command.Status;
            model.TempData = command.AccountId;
        }

        private async Task HandleCashRequisitionType(AddCashReplenishmentApprovalCommand command, CashReplenishment model)
        {
            if (command.CashRequisitionType.Equals("ORDER"))
            {
                // Logic for ORDER type
                await HandleOrderType(command, model);
            }
            else
            {
                // Logic for other types
                await HandleOtherRequisitionType(command, model);
            }
        }

        private async Task HandleOtherRequisitionType(AddCashReplenishmentApprovalCommand command, CashReplenishment model)
        {
            if (model.Status.ToLower().Equals(CashReplishmentRequestStatus.Rejected.ToString().ToLower()))
            {
                var userModel = _userNotificationRepository.FindBy(xx => xx.ActionId.Equals(model.Id)).FirstOrDefault();
                var message = $"{_userInfoToken.FullName}, has rejected the cash requisition of {model.IssuedBy} on {model.IssuedDate} with ref:{model.ReferenceId} with message \"{command.ApprovedMessage}\" ";

                var userv = new UsersNotification("Branch Transfer Request", "/Notification/BranchToBranchTransferRequest?KEY={0}&BranchId=" + model.CorrespondingBranchId, model.Id, BaseUtilities.GenerateInsuranceUniqueNumber(15, "NOTIF-"), _userInfoToken.Id, _userInfoToken.BranchId, model.CorrespondingBranchId);
                model.ApprovedMessage = message;
            

                userModel.Action = userv.Action;
                userModel.ActionUrl = userv.ActionUrl;
                userModel.ActionId = userv.ActionId;
                userModel.BranchId = userv.BranchId;
                userModel.ModifiedBy = _userInfoToken.Id;
                userModel.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
                userModel.TempData = model.CorrespondingBranchId;
                _cashReplenishmentRepository.Update(model);
                _userNotificationRepository.Update(userModel);
            }
        }

        private async Task HandleOrderType(AddCashReplenishmentApprovalCommand command, CashReplenishment model)
        {
            if (command.CashRequisitionType.Equals("ORDER"))
            {
                if (command.CorrespondingBranchId.Length==3)
                {
                    var modelx = (await APICallHelper.GetAllBranchInfos(_pathHelper, _userInfoToken)).Find(x => x.branchCode.Equals(command.CorrespondingBranchId));
                    command.CorrespondingBranchId = modelx.id;
                        }
                // Fetch all bank accounts associated with Account2 "56" for the branch.
                var bankAccounts = _accountRepository.FindBy(x => x.Account2 == "56" && x.AccountOwnerId == command.CorrespondingBranchId);

                if (bankAccounts.Any())
                {
                    // Update the model with corresponding branch ID.
                    model.CorrespondingBranchId = command.CorrespondingBranchId;

                    // Create a new cash replenishment instance for the ORDER type.
                    var message = $"{_userInfoToken.FullName}, authorizes you to transfer funds to Branch ID {model.BranchId}.";
                    var cashReplenishment = new CashReplenishment
                    {
                        Id = BaseUtilities.GenerateInsuranceUniqueNumber(8, "CR-OR"),
                        IsApproved = command.IsApproved,
                        ApprovedBy = _userInfoToken.Id,
                        ApprovedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow),
                        RequestMessage = message,
                        AmountApproved = command.ApprovedAmount,
                        AmountRequested = command.ApprovedAmount,
                        ReferenceId = BaseUtilities.GenerateInsuranceUniqueNumber(8, "CR"),
                        ApprovedMessage = message,
                        Status =command.Status.ToString(),
                        CashRequisitionType = command.CashRequisitionType,
                        IssuedBy = _userInfoToken.Id,
                        IssuedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow),
                        CorrespondingBranchId = command.CorrespondingBranchId,
                        CashReplishmentRequestStatus = command.Status.ToString(),
                        ParentCashReplenishId = model.Id,
                        TempData = command.AccountId
                    };

                    // Prepare and update user notification for the ORDER type request.
                    var userNotification = new UsersNotification(
                        "Branch Transfer Request",
                        $"/Notification/BranchToBranchTransferRequest?KEY={model.Id}&BranchId={model.CorrespondingBranchId}",
                        model.Id,
                        BaseUtilities.GenerateInsuranceUniqueNumber(15, "NOTIF-"),
                        _userInfoToken.Id,
                        _userInfoToken.BranchId,
                        model.CorrespondingBranchId);

                    // Update model status and persist changes to the repository.
                    model.CashReplishmentRequestStatus = command.Status.ToString();
                    model.TempData = command.AccountId;
                    _cashReplenishmentRepository.Update(model);
                    _cashReplenishmentRepository.Add(cashReplenishment);

                    var userModel = _userNotificationRepository.FindBy(xx => xx.ActionId.Equals(model.Id)).FirstOrDefault();
                    // Update user notification repository.
                    userModel.Action = userNotification.Action;
                    userModel.ActionUrl = userNotification.ActionUrl;
                    userModel.ActionId = userNotification.ActionId;
                    userModel.BranchId = userNotification.BranchId;
                    userModel.ModifiedBy = _userInfoToken.Id;
                    userModel.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
                    userModel.TempData = model.CorrespondingBranchId;
                    _userNotificationRepository.Update(userModel);
                }
                else
                {
                    // Handle case when no bank account is registered for the branch.
                    var branch = await GetBranchInfo(command.CorrespondingBranchId);
                    throw new Exception($"No bank account registered for Branch: {branch.branchCode}-{branch.name} in TRUSTSOFT-CREDIT. Kindly redirect the cash requisition to another major branch.");
                }
            }
            else
            {

            }

        }
    }

}
