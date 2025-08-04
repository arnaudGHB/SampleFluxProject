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
    /// Handles the request to retrieve a specific TownName based on its unique identifier.
    /// </summary>
    public class GetTownQueryHandler : IRequestHandler<GetTownQuery, ServiceResponse<TownDto>>
    {
        private readonly ITownRepository _TownNameRepository; // Repository for accessing TownName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetTownQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetTownNameQueryHandler.
        /// </summary>
        /// <param name="TownNameRepository">Repository for TownName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetTownQueryHandler(
            ITownRepository TownNameRepository,
            IMapper mapper,
            ILogger<GetTownQueryHandler> logger)
        {
            _TownNameRepository = TownNameRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetTownNameQuery to retrieve a specific TownName.
        /// </summary>
        /// <param name="request">The GetTownNameQuery containing TownName ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<TownDto>> Handle(GetTownQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the TownName entity with the specified ID from the repository
                var entity = await _TownNameRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    if (entity.IsDeleted)
                    {
                        string message = "TownName has been deleted.";
                        // If the AccountCategory entity was not found, log the error and return a 404 Not Found response
                        _logger.LogError(message);
                        return ServiceResponse<TownDto>.Return404(message);
                    }
                    else
                    {
                        // Map the TownName entity to TownNameDto and return it with a success response
                        var TownNameDto = _mapper.Map<TownDto>(entity);
                        return ServiceResponse<TownDto>.ReturnResultWith200(TownNameDto);
                    }

                }
                else
                {
                    // If the TownName entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("TownName not found.");
                    return ServiceResponse<TownDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting TownName: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<TownDto>.Return500(e);
            }
        }
    }
}