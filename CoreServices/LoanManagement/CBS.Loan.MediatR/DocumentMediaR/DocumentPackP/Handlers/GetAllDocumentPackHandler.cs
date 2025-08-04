using AutoMapper;
using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.DocumentMediaR.DocumentPackP.Queries;
using CBS.NLoan.Repository.DocumentP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentPackP.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllDocumentPackQueryHandler : IRequestHandler<GetAllDocumentPackQuery, ServiceResponse<List<DocumentPackDto>>>
    {
        private readonly IDocumentPackRepository _DocumentPackRepository; // Repository for accessing DocumentPacks data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllDocumentPackQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllDocumentPackQueryHandler.
        /// </summary>
        /// <param name="DocumentPackRepository">Repository for DocumentPacks data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllDocumentPackQueryHandler(
            IDocumentPackRepository DocumentPackRepository,
            IMapper mapper, ILogger<GetAllDocumentPackQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _DocumentPackRepository = DocumentPackRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllDocumentPackQuery to retrieve all DocumentPacks.
        /// </summary>
        /// <param name="request">The GetAllDocumentPackQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<DocumentPackDto>>> Handle(GetAllDocumentPackQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all DocumentPacks entities from the repository
                //var entities = await _DocumentPackRepository.AllIncluding(x => x.DocumentJoins).ToListAsync();
                var entities = _DocumentPackRepository.GetAllDocumentPack();
                return ServiceResponse<List<DocumentPackDto>>.ReturnResultWith200(_mapper.Map<List<DocumentPackDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all DocumentPacks: {e.Message}");
                return ServiceResponse<List<DocumentPackDto>>.Return500(e, "Failed to get all DocumentPacks");
            }
        }
    }
}
