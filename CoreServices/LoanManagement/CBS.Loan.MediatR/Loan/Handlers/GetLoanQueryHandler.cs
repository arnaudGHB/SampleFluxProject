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
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetLoanQueryHandler : IRequestHandler<GetLoanQuery, ServiceResponse<LoanDto>>
    {
        private readonly ILoanRepository _LoanRepository; // Repository for accessing Loan data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLoanQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetLoanQueryHandler.
        /// </summary>
        /// <param name="LoanRepository">Repository for Loan data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLoanQueryHandler(
            ILoanRepository LoanRepository,
            IMapper mapper,
            ILogger<GetLoanQueryHandler> logger)
        {
            _LoanRepository = LoanRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetLoanQuery to retrieve a specific Loan.
        /// </summary>
        /// <param name="request">The GetLoanQuery containing Loan ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanDto>> Handle(GetLoanQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Loan entity with the specified ID from the repository
                var entity = await _LoanRepository.AllIncluding(x => x.LoanApplication, x => x.LoanAmortizations, x => x.Refunds, x => x.DailyInterestCalculations, x => x.DisburstedLoans)
                    .Include(x => x.LoanApplication.LoanCommiteeValidations)
                    .Include(x => x.LoanApplication.LoanProduct)
                    .Include(x => x.LoanApplication.LoanProduct.LoanProductRepaymentCycles)
                    .Include(x => x.LoanApplication.Collateras)
                    .Include(x => x.LoanApplication.Guarantors)
                    .Include(x => x.LoanApplication.LoanPurpose)
                    .Include(x => x.LoanApplication.LoanApplicationFees)
                    .Include(x => x.DisburstedLoans)
                    .Include(x => x.LoanApplication.DocumentAttachedToLoans).FirstOrDefaultAsync(x => x.Id == request.Id);
                if (entity != null)
                {
                    // Map the Loan entity to LoanDto and return it with a success response
                    var LoanDto = _mapper.Map<LoanDto>(entity);
                    return ServiceResponse<LoanDto>.ReturnResultWith200(LoanDto);
                }
                else
                {
                    // If the Loan entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Loan not found.");
                    return ServiceResponse<LoanDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Loan: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanDto>.Return500(e);
            }
        }
    }

}
