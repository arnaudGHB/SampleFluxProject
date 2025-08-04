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

namespace CBS.AccountingEntryManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific AccountingEntry based on its unique identifier.
    /// </summary>
    public class GetJournalEntryQueryHandler : IRequestHandler<GetJournalEntryQuery, ServiceResponse<AccountingEntriesReport>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository; // Repository for accessing AccountingEntry data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetJournalEntryQueryHandler> _logger; // Logger for logging handler actions and errors.
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
        public GetJournalEntryQueryHandler(
            IAccountingEntryRepository AccountingEntryRepository,
            IMapper mapper,
            ILogger<GetJournalEntryQueryHandler> logger,
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
        public async Task<ServiceResponse<AccountingEntriesReport>> Handle(GetJournalEntryQuery request, CancellationToken cancellationToken)
        {
            AccountingEntriesReport accountingGeneralLedger = new AccountingEntriesReport();
            string errorMessage = null;
            try
            {
                List<AccountingEntry > entities = new List<AccountingEntry >();
                // Retrieve the AccountingEntry entity with the specified ID from the repository
                var Branches = await APICallHelper.GetAllBranchInfos(_pathHelper, _userInfoToken);
                var BranchInfo = Branches.Find(x => x.id == request.BranchId);
                accountingGeneralLedger.EntityId = BranchInfo.id;
                accountingGeneralLedger.EntityType = "BRANCH";
                accountingGeneralLedger.FromDate = request.FromDate;
                accountingGeneralLedger.ToDate = request.ToDate;
                accountingGeneralLedger.BranchName = BranchInfo.name;
                accountingGeneralLedger.BranchAddress = BranchInfo.address;
                accountingGeneralLedger.Name = BranchInfo.bank.name;
                accountingGeneralLedger.Location = BranchInfo.location;
                accountingGeneralLedger.Address = BranchInfo.address;
                accountingGeneralLedger.Capital = BranchInfo.bank.capital;
                accountingGeneralLedger.ImmatriculationNumber = BranchInfo.bank.immatriculationNumber;
                accountingGeneralLedger.WebSite = BranchInfo.webSite;
                accountingGeneralLedger.BranchTelephone = BranchInfo.telephone;
                accountingGeneralLedger.HeadOfficeTelePhone = BranchInfo.bank.telephone;
                accountingGeneralLedger.BranchCode = BranchInfo.branchCode;
                accountingGeneralLedger.AccountingEntries = new List<AccountingEntryDto>();
                
                var collectionentity = new List<AccountingEntry>();
                if (_userInfoToken.IsHeadOffice && request.BranchId=="XXXXXX")
                {
                      collectionentity = _accountingEntryRepository.All.Where(x =>  x.IsDeleted == false && x.ValueDate.Date >= request.FromDate.Date && x.ValueDate.Date <= request.ToDate.Date).ToList();

                }
                else
                {
                      collectionentity = _accountingEntryRepository.All.Where(x => x.ExternalBranchId ==request.BranchId || (x.BranchId == request.BranchId)
                     && x.IsDeleted == false
                      && x.ValueDate.Date >= request.FromDate.Date 
                      && x.ValueDate.Date <= request.ToDate.Date).ToList();

                }
                if (collectionentity.Count() != 0)
                {
                    // Map the AccountingEntry entity to AccountingEntryDto and return it with a success response  OrderBy OrderByDescending
                 
                    foreach (var accountingEntry in collectionentity) 
                    {
                        if(accountingEntry.Representative == "MIGRATION")
                        {
                            //accountingEntry.CrAmount = accountingEntry.CrBalanceBroughtForward;
                            //accountingEntry.DrAmount = accountingEntry.DrBalanceBroughtForward;
                            //entities.Add(accountingEntry);
                        }
                        else
                        {
                            entities.Add(accountingEntry);
                        }
                        
                    }
                    var AccountingEntryDto = _mapper.Map<List<AccountingEntryDto>>(entities);
               
                    accountingGeneralLedger.AccountingEntries.AddRange(AccountingEntryDto.OrderByDescending(x=>x.ValueDate).ToList());
                    // Apply Sorting:
                    accountingGeneralLedger.AccountingEntries = AccountingEntryDto.OrderBy(x=>x.ReferenceID)
                        .OrderByDescending(x => x.DrAmount > 0)  // DrAmount first
                        .ThenBy(x => x.AccountNumber)           // AccountNumber ascending
                        .ThenByDescending(x => x.ValueDate)     // ValueDate descending
                        .ToList();

                }
                errorMessage = $"JournalEntry successfully at the level of {request.BranchId}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.OK, LogAction.GetJournalEntryQuery, LogLevelInfo.Information);
                return ServiceResponse<AccountingEntriesReport>.ReturnResultWith200(accountingGeneralLedger);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting AccountingEntry: {e.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.OK, LogAction.GetJournalEntryQuery, LogLevelInfo.Information);
                return ServiceResponse<AccountingEntriesReport>.ReturnResultWith200(accountingGeneralLedger);
                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<AccountingEntriesReport>.Return500(e);
            }
        }

    }
}