using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity.Currency;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR;
using CBS.AccountManagement.MediatR.AccountingEntry.Handlers;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.ExceptionHandler;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CBS.AccountingEntryManagement.MediatR.Handlers
{
    public class Get4ColumnTrialBalanceQueryHandler : IRequestHandler<Get4ColumnTrialBalanceQuery, ServiceResponse<List<TrialBalance4ColumnDto>>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository; // Repository for accessing AccountingEntry data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<Get4ColumnTrialBalanceQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountingEntriesServices _accountingEntriesServices;
        private readonly ITrialBalanceRepository? _trialBalanceRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly AccountManagement.Helper.PathHelper _pathHelper;
        private readonly IChartOfAccountRepository _chartOfAccountRepository;
        /// <summary>
        /// Constructor for initializing the GetAccountingEntryQueryHandler.
        /// </summary>
        /// <param name="AccountingEntryRepository">Repository for AccountingEntry data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public Get4ColumnTrialBalanceQueryHandler(
            IAccountRepository accountRepository,
            IChartOfAccountRepository chartOfAccountRepository,
        IAccountingEntryRepository accountingEntryRepository,
            IConfiguration configuration,
            IMapper mapper,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken,
             ILogger<Get4ColumnTrialBalanceQueryHandler> logger, IAccountingEntriesServices accountingEntriesServices, ITrialBalanceRepository? trialBalanceRepository)
        {
            _accountingEntryRepository = accountingEntryRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _accountRepository = accountRepository;
            _accountingEntriesServices = accountingEntriesServices;
            _trialBalanceRepository = trialBalanceRepository;
            _pathHelper = new AccountManagement.Helper.PathHelper(configuration);
            _chartOfAccountRepository = chartOfAccountRepository;
        }

        /// <summary>
        /// Handles the Get6ColumnTrialBalanceQuery to retrieve a specific AccountingEntry.
        /// </summary>
        /// <param name="request">The GetAccountingEntryQuery containing AccountingEntry ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        //public async Task<ServiceResponse<List<TrialBalance4ColumnDto>>> Handle(Get4ColumnTrialBalanceQuery request, CancellationToken cancellationToken)
        //{
        //    List<AccountingEntry> accountingEntries = new();
        //    List<Account> accounts = new();
        //    List<TrialBalance4ColumnDto> trialBalances = new();
        //    var data = new List<TrialBalance4ColumnDto>();
        //    string errorMessage = null;
        //    try
        //    {
        //        var BranchList = await APICallHelper.GetAllBranchInfos(_pathHelper, _userInfoToken);
        //        #region MyRegion
        //        //accountingEntries = request.BranchId == "XXXXXX" ? await _accountingEntryRepository.All
        //        //                            .Where(x => x.IsDeleted == false && x.EntryDate.Date >= request.FromDate && x.EntryDate.Date <= request.ToDate)
        //        //                            .ToListAsync() : await _accountingEntryRepository.All
        //        //                            .Where(x => x.IsDeleted == false && x.EntryDate.Date >= request.FromDate && x.EntryDate.Date <= request.ToDate && x.BranchId == request.BranchId)
        //        //                            .ToListAsync();

        //        //accounts = request.BranchId == "XXXXXX" ? await _accountRepository.All
        //        //                          .Where(x => x.IsDeleted == false)
        //        //                          .ToListAsync() : await _accountRepository.All
        //        //                          .Where(x => x.IsDeleted == false && x.AccountOwnerId == request.BranchId)
        //        //                          .ToListAsync();
        //        //if (accountingEntries.Any() && accounts.Any() && request.BranchId != "XXXXXX")
        //        //{
        //        //    var BranchInfo = await APICallHelper.GetBankInfos(_pathHelper, _userInfoToken, request.BranchId, _userInfoToken.Token);
        //        //    data = (from e in accountingEntries
        //        //            join a in accounts on e.AccountId equals a.Id
        //        //            group new { e, a } by new { e.AccountId, a.AccountName, a.AccountNumber, a.BeginningBalance, e.AccountCartegory } into g
        //        //            select new TrialBalance4ColumnDto
        //        //            {
        //        //                EntityId = BranchInfo.id,
        //        //                EntityType = "BRANCH",
        //        //                FromDate = request.FromDate,
        //        //                ToDate = request.ToDate,
        //        //                BranchName = BranchInfo.name,
        //        //                BranchAddress = BranchInfo.address,
        //        //                Name = BranchInfo.bank.name,
        //        //                Location = BranchInfo.bank.address,
        //        //                Address = BranchInfo.bank.address,
        //        //                Capital = BranchInfo.bank.capital,
        //        //                ImmatriculationNumber = BranchInfo.bank.immatriculationNumber,
        //        //                WebSite = BranchInfo.webSite,
        //        //                BranchTelephone = BranchInfo.telephone,
        //        //                HeadOfficeTelePhone = BranchInfo.bank.telephone,
        //        //                AccountName = g.Key.AccountName,
        //        //                Cartegory = g.Key.AccountCartegory,
        //        //                AccountNumber = g.Key.AccountNumber,
        //        //                BeginningBalance = Convert.ToDouble(g.Key.BeginningBalance),
        //        //                DebitBalance = Convert.ToDouble(g.Sum(x => x.e.DrAmount)),
        //        //                CreditBalance = Convert.ToDouble(g.Sum(x => x.e.CrAmount)),
        //        //                EndingBalance = Convert.ToDouble((g.Key.BeginningBalance + g.Sum(x => x.e.DrAmount) - g.Sum(x => x.e.CrAmount))),

        //        //            }).ToList();

        //        //}
        //        //else
        //        //{
        //        //    var BranchInfos = (await APICallHelper.GetAllBankInfos(_pathHelper, _userInfoToken, _userInfoToken.Token)).Where(c => c.branchCode == "00").FirstOrDefault();
        //        //    data = (from e in accountingEntries
        //        //            join a in accounts on e.AccountId equals a.Id
        //        //            group new { e, a } by new { e.AccountNumber, a.AccountName, a.BeginningBalance, e.AccountCartegory } into g
        //        //            select new TrialBalance4ColumnDto
        //        //            {
        //        //                EntityId = "",
        //        //                EntityType = "HEADOFFICE",
        //        //                FromDate = request.FromDate,
        //        //                ToDate = request.ToDate,
        //        //                BranchName = "HEADOFFICE",
        //        //                BranchAddress = BranchInfos.bank.address,
        //        //                Name = BranchInfos.bank.name,
        //        //                Location = BranchInfos.bank.address,
        //        //                Address = BranchInfos.bank.address,
        //        //                Capital = BranchInfos.bank.capital,
        //        //                ImmatriculationNumber = BranchInfos.bank.immatriculationNumber,
        //        //                WebSite = BranchInfos.bank.webSite,
        //        //                BranchTelephone = BranchInfos.telephone,
        //        //                HeadOfficeTelePhone = BranchInfos.bank.telephone,
        //        //                AccountName = g.Key.AccountName,
        //        //                Cartegory = g.Key.AccountCartegory,
        //        //                AccountNumber = g.Key.AccountNumber,
        //        //                BeginningBalance = Convert.ToDouble(g.Key.BeginningBalance),
        //        //                DebitBalance = Convert.ToDouble(g.Sum(x => x.e.DrAmount)),
        //        //                CreditBalance = Convert.ToDouble(g.Sum(x => x.e.CrAmount)),
        //        //                EndingBalance = Convert.ToDouble((g.Key.BeginningBalance + g.Sum(x => x.e.DrAmount) - g.Sum(x => x.e.CrAmount))),

        //        //            }).ToList();
        //        //    data = GroupAllBranchAccountBalace(data);
        //        //    //data = SetAccountNameFromChartOfAccounts(data);

        //        //}

        //        //var BranchInfo = await APICallHelper.GetBankInfos(_pathHelper, _userInfoToken, request.BranchId, _userInfoToken.Token);
        //        //var datas = accounts.Select(a =>
        //        //{
        //        //    var entries = accountingEntries.Where(e => e.AccountId == a.Id).ToList();
        //        //    var beforeStartEntries = entries.Where(e => e.EntryDate.Date < request.FromDate);
        //        //    var periodEntries = entries.Where(e => e.EntryDate.Date >= request.FromDate && e.EntryDate.Date <= request.ToDate);

        //        //    var beginningBalance = a.BeginningBalance +
        //        //        beforeStartEntries.Sum(e => e.DrAmount) - beforeStartEntries.Sum(e => e.CrAmount);
        //        //    var periodDebit = periodEntries.Sum(e => e.DrAmount);
        //        //    var periodCredit = periodEntries.Sum(e => e.CrAmount);
        //        //    var endingBalance = beginningBalance + periodDebit - periodCredit;

        //        //    return new TrialBalance4ColumnDto
        //        //    {
        //        //        EntityId = BranchInfo.id,
        //        //        EntityType = "BRANCH",
        //        //        FromDate = request.FromDate,
        //        //        ToDate = request.ToDate,
        //        //        BranchName = BranchInfo.name,
        //        //        BranchAddress = BranchInfo.address,
        //        //        Name = BranchInfo.bank.name,
        //        //        Location = BranchInfo.bank.address,
        //        //        Address = BranchInfo.bank.address,
        //        //        Capital = BranchInfo.bank.capital,
        //        //        ImmatriculationNumber = BranchInfo.bank.immatriculationNumber,
        //        //        WebSite = BranchInfo.webSite,
        //        //        BranchTelephone = BranchInfo.telephone,
        //        //        HeadOfficeTelePhone = BranchInfo.bank.telephone,
        //        //        AccountName = a.AccountName,
        //        //        Cartegory ="cvvvvb",
        //        //        AccountNumber = a.AccountNumber,
        //        //        BeginningBalance = beginningBalance,
        //        //        DebitBalance = periodDebit,
        //        //        CreditBalance = periodCredit,
        //        //        EndingBalance = endingBalance,
        //        //        totalBeginningBalance= beginningBalance,
        //        //        totalDebitBalance = periodDebit,
        //        //        totalCreditBalance = periodCredit,
        //        //        totalEndingBalance = endingBalance,
        //        //        //TransactionCount = periodEntries.Count()
        //        //    };
        //        //}).ToList();
        //        #endregion

        //        #region MyRegion
        //        var startDate = request.FromDate.Date;
        //        var endDate = request.ToDate.Date;

        //        // Fetch accounting entries
        //        accountingEntries = request.BranchId == "XXXXXX"
        //           ? await _accountingEntryRepository.All
        //               .Where(x => !x.IsDeleted && x.ValueDate.Date >= startDate && x.ValueDate.Date <= endDate)
        //               .ToListAsync()
        //           : await _accountingEntryRepository.All
        //               .Where(x => !x.IsDeleted && x.ValueDate.Date >= startDate && x.ValueDate.Date <= endDate && x.BranchId == request.BranchId)
        //               .ToListAsync();

        //        // Fetch accounts
        //        accounts = request.BranchId == "XXXXXX"
        //           ? await _accountRepository.All
        //               .Where(x => !x.IsDeleted)
        //               .ToListAsync()
        //           : await _accountRepository.All
        //               .Where(x => !x.IsDeleted && x.AccountOwnerId == request.BranchId)
        //               .ToListAsync();

        //        var BranchInfo = request.BranchId != "XXXXXX" ? BranchList.Where(x => x.id.Equals(request.BranchId)).FirstOrDefault() : new APICaller.Helper.LoginModel.Authenthication.Branch();

        //        data = (from e in accountingEntries
        //                join a in accounts on e.AccountId equals a.Id
        //                group new { e, a } by new { e.AccountId, a.AccountName, a.AccountNumberCU, a.BeginningBalance, e.AccountCartegory } into g
        //                select new TrialBalance4ColumnDto
        //                {
        //                    EntityId = BranchInfo.id,
        //                    EntityType = "BRANCH",
        //                    FromDate = startDate,
        //                    ToDate = endDate,
        //                    BranchName = BranchInfo.name,
        //                    BranchAddress = BranchInfo.address,
        //                    Name = BranchInfo.bank.name,
        //                    Location = BranchInfo.bank.address,
        //                    Address = BranchInfo.bank.address,
        //                    Capital = BranchInfo.bank.capital,
        //                    ImmatriculationNumber = BranchInfo.bank.immatriculationNumber,
        //                    WebSite = BranchInfo.webSite,
        //                    BranchTelephone = BranchInfo.telephone,
        //                    HeadOfficeTelePhone = BranchInfo.bank.telephone,
        //                    AccountName = g.Key.AccountName,
        //                    Cartegory = Convert.ToString(g.Key.AccountCartegory),
        //                    AccountNumber = g.Key.AccountNumberCU,
        //                    BeginningBalance = Convert.ToDouble(g.Where(x => x.a.Id == g.Key.AccountId).Sum(x => x.a.BeginningBalance)),
        //                    DebitBalance = Convert.ToDouble(g.Where(x => x.e.AccountId == g.Key.AccountId).Sum(x => x.e.DrAmount)),
        //                    CreditBalance = Convert.ToDouble(g.Where(x => x.e.AccountId == g.Key.AccountId).Sum(x => x.e.CrAmount)),
        //                    EndingBalance = Convert.ToDouble(g.Key.BeginningBalance) + Convert.ToDouble(g.Where(x => x.e.AccountId == g.Key.AccountId).Sum(x => x.e.DrAmount)) - Convert.ToDouble(g.Where(x => x.e.AccountId == g.Key.AccountId).Sum(x => x.e.CrAmount)),
        //                    totalBeginningBalance = 0,
        //                    BeginningBookingDirection = Convert.ToDouble(g.Where(x => x.a.Id == g.Key.AccountId).Sum(x => x.a.BeginningBalanceCredit))> Convert.ToDouble(g.Where(x => x.a.Id == g.Key.AccountId).Sum(x => x.a.BeginningBalanceDebit))?"CR":"DR",
        //                    totalDebitBalance = 0,
        //                    totalCreditBalance = 0,
        //                    totalEndingBalance = 0//Convert.ToDouble(g.Key.BeginningBalance + g.Sum(x => x.e.DrAmount) - g.Sum(x => x.e.CrAmount))


        //                    //DebitBalance = Convert.ToDouble(g.Where(x => x.e.AccountId == g.Key.AccountId).Sum(x => x.e.DrAmount)),
        //                    //CreditBalance = Convert.ToDouble(g.Where(x => x.e.AccountId == g.Key.AccountId).Sum(x => x.e.CrAmount)),
        //                    //EndDebitBalance = accounts.Where(x => x.Id == g.Key.AccountId).Select(x => x.BeginningBalanceDebit).FirstOrDefault() == 0 ? 0 : Convert.ToDouble(g.Key.BeginningBalanceDebit + g.Sum(x => x.e.CrAmount) - g.Sum(x => x.e.DrAmount)),//g.Sum(x => x.a.BeginningBalanceCredit).ToString("#,##0.0"),//g.Key.AccountNumberCU  g.Key.BeginningBalanceDebit.ToString("#,##0.0"),                                                                                                                          //
        //                    //EndCreditBalance = accounts.Where(x => x.Id == g.Key.AccountId).Select(x => x.BeginningBalanceCredit).FirstOrDefault() == 0 ? 0 : Convert.ToDouble(g.Key.BeginningBalanceCredit + g.Sum(x => x.e.CrAmount) - g.Sum(x => x.e.DrAmount)),

        //                }).OrderBy(x => x.AccountNumber).ToList();
        //        #endregion
        //        trialBalances = SumTotalCalculation(data);

        //        //trialBalances = SumEndTotalCalculation(trialBalances.ToList());
        //        List<TrialBalance4ColumnDto> trial = new List<TrialBalance4ColumnDto>(); 
        //        foreach (var trialBalance in trialBalances)
        //        {

        //            trial.Add(trialBalance.ConvertTo4ColumnFromTBEndingBalnce(trialBalance));
        //        }
        //        trial= AdjustTrialBalanceCalculation(trial);
        //       errorMessage = $"Trial balance data retrieved successfully at the level of {request.SearchOption}.";
        //        await APICallHelper.AuditLogger(_userInfoToken.Email, "Get6ColumnTrialBalanceQuery",
        //        request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
        //        return ServiceResponse<List<TrialBalance4ColumnDto>>.ReturnResultWith200(trial);
        //    }
        //    catch (Exception e)
        //    {
        //        errorMessage = $"Trial balance data failed to retrieved successfully at the level of {request.SearchOption}.Error occurred while getting AccountingEntry: {e.Message}";

        //        // Log the error and return a 500 Internal Server Error response with the error message
        //        _logger.LogError(errorMessage);
        //        await APICallHelper.AuditLogger(_userInfoToken.Email, "Get6ColumnTrialBalanceQuery",
        //        request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
        //        return ServiceResponse<List<TrialBalance4ColumnDto>>.Return500(e);
        //    }
        //}




        public async Task<ServiceResponse<List<TrialBalance4ColumnDto>>> Handle(Get4ColumnTrialBalanceQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch branch list in advance to avoid repeated API calls
                var branchList = await APICallHelper.GetAllBranchInfos(_pathHelper, _userInfoToken);

                // Determine date range
                var startDate = request.FromDate.Date;
                var endDate = request.ToDate.Date;

                // Fetch accounts & entries in parallel
                var queryBranchId = request.BranchId != "XXXXXX" ? request.BranchId : null;

                var accountsTask = _accountRepository.All
                    .Where(a => !a.IsDeleted && (queryBranchId == null || a.AccountOwnerId == queryBranchId))
                    .ToListAsync();

                var entriesTask = _accountingEntryRepository.All
                    .Where(e => !e.IsDeleted && e.ValueDate.Date >= startDate && e.ValueDate.Date <= endDate
                        && (queryBranchId == null || e.BranchId == queryBranchId))
                    .ToListAsync();

                await Task.WhenAll(accountsTask, entriesTask);
                var accounts = accountsTask.Result;
                var accountingEntries = entriesTask.Result;

                // Get branch info if needed
                var branchInfo = request.BranchId != "XXXXXX"
                    ? branchList.FirstOrDefault(x => x.id.Equals(request.BranchId))
                    : new APICaller.Helper.LoginModel.Authenthication.Branch();

                // Group and compute balances
                var trialBalances = accountingEntries
                    .GroupBy(e => new { e.AccountId })
                    .Select(g =>
                    {
                        var account = accounts.FirstOrDefault(a => a.Id == g.Key.AccountId);
                        if (account == null) return null;

                        var beginningBalance = Convert.ToDouble(account.BeginningBalance);
                        var debitBalance = Convert.ToDouble(g.Sum(x => x.DrAmount));
                        var creditBalance = Convert.ToDouble(g.Sum(x => x.CrAmount));
                        var endingBalance = beginningBalance + debitBalance - creditBalance;

                        return new TrialBalance4ColumnDto
                        {
                            EntityId = branchInfo.id,
                            EntityType = "BRANCH",
                            FromDate = startDate,
                            ToDate = endDate,
                            BranchName = branchInfo.name,
                            BranchAddress = branchInfo.address,
                            Name = branchInfo.bank.name,
                            Location = branchInfo.bank.address,
                            Address = branchInfo.bank.address,
                            Capital = branchInfo.bank.capital,
                            ImmatriculationNumber = branchInfo.bank.immatriculationNumber,
                            WebSite = branchInfo.webSite,
                            BranchTelephone = branchInfo.telephone,
                            HeadOfficeTelePhone = branchInfo.bank.telephone,
                            AccountName = account.AccountName,
                            Cartegory = account.AccountCategoryId?.ToString(),
                            AccountNumber = account.AccountNumberCU,
                            BeginningBalance = beginningBalance,
                            DebitBalance = debitBalance,
                            CreditBalance = creditBalance,
                            EndingBalance = endingBalance,
                            BeginningBookingDirection = beginningBalance >= 0 ? "CR" : "DR",
                            totalBeginningBalance = 0,
                            totalDebitBalance = 0,
                            totalCreditBalance = 0,
                            totalEndingBalance = 0
                        };
                    })
                    .Where(dto => dto != null)
                    .OrderBy(x => x.AccountNumber)
                    .ToList();

                // Apply final total calculations
                trialBalances = SumTotalCalculation(trialBalances);
                trialBalances = AdjustTrialBalanceCalculation(trialBalances);

                // Logging success
                var successMessage = $"Trial balance data retrieved successfully for {request.SearchOption}.";
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.Get4ColumnTrialBalanceQuery, LogLevelInfo.Information);

                return ServiceResponse<List<TrialBalance4ColumnDto>>.ReturnResultWith200(trialBalances);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error retrieving trial balance data for {request.SearchOption}: {ex.Message}";

                // Log using your specified method
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.Get4ColumnTrialBalanceQuery, LogLevelInfo.Warning);

                return ServiceResponse<List<TrialBalance4ColumnDto>>.Return500(ex);
            }
        }

        private List<TrialBalance4ColumnDto> AdjustTrialBalanceCalculation(List<TrialBalance4ColumnDto> trialBalances)
        {
            List<TrialBalance4ColumnDto> listValue = new List<TrialBalance4ColumnDto>();
            try
            {
                int i = 0;
                foreach (var item in trialBalances)
                {
                  
                    decimal beginningBalance = decimal.TryParse(item.BeginningBalance.ToString(), out var bdb) ? bdb : 0;
                   decimal debit = decimal.TryParse(item.DebitBalance.ToString(), out var d) ? d : 0;
                    decimal credit = decimal.TryParse(item.CreditBalance.ToString(), out var c) ? c : 0;
                    i++;
                    var soldCr = Convert.ToDecimal(item.BeginningBalance.ToString()) + credit;
                    var soldDr = Convert.ToDecimal(item.BeginningBalance.ToString()) + debit;
                    decimal endingBalance = beginningBalance+ soldDr - soldCr;
                    if (item.AccountNumber.StartsWith("57102"))
                    {

                    }

                    if (endingBalance > 0)
                    {
                        //item.EndingBalance = Convert.ToDouble(endingBalance);
                        item.EndingBookingDirection = "DR";

                    }
                    else
                    {
                        //item.EndingBalance = Convert.ToDouble((Math.Abs(endingBalance)));
                        item.EndingBookingDirection = "CR";
                    }

                    listValue.Add(item);
                }
                return listValue;

            }
            catch (Exception ex)
            {

                throw (ex);
            };
        }


        private AccountManagement.Data.ChartOfAccount GetChartOfAccount(TrialBalance4ColumnDto item)
        {
            AccountManagement.Data.ChartOfAccount account = null;
            var model=   _chartOfAccountRepository.FindBy(pi=>pi.AccountNumber == item.AccountNumber);
            if (model.Any()) 
            {
                account = model.FirstOrDefault();
            }
            else
            {
                  model = _chartOfAccountRepository.FindBy(pi => pi.AccountNumber == item.AccountNumber.Substring(0, item.AccountNumber.Length - 1));
                if (model.Any())
                {
                    account = model.FirstOrDefault();
                }
                
            }
            return account;
        }

        private List<TrialBalance4ColumnDto> GroupAllBranchAccountBalace(List<TrialBalance4ColumnDto> collection)
        {
            var models = collection.ToList();
            List < TrialBalance4ColumnDto > resultSet = new List<TrialBalance4ColumnDto >();
            foreach (var item in collection)
            {
                var trialBalance = models.Where(x => x.AccountNumber == item.AccountNumber);
                 var model = sumAllAccountperAccountNumber(trialBalance,item);
                if (CheckIfElementIsNotInTheList(item, resultSet))
                {
                    resultSet.Add(model);
                }
               

            }
            return resultSet;
        }

        private bool CheckIfElementIsNotInTheList(TrialBalance4ColumnDto item, List<TrialBalance4ColumnDto> resultSet)
        {
            return resultSet.Where(c=>c.AccountNumber == item.AccountNumber).Any()==false;
        }

        private TrialBalance4ColumnDto sumAllAccountperAccountNumber(IEnumerable<TrialBalance4ColumnDto> trialBalance, TrialBalance4ColumnDto item)
        {
            var BeginningBalance = trialBalance.Sum(c => Convert.ToDouble(c.BeginningBalance));
            var DebitBalance = trialBalance.Sum(c => Convert.ToDouble(c.DebitBalance));
            var CreditBalance = trialBalance.Sum(c => Convert.ToDouble(c.CreditBalance));
            var EndingBalance = trialBalance.Sum(c => Convert.ToDouble(c.EndingBalance));

            item.BeginningBalance = BeginningBalance;
            item.DebitBalance = DebitBalance;
            item.CreditBalance = CreditBalance;
            item.EndingBalance = EndingBalance;
            return item;
        }
    

        private List<TrialBalance4ColumnDto> SumTotalCalculation(List<TrialBalance4ColumnDto> trialBalances)
        {
            List<TrialBalance4ColumnDto> listValue = new List<TrialBalance4ColumnDto>();
            //double totalBeginningBalance = trialBalances.Where(x=>x.BeginningBookingDirection.Equals("CR")).Sum(tb => tb.BeginningBalance);
            double totalBeginningBalance = trialBalances.Sum(tb => tb.BeginningBalance);
            double totalDebitBalance = trialBalances.Sum(tb => tb.DebitBalance);
            double totalCreditBalance = trialBalances.Sum(tb => tb.CreditBalance);
            double totalEndingBalance = trialBalances.Sum(tb => tb.EndingBalance);
           
            foreach (var item in trialBalances)
            {
                if (item.AccountNumber.StartsWith("571"))
                {

                }
                item.totalDebitBalance = totalDebitBalance;
                item.totalCreditBalance = totalCreditBalance;
                item.totalBeginningBalance = totalBeginningBalance;
                item.totalEndingBalance = totalEndingBalance;
                listValue.Add(item);
            }
            return listValue;
        }


        private List<TrialBalance4ColumnDto> SumEndTotalCalculation(List<TrialBalance4ColumnDto> trialBalances)
        {
            List<TrialBalance4ColumnDto> listValue = new List<TrialBalance4ColumnDto>();

            double totalEndingBalance = trialBalances.Sum(tb => tb.EndingBalance);
            double totalBeginningBalance = trialBalances.Sum(tb => tb.BeginningBalance);
            double totalCreditBalance = trialBalances.Sum(tb => tb.CreditBalance);
            double totalDebitBalance = trialBalances.Sum(tb => tb.DebitBalance);
            foreach (var item in trialBalances)
            {

                item.totalDebitBalance = totalDebitBalance;
                item.totalCreditBalance = totalCreditBalance;
                item.totalBeginningBalance = totalBeginningBalance;
                item.totalEndingBalance = totalEndingBalance;



                listValue.Add(item);
            }
            return listValue;
        }
    }

}