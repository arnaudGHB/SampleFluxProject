using AutoMapper;
using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.Helper;
using CBS.SystemConfiguration.MediatR.Queries;
using CBS.SystemConfiguration.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;

namespace CBS.SystemConfiguration.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific CountryName based on its unique identifier.
    /// </summary>
    public class GetLocationQueryHandler : IRequestHandler<GetLocationQuery, ServiceResponse<LocationDto>>
    {
        private readonly ICountryRepository _CountryNameRepository; // Repository for accessing CountryName data.
    
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLocationQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMediator _mediator;
        /// <summary>
        /// Constructor for initializing the GetCountryNameQueryHandler.
        /// </summary>
        /// <param name="CountryNameRepository">Repository for CountryName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLocationQueryHandler(
            ICountryRepository CountryNameRepository,
            IMapper mapper,
            ILogger<GetLocationQueryHandler> logger,
            IMediator mediator)
        {
            _CountryNameRepository = CountryNameRepository;
            _mapper = mapper;
            _logger = logger;
       
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the GetCountryNameQuery to retrieve a specific CountryName.
        /// </summary>
        /// <param name="request">The GetCountryNameQuery containing CountryName ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LocationDto>> Handle(GetLocationQuery request, CancellationToken cancellationToken)
        {
            LocationDto locationDto = new LocationDto();
            string errorMessage = null;
            try
            {
                // Retrieve the CountryName entity with the specified ID from the repository
                var entity =  _CountryNameRepository.FindBy(x=>x.Code==request.Code).FirstOrDefault();
                if (entity != null)
                {
                    if (entity.IsDeleted)
                    {
                        string message = "Country code has been deleted.";
                        // If the AccountCategory entity was not found, log the error and return a 404 Not Found response
                        _logger.LogError(message);
                        return ServiceResponse<LocationDto>.Return404(message);
                    }
                    else
                    {
                        locationDto.Country = _mapper.Map<CountryDto>( entity);
                        var queryModel = new GetRegionByCountryIdQuery
                        {
                            Id = entity.Id
                        };
                        var result = await _mediator.Send(queryModel);
                        locationDto.Regions = result.Data;
                        var queryModel01 = new GetAllRegionQuery
                        {

                        };
                        var result01 = await _mediator.Send(queryModel01);
                        locationDto.Regions = result01.Data;
                        //var result = await _mediator.Send(queryModel);
                        //locationDto.Regions = result.Data;
                        var queryModel21 = new GetAllDivisionQuery
                        {
                        
                        };
                        var result21 = await _mediator.Send(queryModel21);
                        locationDto.Divisions = result21.Data;
       

                        var queryModel3 = new GetAllSubdivisionQuery
                        {

                        };
                        var result3 = await _mediator.Send(queryModel3);
                        locationDto.Subdivisions = result3.Data;
                        var queryModel4 = new GetAllTownQuery
                        {

                        };
                        var result4 = await _mediator.Send(queryModel4);
                        locationDto.Towns = result4.Data;

                        return ServiceResponse<LocationDto>.ReturnResultWith200(locationDto);

                    }

                }
                else
                {
                    // If the CountryName entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Country code not found.");
                    return ServiceResponse<LocationDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting CountryName: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<LocationDto>.Return500(e);
            }
        }
    }
}