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
    /// Handles the request to retrieve a specific AccountFeature based on its unique identifier.
    /// </summary>
    public class GetAccountFeatureQueryHandler : IRequestHandler<GetAccountFeatureQuery, ServiceResponse<AccountFeatureDto>>
    {
        private readonly IAccountFeatureRepository _AccountFeatureRepository; // Repository for accessing AccountFeature data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAccountFeatureQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAccountFeatureQueryHandler.
        /// </summary>
        /// <param name="AccountFeatureRepository">Repository for AccountFeature data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAccountFeatureQueryHandler(
            IAccountFeatureRepository AccountFeatureRepository,
            IMapper mapper,
            ILogger<GetAccountFeatureQueryHandler> logger)
        {
            _AccountFeatureRepository = AccountFeatureRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAccountFeatureQuery to retrieve a specific AccountFeature.
        /// </summary>
        /// <param name="request">The GetAccountFeatureQuery containing AccountFeature ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountFeatureDto>> Handle(GetAccountFeatureQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the AccountFeature entity with the specified ID from the repository
                var entity = await _AccountFeatureRepository.FindAsync(request.Id);
                if (entity != null)
                {
    
                    if (entity.IsDeleted)
                    {
                        string message = "AccountFeature has been deleted.";
                        // If the AccountCategory entity was not found, log the error and return a 404 Not Found response
                        _logger.LogError(message);
                        return ServiceResponse<AccountFeatureDto>.Return404(message);
                    }
                    else
                    {
                        // Map the AccountFeature entity to AccountFeatureDto and return it with a success response
                        var AccountFeatureDto = _mapper.Map<AccountFeatureDto>(entity);
                        return ServiceResponse<AccountFeatureDto>.ReturnResultWith200(AccountFeatureDto);
                    }
                }
                else
                {
                    // If the AccountFeature entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("AccountFeature not found.");
                    return ServiceResponse<AccountFeatureDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting AccountFeature: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<AccountFeatureDto>.Return500(e);
            }
        }
    }
}