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
    public class GetAllDocumentReferenceCodeQueryHandler : IRequestHandler<GetAllDocumentReferenceCodeQuery, ServiceResponse<List<DocumentReferenceCodeDto>>>
    {
        private readonly IDocumentReferenceCodeRepository _documentReferenceCodeRepository; // Repository for accessing OperationEvent data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllDocumentReferenceCodeQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentTypeRepository _documentTypeRepository;
        /// <summary>
        /// Constructor for initializing the GetAllOperationEventNameQueryHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllDocumentReferenceCodeQueryHandler(
            IDocumentReferenceCodeRepository documentReferenceCodeRepository,
            IMapper mapper, ILogger<GetAllDocumentReferenceCodeQueryHandler> logger, IDocumentRepository? documentRepository, IDocumentTypeRepository? documentTypeRepository)
        {
            _documentRepository = documentRepository;
            _documentTypeRepository = documentTypeRepository;
               _documentReferenceCodeRepository = documentReferenceCodeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllDocumentQuery to retrieve all  Document.
        /// </summary>
        /// <param name="request">The GetAllDocumentQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<DocumentReferenceCodeDto>>> Handle(GetAllDocumentReferenceCodeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all OperationEvent entities from the repository
                var DocumentEntities = await _documentRepository.All.Where(x => x.IsDeleted.Equals(false)).ToListAsync();
                var DocumentTypeEntities = await _documentTypeRepository.All.Where(x => x.IsDeleted.Equals(false)).ToListAsync();
                var entities = await _documentReferenceCodeRepository.All.Where(x=>x.IsDeleted.Equals(false)).ToListAsync();

                var joinedQuery = from docRef in entities
                                  join docType in DocumentTypeEntities
                                      on docRef.DocumentTypeId equals docType.Id
                                  join doc in DocumentEntities
                                      on docRef.DocumentId equals doc.Id
                                  select new DocumentReferenceCodeDto
                                  {
                                    Id= docRef.Id,
                                    DocumentTypeId= docRef.DocumentTypeId,  
                                    Document= doc.Name,
                                    DocumentType= docType.Name,
                                    ReferenceCode= docRef.ReferenceCode,
                                    DocumentId= docRef.DocumentId,
                                    Description= docRef.Description
                                  };

                var result = joinedQuery.ToList();
                return ServiceResponse<List<DocumentReferenceCodeDto>>.ReturnResultWith200(_mapper.Map<List<DocumentReferenceCodeDto>>(result));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all DocumentDto: {e.Message}");
                return ServiceResponse<List<DocumentReferenceCodeDto>>.Return500(e, "Failed to get all DocumentDto");
            }
        }
    }
}