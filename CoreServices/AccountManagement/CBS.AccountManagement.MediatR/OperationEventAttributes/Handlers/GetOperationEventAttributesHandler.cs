using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific OperationEventNameAttributes based on its unique identifier.
    /// </summary>
    public class GetOperationEventAttributesHandler : IRequestHandler<GetOperationEventAttributesQuery, ServiceResponse<OperationEventAttributesDto>>
    {
        private readonly IOperationEventAttributeRepository _OperationEventNameAttributesRepository; // Repository for accessing OperationEventNameAttributes data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetOperationEventQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetOperationEventAttributesHandler.
        /// </summary>
        /// <param name="OperationEventNameAttributesRepository">Repository for OperationEventNameAttributes data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetOperationEventAttributesHandler(
            IOperationEventAttributeRepository OperationEventNameAttributesRepository,
            IMapper mapper,
            ILogger<GetOperationEventQueryHandler> logger)
        {
            _OperationEventNameAttributesRepository = OperationEventNameAttributesRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetOperationEventNameAttributes to retrieve a specific OperationEventNameAttributes.
        /// </summary>
        /// <param name="request">The GetOperationEventNameAttributes containing OperationEventNameAttributes ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<OperationEventAttributesDto>> Handle(GetOperationEventAttributesQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the OperationEventNameAttributes entity with the specified ID from the repository
                var entity = await _OperationEventNameAttributesRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    if (entity.IsDeleted)
                    {
                        string message = "OperationEventNameAttributesDto has been deleted.";
                        // If the OperationEventNameAttributesDto entity was not found, log the error and return a 404 Not Found response
                        _logger.LogError(message);
                        return ServiceResponse<OperationEventAttributesDto>.Return404(message);
                    }
                    else
                    {
                        // Map the OperationEventNameAttributes entity to OperationEventNameAttributesDto and return it with a success response
                        var OperationEventNameAttributesDto = _mapper.Map<OperationEventAttributesDto>(entity);
                        return ServiceResponse<OperationEventAttributesDto>.ReturnResultWith200(OperationEventNameAttributesDto);

                    }
                }
                else
                {
                    // If the OperationEventNameAttributes entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("OperationEventNameAttributes not found.");
                    return ServiceResponse<OperationEventAttributesDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting OperationEventNameAttributes: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<OperationEventAttributesDto>.Return500(e);
            }
        }
    }
}