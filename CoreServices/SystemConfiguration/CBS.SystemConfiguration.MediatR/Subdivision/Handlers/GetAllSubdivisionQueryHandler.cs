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
    public class GetAllSubdivisionQueryHandler : IRequestHandler<GetAllSubdivisionQuery, ServiceResponse<List<SubdivisionDto>>>
    {
        private readonly ISubdivisionRepository _SubdivisionRepository; // Repository for accessing OperationEvent data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllSubdivisionQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllOperationEventNameQueryHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllSubdivisionQueryHandler(
            ISubdivisionRepository SubdivisionRepository,
            IMapper mapper, ILogger<GetAllSubdivisionQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _SubdivisionRepository = SubdivisionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllSubdivisionQuery to retrieve all  Subdivision.
        /// </summary>
        /// <param name="request">The GetAllSubdivisionQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<SubdivisionDto>>> Handle(GetAllSubdivisionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all OperationEvent entities from the repository
                var entities = await _SubdivisionRepository.All.Where(x=>x.IsDeleted.Equals(false)).ToListAsync();
                return ServiceResponse<List<SubdivisionDto>>.ReturnResultWith200(_mapper.Map<List<SubdivisionDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all SubdivisionDto: {e.Message}");
                return ServiceResponse<List<SubdivisionDto>>.Return500(e, "Failed to get all SubdivisionDto");
            }
        }
    }
}