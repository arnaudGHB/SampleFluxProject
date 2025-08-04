using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Queries;
using CBS.NLoan.Repository.LoanApplicationP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;

namespace CBS.NLoan.MediatR.LoanApplicationMediaR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetLoanApplicationQueryHandler : IRequestHandler<GetLoanApplicationQuery, ServiceResponse<LoanApplicationDto>>
    {
        private readonly ILoanApplicationRepository _loanApplicationRepository; // Repository for accessing Loan data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLoanApplicationQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetLoanQueryHandler.
        /// </summary>
        /// <param name="LoanRepository">Repository for Loan data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLoanApplicationQueryHandler(
            ILoanApplicationRepository LoanRepository,
            IMapper mapper,
            ILogger<GetLoanApplicationQueryHandler> logger)
        {
            _loanApplicationRepository = LoanRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetLoanQuery to retrieve a specific Loan.
        /// </summary>
        /// <param name="request">The GetLoanQuery containing Loan ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanApplicationDto>> Handle(GetLoanApplicationQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Loan entity with the specified ID from the repository
                var entity = await _loanApplicationRepository.AllIncluding(
                    x => x.LoanProduct,
                    x => x.LoanProduct.LoanTerm,
                    x => x.LoanCommiteeValidations,
                    x => x.DocumentAttachedToLoans,
                    x => x.LoanPurpose,
                    x => x.Collateras,
                    x => x.Guarantors,
                    x => x.LoanProduct.LoanProductCategory,
                    x => x.LoanProduct.LoanProductCollaterals,
                    x => x.LoanProduct.LoanProductRepaymentCycles,
                    x => x.Loans
                ).Include(x => x.LoanApplicationFees).ThenInclude(x => x.FeeRange)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

                if (entity != null)
                {
                    // Map the Loan entity to LoanDto and return it with a success response
                    var LoanDto = _mapper.Map<LoanApplicationDto>(entity);
                    return ServiceResponse<LoanApplicationDto>.ReturnResultWith200(LoanDto);
                }
                else
                {
                    // If the Loan entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Loan not found.");
                    return ServiceResponse<LoanApplicationDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Loan: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanApplicationDto>.Return500(e);
            }
        }
    }

}
