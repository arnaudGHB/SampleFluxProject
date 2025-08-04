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
    public class GetCorrespondingMappingByDocumentReferenceCodeIdQueryHandler : IRequestHandler<GetCorrespondingMappingByDocumentReferenceCodeIdQuery, ServiceResponse<List<CorrespondingMappingDto>>>
    {
        private readonly IDocumentReferenceCodeRepository _documentReferenceCodeRepository; // Repository for accessing OperationEvent data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetCorrespondingMappingByDocumentReferenceCodeIdQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentTypeRepository _documentTypeRepository;

        private readonly ICorrespondingMappingRepository _correspondingMappingRepository;
        /// <summary>
        /// Constructor for initializing the GetAllOperationEventNameQueryHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetCorrespondingMappingByDocumentReferenceCodeIdQueryHandler(
            IDocumentReferenceCodeRepository documentReferenceCodeRepository,
            IMapper mapper, 
            ILogger<GetCorrespondingMappingByDocumentReferenceCodeIdQueryHandler> logger,
            IDocumentRepository? documentRepository,
            IDocumentTypeRepository? documentTypeRepository,
            ICorrespondingMappingRepository? correspondingMappingRepository)
        {
            _documentRepository = documentRepository;
            _documentTypeRepository = documentTypeRepository;
            _documentReferenceCodeRepository = documentReferenceCodeRepository;
            _mapper = mapper;
            _logger = logger;
            _correspondingMappingRepository = correspondingMappingRepository;
        }

        /// <summary>
        /// Handles the GetCorrespondingMappingByDocumentReferenceCodeIdQuery to retrieve all  Document.
        /// </summary>
            /// <param name="request">The GetCorrespondingMappingByDocumentReferenceCodeIdQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CorrespondingMappingDto>>> Handle(GetCorrespondingMappingByDocumentReferenceCodeIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all OperationEvent entities from the repository

                var entities = await _documentReferenceCodeRepository.All.Where(x => x.IsDeleted.Equals(false)).ToListAsync();
                var MappingEntities = await _correspondingMappingRepository.All.Where(x => x.IsDeleted.Equals(false) && x.DocumentReferenceCodeId.Equals(request.Id)).ToListAsync();
                var joinedQuery = from docRef in entities
                                  join corresp in MappingEntities on docRef.Id equals corresp.DocumentReferenceCodeId
                                  select new CorrespondingMappingDto
                                  {
                                      Id = corresp.Id,
                                      ChartOfAccountId = corresp.ChartOfAccountId,
                                      AccountNumber = corresp.AccountNumber,
                                      Cartegory = corresp.Cartegory,
                                      ReferenceCode = docRef.ReferenceCode,

                                  };

                var result = joinedQuery.ToList();
                return ServiceResponse<List<CorrespondingMappingDto>>.ReturnResultWith200(_mapper.Map<List<CorrespondingMappingDto>>(result));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all DocumentDto: {e.Message}");
                return ServiceResponse<List<CorrespondingMappingDto>>.Return500(e, "Failed to get all CorrespondingMappingDto");
            }
        }
    }
}