using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Account.
    /// </summary>
    public class AddAccountCommandHandler : IRequestHandler<AddAccountCommand, ServiceResponse<bool>>
    {
        private readonly IChartOfAccountRepository _ChartOfAccountRepository;
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddAccountCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountManagementPositionRepository;

        /// <summary>
        /// Constructor for initializing the AddAccountCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddAccountCommandHandler(
            IAccountRepository AccountRepository,
            IChartOfAccountRepository ChartOfAccountRepository,
            IMapper mapper,
            ILogger<AddAccountCommandHandler> logger,PathHelper pathHelper,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken, IChartOfAccountManagementPositionRepository? chartOfAccountManagementPositionRepository)
        {
            _AccountRepository = AccountRepository;
            _mapper = mapper;
            _ChartOfAccountRepository = ChartOfAccountRepository;
            _chartOfAccountManagementPositionRepository = chartOfAccountManagementPositionRepository;
            _logger = logger;
            _uow = uow;
        _pathHelper = pathHelper;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddAccountCommand to add a new Account.
        /// </summary>
        /// <param name="request">The AddAccountCommand containing Account data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(AddAccountCommand request, CancellationToken cancellationToken)
        {
            Data.ChartOfAccount model = new Data.ChartOfAccount();
            try
            {

                var chart = await _chartOfAccountManagementPositionRepository.FindAsync(request.ChartOfAccountManagementPositionId);
                chart.ChartOfAccount = await _ChartOfAccountRepository.FindAsync(chart.ChartOfAccountId);
                if (chart.ChartOfAccount == null)
                {

                    var errorMessage = $"ChartOfAccountId :{request.AccountNumber} does not exist in the accounting Chart.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "AddAccountCommand",
                        JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }
             
                // Check if a Account with the same name already exists (case-insensitive)

                // Map the AddAccountCommand to a Account entity
                var AccountEntity = _mapper.Map<Data.Account>(request);

                AccountEntity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "AC");
                AccountEntity.AccountCategoryId = chart.ChartOfAccount.AccountCartegoryId;
                if (request.AccountNumber.Contains("451"))
                {
                    List<Branch> branches = await APICallHelper.GetAllBranchInfos(_pathHelper, _userInfoToken);
                    var tempRemittance = request.AccountTypeId.Split("@");
                  
                        request.AccountName = GetLiaisonName( request.LiaisonBranchCode,branches);
                        AccountEntity.SetAccount451Entity(request.OwnerBranchCode, AccountEntity, chart.ChartOfAccount.AccountCartegoryId, request.AccountNumber, request.AccountNumberCU, request.AccountName, request.ChartOfAccountManagementPositionId, request.AccountOwnerId, _pathHelper.BankManagement_BankCode, request.LiaisonBranchCode, chart.PositionNumber, request.AccountTypeId);
                }
                else
                {
                    AccountEntity.SetAccountEntity(AccountEntity, chart.ChartOfAccount.AccountCartegoryId, request.AccountNumber, request.AccountName, request.ChartOfAccountManagementPositionId, request.AccountOwnerId, request.AccountNumberCU, _userInfoToken.BranchCode, chart.PositionNumber);

                }
                // Add the new Account entity to the repository
                _AccountRepository.Add(AccountEntity);
                await _uow.SaveAsync();



                // Map the Account entity to AccountDto and return it with a success response
                var AccountDto = _mapper.Map<AccountDto>(AccountEntity);
                var errorMessag = $"new Account created Successfully.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddAccountCommand",
                    JsonConvert.SerializeObject(request), errorMessag, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Account: {BaseUtilities.GetInnerExceptionMessages(e)}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddAccountCommand",
                    request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<bool>.Return500(e);
            }
        }

        private string GetLiaisonName(string branchCode, List<Branch> _branches)
        {
            var branch = _branches.FirstOrDefault(x => x.branchCode == branchCode);
            return _userInfoToken.BranchName + "-" + branch?.name + " Liaison Account";
        }
    }
}