using AutoMapper;
using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.Helper;
using CBS.SystemConfiguration.MediatR.Queries;
using CBS.SystemConfiguration.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.SystemConfiguration.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific CountryName based on its unique identifier.
    /// </summary>
    public class GetCountryQueryHandler : IRequestHandler<GetCountryQuery, ServiceResponse<CountryDto>>
    {
        private readonly ICountryRepository _CountryNameRepository; // Repository for accessing CountryName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetCountryQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetCountryNameQueryHandler.
        /// </summary>
        /// <param name="CountryNameRepository">Repository for CountryName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetCountryQueryHandler(
            ICountryRepository CountryNameRepository,
            IMapper mapper,
            ILogger<GetCountryQueryHandler> logger)
        {
            _CountryNameRepository = CountryNameRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetCountryNameQuery to retrieve a specific CountryName.
        /// </summary>
        /// <param name="request">The GetCountryNameQuery containing CountryName ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CountryDto>> Handle(GetCountryQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the CountryName entity with the specified ID from the repository
                var entity = await _CountryNameRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    if (entity.IsDeleted)
                    {
                        string message = "CountryName has been deleted.";
                        // If the AccountCategory entity was not found, log the error and return a 404 Not Found response
                        _logger.LogError(message);
                        return ServiceResponse<CountryDto>.Return404(message);
                    }
                    else
                    {
                        // Map the CountryName entity to CountryNameDto and return it with a success response
                        var CountryNameDto = _mapper.Map<CountryDto>(entity);
                        return ServiceResponse<CountryDto>.ReturnResultWith200(CountryNameDto);
                    }

                }
                else
                {
                    // If the CountryName entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("CountryName not found.");
                    return ServiceResponse<CountryDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting CountryName: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<CountryDto>.Return500(e);
            }
        }
    }
}