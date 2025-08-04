using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Accounts based on the GetAllAccountQuery.
    /// </summary>
    public class GetAllAccountingRuleEntryQueryHandler : IRequestHandler<GetAllAccountingRuleEntryQuery, ServiceResponse<List<AccountingRuleEntryDto>>>
    {
        private readonly IAccountingRuleEntryRepository __AccountingRuleEntryRepository; // Repository for accessing Accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllAccountingRuleEntryQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllAccountQueryHandler.
        /// </summary>
        /// <param name="_AccountingRuleEntryRepository">Repository for Accounts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllAccountingRuleEntryQueryHandler(
            IAccountingRuleEntryRepository _AccountingRuleEntryRepository,
            IMapper mapper, ILogger<GetAllAccountingRuleEntryQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            __AccountingRuleEntryRepository = _AccountingRuleEntryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllAccountQuery to retrieve all Accounts.
        /// </summary>
        /// <param name="request">The GetAllAccountQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AccountingRuleEntryDto>>> Handle(GetAllAccountingRuleEntryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Accounts entities from the repository
                var entities = await __AccountingRuleEntryRepository.All.Where(c=>c.IsDeleted==false).ToListAsync();
                return ServiceResponse<List<AccountingRuleEntryDto>>.ReturnResultWith200(_mapper.Map<List<AccountingRuleEntryDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Accounts: {e.Message}");
                return ServiceResponse<List<AccountingRuleEntryDto>>.Return500(e, "Failed to get all Accounts");
            }
        }
    }
}