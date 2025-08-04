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
    public class GetCorrespondingMappingExceptionByDocumentReferenceCodeIdQueryHandler : IRequestHandler<GetCorrespondingMappingExceptionsByDocumentReferenceCodeIdQuery, ServiceResponse<List<CorrespondingMappingExceptionDto>>>
    {
        private readonly IDocumentReferenceCodeRepository _documentReferenceCodeRepository; // Repository for accessing OperationEvent data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetCorrespondingMappingExceptionByDocumentReferenceCodeIdQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentTypeRepository _documentTypeRepository;
 
        private readonly ICorrespondingMappingExceptionRepository _correspondingMappingExceptionRepository;
        /// <summary>
        /// Constructor for initializing the GetAllOperationEventNameQueryHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetCorrespondingMappingExceptionByDocumentReferenceCodeIdQueryHandler(
            IDocumentReferenceCodeRepository documentReferenceCodeRepository,
            IMapper mapper, ILogger<GetCorrespondingMappingExceptionByDocumentReferenceCodeIdQueryHandler> logger, IDocumentRepository? documentRepository, IDocumentTypeRepository? documentTypeRepository, ICorrespondingMappingExceptionRepository? correspondingMappingRepository)
        {
            _documentRepository = documentRepository;
            _documentTypeRepository = documentTypeRepository;
               _documentReferenceCodeRepository = documentReferenceCodeRepository;
            _mapper = mapper;
            _logger = logger;
            _correspondingMappingExceptionRepository = correspondingMappingRepository;
        }

        /// <summary>
        /// Handles the GetAllDocumentQuery to retrieve all  Document.
        /// </summary>
        /// <param name="request">The GetAllDocumentQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CorrespondingMappingExceptionDto>>> Handle(GetCorrespondingMappingExceptionsByDocumentReferenceCodeIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all OperationEvent entities from the repository
           
                var entities = await _documentReferenceCodeRepository.All.Where(x=>x.IsDeleted.Equals(false)).ToListAsync();
                var MappingEntities = await _correspondingMappingExceptionRepository.All.Where(x => x.DocumentReferenceCodeId.Equals(request.Id)).ToListAsync();
                var joinedQuery = from docRef in entities
                                  join corresp in MappingEntities on docRef.Id equals corresp.DocumentReferenceCodeId
                                
                                  select new CorrespondingMappingExceptionDto
                                  {
                                    Id= corresp.Id,
                                    ChartOfAccountId= corresp.ChartOfAccountId,  
                                    AccountNumber= corresp.AccountNumber,
                                    Cartegory= corresp.Category,
                                    ReferenceCode= docRef.ReferenceCode,
                               
                                  };

                var result = joinedQuery.ToList();
                return ServiceResponse<List<CorrespondingMappingExceptionDto>>.ReturnResultWith200(_mapper.Map<List<CorrespondingMappingExceptionDto>>(result));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all DocumentDto: {e.Message}");
                return ServiceResponse<List<CorrespondingMappingExceptionDto>>.Return500(e, "Failed to get all DocumentDto");
            }
        }
    }
}