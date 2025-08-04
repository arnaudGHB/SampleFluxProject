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
    public class GetAllDocumentQueryHandler : IRequestHandler<GetAllDocumentQuery, ServiceResponse<List<DocumentDto>>>
    {
        private readonly IDocumentRepository _DocumentRepository; // Repository for accessing OperationEvent data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllDocumentQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllOperationEventNameQueryHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllDocumentQueryHandler(
            IDocumentRepository DocumentRepository,
            IMapper mapper, ILogger<GetAllDocumentQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _DocumentRepository = DocumentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllDocumentQuery to retrieve all  Document.
        /// </summary>
        /// <param name="request">The GetAllDocumentQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<DocumentDto>>> Handle(GetAllDocumentQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all OperationEvent entities from the repository
                var entities = await _DocumentRepository.All.Where(x=>x.IsDeleted.Equals(false)).ToListAsync();
                return ServiceResponse<List<DocumentDto>>.ReturnResultWith200(_mapper.Map<List<DocumentDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all DocumentDto: {e.Message}");
                return ServiceResponse<List<DocumentDto>>.Return500(e, "Failed to get all DocumentDto");
            }
        }
    }
}