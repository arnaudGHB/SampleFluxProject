using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;

using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.BankingOperation.Commands;
using CBS.AccountManagement.MediatR.Command;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new AccountCategory.
    /// </summary>
    public class AddDepositNotificationCommandHandler : IRequestHandler<AddDepositNotificationCommand, ServiceResponse<bool>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IDepositNotifcationRepository _depositNotifcationRepository; // Repository for accessing AccountCategory data.
        private readonly UserInfoToken _userInfoToken;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddDepositNotificationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IMediator _mediator;
        private readonly IUserNotificationRepository _userNotificationRepository;
        /// <summary>
        /// Constructor for initializing the AddCashInfusionCommandHandler.
        /// </summary>
        /// <param name="AccountCategoryRepository">Repository for AccountClass data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddDepositNotificationCommandHandler(IDepositNotifcationRepository cashReplenishmentRepository, IAccountClassRepository AccountClassRepository,
            ILogger<AddDepositNotificationCommandHandler> logger,
           UserInfoToken userInfoToken,
            IMapper mapper, IUnitOfWork<POSContext> work, IAccountRepository accountRepository, IUserNotificationRepository? userNotificationRepository, IMediator? mediator)
        {
            _depositNotifcationRepository = cashReplenishmentRepository;
            _accountRepository = accountRepository;
            _userInfoToken = userInfoToken;
            _userNotificationRepository = userNotificationRepository;
            _logger = logger;
            _mapper = mapper;
            _mediator = mediator;
            _uow = work;
        }

        /// <summary>
        /// Handles the AddAccountClassCommand to add a new AccountClass.
        /// </summary>
        /// <param name="request">The AddAccountClassCommand containing AccountClass data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(AddDepositNotificationCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = string.Empty;
            try
            {
                var model = _depositNotifcationRepository.FindBy(x=>x.Amount==request.Amount && x.Message==request.Message);
                if (model.Any())
                {
                   
                          errorMessage = $"Your cash Deposit Notification  already exists, please check your request message and amount.";
                        _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Conflict, LogAction.AddDepositNotificationCommand, LogLevelInfo.Warning);

                    return ServiceResponse<bool>.Return409(errorMessage);
                    
                    
                }
               var Entity = DepositNotification.SetDepositCommand(JsonConvert.SerializeObject(request), _userInfoToken);
                Entity.IssueDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
                Entity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(15, "DP");
              var accounts = _accountRepository.All.Where(account => account.Account2 == "56"&& account.AccountOwnerId==_userInfoToken.BranchId);
                Entity.HasBankAccount = accounts.Any();
                Entity.CorrepondingBranchId = _userInfoToken.BranchId;
                Entity.ApprovalKey = "XXXXX";
                Entity.ApprovedMessage = "XXXXX";
                Entity.BankAccountId= request.BankAccountId;           
                var userv = new UsersNotification("Bank Deposit Approval Request", "/Notification/BankDepositApprovalRequest?KEY={0}", Entity.Id, BaseUtilities.GenerateInsuranceUniqueNumber(15, "NOTIF-"), _userInfoToken.Id,_userInfoToken.BranchId, _userInfoToken.BranchId);
                userv.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
                _userNotificationRepository.Add(userv);
                _depositNotifcationRepository.Add(Entity);
                await _uow.SaveAsync();
                var branchTransferDto = new BranchToBranchTransferDto
                {
                    Id = Entity.Id,
                    ReferenceId = Entity.Id,
                    CurrencyNotesRequest = request.CurrencyNotesRequest,
                    Amount = request.Amount
                };
                await _mediator.Send(branchTransferDto);
                errorMessage = $"AddDepositNotificationCommand was executed successfully";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.OK, LogAction.AddDepositNotificationCommand, LogLevelInfo.Information);


                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                  errorMessage = $"Error occurred while Adding AddDepositNotificationCommand: {e.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.AddDepositNotificationCommand, LogLevelInfo.Error);

                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e + errorMessage);
            }
        }
    }
}