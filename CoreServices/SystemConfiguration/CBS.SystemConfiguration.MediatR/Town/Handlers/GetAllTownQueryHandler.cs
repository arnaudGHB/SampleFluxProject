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
    public class GetAllTownQueryHandler : IRequestHandler<GetAllTownQuery, ServiceResponse<List<TownDto>>>
    {
        private readonly ITownRepository _TownRepository; // Repository for accessing OperationEvent data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllTownQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllOperationEventNameQueryHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllTownQueryHandler(
            ITownRepository TownRepository,
            IMapper mapper, ILogger<GetAllTownQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _TownRepository = TownRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllTownQuery to retrieve all  Town.
        /// </summary>
        /// <param name="request">The GetAllTownQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<TownDto>>> Handle(GetAllTownQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all OperationEvent entities from the repository
                var entities = await _TownRepository.All.Where(x=>x.IsDeleted.Equals(false)).ToListAsync();
                return ServiceResponse<List<TownDto>>.ReturnResultWith200(_mapper.Map<List<TownDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all TownDto: {e.Message}");
                return ServiceResponse<List<TownDto>>.Return500(e, "Failed to get all TownDto");
            }
        }
    }
}