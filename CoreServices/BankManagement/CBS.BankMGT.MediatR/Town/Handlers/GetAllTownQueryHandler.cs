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
    /// Handles the retrieval of all Towns based on the GetAllTownQuery.
    /// </summary>
    public class GetAllTownQueryHandler : IRequestHandler<GetAllTownQuery, ServiceResponse<List<TownDto>>>
    {
        private readonly ITownRepository _TownRepository; // Repository for accessing Towns data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllTownQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllTownQueryHandler.
        /// </summary>
        /// <param name="TownRepository">Repository for Towns data access.</param>
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
        /// Handles the GetAllTownQuery to retrieve all Towns.
        /// </summary>
        /// <param name="request">The GetAllTownQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<TownDto>>> Handle(GetAllTownQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Towns entities from the repository
                var entities = await _TownRepository.AllIncluding(c => c.Subdivision).ToListAsync();
                return ServiceResponse<List<TownDto>>.ReturnResultWith200(_mapper.Map<List<TownDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Towns: {e.Message}");
                return ServiceResponse<List<TownDto>>.Return500(e, "Failed to get all Towns");
            }
        }
    }
}
