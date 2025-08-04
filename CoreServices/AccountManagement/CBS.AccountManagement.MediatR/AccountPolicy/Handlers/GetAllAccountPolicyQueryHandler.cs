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
    /// Handles the retrieval of all AccountPolicy based on the GetAllAccountPolicyNameQuery.
    /// </summary>
    public class GetAllAccountPolicyQueryHandler : IRequestHandler<GetAllAccountPolicyQuery, ServiceResponse<List<CashMovementTrackerDto>>>
    {
        private readonly IAccountPolicyRepository _AccountPolicyNameRepository; // Repository for accessing AccountPolicy data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllAccountPolicyQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllAccountPolicyNameQueryHandler.
        /// </summary>
        /// <param name="AccountPolicyNameRepository">Repository for AccountPolicyName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllAccountPolicyQueryHandler(
            IAccountPolicyRepository AccountPolicyNameRepository,
            IMapper mapper, ILogger<GetAllAccountPolicyQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _AccountPolicyNameRepository = AccountPolicyNameRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllAccountPolicyNameQuery to retrieve all AccountPolicyName.
        /// </summary>
        /// <param name="request">The GetAllAccountPolicyNameQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CashMovementTrackerDto>>> Handle(GetAllAccountPolicyQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all AccountPolicy entities from the repository
                var entities = await _AccountPolicyNameRepository.All.Where(x=>x.IsDeleted.Equals(false)).ToListAsync();
                return ServiceResponse<List<CashMovementTrackerDto>>.ReturnResultWith200(_mapper.Map<List<CashMovementTrackerDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all AccountPolicyNameDto: {e.Message}");
                return ServiceResponse<List<CashMovementTrackerDto>>.Return500(e, "Failed to get all AccountPolicyName");
            }
        }
    }
}