using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Queries;
using CBS.NLoan.Repository.LoanApplicationP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanApplicationMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllLoanApplicationByCustomerIdQueryHandler : IRequestHandler<GetAllLoanApplicationByCustomerIdQuery, ServiceResponse<List<LoanApplicationDto>>>
    {
        private readonly ILoanApplicationRepository _LoanApplicationRepository; // Repository for accessing Loans data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLoanApplicationByCustomerIdQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllLoanQueryHandler.
        /// </summary>
        /// <param name="LoanRepository">Repository for Loans data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLoanApplicationByCustomerIdQueryHandler(
            ILoanApplicationRepository LoanRepository,
            IMapper mapper, ILogger<GetAllLoanApplicationByCustomerIdQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _LoanApplicationRepository = LoanRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllLoanQuery to retrieve all Loans.
        /// </summary>
        /// <param name="request">The GetAllLoanQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LoanApplicationDto>>> Handle(GetAllLoanApplicationByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Loans entities from the repository
                var entities = await _LoanApplicationRepository.FindBy(x=>x.CustomerId==request.CustomerId && x.IsDeleted==false).Include(x=>x.LoanProduct).ThenInclude(x=>x.LoanProductRepaymentCycles).ToListAsync();
                return ServiceResponse<List<LoanApplicationDto>>.ReturnResultWith200(_mapper.Map<List<LoanApplicationDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Loans: {e.Message}");
                return ServiceResponse<List<LoanApplicationDto>>.Return500(e, "Failed to get all Loans");
            }
        }
    }
}
