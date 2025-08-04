using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Helper;

using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.IO;
using System.Net;

namespace CBS.AccountingEntryManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific AccountingEntry based on its unique identifier.
    /// </summary>
    public class GetEndOfDayDataQueryHandler : IRequestHandler<GetEndOfDayDataQuery, ServiceResponse<CloseOfDayData>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository; // Repository for accessing AccountingEntry data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetEndOfDayDataQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly PathHelper _pathHelper;
        private readonly UserInfoToken _userInfoToken;
        private readonly IAccountRepository _accountRepository;
        /// <summary>
        /// Constructor for initializing the GetAccountingEntryQueryHandler
        /// </summary>
        /// <param name="AccountingEntryRepository">Repository for AccountingEntry data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetEndOfDayDataQueryHandler(
            IAccountingEntryRepository AccountingEntryRepository,
            IMapper mapper,
            ILogger<GetEndOfDayDataQueryHandler> logger,
            PathHelper? pathHelper,
            UserInfoToken? userInfoToken,
            IAccountRepository? accountRepository)
        {
            _accountRepository = accountRepository;
            _accountingEntryRepository = AccountingEntryRepository;
            _mapper = mapper;
            _logger = logger;
            _pathHelper = pathHelper;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetAccountingEntryQuery to retrieve a specific AccountingEntry.
        /// </summary>
        /// <param name="request">The GetAccountingEntryQuery containing AccountingEntry ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CloseOfDayData>> Handle(GetEndOfDayDataQuery request, CancellationToken cancellationToken)
        {
            CloseOfDayData model = new CloseOfDayData();
            string errorMessage = null;

            try
            {
                CloseOfDayReconciliation reconciliation = Newtonsoft.Json.JsonConvert.DeserializeObject<CloseOfDayReconciliation>(_pathHelper.EndOfDayActivity);



                if (reconciliation == null)
                {
                    List<EndOfDayData> Entries = new List<EndOfDayData>();

                    EndOfDayData endOfDayData = new EndOfDayData();
                    //Retrieve CashInHand Info
                    if (reconciliation.CashInHand != null)
                    {
                        var account = await _accountRepository.FindBy(x => x.Account5.Equals(reconciliation.CashInHand) && x.AccountOwnerId == _userInfoToken.BranchId).FirstOrDefaultAsync();
                        var entities = await _accountingEntryRepository.All.Where(x => (x.DrAccountId == account.Id || x.CrAccountId == account.Id || x.CrAccountId == account.Id) && (x.BranchId == _userInfoToken.BranchId || x.ExternalBranchId == _userInfoToken.BranchId) && x.IsDeleted == false && x.EntryDate.Date >= BaseUtilities.UtcToLocal().AddDays(-1) && x.EntryDate.Date <= BaseUtilities.UtcToLocal()).ToListAsync();
                        var firstEntry = entities.OrderBy(x => x.EntryDate).FirstOrDefault();
                        endOfDayData.EndingBalance = account.CurrentBalance;
                        endOfDayData.DebitBalance = account.DebitBalance;
                        endOfDayData.CreditBalance = account.CreditBalance;
                        endOfDayData.BeginningBalance = firstEntry.CurrentBalance;
                        endOfDayData.Entries = _mapper.Map<List<AccountingEntryDto>>(entities.ToList());

                    }

                    if (reconciliation.CashInVault != null)
                    {
                        var account = await _accountRepository.FindBy(x => x.Account5.Equals(reconciliation.CashInVault) && x.AccountOwnerId == _userInfoToken.BranchId).FirstOrDefaultAsync();
                        var entities = await _accountingEntryRepository.All.Where(x => (x.DrAccountId == account.Id || x.CrAccountId == account.Id || x.CrAccountId == account.Id) && (x.BranchId == _userInfoToken.BranchId || x.ExternalBranchId == _userInfoToken.BranchId) && x.IsDeleted == false && x.EntryDate.Date >= BaseUtilities.UtcToLocal().AddDays(-1) && x.EntryDate.Date <= BaseUtilities.UtcToLocal()).ToListAsync();
                        var firstEntry = entities.OrderBy(x => x.EntryDate).FirstOrDefault();
                        endOfDayData.EndingBalance = account.CurrentBalance;
                        endOfDayData.DebitBalance = account.DebitBalance;
                        endOfDayData.CreditBalance = account.CreditBalance;
                        endOfDayData.BeginningBalance = firstEntry.CurrentBalance;
                        endOfDayData.Entries = _mapper.Map<List<AccountingEntryDto>>(entities.ToList());
                    }
                    //Retrieve CashInVault Info
                    if (reconciliation.MobileMoneyMTN != null)
                    {
                        var account = await _accountRepository.FindBy(x => x.AccountNumberReference.Equals(reconciliation.MobileMoneyMTN) && x.AccountOwnerId == _userInfoToken.BranchId).FirstOrDefaultAsync();
                        var entities = await _accountingEntryRepository.All.Where(x => (x.DrAccountId == account.Id || x.CrAccountId == account.Id || x.CrAccountId == account.Id) && (x.BranchId == _userInfoToken.BranchId || x.ExternalBranchId == _userInfoToken.BranchId) && x.IsDeleted == false && x.EntryDate.Date >= BaseUtilities.UtcToLocal().AddDays(-1) && x.EntryDate.Date <= BaseUtilities.UtcToLocal()).ToListAsync();
                        var firstEntry = entities.OrderBy(x => x.EntryDate).FirstOrDefault();
                        endOfDayData.EndingBalance = account.CurrentBalance;
                        endOfDayData.DebitBalance = account.DebitBalance;
                        endOfDayData.CreditBalance = account.CreditBalance;
                        endOfDayData.BeginningBalance = firstEntry.CurrentBalance;
                        endOfDayData.Entries = _mapper.Map<List<AccountingEntryDto>>(entities.ToList());
                    }
                    if (reconciliation.MobileMoneyOrange != null)
                    {
                        var account = await _accountRepository.FindBy(x => x.AccountNumberReference.Equals(reconciliation.MobileMoneyOrange) && x.AccountOwnerId == _userInfoToken.BranchId).FirstOrDefaultAsync();
                        var entities = await _accountingEntryRepository.All.Where(x => (x.DrAccountId == account.Id || x.CrAccountId == account.Id || x.CrAccountId == account.Id) && (x.BranchId == _userInfoToken.BranchId || x.ExternalBranchId == _userInfoToken.BranchId) && x.IsDeleted == false && x.EntryDate.Date >= BaseUtilities.UtcToLocal().AddDays(-1) && x.EntryDate.Date <= BaseUtilities.UtcToLocal()).ToListAsync();
                        var firstEntry = entities.OrderBy(x => x.EntryDate).FirstOrDefault();
                        endOfDayData.EndingBalance = account.CurrentBalance;
                        endOfDayData.DebitBalance = account.DebitBalance;
                        endOfDayData.CreditBalance = account.CreditBalance;
                        endOfDayData.BeginningBalance = firstEntry.CurrentBalance;
                        endOfDayData.Entries = _mapper.Map<List<AccountingEntryDto>>(entities.ToList());
                    }
                    if (reconciliation.Savings != null)
                    {
                        var account = await _accountRepository.FindBy(x => x.AccountNumberReference.Equals(reconciliation.Savings) && x.AccountOwnerId == _userInfoToken.BranchId).FirstOrDefaultAsync();
                        var entities = await _accountingEntryRepository.All.Where(x => (x.DrAccountId == account.Id || x.CrAccountId == account.Id || x.CrAccountId == account.Id) && (x.BranchId == _userInfoToken.BranchId || x.ExternalBranchId == _userInfoToken.BranchId) && x.IsDeleted == false && x.EntryDate.Date >= BaseUtilities.UtcToLocal().AddDays(-1) && x.EntryDate.Date <= BaseUtilities.UtcToLocal()).ToListAsync();
                        var firstEntry = entities.OrderBy(x => x.EntryDate).FirstOrDefault();
                        endOfDayData.EndingBalance = account.CurrentBalance;
                        endOfDayData.DebitBalance = account.DebitBalance;
                        endOfDayData.CreditBalance = account.CreditBalance;
                        endOfDayData.BeginningBalance = firstEntry.CurrentBalance;
                        endOfDayData.Entries = _mapper.Map<List<AccountingEntryDto>>(entities.ToList());
                    }
                    if (reconciliation.Deposit != null)
                    {
                        var account = await _accountRepository.FindBy(x => x.AccountNumberReference.Equals(reconciliation.Deposit) && x.AccountOwnerId == _userInfoToken.BranchId).FirstOrDefaultAsync();
                        var entities = await _accountingEntryRepository.All.Where(x => (x.DrAccountId == account.Id || x.CrAccountId == account.Id || x.CrAccountId == account.Id) && (x.BranchId == _userInfoToken.BranchId || x.ExternalBranchId == _userInfoToken.BranchId) && x.IsDeleted == false && x.EntryDate.Date >= BaseUtilities.UtcToLocal().AddDays(-1) && x.EntryDate.Date <= BaseUtilities.UtcToLocal()).ToListAsync();
                        var firstEntry = entities.OrderBy(x => x.EntryDate).FirstOrDefault();
                        endOfDayData.EndingBalance = account.CurrentBalance;
                        endOfDayData.DebitBalance = account.DebitBalance;
                        endOfDayData.CreditBalance = account.CreditBalance;
                        endOfDayData.BeginningBalance = firstEntry.CurrentBalance;
                        endOfDayData.Entries = _mapper.Map<List<AccountingEntryDto>>(entities.ToList());
                    }
                    if (reconciliation.OrdinaryShares != null)
                    {
                        var account = await _accountRepository.FindBy(x => x.AccountNumberReference.Equals(reconciliation.OrdinaryShares) && x.AccountOwnerId == _userInfoToken.BranchId).FirstOrDefaultAsync();
                        var entities = await _accountingEntryRepository.All.Where(x => (x.DrAccountId == account.Id || x.CrAccountId == account.Id || x.CrAccountId == account.Id) && (x.BranchId == _userInfoToken.BranchId || x.ExternalBranchId == _userInfoToken.BranchId) && x.IsDeleted == false && x.EntryDate.Date >= BaseUtilities.UtcToLocal().AddDays(-1) && x.EntryDate.Date <= BaseUtilities.UtcToLocal()).ToListAsync();
                        var firstEntry = entities.OrderBy(x => x.EntryDate).FirstOrDefault();
                        endOfDayData.EndingBalance = account.CurrentBalance;
                        endOfDayData.DebitBalance = account.DebitBalance;
                        endOfDayData.CreditBalance = account.CreditBalance;
                        endOfDayData.BeginningBalance = firstEntry.CurrentBalance;
                        endOfDayData.Entries = _mapper.Map<List<AccountingEntryDto>>(entities.ToList());
                    }
                    if (reconciliation.PreferenceShare != null)
                    {
                        var account = await _accountRepository.FindBy(x => x.AccountNumberReference.Equals(reconciliation.PreferenceShare) && x.AccountOwnerId == _userInfoToken.BranchId).FirstOrDefaultAsync();
                        var entities = await _accountingEntryRepository.All.Where(x => (x.DrAccountId == account.Id || x.CrAccountId == account.Id || x.CrAccountId == account.Id) && (x.BranchId == _userInfoToken.BranchId || x.ExternalBranchId == _userInfoToken.BranchId) && x.IsDeleted == false && x.EntryDate.Date >= BaseUtilities.UtcToLocal().AddDays(-1) && x.EntryDate.Date <= BaseUtilities.UtcToLocal()).ToListAsync();
                        var firstEntry = entities.OrderBy(x => x.EntryDate).FirstOrDefault();
                        endOfDayData.EndingBalance = account.CurrentBalance;
                        endOfDayData.DebitBalance = account.DebitBalance;
                        endOfDayData.CreditBalance = account.CreditBalance;
                        endOfDayData.BeginningBalance = firstEntry.CurrentBalance;
                        endOfDayData.Entries = _mapper.Map<List<AccountingEntryDto>>(entities.ToList());
                    }
                    if (reconciliation.DailyCollection != null)
                    {
                        var account = await _accountRepository.FindBy(x => x.AccountNumberReference.Equals(reconciliation.DailyCollection) && x.AccountOwnerId == _userInfoToken.BranchId).FirstOrDefaultAsync();
                        var entities = await _accountingEntryRepository.All.Where(x => (x.DrAccountId == account.Id || x.CrAccountId == account.Id || x.CrAccountId == account.Id) && (x.BranchId == _userInfoToken.BranchId || x.ExternalBranchId == _userInfoToken.BranchId) && x.IsDeleted == false && x.EntryDate.Date >= BaseUtilities.UtcToLocal().AddDays(-1) && x.EntryDate.Date <= BaseUtilities.UtcToLocal()).ToListAsync();
                        var firstEntry = entities.OrderBy(x => x.EntryDate).FirstOrDefault();
                        endOfDayData.EndingBalance = account.CurrentBalance;
                        endOfDayData.DebitBalance = account.DebitBalance;
                        endOfDayData.CreditBalance = account.CreditBalance;
                        endOfDayData.BeginningBalance = firstEntry.CurrentBalance;
                        endOfDayData.Entries = _mapper.Map<List<AccountingEntryDto>>(entities.ToList());
                    }
                    if (reconciliation.DailyCollection != null)
                    {
                        var account = await _accountRepository.FindBy(x => x.AccountNumberReference.Equals(reconciliation.DailyCollection) && x.AccountOwnerId == _userInfoToken.BranchId).FirstOrDefaultAsync();
                        var entities = await _accountingEntryRepository.All.Where(x => (x.DrAccountId == account.Id || x.CrAccountId == account.Id || x.CrAccountId == account.Id) && (x.BranchId == _userInfoToken.BranchId || x.ExternalBranchId == _userInfoToken.BranchId) && x.IsDeleted == false && x.EntryDate.Date >= BaseUtilities.UtcToLocal().AddDays(-1) && x.EntryDate.Date <= BaseUtilities.UtcToLocal()).ToListAsync();
                        var firstEntry = entities.OrderBy(x => x.EntryDate).FirstOrDefault();
                        endOfDayData.EndingBalance = account.CurrentBalance;
                        endOfDayData.DebitBalance = account.DebitBalance;
                        endOfDayData.CreditBalance = account.CreditBalance;
                        endOfDayData.BeginningBalance = firstEntry.CurrentBalance;
                        endOfDayData.Entries = _mapper.Map<List<AccountingEntryDto>>(entities.ToList());
                    }
                    if (reconciliation.SalaryAccount != null)
                    {
                        var account = await _accountRepository.FindBy(x => x.AccountNumberReference.Equals(reconciliation.DailyCollection) && x.AccountOwnerId == _userInfoToken.BranchId).FirstOrDefaultAsync();
                        var entities = await _accountingEntryRepository.All.Where(x => (x.DrAccountId == account.Id || x.CrAccountId == account.Id || x.CrAccountId == account.Id) && (x.BranchId == _userInfoToken.BranchId || x.ExternalBranchId == _userInfoToken.BranchId) && x.IsDeleted == false && x.EntryDate.Date >= BaseUtilities.UtcToLocal().AddDays(-1) && x.EntryDate.Date <= BaseUtilities.UtcToLocal()).ToListAsync();
                        var firstEntry = entities.OrderBy(x => x.EntryDate).FirstOrDefault();
                        endOfDayData.EndingBalance = account.CurrentBalance;
                        endOfDayData.DebitBalance = account.DebitBalance;
                        endOfDayData.CreditBalance = account.CreditBalance;
                        endOfDayData.BeginningBalance = firstEntry.CurrentBalance;
                        endOfDayData.Entries = _mapper.Map<List<AccountingEntryDto>>(entities.ToList());
                    }
                    if (reconciliation.SalaryAccount != null)
                    {
                        var account = await _accountRepository.FindBy(x => x.AccountNumberReference.Equals(reconciliation.DailyCollection) && x.AccountOwnerId == _userInfoToken.BranchId).FirstOrDefaultAsync();
                        var entities = await _accountingEntryRepository.All.Where(x => (x.DrAccountId == account.Id || x.CrAccountId == account.Id || x.CrAccountId == account.Id) && (x.BranchId == _userInfoToken.BranchId || x.ExternalBranchId == _userInfoToken.BranchId) && x.IsDeleted == false && x.EntryDate.Date >= BaseUtilities.UtcToLocal().AddDays(-1) && x.EntryDate.Date <= BaseUtilities.UtcToLocal()).ToListAsync();
                        var firstEntry = entities.OrderBy(x => x.EntryDate).FirstOrDefault();
                        endOfDayData.EndingBalance = account.CurrentBalance;
                        endOfDayData.DebitBalance = account.DebitBalance;
                        endOfDayData.CreditBalance = account.CreditBalance;
                        endOfDayData.BeginningBalance = firstEntry.CurrentBalance;
                        endOfDayData.Entries = _mapper.Map<List<AccountingEntryDto>>(entities.ToList());
                    }
                    if (reconciliation.SalaryAccount != null)
                    {
                        var account = await _accountRepository.FindBy(x => x.AccountNumberReference.Equals(reconciliation.DailyCollection) && x.AccountOwnerId == _userInfoToken.BranchId).FirstOrDefaultAsync();
                        var entities = await _accountingEntryRepository.All.Where(x => (x.DrAccountId == account.Id || x.CrAccountId == account.Id || x.CrAccountId == account.Id) && (x.BranchId == _userInfoToken.BranchId || x.ExternalBranchId == _userInfoToken.BranchId) && x.IsDeleted == false && x.EntryDate.Date >= BaseUtilities.UtcToLocal().AddDays(-1) && x.EntryDate.Date <= BaseUtilities.UtcToLocal()).ToListAsync();
                        var firstEntry = entities.OrderBy(x => x.EntryDate).FirstOrDefault();
                        endOfDayData.EndingBalance = account.CurrentBalance;
                        endOfDayData.DebitBalance = account.DebitBalance;
                        endOfDayData.CreditBalance = account.CreditBalance;
                        endOfDayData.BeginningBalance = firstEntry.CurrentBalance;
                        endOfDayData.Entries = _mapper.Map<List<AccountingEntryDto>>(entities.ToList());
                    }
       
                    //public string  { get; set; }

                    //public string Income { get; set; }
                    //public string ExpenseExcludingVAT { get; set; }
                    //public string LoanDisbursement { get; set; }
                    //public string LoanApproval { get; set; }
                    //public string VAT { get; set; }
                }
                // Retrieve the AccountingEntry entity with the specified ID from the repository

                var entity = await _accountingEntryRepository.All.Where(x => (x.DrAccountId == request.AccountId || x.CrAccountId == request.AccountId) && x.IsDeleted == false && x.EntryDate.Date >= BaseUtilities.UtcToLocal().AddDays(-1) && x.EntryDate.Date <= BaseUtilities.UtcToLocal()).ToListAsync();

                return ServiceResponse<CloseOfDayData>.ReturnResultWith201(model, "End of day transaction successfully loaded");
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting AccountingEntry: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<CloseOfDayData>.Return500(e);
            }
        }

    }
}