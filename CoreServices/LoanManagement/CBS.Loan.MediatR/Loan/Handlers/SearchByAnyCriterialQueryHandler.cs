using AutoMapper;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.Queries;
using CBS.NLoan.Repository.LoanApplicationP.LoanP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the SearchByAnyCriterialQuery.
    /// </summary>
    public class SearchByAnyCriterialQueryHandler : IRequestHandler<SearchByAnyCriterialQuery, ServiceResponse<LoanList>>
    {
        private readonly ILoanRepository _LoanRepository; // Repository for accessing Loans data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<SearchByAnyCriterialQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the SearchByAnyCriterialQueryHandler.
        /// </summary>
        /// <param name="LoanRepository">Repository for Loans data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public SearchByAnyCriterialQueryHandler(
            ILoanRepository LoanRepository,
            IMapper mapper, ILogger<SearchByAnyCriterialQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _LoanRepository = LoanRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the SearchByAnyCriterialQuery to retrieve all Loans.
        /// </summary>
        /// <param name="request">The SearchByAnyCriterialQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanList>> Handle(SearchByAnyCriterialQuery request, CancellationToken cancellationToken)
        {
            try
            {

                //var export = await _LoanRepository.ExportLoansAsync();
                
                // Retrieve all Loans entities from the repository
                var entities = await _LoanRepository.GetLoansAsync(request.LoanResource);
                return ServiceResponse<LoanList>.ReturnResultWith200(entities);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Loans: {e.Message}");
                return ServiceResponse<LoanList>.Return500(e, "Failed to get all Loans");
            }
        }
    }
}
