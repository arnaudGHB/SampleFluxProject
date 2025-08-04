using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;

using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.BankingOperation.Commands;
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
    public class AddCashInfusionCommandHandler : IRequestHandler<AddCashInfusionCommand, ServiceResponse<bool>>
    {
        private readonly ICashReplenishmentRepository _cashReplenishmentRepository; // Repository for accessing AccountCategory data.
        private readonly UserInfoToken _userInfoToken;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddCashInfusionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IUserNotificationRepository _userNotificationRepository;
        /// <summary>
        /// Constructor for initializing the AddCashInfusionCommandHandler.
        /// </summary>
        /// <param name="AccountCategoryRepository">Repository for AccountClass data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddCashInfusionCommandHandler(ICashReplenishmentRepository cashReplenishmentRepository, IAccountClassRepository AccountClassRepository,
            ILogger<AddCashInfusionCommandHandler> logger,
           UserInfoToken userInfoToken,
            IMapper mapper, IUnitOfWork<POSContext> work, IUserNotificationRepository? userNotificationRepository)
        {
            _cashReplenishmentRepository = cashReplenishmentRepository;
            _userNotificationRepository = userNotificationRepository;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _mapper = mapper;
            _uow = work;
        }

        /// <summary>
        /// Handles the AddAccountClassCommand to add a new AccountClass.
        /// </summary>
        /// <param name="request">The AddAccountClassCommand containing AccountClass data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(AddCashInfusionCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = "";
            try
            {
                var model = _cashReplenishmentRepository.All.Where(x =>
                               x.BranchId == _userInfoToken.BranchId &&
                                    x.Status.ToLower() != CashReplishmentRequestStatus.Completed.ToString().ToLower() &&
                                    x.Status.ToLower() != CashReplishmentRequestStatus.Rejected.ToString().ToLower()); 
                if (_userInfoToken.IsHeadOffice) 
                {
                      errorMessage = $"You are not authourized to perform this operation.";
                    _logger.LogError(errorMessage);
                              await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Conflict, LogAction.AddCashInfusionCommand, LogLevelInfo.Warning);

                    return ServiceResponse<bool>.Return409(errorMessage);


                }
                 
                if (model.Any())
                {
                   
                          errorMessage = $"There is an existing cash replenishment request not yet completed, please check your  cash replenishment request tracker.";
                        _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Unauthorized, LogAction.AddCashInfusionCommand, LogLevelInfo.Warning);

                    return ServiceResponse<bool>.Return401(errorMessage);                
                }
               var Entity = CashReplenishment.SetCashInfusionCommand(JsonConvert.SerializeObject(request), _userInfoToken);
                Entity.IssuedDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
                Entity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(15, "CR");
                Entity.ReferenceId = Entity.Id;
                Entity.CashRequisitionType = "REQUEST";
                Entity.CurrencyCode = "XAF";
                _cashReplenishmentRepository.Add(Entity);
                var user = new UsersNotification("Approve Cash Request", "/Notification/ApproveCashRequest?KEY={0}", Entity.Id, BaseUtilities.GenerateInsuranceUniqueNumber(15, "NOTIF-"),_userInfoToken.Id,_userInfoToken.BranchId, _userInfoToken.BranchId);
                user.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
                user.BranchId = _userInfoToken.BranchId;
                _userNotificationRepository.Add(user);
                await _uow.SaveAsync();
                errorMessage = $"Cash requisition request successfully sent";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.OK, LogAction.AddCashInfusionCommand, LogLevelInfo.Information);

                return ServiceResponse<bool>.ReturnResultWith200(true, errorMessage);

            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                  errorMessage = $"Error occurred while Adding CashInfusionCommands: {e.Message}";

                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.AddCashInfusionCommand, LogLevelInfo.Warning);

                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e + errorMessage);
            }
        }

        public  static bool IsActiveStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
                return false;

            var excludedStatuses = new[] {
        CashReplishmentRequestStatus.Completed.ToString().ToLower(),
        "rejected"
    };

            return !excludedStatuses.Contains(status.ToLower());
        }
    }
}