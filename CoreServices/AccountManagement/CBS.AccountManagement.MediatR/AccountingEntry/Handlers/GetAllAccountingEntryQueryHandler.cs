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

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Accounts based on the GetAllAccountQuery.
    /// </summary>
    public class GetAllAccountingEntryQueryHandler : IRequestHandler<GetAllAccountingEntryQuery, ServiceResponse<List<AccountingEntryDto>>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository; // Repository for accessing Accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllAccountingEntryQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly string BranchId= "XXXXXX";
        private readonly string AccountId = "XXXXXX";
        /// <summary>
        /// Constructor for initializing the GetAllAccountQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Accounts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllAccountingEntryQueryHandler(
            IAccountingEntryRepository accountingEntryRepository,
            IMapper mapper, ILogger<GetAllAccountingEntryQueryHandler> logger, UserInfoToken userInfoToken)
        {
            // Assign provided dependencies to local variables.
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
        public async Task<ServiceResponse<List<AccountingEntryDto>>> Handle(GetAllAccountingEntryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                List<Data.AccountingEntry> entitiesAccounts = new List<Data.AccountingEntry>();
                List<AccountingEntryDto> entities = new List<AccountingEntryDto>();
                if (_userInfoToken.IsHeadOffice)
                {
                    if (BranchId != request.BranchId)
                    {
                        if (request.DateIsNull(request.FromDate)&& request.DateIsNull(request.ToDate))
                        {
                            if (request.AccountId!=null&& request.BranchId != null) 
                            {
                                entitiesAccounts = _accountingEntryRepository.FindBy(x => x.BranchId==request.BranchId && x.AccountId == request.AccountId && x.IsDeleted == false).ToList();
                            }
                            else
                            {
                                entitiesAccounts = _accountingEntryRepository.All.Where(x => x.BranchId.Equals(request.BranchId) && x.IsDeleted.Equals(false)).ToList();

                            }

                        }
                        else
                        {
                            entitiesAccounts = _accountingEntryRepository.All.Where(x => x.BranchId.Equals(request.BranchId) && x.IsDeleted.Equals(false) && x.ValueDate.Date <= (request.ToDate.Date) && x.ValueDate.Date >= (request.FromDate)).ToList();

                        }

                    }
                    else 
                    {
                      
                        entitiesAccounts = _accountingEntryRepository.All.Where(x => x.IsDeleted.Equals(false) && x.ValueDate.Date<=(request.ToDate.Date) && x.ValueDate.Date>=(request.FromDate.Date)).ToList();  
                    }
                }
                else
                {
                    entitiesAccounts = _accountingEntryRepository.All.Where(x => x.BranchId.Equals(_userInfoToken.BranchId) && x.IsDeleted.Equals(false) && x.ValueDate.Equals(request.ToDate) && x.ValueDate.Equals(request.FromDate)).ToList(); 

                }
 
                entities = _mapper.Map(entitiesAccounts,entities );
                return ServiceResponse<List<AccountingEntryDto>>.ReturnResultWith200(entities);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get all accounts: {e.Message}");
                return ServiceResponse<List<AccountingEntryDto>>.Return500(e, "Failed to get all accounts");
            }
        }

      
    }
}