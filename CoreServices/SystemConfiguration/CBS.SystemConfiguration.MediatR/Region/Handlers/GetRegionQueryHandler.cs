using AutoMapper;
using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.Helper;
using CBS.SystemConfiguration.MediatR.Queries;
using CBS.SystemConfiguration.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.SystemConfiguration.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific RegionName based on its unique identifier.
    /// </summary>
    public class GetRegionQueryHandler : IRequestHandler<GetRegionQuery, ServiceResponse<RegionDto>>
    {
        private readonly IRegionRepository _RegionNameRepository; // Repository for accessing RegionName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetRegionQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetRegionNameQueryHandler.
        /// </summary>
        /// <param name="RegionNameRepository">Repository for RegionName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetRegionQueryHandler(
            IRegionRepository RegionNameRepository,
            IMapper mapper,
            ILogger<GetRegionQueryHandler> logger)
        {
            _RegionNameRepository = RegionNameRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetRegionNameQuery to retrieve a specific RegionName.
        /// </summary>
        /// <param name="request">The GetRegionNameQuery containing RegionName ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<RegionDto>> Handle(GetRegionQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the RegionName entity with the specified ID from the repository
                var entity = await _RegionNameRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    if (entity.IsDeleted)
                    {
                        string message = "RegionName has been deleted.";
                        // If the AccountCategory entity was not found, log the error and return a 404 Not Found response
                        _logger.LogError(message);
                        return ServiceResponse<RegionDto>.Return404(message);
                    }
                    else
                    {
                        // Map the RegionName entity to RegionNameDto and return it with a success response
                        var RegionNameDto = _mapper.Map<RegionDto>(entity);
                        return ServiceResponse<RegionDto>.ReturnResultWith200(RegionNameDto);
                    }

                }
                else
                {
                    // If the RegionName entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("RegionName not found.");
                    return ServiceResponse<RegionDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting RegionName: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<RegionDto>.Return500(e);
            }
        }
    }
}