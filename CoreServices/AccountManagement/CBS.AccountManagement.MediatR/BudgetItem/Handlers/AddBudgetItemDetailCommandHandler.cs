using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Account.
    /// </summary>
    public class AddBudgetItemDetailCommandHandler : IRequestHandler<AddBudgetItemDetailCommand, ServiceResponse<bool>>
    {
        private readonly ICashMovementTrackingConfigurationRepository _ChartOfAccountRepository;  
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddBudgetItemDetailCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IBudgetItemDetailRepository _budgetItemDetailRepository;
        /// <summary>
        /// Constructor for initializing the AddBudgetItemDetailCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddBudgetItemDetailCommandHandler(
            IAccountRepository AccountRepository,
            ICashMovementTrackingConfigurationRepository ChartOfAccountRepository,
            IBudgetItemDetailRepository budgetItemDetailRepository,
            IMapper mapper,
            ILogger<AddBudgetItemDetailCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _AccountRepository = AccountRepository;
            _mapper = mapper;
            _ChartOfAccountRepository = ChartOfAccountRepository;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _budgetItemDetailRepository = budgetItemDetailRepository;
        }

        /// <summary>
        /// Handles the AddAccountCommand to add a new Account.
        /// </summary>
        /// <param name="request">The AddAccountCommand containing Account data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(AddBudgetItemDetailCommand request, CancellationToken cancellationToken)
        {
            Data.BudgetItemDetail model = new Data.BudgetItemDetail();
            try
            {

                var _budgetItemDetail = _budgetItemDetailRepository.FindBy(c => c.BudgetItem == request.BudgetItem&&c.BranchId==_userInfoToken.BranchId && c.BudgetId == request.BudgetId);

                if (_budgetItemDetail.Any())
                {

                    var errorMessage = $"BudgetItemDetail  :{request.BudgetItem} is already existing in budgetId: {request.BudgetId}.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "AddBudgetItemDetailCommand",
                        JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                model = _mapper.Map<BudgetItemDetail>(request);
                model.Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "BID");
                _budgetItemDetailRepository.Add(model);
                await _uow.SaveAsync();
                // Map the Account entity to AccountDto and return it with a success response
              
                    var errorMessag = $"new Account created Successfully.";
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "AddBudgetItemDetailCommand",
                        JsonConvert.SerializeObject(request), errorMessag, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    return ServiceResponse<bool>.ReturnResultWith200(true);
                       
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving BudgetItemDetail: {BaseUtilities.GetInnerExceptionMessages(e)}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddBudgetItemDetailCommand",
                    request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}