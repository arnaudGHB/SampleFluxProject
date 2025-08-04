using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Identity.Client;
using CBS.AccountManagement.Data.Dto.AccountingEntryDto;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Accounts based on the GetAllAccountQuery.
    /// </summary>
    public class GetBranchLiaisonLedgerEntryQueryHandler : IRequestHandler<GetBranchLiaisonLedgerEntryQuery, ServiceResponse<List<BranchLiaisonLedgerEntry>>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository; // Repository for accessing Accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetBranchLiaisonLedgerEntryQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly string BranchId= "XXXXXX";
        private readonly string AccountId = "XXXXXX";
        private readonly IAccountRepository _accountRepository;
        /// <summary>
        /// Constructor for initializing the GetAllAccountQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Accounts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetBranchLiaisonLedgerEntryQueryHandler(
            IAccountingEntryRepository accountingEntryRepository,
            IMapper mapper, ILogger<GetBranchLiaisonLedgerEntryQueryHandler> logger, UserInfoToken userInfoToken, IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
            _accountingEntryRepository = accountingEntryRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }




        /// <summary>
        /// Handles the query for retrieving accounting entries based on specific query options. 
        /// </summary>
        /// <param name="request">The GetAllAccountQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<BranchLiaisonLedgerEntry>>> Handle(GetBranchLiaisonLedgerEntryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                List<BranchLiaisonLedgerEntry> entities = new List<BranchLiaisonLedgerEntry>();
                List<Data.AccountingEntry> entitiesEntry = new List<Data.AccountingEntry>();

                if (_userInfoToken.IsHeadOffice)
                {
                    if (BranchId != request.BranchId)
                    {
                        entitiesEntry = _accountingEntryRepository.All.Where(x => x.BranchId.Equals(request.BranchId) &&  !x.IsDeleted && x.ValueDate.Date >= request.FromDate.Date && x.ValueDate.Date <= request.ToDate.Date).ToList();
                        entities = await GenerateLiaisonLedgerAsync(entitiesEntry);
                    }
                    else
                    {
                        entitiesEntry = _accountingEntryRepository.FindBy(x => x.BranchId.Equals(request.BranchId)&&!x.IsDeleted && x.ValueDate.Date >= request.FromDate.Date && x.ValueDate.Date <= request.ToDate.Date).ToList();
                        entities = await GenerateLiaisonLedgerAsync(entitiesEntry);
                    }
                }
                else
                {
                    entitiesEntry = _accountingEntryRepository.FindBy(x => x.BranchId.Equals(request.BranchId) && (x.AccountId == request.AccountId) && !x.IsDeleted && x.ValueDate >= request.FromDate && x.ValueDate <= request.ToDate).ToList();
                    entities = await GenerateLiaisonLedgerAsync(entitiesEntry);
                }

                return ServiceResponse<List<BranchLiaisonLedgerEntry>>.ReturnResultWith200(entities);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get all accounts: {e.Message}");
                return ServiceResponse<List<BranchLiaisonLedgerEntry>>.Return500(e, "Failed to get all accounts");
            }
        }

        public async Task<List<BranchLiaisonLedgerEntry>> GenerateLiaisonLedgerAsync(List<Data.AccountingEntry> entries)
        {
            var branchLedgers = new Dictionary<string, List<BranchLiaisonLedgerEntry>>();

            foreach (var entry in entries)
            {
                 
                await UpdateBranchLedgerAsync(entry, entry.AccountId, entry.CrAmount, entry.DrAmount, branchLedgers, entry.BranchId);
            }

            // Flatten branch ledgers into a single list
            var allLedgerEntries = branchLedgers.Values.SelectMany(x => x).ToList();
            return allLedgerEntries;
        }

    

        private async Task UpdateBranchLedgerAsync(Data.AccountingEntry entry, string sourceAccountId, decimal sourceAmount, decimal targetAmount, Dictionary<string, List<BranchLiaisonLedgerEntry>> branchLedgers, string branchId)
        {
            if (!branchLedgers.ContainsKey(branchId))
            {
                branchLedgers[branchId] = new List<BranchLiaisonLedgerEntry>();
            }

            var ledger = branchLedgers[branchId];
            var entryToUpdate = ledger.FirstOrDefault(e => e.AccountId == sourceAccountId && e.BranchId == branchId);
            if (entryToUpdate == null)
            {
                entryToUpdate = new BranchLiaisonLedgerEntry
                {
                    BranchId = branchId,
                    AccountId = sourceAccountId,
                    
                    //AccountNumber = entry.AccountNumber,
                    AccountName = (await _accountRepository.FindAsync(sourceAccountId)).AccountName,
                    Balance = sourceAmount - targetAmount
                };
                ledger.Add(entryToUpdate);
            }
            else
            {
                entryToUpdate.Balance += sourceAmount - targetAmount;
            }
            entryToUpdate.TotalDrAmount += sourceAmount;
            entryToUpdate.TotalCrAmount += targetAmount;
        }

    }
}