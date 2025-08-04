using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all OperationEventAttributes based on the GetAllAccountCategoryQuery.
    /// </summary>
    public class GetAllOperationEventAttributesByOperationEventIdQueryHandler : IRequestHandler<GetAllOperationEventAttributesByOperationEventIdQuery, ServiceResponse<List<OperationEventAttributesDto>>>
    {
        private readonly IOperationEventAttributeRepository _OperationEventAttributesRepository; // Repository for accessing OperationEventAttributess data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllOperationEventAttributesByOperationEventIdQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllOperationEventAttributesQueryHandler.
        /// </summary>
        /// <param name="OperationEventAttributesRepository">Repository for OperationEventAttributes data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllOperationEventAttributesByOperationEventIdQueryHandler(
            IOperationEventAttributeRepository OperationEventAttributesRepository,
            IMapper mapper, ILogger<GetAllOperationEventAttributesByOperationEventIdQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _OperationEventAttributesRepository = OperationEventAttributesRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllOperationEventAttributesQuery to retrieve all OperationEventAttributes.
        /// </summary>
        /// <param name="request">The GetAllOperationEventAttributesQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<OperationEventAttributesDto>>> Handle(GetAllOperationEventAttributesByOperationEventIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all OperationEventAttributess entities from the repository
                var entities = await _OperationEventAttributesRepository.All.ToListAsync();
                // Write the JSON to a file
               
                return ServiceResponse<List<OperationEventAttributesDto>>.ReturnResultWith200(_mapper.Map<List<OperationEventAttributesDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all OperationEventAttributesDto: {e.Message}");
                return ServiceResponse<List<OperationEventAttributesDto>>.Return500(e, "Failed to get all OperationEventAttributes");
            }
        }
    }
}