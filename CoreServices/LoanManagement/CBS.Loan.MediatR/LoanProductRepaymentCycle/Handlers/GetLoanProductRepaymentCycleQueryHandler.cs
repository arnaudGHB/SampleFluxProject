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
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetLoanProductRepaymentCycleQueryHandler : IRequestHandler<GetLoanProductRepaymentCycleQuery, ServiceResponse<LoanProductRepaymentCycleDto>>
    {
        private readonly ILoanProductRepaymentCycleRepository _LoanProductRepaymentCycleRepository; // Repository for accessing LoanProductRepaymentCycle data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLoanProductRepaymentCycleQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetLoanProductRepaymentCycleQueryHandler.
        /// </summary>
        /// <param name="LoanProductRepaymentCycleRepository">Repository for LoanProductRepaymentCycle data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLoanProductRepaymentCycleQueryHandler(
            ILoanProductRepaymentCycleRepository LoanProductRepaymentCycleRepository,
            IMapper mapper,
            ILogger<GetLoanProductRepaymentCycleQueryHandler> logger)
        {
            _LoanProductRepaymentCycleRepository = LoanProductRepaymentCycleRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetLoanProductRepaymentCycleQuery to retrieve a specific LoanProductRepaymentCycle.
        /// </summary>
        /// <param name="request">The GetLoanProductRepaymentCycleQuery containing LoanProductRepaymentCycle ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanProductRepaymentCycleDto>> Handle(GetLoanProductRepaymentCycleQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the LoanProductRepaymentCycle entity with the specified ID from the repository
                var entity = await _LoanProductRepaymentCycleRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the LoanProductRepaymentCycle entity to LoanProductRepaymentCycleDto and return it with a success response
                    var LoanProductRepaymentCycleDto = _mapper.Map<LoanProductRepaymentCycleDto>(entity);
                    return ServiceResponse<LoanProductRepaymentCycleDto>.ReturnResultWith200(LoanProductRepaymentCycleDto);
                }
                else
                {
                    // If the LoanProductRepaymentCycle entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("LoanProductRepaymentCycle not found.");
                    return ServiceResponse<LoanProductRepaymentCycleDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting LoanProductRepaymentCycle: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanProductRepaymentCycleDto>.Return500(e);
            }
        }
    }

}
