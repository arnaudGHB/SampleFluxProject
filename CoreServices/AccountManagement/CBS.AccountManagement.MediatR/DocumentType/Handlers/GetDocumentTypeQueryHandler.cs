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
    /// Handles the request to retrieve a specific DocumentTypeName based on its unique identifier.
    /// </summary>
    public class GetDocumentTypeQueryHandler : IRequestHandler<GetDocumentTypeQuery, ServiceResponse<DocumentTypeDto>>
    {
        private readonly IDocumentTypeRepository _DocumentTypeNameRepository; // Repository for accessing DocumentTypeName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetDocumentTypeQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetDocumentTypeNameQueryHandler.
        /// </summary>
        /// <param name="DocumentTypeNameRepository">Repository for DocumentTypeName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetDocumentTypeQueryHandler(
            IDocumentTypeRepository DocumentTypeNameRepository,
            IMapper mapper,
            ILogger<GetDocumentTypeQueryHandler> logger)
        {
            _DocumentTypeNameRepository = DocumentTypeNameRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetDocumentTypeNameQuery to retrieve a specific DocumentTypeName.
        /// </summary>
        /// <param name="request">The GetDocumentTypeNameQuery containing DocumentTypeName ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DocumentTypeDto>> Handle(GetDocumentTypeQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the DocumentTypeName entity with the specified ID from the repository
                var entity = await _DocumentTypeNameRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    if (entity.IsDeleted)
                    {
                        string message = "DocumentTypeName has been deleted.";
                        // If the AccountCategory entity was not found, log the error and return a 404 Not Found response
                        _logger.LogError(message);
                        return ServiceResponse<DocumentTypeDto>.Return404(message);
                    }
                    else
                    {
                        // Map the DocumentTypeName entity to DocumentTypeNameDto and return it with a success response
                        var DocumentTypeNameDto = _mapper.Map<DocumentTypeDto>(entity);
                        return ServiceResponse<DocumentTypeDto>.ReturnResultWith200(DocumentTypeNameDto);
                    }

                }
                else
                {
                    // If the DocumentTypeName entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("DocumentTypeName not found.");
                    return ServiceResponse<DocumentTypeDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting DocumentTypeName: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<DocumentTypeDto>.Return500(e);
            }
        }
    }
}