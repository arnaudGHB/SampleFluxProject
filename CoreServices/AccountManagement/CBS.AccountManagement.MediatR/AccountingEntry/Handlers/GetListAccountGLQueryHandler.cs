using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System.Net;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific AccountingEntry based on its unique identifier.
    /// </summary>
    public class GetListAccountGLQueryHandler : IRequestHandler<GetListAccountGLQuery, ServiceResponse<AccountLedgerDetails>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository; // Repository for accessing AccountingEntry data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetListAccountGLQueryHandler> _logger; // Logger for logging handler actions and errors.
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
        public GetListAccountGLQueryHandler(
            IAccountingEntryRepository AccountingEntryRepository,
            IMapper mapper,
            ILogger<GetListAccountGLQueryHandler> logger,
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
        public async Task<ServiceResponse<AccountLedgerDetails>> Handle(GetListAccountGLQuery request, CancellationToken cancellationToken)
        {
            AccountLedgerDetails models = new AccountLedgerDetails();
            models.LedgerDetails = new List<LedgerDetails> { };

            string errorMessage = null;
            try
            {

                // Retrieve the AccountingEntry entity with the specified ID from the repository
                foreach (var item in request.AccountIds)
                {
                    var model = new LedgerDetails();
                    var account = await _accountRepository.FindAsync(item);
                    model.AccountNumber = account.AccountNumberCU;
                    model.AccountName = account.AccountName;
       
                    var entrisx = await _accountingEntryRepository.All.Where(x => x.BranchId == request.BranchId && (x.DrAccountId == item || x.CrAccountId == item) || (x.ExternalBranchId == request.BranchId) && x.IsDeleted == false && x.ValueDate.Date >= request.FromDate.Date && x.ValueDate.Date <= request.ToDate.Date).ToListAsync();
                    model.BeginningBalance = GetBeginningBalance(request, item, entrisx);
                    var accounts = _accountRepository.All.Where(d => d.BranchId == request.BranchId&&d.Id.Equals(account.Id));
                    var entries = from entry in entrisx
                                  join a in accounts on entry.AccountId equals account.Id
                                  select new AccountingEntryDto
                                  {
                                      EntryDatetime = entry.ValueDate.ToString("dd-MM-yyyy HH:mm:ss"),
                                      ReferenceID = entry.ReferenceID,
                                      AccountNumber = a.AccountNumberCU,
                                      AccountName = a.AccountName,
                                      Description = entry.Naration,
                                      DrAmount = entry.DrAmount,
                                      CrAmount = entry.CrAmount,
                                      Representative= entry.Representative,
                                      CurrentBalance = GetBalance(entry),
                                      EntryDate = entry.CreatedDate,

                                  };
                  
                    model.AccountingEntries = entries.OrderBy(x=>x.EntryDate).ToList();
                    model.AccountingEntries.ToArray()[0].DrAmount = 0;
                    model.AccountingEntries.ToArray()[0].CrAmount = 0;
                    decimal tempBalance = Convert.ToDecimal( model.BeginningBalance);
                    foreach (var elt in model.AccountingEntries)
                    {

                        if (elt.AccountNumber.StartsWith("571"))
                        {

                        }
                        if (IsdebitNormal(elt.AccountNumber))
                        {
                            elt.CurrentBalance = tempBalance + elt.DrAmount-elt.CrAmount;
                              tempBalance = elt.CurrentBalance;
                        }
                        else
                        {
                            elt.CurrentBalance = tempBalance + elt.CrAmount-elt.DrAmount  ;
                            tempBalance = elt.CurrentBalance;
                        }
                   
                    }
                    models.LedgerDetails.Add(model);
                }
                if (models != null)
                {

                    var Branches = await APICallHelper.GetAllBranchInfos(_pathHelper, _userInfoToken);
                    var BranchInfo = Branches.Find(x => x.id == request.BranchId);
                    models.EntityId = BranchInfo.id;
                    models.EntityType = "BRANCH";
                    models.FromDate = request.FromDate;
                    models.ToDate = request.ToDate;
                    models.BranchName = BranchInfo.name;
                    models.BranchAddress = BranchInfo.address;
                    models.Name = BranchInfo.bank.name;
                    models.Location = BranchInfo.bank.address;
                    models.Address = BranchInfo.bank.address;
                    models.Capital = BranchInfo.bank.capital;
                    models.ImmatriculationNumber = BranchInfo.bank.immatriculationNumber;
                    models.WebSite = BranchInfo.webSite;
                    models.BranchTelephone = BranchInfo.telephone;
                    models.HeadOfficeTelePhone = BranchInfo.bank.telephone;

                    return ServiceResponse<AccountLedgerDetails>.ReturnResultWith200(models);


                }
                else
                {
                    // If the AccountingEntry entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("AccountingEntry not found.");
                    return ServiceResponse<AccountLedgerDetails>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting AccountingEntry: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<AccountLedgerDetails>.Return500(e);
            }
        }

        private bool IsdebitNormal(string? accountNumber)
        {
            // Determine account behavior based on OHADA rules
            return accountNumber.StartsWith("2") || // Fixed Assets
                                                                  //   || // Inventory
                               accountNumber.StartsWith("5") || // Financial
                                accountNumber.StartsWith("6");   // Expenses

           
        }

        private decimal GetBalance(Data.AccountingEntry entry)
        {
            //decimal balance = 0;
            var accountNumber = entry.DrAmount > entry.CrAmount ? entry.DrAccountNumber : entry.CrAccountNumber;
            decimal balance = entry.DrAmount > entry.CrAmount ? entry.DrCurrentBalance : entry.CrCurrentBalance;
            //model.Dr = DrAccount.LastBalance;
            //model.CrBalanceBroughtForward = CrAccount.LastBalance;
            // Determine account behavior based on OHADA rules
            bool isDebitNormal = accountNumber.StartsWith("2") || // Fixed Assets
                                                                  //   || // Inventory
                               accountNumber.StartsWith("5") || // Financial
                                accountNumber.StartsWith("6");   // Expenses

            bool isCreditNormal = accountNumber.StartsWith("1") || // Capital
                                accountNumber.StartsWith("7") ||    // Income
                                accountNumber.StartsWith("3");
            if (isDebitNormal)
            {
               // balance = BalanceBroughtForward
            }
            return balance;
        }

        private static string GetBeginningBalance(GetListAccountGLQuery request, string item, List<Data.AccountingEntry> AccountingEntries)
        {
          string  BeginningBalance = "0";
            if (   AccountingEntries.Count() == 0)
            {
               BeginningBalance = "0";
            }
            else
            {
                var modelx = AccountingEntries.Where(x=>x.AccountId==item).OrderByDescending(x => x.ValueDate);
                var model = AccountingEntries.Where(x => x.AccountId == item).OrderBy(x => x.CreatedDate).FirstOrDefault();
                BeginningBalance = (AccountingEntries.Where(x => x.AccountId == item).OrderBy(x => x.CreatedDate).FirstOrDefault().CurrentBalance).ToString();

            }
            return BeginningBalance;
        }
    }
}