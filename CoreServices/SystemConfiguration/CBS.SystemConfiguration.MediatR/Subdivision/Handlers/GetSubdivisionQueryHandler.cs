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
    /// Handles the request to retrieve a specific SubdivisionName based on its unique identifier.
    /// </summary>
    public class GetSubdivisionQueryHandler : IRequestHandler<GetSubdivisionQuery, ServiceResponse<SubdivisionDto>>
    {
        private readonly ISubdivisionRepository _SubdivisionNameRepository; // Repository for accessing SubdivisionName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetSubdivisionQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetSubdivisionNameQueryHandler.
        /// </summary>
        /// <param name="SubdivisionNameRepository">Repository for SubdivisionName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetSubdivisionQueryHandler(
            ISubdivisionRepository SubdivisionNameRepository,
            IMapper mapper,
            ILogger<GetSubdivisionQueryHandler> logger)
        {
            _SubdivisionNameRepository = SubdivisionNameRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetSubdivisionNameQuery to retrieve a specific SubdivisionName.
        /// </summary>
        /// <param name="request">The GetSubdivisionNameQuery containing SubdivisionName ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<SubdivisionDto>> Handle(GetSubdivisionQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the SubdivisionName entity with the specified ID from the repository
                var entity = await _SubdivisionNameRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    if (entity.IsDeleted)
                    {
                        string message = "SubdivisionName has been deleted.";
                        // If the AccountCategory entity was not found, log the error and return a 404 Not Found response
                        _logger.LogError(message);
                        return ServiceResponse<SubdivisionDto>.Return404(message);
                    }
                    else
                    {
                        // Map the SubdivisionName entity to SubdivisionNameDto and return it with a success response
                        var SubdivisionNameDto = _mapper.Map<SubdivisionDto>(entity);
                        return ServiceResponse<SubdivisionDto>.ReturnResultWith200(SubdivisionNameDto);
                    }

                }
                else
                {
                    // If the SubdivisionName entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("SubdivisionName not found.");
                    return ServiceResponse<SubdivisionDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting SubdivisionName: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<SubdivisionDto>.Return500(e);
            }
        }
    }
}