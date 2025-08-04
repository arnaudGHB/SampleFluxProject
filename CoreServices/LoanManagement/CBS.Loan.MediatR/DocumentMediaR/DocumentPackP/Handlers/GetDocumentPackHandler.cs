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
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetDocumentPackQueryHandler : IRequestHandler<GetDocumentPackQuery, ServiceResponse<DocumentPackDto>>
    {
        private readonly IDocumentPackRepository _DocumentPackRepository; // Repository for accessing DocumentPack data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetDocumentPackQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetDocumentPackQueryHandler.
        /// </summary>
        /// <param name="DocumentPackRepository">Repository for DocumentPack data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetDocumentPackQueryHandler(
            IDocumentPackRepository DocumentPackRepository,
            IMapper mapper,
            ILogger<GetDocumentPackQueryHandler> logger)
        {
            _DocumentPackRepository = DocumentPackRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetDocumentPackQuery to retrieve a specific DocumentPack.
        /// </summary>
        /// <param name="request">The GetDocumentPackQuery containing DocumentPack ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DocumentPackDto>> Handle(GetDocumentPackQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the DocumentPack entity with the specified ID from the repository
                var entity = _DocumentPackRepository.GetDocumentPack(request.Id);
                //var entity = await _DocumentPackRepository.AllIncluding(x => x.DocumentJoins).FirstOrDefaultAsync(x => x.Id == request.Id);
                if (entity != null)
                {
                    // Map the DocumentPack entity to DocumentPackDto and return it with a success response
                    var DocumentPackDto = _mapper.Map<DocumentPackDto>(entity);
                    return ServiceResponse<DocumentPackDto>.ReturnResultWith200(DocumentPackDto);
                }
                else
                {
                    // If the DocumentPack entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("DocumentPack not found.");
                    return ServiceResponse<DocumentPackDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting DocumentPack: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<DocumentPackDto>.Return500(e);
            }
        }
    }

}
