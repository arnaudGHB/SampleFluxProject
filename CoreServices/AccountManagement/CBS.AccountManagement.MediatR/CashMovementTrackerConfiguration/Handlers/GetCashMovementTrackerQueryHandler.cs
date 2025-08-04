using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific AccountPolicyName based on its unique identifier.
    /// </summary>
    public class GetCashMovementTrackingConfigurationQueryHandler : IRequestHandler<GetCashMovementTrackingConfigurationQuery, ServiceResponse<CashMovementTrackingConfigurationDto>>
    {
        private readonly ICashMovementTrackingConfigurationRepository _cashMovementTrackerRepository; // Repository for accessing AccountPolicyName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetCashMovementTrackingConfigurationQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAccountPolicyNameQueryHandler.
        /// </summary>
        /// <param name="AccountPolicyNameRepository">Repository for AccountPolicyName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetCashMovementTrackingConfigurationQueryHandler(
            ICashMovementTrackingConfigurationRepository cashMovementTrackerRepository,
            IMapper mapper,
            ILogger<GetCashMovementTrackingConfigurationQueryHandler> logger)
        {
            _cashMovementTrackerRepository = cashMovementTrackerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAccountPolicyNameQuery to retrieve a specific AccountPolicyName.
        /// </summary>
        /// <param name="request">The GetAccountPolicyNameQuery containing AccountPolicyName ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CashMovementTrackingConfigurationDto>> Handle(GetCashMovementTrackingConfigurationQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the AccountPolicyName entity with the specified ID from the repository
                var entity = await _cashMovementTrackerRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    if (entity.IsDeleted)
                    {
                        string message = "Cash Movement Tracker configuration has been deleted.";
                        // If the AccountCategory entity was not found, log the error and return a 404 Not Found response
                        _logger.LogError(message);
                        return ServiceResponse<CashMovementTrackingConfigurationDto>.Return404(message);
                    }
                    else
                    {
                        // Map the AccountPolicyName entity to AccountPolicyNameDto and return it with a success response
                        var AccountPolicyNameDto = _mapper.Map<CashMovementTrackingConfigurationDto>(entity);
                        return ServiceResponse<CashMovementTrackingConfigurationDto>.ReturnResultWith200(AccountPolicyNameDto);
                    }

                }
                else
                {
                    // If the AccountPolicyName entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Cash Movement Tracker configuration not found.");
                    return ServiceResponse<CashMovementTrackingConfigurationDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting AccountPolicyName: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<CashMovementTrackingConfigurationDto>.Return500(e);
            }
        }
    }
}