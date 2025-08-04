using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanProductMediaR.Queries;
using CBS.NLoan.Repository.LoanApplicationP.LoanCommentryP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanProductMediaR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve loan product repayment orders by product ID.
    /// </summary>
    public class GetLoanProductRepaymentCycleByProductIDQueryHandler : IRequestHandler<GetLoanProductRepaymentCycleByProductIDQuery, ServiceResponse<List<LoanProductRepaymentCycleDto>>>
    {
        private readonly ILoanProductRepaymentCycleRepository _LoanProductRepository; // Repository for accessing LoanProduct data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLoanProductRepaymentCycleByProductIDQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetLoanProductRepaymentCycleByProductIDQueryHandler.
        /// </summary>
        /// <param name="LoanProductRepository">Repository for LoanProduct data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLoanProductRepaymentCycleByProductIDQueryHandler(
            ILoanProductRepaymentCycleRepository LoanProductRepository,
            IMapper mapper,
            ILogger<GetLoanProductRepaymentCycleByProductIDQueryHandler> logger)
        {
            _LoanProductRepository = LoanProductRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetLoanProductRepaymentCycleByProductIDQuery to retrieve loan product repayment orders by product ID.
        /// </summary>
        /// <param name="request">The GetLoanProductRepaymentCycleByProductIDQuery containing the product ID.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LoanProductRepaymentCycleDto>>> Handle(GetLoanProductRepaymentCycleByProductIDQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the LoanProductRepaymentCycle entities with the specified product ID from the repository
                var entities = await _LoanProductRepository.FindBy(x => x.LoanProductId == request.Id).ToListAsync();
                if (entities != null)
                {
                    // Map the LoanProductRepaymentCycle entities to LoanProductRepaymentCycleDto and return them with a success response
                    var LoanProductRepaymentCycleDtos = _mapper.Map<List<LoanProductRepaymentCycleDto>>(entities);
                    return ServiceResponse<List<LoanProductRepaymentCycleDto>>.ReturnResultWith200(LoanProductRepaymentCycleDtos);
                }
                else
                {
                    // If the LoanProductRepaymentCycle entities were not found, log the error and return a 404 Not Found response
                    _logger.LogError("LoanProductRepaymentCycles not found.");
                    return ServiceResponse<List<LoanProductRepaymentCycleDto>>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting LoanProductRepaymentCycles: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<List<LoanProductRepaymentCycleDto>>.Return500(e);
            }
        }
    }

}
