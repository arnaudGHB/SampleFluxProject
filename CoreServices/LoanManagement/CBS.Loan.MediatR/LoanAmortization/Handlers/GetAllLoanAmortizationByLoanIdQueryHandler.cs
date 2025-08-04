using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.Queries;
using CBS.NLoan.Repository.LoanApplicationP.LoanAmortizationP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllLoanAmortizationByLoanIdQueryHandler : IRequestHandler<GetAllLoanAmortizationByLoanIdQuery, ServiceResponse<List<LoanAmortizationDto>>>
    {
        private readonly ILoanAmortizationRepository _LoanAmortizationRepository; // Repository for accessing LoanAmortizations data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLoanAmortizationByLoanIdQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllLoanAmortizationByLoanIdQueryQueryHandler.
        /// </summary>
        /// <param name="LoanAmortizationRepository">Repository for LoanAmortizations data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLoanAmortizationByLoanIdQueryHandler(
            ILoanAmortizationRepository LoanAmortizationRepository,
            IMapper mapper, ILogger<GetAllLoanAmortizationByLoanIdQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _LoanAmortizationRepository = LoanAmortizationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllLoanAmortizationByLoanIdQueryQuery to retrieve all LoanAmortizations.
        /// </summary>
        /// <param name="request">The GetAllLoanAmortizationByLoanIdQueryQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LoanAmortizationDto>>> Handle(GetAllLoanAmortizationByLoanIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all LoanAmortizations entities from the repository
                var entities = await _LoanAmortizationRepository.AllIncluding(x=>x.Loan,x=>x.RefundDetails).Where(x=>x.LoanId==request.LoanId).ToListAsync();
                return ServiceResponse<List<LoanAmortizationDto>>.ReturnResultWith200(_mapper.Map<List<LoanAmortizationDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all LoanAmortizations for {request.LoanId}: {e.Message}");
                return ServiceResponse<List<LoanAmortizationDto>>.Return500(e, "Failed to get all LoanAmortizations");
            }
        }
    }
}
