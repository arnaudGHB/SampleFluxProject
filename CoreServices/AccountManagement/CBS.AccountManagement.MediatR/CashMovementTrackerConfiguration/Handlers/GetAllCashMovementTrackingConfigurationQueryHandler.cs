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
    public class GetAllCashMovementTrackingConfigurationQueryHandler : IRequestHandler<GetAllCashMovementTrackingConfigurationQuery, ServiceResponse<List<CashMovementTrackingConfigurationDto>>>
    {
        private readonly ICashMovementTrackingConfigurationRepository _cashMovementTrackerRepository; // Repository for accessing AccountPolicyName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllCashMovementTrackingConfigurationQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAccountPolicyNameQueryHandler.
        /// </summary>
        /// <param name="AccountPolicyNameRepository">Repository for AccountPolicyName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllCashMovementTrackingConfigurationQueryHandler(
            ICashMovementTrackingConfigurationRepository cashMovementTrackerRepository,
            IMapper mapper,
            ILogger<GetAllCashMovementTrackingConfigurationQueryHandler> logger)
        {
            _cashMovementTrackerRepository = cashMovementTrackerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllAccountPolicyNameQuery to retrieve all AccountPolicyName.
        /// </summary>
        /// <param name="request">The GetAllAccountPolicyNameQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CashMovementTrackingConfigurationDto>>> Handle(GetAllCashMovementTrackingConfigurationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all AccountPolicy entities from the repository
                var entities = await _cashMovementTrackerRepository.All.Where(x=>x.IsDeleted.Equals(false)).ToListAsync();
                return ServiceResponse<List<CashMovementTrackingConfigurationDto>>.ReturnResultWith200(_mapper.Map<List<CashMovementTrackingConfigurationDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Cash Movement Tracker: {e.Message}");
                return ServiceResponse<List<CashMovementTrackingConfigurationDto>>.Return500(e, "Failed to get all AccountPolicyName");
            }
        }
    }
}