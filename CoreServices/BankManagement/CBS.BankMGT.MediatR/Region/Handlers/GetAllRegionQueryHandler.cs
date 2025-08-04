using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Queries;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
using Microsoft.EntityFrameworkCore;
using CBS.BankMGT.Data.Dto;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Regions based on the GetAllRegionQuery.
    /// </summary>
    public class GetAllRegionQueryHandler : IRequestHandler<GetAllRegionQuery, ServiceResponse<List<RegionDto>>>
    {
        private readonly IRegionRepository _RegionRepository; // Repository for accessing Regions data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllRegionQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllRegionQueryHandler.
        /// </summary>
        /// <param name="RegionRepository">Repository for Regions data access.</param>
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
        /// Handles the GetAllRegionQuery to retrieve all Regions.
        /// </summary>
        /// <param name="request">The GetAllRegionQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<RegionDto>>> Handle(GetAllRegionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Regions entities from the repository
                var entities = await _RegionRepository.AllIncluding(c => c.Divisions, cy=>cy.Country).ToListAsync();
                return ServiceResponse<List<RegionDto>>.ReturnResultWith200(_mapper.Map<List<RegionDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Regions: {e.Message}");
                return ServiceResponse<List<RegionDto>>.Return500(e, "Failed to get all Regions");
            }
        }
    }
}
