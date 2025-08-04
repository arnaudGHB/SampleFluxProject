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
    public class GetAllDocumentTypeQueryHandler : IRequestHandler<GetAllDocumentTypeQuery, ServiceResponse<List<DocumentTypeDto>>>
    {
        private readonly IDocumentTypeRepository _documentTypeRepository; // Repository for accessing OperationEvent data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllDocumentTypeQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllOperationEventNameQueryHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllDocumentTypeQueryHandler(
            IDocumentTypeRepository documentTypeRepository,
            IMapper mapper, ILogger<GetAllDocumentTypeQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _documentTypeRepository = documentTypeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllDocumentTypeQuery to retrieve all  DocumentType.
        /// </summary>
        /// <param name="request">The GetAllDocumentTypeQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<DocumentTypeDto>>> Handle(GetAllDocumentTypeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all OperationEvent entities from the repository
                var entities = await _documentTypeRepository.All.Where(x=>x.IsDeleted.Equals(false)).ToListAsync();
                return ServiceResponse<List<DocumentTypeDto>>.ReturnResultWith200(_mapper.Map<List<DocumentTypeDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all DocumentTypeDto: {e.Message}");
                return ServiceResponse<List<DocumentTypeDto>>.Return500(e, "Failed to get all DocumentTypeDto");
            }
        }
    }
}