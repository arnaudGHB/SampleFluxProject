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
    /// Handles the request to retrieve a specific DocumentName based on its unique identifier.
    /// </summary>
    public class GetDocumentQueryHandler : IRequestHandler<GetDocumentQuery, ServiceResponse<DocumentDto>>
    {
        private readonly IDocumentRepository _DocumentNameRepository; // Repository for accessing DocumentName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetDocumentQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetDocumentNameQueryHandler.
        /// </summary>
        /// <param name="DocumentNameRepository">Repository for DocumentName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetDocumentQueryHandler(
            IDocumentRepository DocumentNameRepository,
            IMapper mapper,
            ILogger<GetDocumentQueryHandler> logger)
        {
            _DocumentNameRepository = DocumentNameRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetDocumentNameQuery to retrieve a specific DocumentName.
        /// </summary>
        /// <param name="request">The GetDocumentNameQuery containing DocumentName ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DocumentDto>> Handle(GetDocumentQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the DocumentName entity with the specified ID from the repository
                var entity = await _DocumentNameRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    if (entity.IsDeleted)
                    {
                        string message = "DocumentName has been deleted.";
                        // If the AccountCategory entity was not found, log the error and return a 404 Not Found response
                        _logger.LogError(message);
                        return ServiceResponse<DocumentDto>.Return404(message);
                    }
                    else
                    {
                        // Map the DocumentName entity to DocumentNameDto and return it with a success response
                        var DocumentNameDto = _mapper.Map<DocumentDto>(entity);
                        return ServiceResponse<DocumentDto>.ReturnResultWith200(DocumentNameDto);
                    }

                }
                else
                {
                    // If the DocumentName entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("DocumentName not found.");
                    return ServiceResponse<DocumentDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting DocumentName: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<DocumentDto>.Return500(e);
            }
        }
    }
}