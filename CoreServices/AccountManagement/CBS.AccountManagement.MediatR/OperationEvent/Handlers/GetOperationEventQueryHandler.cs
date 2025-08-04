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
    /// Handles the request to retrieve a specific OperationEventName based on its unique identifier.
    /// </summary>
    public class GetOperationEventQueryHandler : IRequestHandler<GetOperationEventQuery, ServiceResponse<OperationEventDto>>
    {
        private readonly IOperationEventRepository _OperationEventNameRepository; // Repository for accessing OperationEventName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetOperationEventQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetOperationEventNameQueryHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetOperationEventQueryHandler(
            IOperationEventRepository OperationEventNameRepository,
            IMapper mapper,
            ILogger<GetOperationEventQueryHandler> logger)
        {
            _OperationEventNameRepository = OperationEventNameRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetOperationEventNameQuery to retrieve a specific OperationEventName.
        /// </summary>
        /// <param name="request">The GetOperationEventNameQuery containing OperationEventName ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<OperationEventDto>> Handle(GetOperationEventQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the OperationEventName entity with the specified ID from the repository
                var entity = await _OperationEventNameRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    if (entity.IsDeleted)
                    {
                        string message = "OperationEventName has been deleted.";
                        // If the AccountCategory entity was not found, log the error and return a 404 Not Found response
                        _logger.LogError(message);
                        return ServiceResponse<OperationEventDto>.Return404(message);
                    }
                    else
                    {
                        // Map the OperationEventName entity to OperationEventNameDto and return it with a success response
                        var OperationEventNameDto = _mapper.Map<OperationEventDto>(entity);
                        return ServiceResponse<OperationEventDto>.ReturnResultWith200(OperationEventNameDto);
                    }

                }
                else
                {
                    // If the OperationEventName entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("OperationEventName not found.");
                    return ServiceResponse<OperationEventDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting OperationEventName: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<OperationEventDto>.Return500(e);
            }
        }
    }
}