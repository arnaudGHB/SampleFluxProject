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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CBS.AccountingManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific AccountingEntry based on its unique identifier.
    /// </summary>
    public class Get6ColumnTrialBalanceQueryHandler : IRequestHandler<Get6ColumnTrialBalanceQuery, ServiceResponse<List<TrialBalance6ColumnDto>>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository; // Repository for accessing AccountingEntry data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<Get6ColumnTrialBalanceQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountingEntriesServices _accountingEntriesServices;
        private readonly ITrialBalanceRepository? _trialBalanceRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly AccountManagement.Helper.PathHelper _pathHelper;
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountRepository;

        /// <summary>
        /// Constructor for initializing the GetAccountingEntryQueryHandler.
        /// </summary>
        /// <param name="AccountingEntryRepository">Repository for AccountingEntry data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public Get6ColumnTrialBalanceQueryHandler(
            IAccountRepository accountRepository,
            IChartOfAccountManagementPositionRepository chartOfAccountRepository,
            IAccountingEntryRepository accountingEntryRepository,
            IConfiguration configuration,
            IMapper mapper,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken,
             ILogger<Get6ColumnTrialBalanceQueryHandler> logger, IAccountingEntriesServices accountingEntriesServices, ITrialBalanceRepository? trialBalanceRepository)
        {
            _accountingEntryRepository = accountingEntryRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _chartOfAccountRepository = chartOfAccountRepository;
            _userInfoToken = userInfoToken;
            _accountRepository = accountRepository;
            _accountingEntriesServices = accountingEntriesServices;
            _trialBalanceRepository = trialBalanceRepository;
            _pathHelper = new AccountManagement.Helper.PathHelper(configuration);
        }

        /// <summary>
        /// Handles the Get6ColumnTrialBalanceQuery to retrieve a specific AccountingEntry.
        /// </summary>
        /// <param name="request">The GetAccountingEntryQuery containing AccountingEntry ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        //public async Task<ServiceResponse<List<TrialBalance6ColumnDto>>> Handle(Get6ColumnTrialBalanceQuery request, CancellationToken cancellationToken)
        //{
        //    List<AccountingEntry> accountingEntries = new();
        //    List<Account> accounts = new();
        //    List<TrialBalance6ColumnDto> trialBalances = new();
        //    var data = new List<TrialBalance6ColumnDto>();
        //    APICaller.Helper.LoginModel.Authenthication.Branch BranchInfo;
        //    string errorMessage = null;
        //    try
        //    {


        //        accountingEntries = request.BranchId == "XXXXXX" ? await _accountingEntryRepository.All
        //                                                               .Where(x => x.IsDeleted == false)
        //                                                               .Where(x => x.ValueDate.Date >= request.FromDate.Date)
        //                                                               .Where(x => x.ValueDate.Date <= request.ToDate.Date).ToListAsync() :
        //                                                            await _accountingEntryRepository.All
        //                                                               .Where(x => x.IsDeleted == false)
        //                                                               .Where(x => x.ValueDate.Date >= request.FromDate.Date)
        //                                                               .Where(x => x.ValueDate.Date <= request.ToDate.Date)
        //                                                               .Where(x => x.BranchId == request.BranchId)
        //                                                               .ToListAsync();

        //        accounts = request.BranchId == "XXXXXX" ? await _accountRepository.FindBy(x => x.IsDeleted == false).ToListAsync() :
        //            await _accountRepository.FindBy(x => x.IsDeleted == false && x.AccountOwnerId == request.BranchId).ToListAsync();

        //        _logger.LogInformation(accountingEntries.Count().ToString());
        //        if (request.BranchId != "XXXXXX")
        //        {
        //            var Branches = await APICallHelper.GetAllBranchInfos(_pathHelper, _userInfoToken);
        //            BranchInfo = Branches.Find(x => x.id == request.BranchId);
        //            data = (from e in accountingEntries
        //                    join a in accounts on e.AccountId equals a.Id
        //                    group new { e, a } by new { e.AccountId, a.AccountName, a.AccountNumberCU, a.BeginningBalance, a.BeginningBalanceDebit, a.BeginningBalanceCredit, e.AccountCartegory } into g
        //                    select new TrialBalance6ColumnDto
        //                    {
        //                        EntityId = BranchInfo.id,
        //                        EntityType = "BRANCH",
        //                        FromDate = request.FromDate,
        //                        ToDate = request.ToDate,
        //                        BranchName = BranchInfo.name,
        //                        BranchAddress = BranchInfo.address,
        //                        Name = BranchInfo.bank.name,
        //                        Location = BranchInfo.bank.address,
        //                        Address = BranchInfo.bank.address,
        //                        Capital = BranchInfo.bank.capital,
        //                        ImmatriculationNumber = BranchInfo.bank.immatriculationNumber,
        //                        WebSite = BranchInfo.webSite,
        //                        BranchTelephone = BranchInfo.telephone,
        //                        HeadOfficeTelePhone = BranchInfo.bank.telephone,
        //                        BranchCode = BranchInfo.branchCode,
        //                        AccountName = g.Key.AccountName,
        //                        Cartegory = g.Key.AccountCartegory,
        //                        AccountNumber = g.Key.AccountNumberCU,
        //                        BeginningCreditBalance = Convert.ToDouble(accounts.Where(x => x.Id == g.Key.AccountId).Select(x => x.BeginningBalanceCredit).FirstOrDefault()),//g.Sum(x => x.a.BeginningBalanceCredit).ToString("#,##0.0"),//g.Key.AccountNumberCU  g.Key.BeginningBalanceDebit.ToString("#,##0.0"),
        //                        BeginningDebitBalance = Convert.ToDouble(accounts.Where(x => x.Id == g.Key.AccountId).Select(x => x.BeginningBalanceDebit).FirstOrDefault()),//g.Sum(x => x.a.BeginningBalanceCredit).ToString("#,##0.0"),//g.Key.AccountNumberCU  g.Key.BeginningBalanceDebit.ToString("#,##0.0"),
        //                        DebitBalance = Convert.ToDouble(g.Where(x => x.e.AccountId == g.Key.AccountId).Sum(x => x.e.DrAmount)),
        //                        CreditBalance = Convert.ToDouble(g.Where(x => x.e.AccountId == g.Key.AccountId).Sum(x => x.e.CrAmount)),
        //                        EndDebitBalance = accounts.Where(x => x.Id == g.Key.AccountId).Select(x => x.BeginningBalanceDebit).FirstOrDefault() == 0 ? 0 : Convert.ToDouble(g.Key.BeginningBalanceDebit + g.Sum(x => x.e.CrAmount) - g.Sum(x => x.e.DrAmount)),//g.Sum(x => x.a.BeginningBalanceCredit).ToString("#,##0.0"),//g.Key.AccountNumberCU  g.Key.BeginningBalanceDebit.ToString("#,##0.0"),                                                                                                                          //
        //                        EndCreditBalance = accounts.Where(x => x.Id == g.Key.AccountId).Select(x => x.BeginningBalanceCredit).FirstOrDefault() == 0 ? 0 : Convert.ToDouble(g.Key.BeginningBalanceCredit + g.Sum(x => x.e.CrAmount) - g.Sum(x => x.e.DrAmount)),
        //                    }).ToList();


        //            trialBalances = AdjustTrialBalanceCalculation(data.ToList());
        //        }
        //        else
        //        {
        //            var Branches = await APICallHelper.GetAllBranchInfos(_pathHelper, _userInfoToken);
        //            BranchInfo = Branches.Find(x => x.id == _userInfoToken.BranchId);
        //            data = (from e in accountingEntries
        //                    join a in accounts on e.AccountId equals a.Id
        //                    group new { e, a } by new { e.AccountId, a.AccountName, a.AccountNumberReference, a.BeginningBalance, a.BeginningBalanceDebit, a.BeginningBalanceCredit, e.AccountCartegory } into g
        //                    select new TrialBalance6ColumnDto
        //                    {
        //                        EntityId = BranchInfo.id,
        //                        EntityType = "BRANCH",
        //                        FromDate = request.FromDate,
        //                        ToDate = request.ToDate,
        //                        BranchName = BranchInfo.name,
        //                        BranchAddress = BranchInfo.address,
        //                        Name = BranchInfo.bank.name,
        //                        Location = BranchInfo.bank.address,
        //                        Address = BranchInfo.bank.address,
        //                        Capital = BranchInfo.bank.capital,
        //                        ImmatriculationNumber = BranchInfo.bank.immatriculationNumber,
        //                        WebSite = BranchInfo.webSite,
        //                        BranchTelephone = BranchInfo.telephone,
        //                        HeadOfficeTelePhone = BranchInfo.bank.telephone,
        //                        BranchCode = BranchInfo.branchCode,
        //                        AccountName = g.Key.AccountName,
        //                        Cartegory = g.Key.AccountCartegory,
        //                        AccountNumber = g.Key.AccountNumberReference,
        //                        BeginningCreditBalance = Convert.ToDouble(accounts.Where(x => x.Id == g.Key.AccountId).Select(x => x.BeginningBalanceCredit).FirstOrDefault()),//g.Sum(x => x.a.BeginningBalanceCredit).ToString("#,##0.0"),//g.Key.AccountNumberCU  g.Key.BeginningBalanceDebit.ToString("#,##0.0"),
        //                        BeginningDebitBalance = Convert.ToDouble(accounts.Where(x => x.Id == g.Key.AccountId).Select(x => x.BeginningBalanceDebit).FirstOrDefault()),//g.Sum(x => x.a.BeginningBalanceCredit).ToString("#,##0.0"),//g.Key.AccountNumberCU  g.Key.BeginningBalanceDebit.ToString("#,##0.0"),
        //                        DebitBalance = Convert.ToDouble(g.Where(x => x.e.AccountId == g.Key.AccountId).Sum(x => x.e.DrAmount)),
        //                        CreditBalance = Convert.ToDouble(g.Where(x => x.e.AccountId == g.Key.AccountId).Sum(x => x.e.CrAmount)),

        //                        EndDebitBalance = accounts.Where(x => x.Id == g.Key.AccountId).Select(x => x.BeginningBalanceDebit).FirstOrDefault() == 0 ? 0 : Convert.ToDouble((g.Key.BeginningBalanceDebit + g.Sum(x => x.e.CrAmount) - g.Sum(x => x.e.DrAmount))),//g.Sum(x => x.a.BeginningBalanceCredit).ToString("#,##0.0"),//g.Key.AccountNumberCU  g.Key.BeginningBalanceDebit.ToString("#,##0.0"),                                                                                                                          //
        //                        EndCreditBalance = accounts.Where(x => x.Id == g.Key.AccountId).Select(x => x.BeginningBalanceCredit).FirstOrDefault() == 0 ? 0 : Convert.ToDouble((g.Key.BeginningBalanceCredit + g.Sum(x => x.e.CrAmount) - g.Sum(x => x.e.DrAmount))),
        //                    }).ToList();

        //            var groupedResults = data.GroupBy(tb => tb.AccountNumber)
        //                               .Select(group => new TrialBalance6ColumnDto
        //                               {
        //                                   AccountNumber = group.Key.PadRight(6, '0'),
        //                                   AccountName = group.First().AccountName,
        //                                   BeginningDebitBalance = Convert.ToDouble(group.Sum(tb => double.TryParse(tb.BeginningDebitBalance.ToString(), out double bdb) ? bdb : 0)),
        //                                   BeginningCreditBalance = Convert.ToDouble(group.Sum(tb => decimal.TryParse(tb.BeginningCreditBalance.ToString(), out decimal bcb) ? bcb : 0)),
        //                                   DebitBalance = Convert.ToDouble(group.Sum(tb => decimal.TryParse(tb.DebitBalance.ToString(), out decimal db) ? db : 0)),
        //                                   CreditBalance = Convert.ToDouble(group.Sum(tb => decimal.TryParse(tb.CreditBalance.ToString(), out decimal cb) ? cb : 0)),
        //                                   EndDebitBalance = Convert.ToDouble(group.Sum(tb => decimal.TryParse(tb.EndDebitBalance.ToString(), out decimal edb) ? edb : 0)),
        //                                   EndCreditBalance = Convert.ToDouble(group.Sum(tb => decimal.TryParse(tb.EndCreditBalance.ToString(), out decimal ecb) ? ecb : 0)),
        //                                   // Copy other non-summed fields if needed
        //                                   EntityId = group.First().EntityId,
        //                                   EntityType = "HEAD OFFICE",
        //                                   FromDate = group.First().FromDate,
        //                                   ToDate = group.First().ToDate,

        //                                   BranchCode = BranchInfo.branchCode,
        //                                   BranchName = BranchInfo.name,
        //                                   BranchAddress = BranchInfo.address,
        //                                   Name = BranchInfo.bank.name,
        //                                   Location = BranchInfo.bank.address,
        //                                   Address = BranchInfo.bank.address,
        //                                   Capital = BranchInfo.bank.capital,
        //                                   ImmatriculationNumber = BranchInfo.bank.immatriculationNumber,
        //                                   WebSite = BranchInfo.webSite,
        //                                   BranchTelephone = BranchInfo.telephone,
        //                                   HeadOfficeTelePhone = BranchInfo.bank.telephone,
        //                               })
        //                                .ToList();
        //            trialBalances = AdjustTrialBalanceCalculation(groupedResults.ToList());
        //        }



        //       trialBalances = SumTotalCalculation(trialBalances);

        //    trialBalances = SumEndTotalCalculation(trialBalances.ToList());
        //        List<TrialBalance> trialBalances1 = new List<TrialBalance>();
        //        foreach (var item in trialBalances)
        //        {
        //            trialBalances1.Add(item.ConvertToReportedTrialBalance());
        //        }
        //        List<TrialBalance> existingTrialBalance = null;

        //        existingTrialBalance = _trialBalanceRepository.All.Where(x => x.BranchId == request.BranchId).ToList();

        //        if (existingTrialBalance.Count() > 0)
        //        {
        //            _trialBalanceRepository.RemoveRange(existingTrialBalance);
        //            trialBalances1 = request.BranchId != "XXXXXX" ? InitialisedExpectedBranchId(trialBalances1, request.BranchId) : trialBalances1;
        //           _trialBalanceRepository.AddRange(trialBalances1);
        //        }
        //        else
        //        {
        //            trialBalances1 = request.BranchId != "XXXXXX" ? InitialisedExpectedBranchId(trialBalances1, request.BranchId) : trialBalances1;

        //            _trialBalanceRepository.AddRange(trialBalances1);
        //        }

        //        await _uow.SaveAsyncWithOutAffectingBranchId();
        //        errorMessage = $"Trial balance data retrieved successfully at the level of {request.SearchOption}.";
        //        await APICallHelper.AuditLogger(_userInfoToken.Email, "Get6ColumnTrialBalanceQuery",
        //        request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
        //        return ServiceResponse<List<TrialBalance6ColumnDto>>.ReturnResultWith200(trialBalances.OrderBy(x => x.AccountNumber).ToList());
        //    }
        //    catch (Exception e)
        //    {
        //        errorMessage = $"Trial balance data failed to retrieved successfully at the level of {request.SearchOption}.Error occurred while getting AccountingEntry: {e.Message}";

        //        // Log the error and return a 500 Internal Server Error response with the error message
        //        _logger.LogError(errorMessage);
        //        await APICallHelper.AuditLogger(_userInfoToken.Email, "Get6ColumnTrialBalanceQuery",
        //        request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
        //        return ServiceResponse<List<TrialBalance6ColumnDto>>.Return500(e);
        //    }
        //}


        public async Task<ServiceResponse<List<TrialBalance6ColumnDto>>> Handle(Get6ColumnTrialBalanceQuery request, CancellationToken cancellationToken)
        {
            List<AccountingEntry> accountingEntries = new();
            List<Account> accounts = new();
            List<TrialBalance6ColumnDto> trialBalances = new();
            var data = new List<TrialBalance6ColumnDto>();
            APICaller.Helper.LoginModel.Authenthication.Branch BranchInfo;
            string errorMessage = null;

            try
            {
                // Fetch accounting entries
                accountingEntries = request.BranchId == "XXXXXX"
                    ? await _accountingEntryRepository.All
                        .Where(x => !x.IsDeleted && x.ValueDate.Date >= request.FromDate.Date && x.ValueDate.Date <= request.ToDate.Date)
                        .ToListAsync()
                    : await _accountingEntryRepository.All
                        .Where(x => !x.IsDeleted && x.ValueDate.Date >= request.FromDate.Date && x.ValueDate.Date <= request.ToDate.Date &&  x.ExternalBranchId == request.BranchId || x.BranchId == request.BranchId)
                        .ToListAsync();

                // Fetch account details
                accounts = request.BranchId == "XXXXXX"
                    ? await _accountRepository.FindBy(x => !x.IsDeleted).ToListAsync()
                    : await _accountRepository.FindBy(x => !x.IsDeleted && x.AccountOwnerId == request.BranchId).ToListAsync();

                _logger.LogInformation($"Accounting entries count: {accountingEntries.Count()}");

                // Fetch branch information
                var branches = await APICallHelper.GetAllBranchInfos(_pathHelper, _userInfoToken);
                BranchInfo = request.BranchId != "XXXXXX"
                    ? branches.Find(x => x.id == request.BranchId)
                    : branches.Find(x => x.id == _userInfoToken.BranchId);

                // Group and calculate trial balances
                data = (from e in accountingEntries
                        join a in accounts on e.AccountId equals a.Id
                        group new { e, a } by new
                        {
                            e.AccountId,
                            a.AccountName,
                            AccountNumber = request.BranchId == "XXXXXX" ? a.AccountNumberReference : a.AccountNumberCU,
                            a.BeginningBalance,
                            a.BeginningBalanceDebit,
                            a.BeginningBalanceCredit,
                            e.AccountCartegory
                        } into g
                        select new TrialBalance6ColumnDto
                        {
                            EntityId = BranchInfo.id,
                            EntityType = request.BranchId == "XXXXXX" ? "HEAD OFFICE" : "BRANCH",
                            FromDate = request.FromDate,
                            ToDate = request.ToDate,
                            BranchName = BranchInfo.name,
                            BranchAddress = BranchInfo.address,
                            Name = BranchInfo.bank.name,
                            Location = BranchInfo.bank.address,
                            Address = BranchInfo.bank.address,
                            Capital = BranchInfo.bank.capital,
                            ImmatriculationNumber = BranchInfo.bank.immatriculationNumber,
                            WebSite = BranchInfo.webSite,
                            BranchTelephone = BranchInfo.telephone,
                            HeadOfficeTelePhone = BranchInfo.bank.telephone,
                            BranchCode = BranchInfo.branchCode,
                            AccountName = g.Key.AccountName,
                            Cartegory = g.Key.AccountCartegory,
                            AccountNumber = g.Key.AccountNumber,
                            BeginningCreditBalance = Convert.ToDouble(accounts.Where(x => x.Id == g.Key.AccountId).Select(x => x.BeginningBalanceCredit).FirstOrDefault()),
                            BeginningDebitBalance = Convert.ToDouble(accounts.Where(x => x.Id == g.Key.AccountId).Select(x => x.BeginningBalanceDebit).FirstOrDefault()),
                            DebitBalance = Convert.ToDouble(g.Sum(x => x.e.DrAmount)),
                            CreditBalance = Convert.ToDouble(g.Sum(x => x.e.CrAmount)),
                            EndDebitBalance = accounts.Where(x => x.Id == g.Key.AccountId).Select(x => x.BeginningBalanceDebit).FirstOrDefault() == 0
                                ? 0
                                : Convert.ToDouble(g.Key.BeginningBalanceDebit + g.Sum(x => x.e.CrAmount) - g.Sum(x => x.e.DrAmount)),
                            EndCreditBalance = accounts.Where(x => x.Id == g.Key.AccountId).Select(x => x.BeginningBalanceCredit).FirstOrDefault() == 0
                                ? 0
                                : Convert.ToDouble(g.Key.BeginningBalanceCredit + g.Sum(x => x.e.CrAmount) - g.Sum(x => x.e.DrAmount)),
                        }).ToList();

                // Apply calculations
                trialBalances = AdjustTrialBalanceCalculation(data.ToList());
                trialBalances = SumTotalCalculation(trialBalances);
                trialBalances = SumEndTotalCalculation(trialBalances.ToList());

                // Convert to ReportedTrialBalance
                var trialBalances1 = trialBalances.Select(tb => tb.ConvertToReportedTrialBalance()).ToList();

                // Remove existing trial balances and save new ones
                var existingTrialBalance = _trialBalanceRepository.All.Where(x => x.BranchId == request.BranchId).ToList();
                if (existingTrialBalance.Count() > 0)
                {
                    _trialBalanceRepository.RemoveRange(existingTrialBalance);
                }

                trialBalances1 = request.BranchId != "XXXXXX" ? InitialisedExpectedBranchId(trialBalances1, request.BranchId) : trialBalances1;
                _trialBalanceRepository.AddRange(trialBalances1);
                await _uow.SaveAsyncWithOutAffectingBranchId();

                // Logging success
                errorMessage = $"Trial balance data retrieved successfully at the level of {request.SearchOption}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.OK, LogAction.Get6ColumnTrialBalanceQuery, LogLevelInfo.Information);

                return ServiceResponse<List<TrialBalance6ColumnDto>>.ReturnResultWith200(trialBalances.OrderBy(x => x.AccountNumber).ToList());
            }
            catch (Exception e)
            {
                // Log error
                errorMessage = $"Trial balance data retrieval failed at the level of {request.SearchOption}. Error: {e.Message}";
                _logger.LogError(errorMessage);

                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.Get6ColumnTrialBalanceQuery, LogLevelInfo.Warning);

                return ServiceResponse<List<TrialBalance6ColumnDto>>.Return500(e);
            }
        }


        private List<TrialBalance> InitialisedExpectedBranchId(List<TrialBalance> trialBalances1, string branchId)
        {
            List<TrialBalance> trials = new List<TrialBalance>();
            foreach (var item in trialBalances1)
            {
                item.BranchId = branchId;
                trials.Add(item);
            }
            return trials;
        }

        private List<TrialBalance6ColumnDto> SumTotalCalculation(List<TrialBalance6ColumnDto> trialBalances)
        {
            List<TrialBalance6ColumnDto> listValue = new List<TrialBalance6ColumnDto>();
            double totalBeginningDebitBalance = Convert.ToDouble(trialBalances.Sum(tb => double.Parse(tb.BeginningDebitBalance.ToString() ?? "0.0")));
            double totalBeginningCreditBalance = Convert.ToDouble(trialBalances.Sum(tb => decimal.Parse(tb.BeginningCreditBalance.ToString() ?? "0.0")));
            double totalDebitBalance = Convert.ToDouble(trialBalances.Sum(tb => decimal.Parse(tb.DebitBalance.ToString() ?? "0.0")));
            double totalCreditBalance = Convert.ToDouble(trialBalances.Sum(tb => decimal.Parse(tb.CreditBalance.ToString() ?? "0.0")));
            double totalEndingDebitBalance = Convert.ToDouble(trialBalances.Sum(tb => decimal.Parse(tb.EndDebitBalance.ToString() ?? "0.0")));
            double totalEndingCreditBalance = Convert.ToDouble(trialBalances.Sum(tb => decimal.Parse(tb.EndCreditBalance.ToString() ?? "0.0")));
            foreach (var item in trialBalances)
            {
                item.TotalBeginningCreditBalance = totalBeginningCreditBalance.ToString();
                item.TotalDebitBalance = totalDebitBalance.ToString();
                item.TotalCreditBalance = totalCreditBalance.ToString();
                item.TotalBeginningDebitBalance = totalBeginningDebitBalance.ToString();
                item.TotalEndCreditBalance = totalEndingCreditBalance.ToString();
                item.TotalEndDebitBalance = totalEndingDebitBalance.ToString("#,##0.0");
                if (item.AccountNumber.StartsWith("7"))
                {
                    if (Convert.ToDouble(item.EndDebitBalance) < 0)
                    {
                        item.EndCreditBalance = (-1 * Convert.ToDouble(item.EndDebitBalance));
                        item.EndDebitBalance = 0;
                    }

                }
                else if (item.AccountNumber.StartsWith("5"))
                {
                    if (Convert.ToDouble(item.EndDebitBalance) < 0)
                    {
                        item.EndCreditBalance = -1 * Convert.ToDouble(item.EndDebitBalance);
                        item.EndDebitBalance = 0;
                    }
                }
                else if (item.AccountNumber.StartsWith("4"))
                {
                    if (Convert.ToDouble(item.EndDebitBalance) < 0)
                    {
                        item.EndCreditBalance = -1 * Convert.ToDouble(item.EndDebitBalance);
                        item.EndDebitBalance = 0;
                    }
                }
                else if (item.AccountNumber.StartsWith("3"))
                {
                    if (Convert.ToDouble(item.EndDebitBalance) < 0)
                    {
                        item.EndCreditBalance = -1 * Convert.ToDouble(item.EndDebitBalance);
                        item.EndDebitBalance = 0;
                    }
                }
                else if (item.AccountNumber.StartsWith("6"))
                {
                    if (Convert.ToDouble(item.EndDebitBalance) < 0)
                    {
                        item.EndDebitBalance = -1 * Convert.ToDouble(item.EndDebitBalance);
                        item.EndCreditBalance = 0;
                    }
                }
                else if (item.AccountNumber.StartsWith("2"))
                {
                    if (Convert.ToDouble(item.EndDebitBalance) < 0)
                    {
                        item.EndDebitBalance = -1 * Convert.ToDouble(item.EndDebitBalance);
                        item.EndCreditBalance = 0;
                    }
                }
                else if (item.AccountNumber.StartsWith("1"))
                {
                    if (Convert.ToDouble(item.EndDebitBalance) < 0)
                    {
                        item.EndDebitBalance = -1 * Convert.ToDouble(item.EndDebitBalance);
                        item.EndCreditBalance = 0;
                    }
                }




                listValue.Add(item);
            }
            return listValue;
        }
        private List<TrialBalance6ColumnDto> AdjustTrialBalanceCalculation(List<TrialBalance6ColumnDto> trialBalances)
        {
            List<TrialBalance6ColumnDto> listValue = new List<TrialBalance6ColumnDto>();
            try
            {
                int i = 0;
                foreach (var item in trialBalances)
                {
                    decimal beginningDebitBalance = decimal.TryParse(item.BeginningDebitBalance.ToString(), out var bdb) ? bdb : 0;
                    decimal beginningCreditBalance = decimal.TryParse(item.BeginningCreditBalance.ToString(), out var bcb) ? bcb : 0;
                    decimal debit = decimal.TryParse(item.DebitBalance.ToString(), out var d) ? d : 0;
                    decimal credit = decimal.TryParse(item.CreditBalance.ToString(), out var c) ? c : 0;
                    i++;
                    var soldCr = beginningCreditBalance + credit;
                    var soldDr = beginningDebitBalance + debit;
                    decimal endingBalance = soldDr - soldCr;
                    if (endingBalance > 0)
                    {
                        item.EndDebitBalance = Convert.ToDouble(endingBalance);
                        item.EndCreditBalance = 0;

                    }
                    else
                    {
                        item.EndCreditBalance = Convert.ToDouble((Math.Abs(endingBalance)));
                        item.EndDebitBalance = 0;
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


        private List<TrialBalance6ColumnDto> SumEndTotalCalculation(List<TrialBalance6ColumnDto> trialBalances)
        {
            List<TrialBalance6ColumnDto> listValue = new List<TrialBalance6ColumnDto>();

            decimal totalEndingDebitBalance = trialBalances.Sum(tb => decimal.Parse(tb.EndDebitBalance.ToString() ?? "0"));
            decimal totalEndingCreditBalance = trialBalances.Sum(tb => decimal.Parse(tb.EndCreditBalance.ToString() ?? "0"));
            foreach (var item in trialBalances)
            {

                item.TotalEndCreditBalance = totalEndingCreditBalance.ToString("#,##0.0");
                item.TotalEndDebitBalance = totalEndingDebitBalance.ToString("#,##0.0");
                if (item.AccountNumber.StartsWith("7"))
                {
                    if (Convert.ToDouble(item.EndDebitBalance) < 0)
                    {
                        item.EndCreditBalance = -1 * Convert.ToDouble(item.EndDebitBalance);
                        item.EndDebitBalance = 0;
                    }

                }



                listValue.Add(item);
            }
            return listValue;
        }
    }
}