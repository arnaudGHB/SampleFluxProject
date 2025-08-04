using AutoMapper;
using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.DocumentMediaR.DocumentAttachedToLoanP.Queries;
using CBS.NLoan.Repository.DocumentP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentAttachedToLoanP.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllDocumentAttachedToLoanHandler : IRequestHandler<GetAllDocumentAttachedToLoanQuery, ServiceResponse<List<DocumentAttachedToLoanDto>>>
    {
        private readonly IDocumentAttachedToLoanRepository _DocumentAttachedToLoanRepository; // Repository for accessing DocumentAttachedToLoans data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllDocumentAttachedToLoanHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllDocumentAttachedToLoanQueryHandler.
        /// </summary>
        /// <param name="DocumentAttachedToLoanRepository">Repository for DocumentAttachedToLoans data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllDocumentAttachedToLoanHandler(
            IDocumentAttachedToLoanRepository DocumentAttachedToLoanRepository,
            IMapper mapper, ILogger<GetAllDocumentAttachedToLoanHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _DocumentAttachedToLoanRepository = DocumentAttachedToLoanRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllDocumentAttachedToLoanQuery to retrieve all DocumentAttachedToLoans.
        /// </summary>
        /// <param name="request">The GetAllDocumentAttachedToLoanQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<DocumentAttachedToLoanDto>>> Handle(GetAllDocumentAttachedToLoanQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all DocumentAttachedToLoans entities from the repository
                var entities = await _DocumentAttachedToLoanRepository.All.ToListAsync();
                return ServiceResponse<List<DocumentAttachedToLoanDto>>.ReturnResultWith200(_mapper.Map<List<DocumentAttachedToLoanDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all DocumentAttachedToLoans: {e.Message}");
                return ServiceResponse<List<DocumentAttachedToLoanDto>>.Return500(e, "Failed to get all DocumentAttachedToLoans");
            }
        }
    }
}
