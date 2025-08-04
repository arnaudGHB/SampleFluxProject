using AutoMapper;
using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.Helper;
using CBS.SystemConfiguration.MediatR.Queries;
using CBS.SystemConfiguration.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Data.Entity;

namespace CBS.SystemConfiguration.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific RegionName based on its unique identifier.
    /// </summary>
    public class GetRegionByCountryIdQueryHandler : IRequestHandler<GetRegionByCountryIdQuery, ServiceResponse<List<RegionDto>>>
    {
        private readonly IRegionRepository _RegionNameRepository; // Repository for accessing RegionName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetRegionByCountryIdQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetRegionNameQueryHandler.
        /// </summary>
        /// <param name="RegionNameRepository">Repository for RegionName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetRegionByCountryIdQueryHandler(
            IRegionRepository RegionNameRepository,
            IMapper mapper,
            ILogger<GetRegionByCountryIdQueryHandler> logger)
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
        public async Task<ServiceResponse<List<RegionDto>>> Handle(GetRegionByCountryIdQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the RegionName entity with the specified ID from the repository
                var entity = await _RegionNameRepository.All.Where(x=>x.CountryId==request.Id).ToListAsync();
                if (entity.Any())
                {
                    var entities = await _RegionNameRepository.All.Where(x => x.IsDeleted.Equals(false)).ToListAsync();
                    return ServiceResponse<List<RegionDto>>.ReturnResultWith200(_mapper.Map<List<RegionDto>>(entities));
                }
                else
                {
                    // If the RegionName entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("RegionName not found.");
                    return ServiceResponse<List<RegionDto>>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting RegionName: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<List<RegionDto>>.Return500(e);
            }
        }
    }
}