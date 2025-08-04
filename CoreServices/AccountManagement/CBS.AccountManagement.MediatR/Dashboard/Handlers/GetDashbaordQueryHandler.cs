using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Dashboard.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Account based on its unique identifier.
    /// </summary>
    public class GetDashbaordQueryHandler : IRequestHandler<GetDashbaordQuery, ServiceResponse<DashbaordDto>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetDashbaordQueryHandler> _logger; // Logger for logging handler actions and errors.
        public readonly UserInfoToken _userTokenInfo;
        /// <summary>
        /// Constructor for initializing the GetAccountQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetDashbaordQueryHandler(
            IAccountRepository AccountRepository,
            IMapper mapper,
            ILogger<GetDashbaordQueryHandler> logger,
            UserInfoToken? userInfoToken)
        {
            _AccountRepository = AccountRepository;
            _mapper = mapper;
            _logger = logger;
            _userTokenInfo = userInfoToken;
        }

        /// <summary>
        /// Handles the GetDashbaordQuery to retrieve a specific Account.
        /// </summary>
        /// <param name="request">The GetDashbaordQuery containing Account ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DashbaordDto>> Handle(GetDashbaordQuery request, CancellationToken cancellationToken)
        {
            DashbaordDto dashbaord = new DashbaordDto();
            string errorMessage = null;
            try
            {
                // Retrieve the Account entity with the specified ID from the repository
                if (_userTokenInfo.IsHeadOffice)
                {
                    dashbaord.CashInBank.AccountName = "Sight Current and saving account in other financial institution";
                    dashbaord.CashInBank.AccountNumber = "56";
                    dashbaord. CashInBank.Balance =Convert.ToDouble( _AccountRepository.FindBy(x => x.Account2 == "56").Sum(x => x.CurrentBalance));
                    dashbaord.CashInHand.AccountName = "Cash in vault";
                    dashbaord.CashInHand.AccountNumber = "571";
                    dashbaord. CashInHand.Balance = Convert.ToDouble(_AccountRepository.FindBy(x => x.Account3 == "571").Sum(x => x.CurrentBalance));
                    dashbaord.TotalDeposit.AccountName = "Members deposits";
                    dashbaord.TotalDeposit.AccountNumber = "3711";
                    dashbaord.TotalDeposit.Balance = Convert.ToDouble(_AccountRepository.FindBy(x => x.Account4 == "3711").Sum(x => x.CurrentBalance));
                    dashbaord.TotalSavings.AccountName = "Members savings";
                    dashbaord.TotalSavings.AccountNumber = "373";
                    dashbaord. TotalSavings.Balance = Convert.ToDouble(  _AccountRepository.FindBy(x => x.Account3 == "373").Sum(x => x.CurrentBalance));
                    dashbaord.TotalShares.AccountName = "Members shares";
                    dashbaord.TotalShares.AccountNumber = "10000";
                    dashbaord. TotalShares.Balance = Convert.ToDouble( _AccountRepository.FindBy(x => x.Account5 == "10000").Sum(x => x.CurrentBalance));
                    dashbaord.TotalIncome.AccountName = "Total Income Generated";
                    dashbaord.TotalIncome.AccountNumber = "7";
                    dashbaord. TotalIncome.Balance = Convert.ToDouble(   _AccountRepository.FindBy(x => x.Account1 == "7").Sum(x => x.CurrentBalance));
                    dashbaord.TotalExpense.AccountName = "Total Expense";
                    dashbaord.TotalExpense.AccountNumber = "6";
                    dashbaord. TotalExpense.Balance = Convert.ToDouble(_AccountRepository.FindBy(x => x.Account1 == "6").Sum(x => x.CurrentBalance));
                }
                else 
                {
                    dashbaord.CashInBank.AccountName = "Sight Current and saving account in other financial institution";
                    dashbaord.CashInBank.AccountNumber = "56";
                    dashbaord.CashInBank.Balance = Convert.ToDouble(_AccountRepository.FindBy(x =>x.AccountOwnerId == _userTokenInfo.BranchId && x.Account2 == "56").Sum(x => x.CurrentBalance));

                    dashbaord.CashInHand.AccountName = "Cash in vault";
                    dashbaord.CashInHand.AccountNumber = "571"; 
                    dashbaord.CashInHand.Balance = Convert.ToDouble(_AccountRepository.FindBy(x => x.AccountOwnerId == _userTokenInfo.BranchId && x.Account2 == "57").Sum(x => x.CurrentBalance));
                    dashbaord.TotalDeposit.AccountName = "Members deposits";
                    dashbaord.TotalDeposit.AccountNumber = "3711";
                    dashbaord.TotalDeposit.Balance = Convert.ToDouble(_AccountRepository.FindBy(x => x.AccountOwnerId == _userTokenInfo.BranchId && x.Account4 == "3711").Sum(x => x.CurrentBalance));
                    dashbaord.TotalSavings.AccountName = "Members savings";
                    dashbaord.TotalSavings.AccountNumber = "373";
                    dashbaord.TotalSavings.Balance = Convert.ToDouble(_AccountRepository.FindBy(x => x.AccountOwnerId == _userTokenInfo.BranchId && x.Account3 == "373").Sum(x => x.CurrentBalance));
                    dashbaord.TotalShares.AccountName = "Members shares";
                    dashbaord.TotalShares.AccountNumber = "10000";
                    dashbaord.TotalShares.Balance = Convert.ToDouble(_AccountRepository.FindBy(x => x.AccountOwnerId == _userTokenInfo.BranchId && x.Account5 == "10000").Sum(x => x.CurrentBalance));
                    dashbaord.TotalIncome.AccountName = "Total Income Generated";
                    dashbaord.TotalIncome.AccountNumber = "7";
                    dashbaord.TotalIncome.Balance = Convert.ToDouble(_AccountRepository.FindBy(x => x.AccountOwnerId == _userTokenInfo.BranchId && x.Account1 == "7").Sum(x => x.CurrentBalance));
                    dashbaord.TotalExpense.AccountName = "Total Expense";
                    dashbaord.TotalExpense.AccountNumber = "6";
                    dashbaord.TotalExpense.Balance = Convert.ToDouble(_AccountRepository.FindBy(x => x.AccountOwnerId == _userTokenInfo.BranchId && x.Account1 == "6").Sum(x => x.CurrentBalance));

                }
             
               
                return ServiceResponse<DashbaordDto>.ReturnResultWith200(dashbaord);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Account: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<DashbaordDto>.Return500(e);
            }
        }
    }
}
