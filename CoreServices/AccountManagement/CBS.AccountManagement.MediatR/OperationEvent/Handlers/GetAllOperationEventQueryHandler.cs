using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all OperationEvent based on the GetAllOperationEventNameQuery.
    /// </summary>
    public class GetAllOperationEventQueryHandler : IRequestHandler<GetAllOperationEventQuery, ServiceResponse<List<OperationEventDto>>>
    {
        private readonly IOperationEventRepository _OperationEventNameRepository; // Repository for accessing OperationEvent data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllOperationEventQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllOperationEventNameQueryHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllOperationEventQueryHandler(
            IOperationEventRepository OperationEventNameRepository,
            IMapper mapper, ILogger<GetAllOperationEventQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _OperationEventNameRepository = OperationEventNameRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllOperationEventNameQuery to retrieve all OperationEventName.
        /// </summary>
        /// <param name="request">The GetAllOperationEventNameQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<OperationEventDto>>> Handle(GetAllOperationEventQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all OperationEvent entities from the repository
                var entities = await _OperationEventNameRepository.All.Where(x=>x.IsDeleted.Equals(false)).ToListAsync();
                return ServiceResponse<List<OperationEventDto>>.ReturnResultWith200(_mapper.Map<List<OperationEventDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all OperationEventNameDto: {e.Message}");
                return ServiceResponse<List<OperationEventDto>>.Return500(e, "Failed to get all OperationEventName");
            }
        }
    }
}