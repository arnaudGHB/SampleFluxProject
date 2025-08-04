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
    /// Handles the request to retrieve a specific DivisionName based on its unique identifier.
    /// </summary>
    public class GetDivisionQueryHandler : IRequestHandler<GetDivisionQuery, ServiceResponse<DivisionDto>>
    {
        private readonly IDivisionRepository _DivisionNameRepository; // Repository for accessing DivisionName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetDivisionQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetDivisionNameQueryHandler.
        /// </summary>
        /// <param name="DivisionNameRepository">Repository for DivisionName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetDivisionQueryHandler(
            IDivisionRepository DivisionNameRepository,
            IMapper mapper,
            ILogger<GetDivisionQueryHandler> logger)
        {
            _DivisionNameRepository = DivisionNameRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetDivisionNameQuery to retrieve a specific DivisionName.
        /// </summary>
        /// <param name="request">The GetDivisionNameQuery containing DivisionName ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DivisionDto>> Handle(GetDivisionQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the DivisionName entity with the specified ID from the repository
                var entity = await _DivisionNameRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    if (entity.IsDeleted)
                    {
                        string message = "DivisionName has been deleted.";
                        // If the AccountCategory entity was not found, log the error and return a 404 Not Found response
                        _logger.LogError(message);
                        return ServiceResponse<DivisionDto>.Return404(message);
                    }
                    else
                    {
                        // Map the DivisionName entity to DivisionNameDto and return it with a success response
                        var DivisionNameDto = _mapper.Map<DivisionDto>(entity);
                        return ServiceResponse<DivisionDto>.ReturnResultWith200(DivisionNameDto);
                    }

                }
                else
                {
                    // If the DivisionName entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("DivisionName not found.");
                    return ServiceResponse<DivisionDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting DivisionName: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<DivisionDto>.Return500(e);
            }
        }
    }
}