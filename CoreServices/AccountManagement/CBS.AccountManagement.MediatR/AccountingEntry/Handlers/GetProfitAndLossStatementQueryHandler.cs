using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.Repository;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Handlers
{
    public class GetProfitAndLossStatementQueryHandler : IRequestHandler<GetProfitAndLossStatementQuery, ServiceResponse<IncomeExpenseStatementDto>>
    {
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetProfitAndLossStatementQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IAccountRepository _accountRepository;
        private readonly IChartOfAccountRepository _chartOfaccountRepository;
        private readonly IAccountCategoryRepository _accountCategoryRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;
        /// <summary>
        /// Constructor for initializing the GetAccountingEntryQueryHandler.
        /// </summary>
        /// <param name="AccountingEntryRepository">Repository for AccountingEntry data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetProfitAndLossStatementQueryHandler(
            IAccountRepository accountRepository,


            IConfiguration configuration,
            IMapper mapper,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken,
             ILogger<GetProfitAndLossStatementQueryHandler> logger,
             IChartOfAccountRepository? chartOfaccountRepository, IAccountCategoryRepository? accountCategoryRepository)
        {

            _mapper = mapper;
            _logger = logger;
            _accountRepository = accountRepository;
            _chartOfaccountRepository = chartOfaccountRepository;
            _userInfoToken = userInfoToken;
            _accountCategoryRepository = accountCategoryRepository;
            _pathHelper = new PathHelper(configuration);

        }


        public async Task<ServiceResponse<IncomeExpenseStatementDto>> Handle(GetProfitAndLossStatementQuery request, CancellationToken cancellationToken)
        {
            List<Data.Account> accounts = new List<Data.Account>();
            List<Data.Account> incomeAccounts = new List<Data.Account>();
            List<Data.Account> expenseAccounts = new List<Data.Account>();
            List<Data.ChartOfAccount> chartOfAccounts = new List<Data.ChartOfAccount>();
            List<IncomeExpenseStatementDto> IncomeExpenseAccounts = new List<IncomeExpenseStatementDto>();
            string errorMessage = "";
            string accountCategoryIdForexpense = "";
            string accountCategoryIdForincome = "";
     
            try
            {
        


                decimal totalIncomes = CalculateTotalIncomes(incomeAccounts);
                decimal totalExpense = CalculateTotalExpense(expenseAccounts);
                var Branches = await APICallHelper.GetAllBranchInfos(_pathHelper,_userInfoToken);
                Branch Branch = Branches.Where(x=>x.id==request.BranchId).FirstOrDefault();
                IncomeExpenseAccounts.AddRange(ConvertAccountsToIncomeExpenseStatement(incomeAccounts, request.QueryLevel, totalIncomes, totalExpense, Branch));
                IncomeExpenseAccounts.AddRange(ConvertAccountsToIncomeExpenseStatement(expenseAccounts, request.QueryLevel, totalIncomes, totalExpense, Branch));



              
                var balanceSheetDto = new IncomeExpenseStatementDto
                {
                    Address = Branch.address,
                    Name = Branch.name,
                    Location = Branch.location,
                    EntityType = request.QueryLevel,
                   BranchId = Branch.id,
                  
                    Date = BaseUtilities.UtcToDoualaTime(DateTime.Now),
                    TotalRevenue = totalIncomes,
                    TotalExpenses = totalExpense,

                };
                errorMessage = $"IncomeExpenseStatement data retrieved successfully at the level of {request.QueryLevel}.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetGeneralBalanceSheetQuery",
                request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<IncomeExpenseStatementDto>.ReturnResultWith200(balanceSheetDto);
            }
            catch (Exception ex)
            {

                errorMessage = $"IncomeExpenseStatement data failed to retrieved successfully at the level of {request.QueryLevel}.Error occurred while getting AccountingEntry: {ex.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetGeneralBalanceSheetQuery",
                request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<IncomeExpenseStatementDto>.Return500(ex);
            }
        }

        private List<IncomeExpenseStatementDto> ConvertAccountsToIncomeExpenseStatement(List<Data.Account> assetAccounts, string QueryLevel,decimal totalIncome,decimal totalExpense, Branch branch)
        {
            List<IncomeExpenseStatementDto> balanceSheetAccounts = new List<IncomeExpenseStatementDto>();
            if (QueryLevel.Equals(SearchOptions.Branch))
            {
                balanceSheetAccounts = (from account in assetAccounts
                                        select new IncomeExpenseStatementDto
                                        {
                                            TotalRevenue = totalIncome,
                                            TotalExpenses=totalExpense,
                                            Address=branch.address,
                                            EntityType= "Branch",
                                            Name=branch.name,
                                            Location=branch.location,
                                            BranchId=branch.id,

                                           
                                            AccountName = account.AccountName,
                                            AccountNumber = account.AccountNumber,
                                            CurrentBalance = account.CurrentBalance,
                                            Cartegory = account.AccountCategoryId
                                        }).ToList();
            }
            else if (QueryLevel.Equals(SearchOptions.Zone) || QueryLevel.Equals(SearchOptions.HeadOffice))
            {
        
                balanceSheetAccounts = assetAccounts
                                         .GroupBy(a => new { a.AccountNumber, a.BranchId })
                                         .Select(g => new IncomeExpenseStatementDto
                                         {
                                             TotalRevenue = totalIncome,
                                             TotalExpenses = totalExpense,
                                             Address = branch.address,
                                             EntityType = QueryLevel.ToUpper(),
                                             Name = branch.name,
                                             Location = branch.location,
                                             BranchId = branch.id,
                                             AccountName = g.Max(a => a.AccountName),
                                             AccountNumber = g.Key.AccountNumber,
                      
                                             CurrentBalance = g.Sum(a => a.CurrentBalance),
                                             Cartegory = g.Max(a => a.ChartOfAccountManagementPosition.ChartOfAccount.AccountCartegory.Name)
                                         })
                                         .ToList();
            }


            return balanceSheetAccounts;
        }

        private string GetAccountCategoryIdForIncomeOrExpense(string value)
        {

            return _accountCategoryRepository.FindBy(ac => ac.Name.ToLower() == value).FirstOrDefault().Id;

        }

        private decimal CalculateTotalIncomes(List<Data.Account> accounts)
        {


            return accounts.Sum(a => a.CurrentBalance);
        }

        private decimal CalculateTotalExpense(List<Data.Account> accounts)
        {
            return accounts.Where(a => a.CurrentBalance < 0).Sum(a => Math.Abs(a.CurrentBalance));
        }

        private decimal CalculateProfitOrLoss(decimal income, decimal expenses)
        {
            return income - expenses;
        }

    }
}
