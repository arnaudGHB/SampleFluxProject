using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Queries;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Data.Dto;
using Microsoft.EntityFrameworkCore;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Country based on its unique identifier.
    /// </summary>
    public class GetCountryQueryHandler : IRequestHandler<GetCountryQuery, ServiceResponse<CountryDto>>
    {
        private readonly ICountryRepository _CountryRepository; // Repository for accessing Country data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetCountryQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetCountryQueryHandler.
        /// </summary>
        /// <param name="CountryRepository">Repository for Country data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetCountryQueryHandler(
            ICountryRepository CountryRepository,
            IMapper mapper,
            ILogger<GetCountryQueryHandler> logger)
        {
            _CountryRepository = CountryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetCountryQuery to retrieve a specific Country.
        /// </summary>
        /// <param name="request">The GetCountryQuery containing Country ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CountryDto>> Handle(GetCountryQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Country entity with the specified ID from the repository
                var entity = await _CountryRepository.AllIncluding(c => c.Regions, x => x.Currencies).FirstOrDefaultAsync(c => c.Id == request.Id);
                if (entity != null)
                {
                    // Map the Country entity to CountryDto and return it with a success response
                    var CountryDto = _mapper.Map<CountryDto>(entity);
                    return ServiceResponse<CountryDto>.ReturnResultWith200(CountryDto);
                }
                else
                {
                    // If the Country entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Country not found.");
                    return ServiceResponse<CountryDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Country: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<CountryDto>.Return500(e);
            }
        }
    }

}
