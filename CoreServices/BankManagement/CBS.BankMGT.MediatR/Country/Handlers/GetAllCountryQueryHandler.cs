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
    /// Handles the retrieval of all Countrys based on the GetAllCountryQuery.
    /// </summary>
    public class GetAllCountryQueryHandler : IRequestHandler<GetAllCountryQuery, ServiceResponse<List<CountryDto>>>
    {
        private readonly ICountryRepository _CountryRepository; // Repository for accessing Countrys data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllCountryQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllCountryQueryHandler.
        /// </summary>
        /// <param name="CountryRepository">Repository for Countrys data access.</param>
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
        /// Handles the GetAllCountryQuery to retrieve all Countrys.
        /// </summary>
        /// <param name="request">The GetAllCountryQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CountryDto>>> Handle(GetAllCountryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Countrys entities from the repository
                var entities = await _CountryRepository.AllIncluding(c => c.Regions,x=>x.Currencies).ToListAsync();
                return ServiceResponse<List<CountryDto>>.ReturnResultWith200(_mapper.Map<List<CountryDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Countrys: {e.Message}");
                return ServiceResponse<List<CountryDto>>.Return500(e, "Failed to get all Countrys");
            }
        }
    }
}
