using AutoMapper;
using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanApplicationFeeMediaR.LoanApplicationFeeP.Queries;
using CBS.NLoan.Repository.LoanApplicationFeeP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanApplicationFeeMediaR.LoanApplicationFeeP.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllLoanApplicationFeeHandler : IRequestHandler<GetAllLoanApplicationFeeQuery, ServiceResponse<List<LoanApplicationFeeDto>>>
    {
        private readonly ILoanApplicationFeeRepository _loanApplicationFeeRepository; // Repository for accessing LoanApplicationFees data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLoanApplicationFeeHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllLoanApplicationFeeHandler.
        /// </summary>
        /// <param name="loanApplicationFeeRepository">Repository for LoanApplicationFees data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLoanApplicationFeeHandler(
            ILoanApplicationFeeRepository loanApplicationFeeRepository,
            IMapper mapper,
            ILogger<GetAllLoanApplicationFeeHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _loanApplicationFeeRepository = loanApplicationFeeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllLoanApplicationFeeQuery to retrieve all LoanApplicationFees.
        /// </summary>
        /// <param name="request">The GetAllLoanApplicationFeeQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LoanApplicationFeeDto>>> Handle(GetAllLoanApplicationFeeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all LoanApplicationFees entities from the repository
                var entities = await _loanApplicationFeeRepository.All.Where(x=>!x.IsDeleted).ToListAsync();
                return ServiceResponse<List<LoanApplicationFeeDto>>.ReturnResultWith200(_mapper.Map<List<LoanApplicationFeeDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all LoanApplicationFees: {e.Message}");
                return ServiceResponse<List<LoanApplicationFeeDto>>.Return500(e, "Failed to get all LoanApplicationFees");
            }
        }
    }
}
