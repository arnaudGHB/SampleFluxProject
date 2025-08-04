using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using CBS.ChartOfAccount.MediatR.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.ChartOfAccount.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all ChartOfAccounts based on the GetAllChartOfAccountQuery.
    /// </summary>
    public class GetAllChartOfAccountQueryHandler : IRequestHandler<GetAllChartOfAccountQuery, ServiceResponse<List<ChartOfAccountDto>>>
    {
        private readonly IChartOfAccountRepository _AccountRepository; // Repository for accessing Accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllChartOfAccountQueryHandler> _logger; // Logger for logging handler actions and errors.
 
        /// <summary>
        /// Constructor for initializing the GetAllAccountQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Accounts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllChartOfAccountQueryHandler(
            IChartOfAccountRepository AccountRepository,
            IMapper mapper, ILogger<GetAllChartOfAccountQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _AccountRepository = AccountRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllChartOfAccountQuery to retrieve all Accounts.
        /// </summary>
        /// <param name="request">The GetAllChartOfAccountQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<ChartOfAccountDto>>> Handle(GetAllChartOfAccountQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Accounts entities from the repository
                var entities = await _AccountRepository.All.Where(x => x.IsDeleted.Equals(false)).ToListAsync();

                return ServiceResponse<List<ChartOfAccountDto>>.ReturnResultWith200(_mapper.Map<List<ChartOfAccountDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Accounts: {e.Message}");
                return ServiceResponse<List<ChartOfAccountDto>>.Return500(e, "Failed to get all Accounts");
            }
        }
    }
}