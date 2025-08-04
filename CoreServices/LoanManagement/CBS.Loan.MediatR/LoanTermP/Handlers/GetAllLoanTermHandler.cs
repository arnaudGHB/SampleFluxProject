using AutoMapper;
using CBS.NLoan.Data.Dto.LoanTermP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanTermP.Queries;
using CBS.NLoan.Repository.LoanTermP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanTermP.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllLoanTermHandler : IRequestHandler<GetAllLoanTermQuery, ServiceResponse<List<LoanTermDto>>>
    {
        private readonly ILoanTermRepository _LoanTermRepository; // Repository for accessing LoanTerms data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLoanTermHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllLoanTermQueryHandler.
        /// </summary>
        /// <param name="LoanTermRepository">Repository for LoanTerms data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLoanTermHandler(
            ILoanTermRepository LoanTermRepository,
            IMapper mapper, ILogger<GetAllLoanTermHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _LoanTermRepository = LoanTermRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllLoanTermQuery to retrieve all LoanTerms.
        /// </summary>
        /// <param name="request">The GetAllLoanTermQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LoanTermDto>>> Handle(GetAllLoanTermQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all LoanTerms entities from the repository
                var entities = await _LoanTermRepository.All.Where(x=>!x.IsDeleted).ToListAsync();
                return ServiceResponse<List<LoanTermDto>>.ReturnResultWith200(_mapper.Map<List<LoanTermDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all LoanTerms: {e.Message}");
                return ServiceResponse<List<LoanTermDto>>.Return500(e, "Failed to get all LoanTerms");
            }
        }
    }
}
