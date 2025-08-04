using AutoMapper;
using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.DocumentMediaR.DocumentP.Queries;
using CBS.NLoan.Repository.DocumentP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentP.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetDocumentHandler : IRequestHandler<GetDocumentQuery, ServiceResponse<DocumentDto>>
    {
        private readonly IDocumentRepository _DocumentRepository; // Repository for accessing Document data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetDocumentHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetDocumentQueryHandler.
        /// </summary>
        /// <param name="DocumentRepository">Repository for Document data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetDocumentHandler(
            IDocumentRepository DocumentRepository,
            IMapper mapper,
            ILogger<GetDocumentHandler> logger)
        {
            _DocumentRepository = DocumentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetDocumentQuery to retrieve a specific Document.
        /// </summary>
        /// <param name="request">The GetDocumentQuery containing Document ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DocumentDto>> Handle(GetDocumentQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Document entity with the specified ID from the repository
                var entity = await _DocumentRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the Document entity to DocumentDto and return it with a success response
                    var DocumentDto = _mapper.Map<DocumentDto>(entity);
                    return ServiceResponse<DocumentDto>.ReturnResultWith200(DocumentDto);
                }
                else
                {
                    // If the Document entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Document not found.");
                    return ServiceResponse<DocumentDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Document: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<DocumentDto>.Return500(e);
            }
        }
    }

}
