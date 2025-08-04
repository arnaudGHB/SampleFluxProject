using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanMediaR.Queries;
using CBS.NLoan.Repository.LoanApplicationP.LoanP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllLoanByBranchIdQueryHandler : IRequestHandler<GetAllLoanByBranchIdQuery, ServiceResponse<List<LoanDto>>>
    {
        private readonly ILoanRepository _LoanRepository; // Repository for accessing Loans data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLoanByBranchIdQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllLoanQueryHandler.
        /// </summary>
        /// <param name="LoanRepository">Repository for Loans data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLoanByBranchIdQueryHandler(
            ILoanRepository LoanRepository,
            IMapper mapper, ILogger<GetAllLoanByBranchIdQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _LoanRepository = LoanRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllLoanQuery to retrieve all Loans.
        /// </summary>
        /// <param name="request">The GetAllLoanQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LoanDto>>> Handle(GetAllLoanByBranchIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Loans entities from the repository
                var entities = await _LoanRepository.AllIncluding(x=>x.LoanApplication, x=>x.LoanAmortizations, x=>x.Refunds)
                    .Where(x=>x.BranchId==request.BranchId)
                    .Include(x=>x.LoanApplication.LoanCommiteeValidations)
                    .Include(x => x.LoanApplication.LoanProduct)
                    .Include(x => x.LoanApplication.Collateras)
                    .Include(x => x.LoanApplication.Guarantors)
                    .Include(x => x.LoanApplication.LoanPurpose)
                    .Include(x => x.LoanApplication.DocumentAttachedToLoans).ToListAsync();
                return ServiceResponse<List<LoanDto>>.ReturnResultWith200(_mapper.Map<List<LoanDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Loans: {e.Message}");
                return ServiceResponse<List<LoanDto>>.Return500(e, "Failed to get all Loans");
            }
        }
    }
}
