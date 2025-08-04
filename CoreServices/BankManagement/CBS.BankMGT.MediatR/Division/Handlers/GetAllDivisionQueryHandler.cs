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
    /// Handles the retrieval of all Divisions based on the GetAllDivisionQuery.
    /// </summary>
    public class GetAllDivisionQueryHandler : IRequestHandler<GetAllDivisionQuery, ServiceResponse<List<DivisionDto>>>
    {
        private readonly IDivisionRepository _DivisionRepository; // Repository for accessing Divisions data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllDivisionQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllDivisionQueryHandler.
        /// </summary>
        /// <param name="DivisionRepository">Repository for Divisions data access.</param>
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
        /// Handles the GetAllDivisionQuery to retrieve all Divisions.
        /// </summary>
        /// <param name="request">The GetAllDivisionQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<DivisionDto>>> Handle(GetAllDivisionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Divisions entities from the repository
                var entities = await _DivisionRepository.AllIncluding(c => c.Subdivisions, x=>x.Region).ToListAsync();
                return ServiceResponse<List<DivisionDto>>.ReturnResultWith200(_mapper.Map<List<DivisionDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Divisions: {e.Message}");
                return ServiceResponse<List<DivisionDto>>.Return500(e, "Failed to get all Divisions");
            }
        }
    }
}
