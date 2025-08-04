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
    /// Handles the retrieval of all Subdivisions based on the GetAllSubdivisionQuery.
    /// </summary>
    public class GetAllSubdivisionQueryHandler : IRequestHandler<GetAllSubdivisionQuery, ServiceResponse<List<SubdivisionDto>>>
    {
        private readonly ISubdivisionRepository _SubdivisionRepository; // Repository for accessing Subdivisions data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllSubdivisionQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllSubdivisionQueryHandler.
        /// </summary>
        /// <param name="SubdivisionRepository">Repository for Subdivisions data access.</param>
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
        /// Handles the GetAllSubdivisionQuery to retrieve all Subdivisions.
        /// </summary>
        /// <param name="request">The GetAllSubdivisionQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<SubdivisionDto>>> Handle(GetAllSubdivisionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Subdivisions entities from the repository
                var entities = await _SubdivisionRepository.AllIncluding(c => c.Towns, x=>x.Division).ToListAsync();
                return ServiceResponse<List<SubdivisionDto>>.ReturnResultWith200(_mapper.Map<List<SubdivisionDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Subdivisions: {e.Message}");
                return ServiceResponse<List<SubdivisionDto>>.Return500(e, "Failed to get all Subdivisions");
            }
        }
    }
}
