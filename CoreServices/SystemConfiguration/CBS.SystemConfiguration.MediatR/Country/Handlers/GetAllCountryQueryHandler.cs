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
    public class GetAllCountryQueryHandler : IRequestHandler<GetAllCountryQuery, ServiceResponse<List<CountryDto>>>
    {
        private readonly ICountryRepository _CountryRepository; // Repository for accessing OperationEvent data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllCountryQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllOperationEventNameQueryHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllCountryQueryHandler(
            ICountryRepository CountryRepository,
            IMapper mapper, ILogger<GetAllCountryQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _CountryRepository = CountryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllCountryQuery to retrieve all  Country.
        /// </summary>
        /// <param name="request">The GetAllCountryQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CountryDto>>> Handle(GetAllCountryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all OperationEvent entities from the repository
                var entities = await _CountryRepository.All.Where(x=>x.IsDeleted.Equals(false)).ToListAsync();
                return ServiceResponse<List<CountryDto>>.ReturnResultWith200(_mapper.Map<List<CountryDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all CountryDto: {e.Message}");
                return ServiceResponse<List<CountryDto>>.Return500(e, "Failed to get all CountryDto");
            }
        }
    }
}