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

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Accounts based on the GetAllAccountQuery.
    /// </summary>
    public class GetLiaisonLedgerEntryQueryHandler : IRequestHandler<GetLiaisonLedgerEntryQuery, ServiceResponse<List<LiaisonLedgerEntry>>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository; // Repository for accessing Accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLiaisonLedgerEntryQueryHandler> _logger; // Logger for logging handler actions and errors.
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
        public GetLiaisonLedgerEntryQueryHandler(
            IAccountingEntryRepository accountingEntryRepository,
            IMapper mapper, ILogger<GetLiaisonLedgerEntryQueryHandler> logger, UserInfoToken userInfoToken, IAccountRepository accountRepository)
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
        public async Task<ServiceResponse<List<LiaisonLedgerEntry>>> Handle(GetLiaisonLedgerEntryQuery request, CancellationToken cancellationToken)
        {
            try
            {
            
                List<LiaisonLedgerEntry> entities = new List<LiaisonLedgerEntry>();
                List<Data.AccountingEntry> entitiesEntry = new List<Data.AccountingEntry>();
                if (_userInfoToken.IsHeadOffice)
                {
                    if (BranchId != request.BranchId)
                    {
                        entitiesEntry = _accountingEntryRepository.FindBy(x=>x.AccountId==request.AccountId&& x.IsDeleted.Equals(false) && x.ValueDate.Equals(request.ToDate) && x.ValueDate.Equals(request.FromDate)).ToList();
                        entities= await GenerateLiaisonLedgerAsync(entitiesEntry);
                    }
                    else 
                    {
                        entitiesEntry = _accountingEntryRepository.FindBy(x => x.AccountId == request.AccountId && x.IsDeleted.Equals(false) && x.ValueDate.Equals(request.ToDate) && x.ValueDate.Equals(request.FromDate)).ToList();
                        entities = await GenerateLiaisonLedgerAsync(entitiesEntry);
                    }
                }
                else
                {
                    entitiesEntry = _accountingEntryRepository.FindBy(x => x.AccountId == request.AccountId  && x.IsDeleted.Equals(false) && x.ValueDate.Equals(request.ToDate) && x.ValueDate.Equals(request.FromDate)).ToList();
                    entities = await GenerateLiaisonLedgerAsync(entitiesEntry);
                }
                
                return ServiceResponse<List<LiaisonLedgerEntry>>.ReturnResultWith200(entities);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get all accounts: {e.Message}");
                return ServiceResponse<List<LiaisonLedgerEntry>>.Return500(e, "Failed to get all accounts");
            }
        }


        public  async Task<List<LiaisonLedgerEntry>> GenerateLiaisonLedgerAsync(List<Data.AccountingEntry> entries)
        {
            var ledger = new List<LiaisonLedgerEntry>();

            foreach (var entry in entries)
            {
                var Entry = ledger.FirstOrDefault(e => e.AccountId == entry.AccountId);
            
                if (Entry == null)
                {
                    Entry = new LiaisonLedgerEntry
                    {
                        AccountId = entry.AccountId,
                        AccountNumber = entry.DrAccountNumber,                  
                        AccountName = (await _accountRepository.FindAsync(entry.AccountId)).AccountName,
                        Balance = entry.DrAmount - entry.CrAmount
                    };
                    ledger.Add(Entry);
                }
                else
                {
                    Entry.Balance += entry.DrAmount - entry.CrAmount;
                }
                
            }

            return ledger;
        }

     
    }
}