using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Handlers
{
  
    public class GetDashboardStatisticsQueryHandler : IRequestHandler<GetDashboardStatisticsQuery, ServiceResponse<List<DashboardStatisticsDto>>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetDashboardStatisticsQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IAccountService accountService;
        public readonly UserInfoToken _userTokenInfo;
        
        public readonly PathHelper _pathHelper;
        /// <summary>
        /// Constructor for initializing the GetAccountQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetDashboardStatisticsQueryHandler(
            IAccountRepository AccountRepository,
            IMapper mapper,
            ILogger<GetDashboardStatisticsQueryHandler> logger,
            UserInfoToken? userInfoToken,
            IAccountService accountService,PathHelper pathHelper)
        {
            _AccountRepository = AccountRepository;
            _mapper = mapper;
            _logger = logger;
            _userTokenInfo = userInfoToken;
            this.accountService = accountService;
            _pathHelper = pathHelper;
        }

        /// <summary>
        /// Handles the GetDashbaordQuery to retrieve a specific Account.
        /// </summary>
        /// <param name="request">The GetDashbaordQuery containing Account ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<DashboardStatisticsDto>>> Handle(GetDashboardStatisticsQuery request, CancellationToken cancellationToken)
        {
            List<Branch> branches = await APICallHelper.GetAllBranchInfos(_pathHelper, _userTokenInfo);
            DashboardStatisticsDto dashbaord = new DashboardStatisticsDto();
            string errorMessage = null;
            List < DashboardStatisticsDto > modelList = new();
            try
            {
                // Retrieve the Account entity with the specified ID from the repository
                if (_userTokenInfo.IsHeadOffice)
                {
                    if (QueryParameter.all.ToString()=="all")
                    {
                        foreach (var item in branches)
                        {
                            var list = await GetDashbaordDataAsync(item.id, item.name, item.branchCode);
                            modelList.AddRange(list);
                        }
                    }
                    else
                    {
                        modelList = await GetDashbaordDataAsync(_userTokenInfo.BranchId,_userTokenInfo.BranchName,_userTokenInfo.BranchCode) ;
                    }
                }
                else
                {
                    modelList = await GetDashbaordDataAsync(_userTokenInfo.BranchId, _userTokenInfo.BranchName, _userTokenInfo.BranchCode);

                }


                return ServiceResponse<List<DashboardStatisticsDto>>.ReturnResultWith200(modelList);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Account: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<List<DashboardStatisticsDto>>.Return500(e);
            }
        }

        private async Task<List<DashboardStatisticsDto>> GetDashbaordDataAsync(string branchId, string branchName, string branchCode)
        {
            List < DashboardStatisticsDto > dashboards = new List< DashboardStatisticsDto >();
            foreach (var item in accountService._accounts)
            {
                var model = accountService.GetAccountByKey(item.Key);

                var dashboardModel = await GetAccountStatusAsync(model, item.Key);
                dashboardModel.BranchName = branchName;
                dashboardModel.BranchCode = branchCode;
                dashboardModel.BranchId = branchId;
                dashboards.Add(dashboardModel);

            }
            return dashboards;
        }

        private async Task<DashboardStatisticsDto> GetAccountStatusAsync(AccountTypeDefinition model, string Key)
        {
            DashboardStatisticsDto dashbaord = new DashboardStatisticsDto();
            Data.Account account = new Data.Account();
            dashbaord.AccountNumber = model.Code.ToString();
            dashbaord.DashboardAccountType = Key;
            decimal Balance = 0;
            if (model.Code.ToString().Length == 1)
            {
                dashbaord.Balance = Convert.ToDecimal(_AccountRepository.FindBy(x => x.Account1 == model.Code.ToString()).Sum(x => x.CurrentBalance));
                account = (_AccountRepository.FindBy(x => x.Account1 == model.Code.ToString())).FirstOrDefault();
            }
            else if (model.Code.ToString().Length == 2)
            {
                dashbaord.Balance = Convert.ToDecimal(_AccountRepository.FindBy(x => x.Account2 == model.Code.ToString()).Sum(x => x.CurrentBalance));
                account = (_AccountRepository.FindBy(x => x.Account2 == model.Code.ToString())).FirstOrDefault();
            }
            else if (model.Code.ToString().Length == 3)
            {
                dashbaord.Balance = Convert.ToDecimal(_AccountRepository.FindBy(x => x.Account3 == model.Code.ToString()).Sum(x => x.CurrentBalance));
                account = (_AccountRepository.FindBy(x => x.Account3 == model.Code.ToString())).FirstOrDefault();

            }
            else if (model.Code.ToString().Length == 4)
            {
                dashbaord.Balance = Convert.ToDecimal(_AccountRepository.FindBy(x => x.Account4 == model.Code.ToString()).Sum(x => x.CurrentBalance));
                account = (_AccountRepository.FindBy(x => x.Account4 == model.Code.ToString())).FirstOrDefault();

            }
            else if (model.Code.ToString().Length == 5)
            {
                dashbaord.Balance = Convert.ToDecimal(_AccountRepository.FindBy(x => x.Account5 == model.Code.ToString()).Sum(x => x.CurrentBalance));
                account = (_AccountRepository.FindBy(x => x.Account5 == model.Code.ToString())).FirstOrDefault();

            }
            else if (model.Code.ToString().Length == 6)
            {
                dashbaord.Balance = Convert.ToDecimal(_AccountRepository.FindBy(x => x.Account6 == model.Code.ToString()).Sum(x => x.CurrentBalance));
                account = (_AccountRepository.FindBy(x => x.Account6 == model.Code.ToString())).FirstOrDefault();

            }
            else
            {
                if (model.HasArray)
                {
                    Balance = 0;
                    foreach (var item in model.CodeArray)
                    {
                        Balance += Convert.ToDecimal(_AccountRepository.FindBy(x => x.AccountNumberCU.Contains(item.ToString())).Sum(x => x.CurrentBalance));
                        account = (_AccountRepository.FindBy(x => x.AccountNumberCU == item.ToString())).FirstOrDefault();

                    }
                    dashbaord.Balance = Balance;
                }
            }
            dashbaord.AccountName = "";// account.AccountName;
            return dashbaord;
        }
    }
}
