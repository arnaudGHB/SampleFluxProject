using AutoMapper;
using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.DocumentMediaR.DocumentAttachedToLoanP.Queries;
using CBS.NLoan.Repository.DocumentP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentAttachedToLoanP.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetDocumentAttachedToLoanHandler : IRequestHandler<GetDocumentAttachedToLoanQuery, ServiceResponse<DocumentAttachedToLoanDto>>
    {
        private readonly IDocumentAttachedToLoanRepository _DocumentAttachedToLoanRepository; // Repository for accessing DocumentAttachedToLoan data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetDocumentAttachedToLoanHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetDocumentAttachedToLoanQueryHandler.
        /// </summary>
        /// <param name="DocumentAttachedToLoanRepository">Repository for DocumentAttachedToLoan data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetDocumentAttachedToLoanHandler(
            IDocumentAttachedToLoanRepository DocumentAttachedToLoanRepository,
            IMapper mapper,
            ILogger<GetDocumentAttachedToLoanHandler> logger)
        {
            _DocumentAttachedToLoanRepository = DocumentAttachedToLoanRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetDocumentAttachedToLoanQuery to retrieve a specific DocumentAttachedToLoan.
        /// </summary>
        /// <param name="request">The GetDocumentAttachedToLoanQuery containing DocumentAttachedToLoan ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DocumentAttachedToLoanDto>> Handle(GetDocumentAttachedToLoanQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the DocumentAttachedToLoan entity with the specified ID from the repository
                var entity = await _DocumentAttachedToLoanRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the DocumentAttachedToLoan entity to DocumentAttachedToLoanDto and return it with a success response
                    var DocumentAttachedToLoanDto = _mapper.Map<DocumentAttachedToLoanDto>(entity);
                    return ServiceResponse<DocumentAttachedToLoanDto>.ReturnResultWith200(DocumentAttachedToLoanDto);
                }
                else
                {
                    // If the DocumentAttachedToLoan entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("DocumentAttachedToLoan not found.");
                    return ServiceResponse<DocumentAttachedToLoanDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting DocumentAttachedToLoan: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<DocumentAttachedToLoanDto>.Return500(e);
            }
        }
    }

}
