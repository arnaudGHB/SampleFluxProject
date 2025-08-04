using AutoMapper;
using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.DocumentMediaR.DocumentP.Queries;
using CBS.NLoan.Repository.DocumentP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentP.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllDocumentHandler : IRequestHandler<GetAllDocumentQuery, ServiceResponse<List<DocumentDto>>>
    {
        private readonly IDocumentRepository _DocumentRepository; // Repository for accessing Documents data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllDocumentHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllDocumentQueryHandler.
        /// </summary>
        /// <param name="DocumentRepository">Repository for Documents data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllDocumentHandler(
            IDocumentRepository DocumentRepository,
            IMapper mapper, ILogger<GetAllDocumentHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _DocumentRepository = DocumentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllDocumentQuery to retrieve all Documents.
        /// </summary>
        /// <param name="request">The GetAllDocumentQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<DocumentDto>>> Handle(GetAllDocumentQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Documents entities from the repository
                var entities = await _DocumentRepository.All.Where(x => !x.IsDeleted).ToListAsync();
                return ServiceResponse<List<DocumentDto>>.ReturnResultWith200(_mapper.Map<List<DocumentDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Documents: {e.Message}");
                return ServiceResponse<List<DocumentDto>>.Return500(e, "Failed to get all Documents");
            }
        }
    }
}
