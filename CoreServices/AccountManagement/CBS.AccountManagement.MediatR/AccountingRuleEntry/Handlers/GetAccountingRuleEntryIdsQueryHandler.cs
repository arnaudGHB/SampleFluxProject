using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Accounts based on the GetAllAccountQuery.
    /// </summary>
    public class GetAccountingRuleEntryIdsQueryHandler : IRequestHandler<GetAccountingRuleEntryIdsQuery, ServiceResponse<List<RuleEntries>>>
    {
        private readonly IAccountingRuleEntryRepository _AccountingRuleEntryRepository; // Repository for accessing Accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAccountingRuleEntryIdsQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllAccountQueryHandler.
        /// </summary>
        /// <param name="_AccountingRuleEntryRepository">Repository for Accounts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAccountingRuleEntryIdsQueryHandler(
            IAccountingRuleEntryRepository AccountingRuleEntryRepository,
            IMapper mapper, ILogger<GetAccountingRuleEntryIdsQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _AccountingRuleEntryRepository = AccountingRuleEntryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllAccountQuery to retrieve all Accounts.
        /// </summary>
        /// <param name="request">The GetAllAccountQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<RuleEntries>>> Handle(GetAccountingRuleEntryIdsQuery request, CancellationToken cancellationToken)
        {
            List <RuleEntries> ruleEntryDtos= new List<RuleEntries> (); 
            try
            {
                // Retrieve all Accounts entities from the repository
                var entities = await _AccountingRuleEntryRepository.All.Where(c=>c.IsDeleted==false ).ToListAsync();
                if (entities.Any())
                {
                    ruleEntryDtos = (from entity in entities
                                    select new RuleEntries
                                    {
                                        Id = entity.Id,
                                        AccountingRuleEntryName = entity.AccountingRuleEntryName
                                    }).ToList();
                    return ServiceResponse<List<RuleEntries>>.ReturnResultWith200(ruleEntryDtos);

                }
                else {
                    var message = string.Empty;
                    message = "No fee entry has been configured for the now. Please contact System Admin";
                    return ServiceResponse<List<RuleEntries>>.Return403(message);
                }
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Accounts: {e.Message}");
                return ServiceResponse<List<RuleEntries>>.Return500(e, "Failed to get all Accounts");
            }
        }
    }
}