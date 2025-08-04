using AutoMapper;
using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.Helper;
using CBS.SystemConfiguration.MediatR.Queries;
using CBS.SystemConfiguration.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.SystemConfiguration.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all OperationEvent based on the GetAllOperationEventNameQuery.
    /// </summary>
    public class GetAllRegionQueryHandler : IRequestHandler<GetAllRegionQuery, ServiceResponse<List<RegionDto>>>
    {
        private readonly IRegionRepository _RegionRepository; // Repository for accessing OperationEvent data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllRegionQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllOperationEventNameQueryHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllRegionQueryHandler(
            IRegionRepository RegionRepository,
            IMapper mapper, ILogger<GetAllRegionQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _RegionRepository = RegionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllRegionQuery to retrieve all  Region.
        /// </summary>
        /// <param name="request">The GetAllRegionQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<RegionDto>>> Handle(GetAllRegionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all OperationEvent entities from the repository
                var entities = await _RegionRepository.All.Where(x=>x.IsDeleted.Equals(false)).ToListAsync();
                return ServiceResponse<List<RegionDto>>.ReturnResultWith200(_mapper.Map<List<RegionDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all RegionDto: {e.Message}");
                return ServiceResponse<List<RegionDto>>.Return500(e, "Failed to get all RegionDto");
            }
        }
    }
}