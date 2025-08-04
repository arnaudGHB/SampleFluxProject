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
    public class GetAllDivisionQueryHandler : IRequestHandler<GetAllDivisionQuery, ServiceResponse<List<DivisionDto>>>
    {
        private readonly IDivisionRepository _DivisionRepository; // Repository for accessing OperationEvent data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllDivisionQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllOperationEventNameQueryHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllDivisionQueryHandler(
            IDivisionRepository DivisionRepository,
            IMapper mapper, ILogger<GetAllDivisionQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _DivisionRepository = DivisionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllDivisionQuery to retrieve all  Division.
        /// </summary>
        /// <param name="request">The GetAllDivisionQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<DivisionDto>>> Handle(GetAllDivisionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all OperationEvent entities from the repository
                var entities = await _DivisionRepository.All.Where(x=>x.IsDeleted.Equals(false)).ToListAsync();
                return ServiceResponse<List<DivisionDto>>.ReturnResultWith200(_mapper.Map<List<DivisionDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all DivisionDto: {e.Message}");
                return ServiceResponse<List<DivisionDto>>.Return500(e, "Failed to get all DivisionDto");
            }
        }
    }
}