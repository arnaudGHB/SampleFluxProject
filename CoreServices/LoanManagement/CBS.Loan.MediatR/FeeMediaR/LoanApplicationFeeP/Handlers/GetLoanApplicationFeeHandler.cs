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
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetLoanApplicationFeeHandler : IRequestHandler<GetLoanApplicationFeeQuery, ServiceResponse<LoanApplicationFeeDto>>
    {
        private readonly ILoanApplicationFeeRepository _loanApplicationFeeRepository; // Repository for accessing LoanApplicationFee data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLoanApplicationFeeHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetLoanApplicationFeeHandler.
        /// </summary>
        /// <param name="loanApplicationFeeRepository">Repository for LoanApplicationFee data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLoanApplicationFeeHandler(
            ILoanApplicationFeeRepository loanApplicationFeeRepository,
            IMapper mapper,
            ILogger<GetLoanApplicationFeeHandler> logger)
        {
            _loanApplicationFeeRepository = loanApplicationFeeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetLoanApplicationFeeQuery to retrieve a specific LoanApplicationFee.
        /// </summary>
        /// <param name="request">The GetLoanApplicationFeeQuery containing LoanApplicationFee ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanApplicationFeeDto>> Handle(GetLoanApplicationFeeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the LoanApplicationFee entity with the specified ID from the repository
                var entity = await _loanApplicationFeeRepository.FindBy(x=>x.Id==request.Id).Include(x=>x.LoanApplication).Include(x=>x.FeeRange).Include(x=>x.LoanApplication.LoanProduct).FirstOrDefaultAsync();
                if (entity != null)
                {
                    // Map the LoanApplicationFee entity to LoanApplicationFeeDto and return it with a success response
                    var loanApplicationFeeDto = _mapper.Map<LoanApplicationFeeDto>(entity);
                    return ServiceResponse<LoanApplicationFeeDto>.ReturnResultWith200(loanApplicationFeeDto);
                }
                else
                {
                    // If the LoanApplicationFee entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("LoanApplicationFee not found.");
                    return ServiceResponse<LoanApplicationFeeDto>.Return404();
                }
            }
            catch (Exception e)
            {
                // Log the error and return a 500 Internal Server Error response with the error message
                var errorMessage = $"Error occurred while getting LoanApplicationFee: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanApplicationFeeDto>.Return500(e);
            }
        }
    }

}
