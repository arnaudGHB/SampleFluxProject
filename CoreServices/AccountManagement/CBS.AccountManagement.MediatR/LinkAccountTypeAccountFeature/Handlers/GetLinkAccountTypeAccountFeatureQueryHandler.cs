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
    /// Handles the request to retrieve a specific LinkAccountTypeAccountFeature based on its unique identifier.
    /// </summary>
    public class GetLinkAccountTypeAccountFeatureQueryHandler : IRequestHandler<GetLinkAccountTypeAccountFeatureQuery, ServiceResponse<LinkAccountTypeAccountFeatureDto>>
    {
        private readonly ILinkAccountTypeAccountFeatureRepository _LinkAccountTypeAccountFeatureRepository; // Repository for accessing LinkAccountTypeAccountFeature data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLinkAccountTypeAccountFeatureQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetLinkAccountTypeAccountFeatureQueryHandler.
        /// </summary>
        /// <param name="LinkAccountTypeAccountFeatureRepository">Repository for LinkAccountTypeAccountFeature data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLinkAccountTypeAccountFeatureQueryHandler(
            ILinkAccountTypeAccountFeatureRepository LinkAccountTypeAccountFeatureRepository,
            IMapper mapper,
            ILogger<GetLinkAccountTypeAccountFeatureQueryHandler> logger)
        {
            _LinkAccountTypeAccountFeatureRepository = LinkAccountTypeAccountFeatureRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetLinkAccountTypeAccountFeatureQuery to retrieve a specific LinkAccountTypeAccountFeature.
        /// </summary>
        /// <param name="request">The GetLinkAccountTypeAccountFeatureQuery containing LinkAccountTypeAccountFeature ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LinkAccountTypeAccountFeatureDto>> Handle(GetLinkAccountTypeAccountFeatureQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the LinkAccountTypeAccountFeature entity with the specified ID from the repository
                var entity = await _LinkAccountTypeAccountFeatureRepository.FindAsync(request.Id);
                if (entity != null)
                {
    
                    if (entity.IsDeleted)
                    {
                        string message = "LinkAccountTypeAccountFeature has been deleted.";
                        // If the AccountCategory entity was not found, log the error and return a 404 Not Found response
                        _logger.LogError(message);
                        return ServiceResponse<LinkAccountTypeAccountFeatureDto>.Return404(message);
                    }
                    else
                    {
                        // Map the LinkAccountTypeAccountFeature entity to LinkAccountTypeAccountFeatureDto and return it with a success response
                        var LinkAccountTypeAccountFeatureDto = _mapper.Map<LinkAccountTypeAccountFeatureDto>(entity);
                        return ServiceResponse<LinkAccountTypeAccountFeatureDto>.ReturnResultWith200(LinkAccountTypeAccountFeatureDto);
                    }
                }
                else
                {
                    // If the LinkAccountTypeAccountFeature entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("LinkAccountTypeAccountFeature not found.");
                    return ServiceResponse<LinkAccountTypeAccountFeatureDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting LinkAccountTypeAccountFeature: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<LinkAccountTypeAccountFeatureDto>.Return500(e);
            }
        }
    }
}