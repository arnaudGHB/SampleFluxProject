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
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetLoanAmortizationHandler : IRequestHandler<GetLoanAmortizationQuery, ServiceResponse<LoanAmortizationDto>>
    {
        private readonly ILoanAmortizationRepository _LoanAmortizationRepository; // Repository for accessing LoanAmortization data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLoanAmortizationHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetLoanAmortizationQueryHandler.
        /// </summary>
        /// <param name="LoanAmortizationRepository">Repository for LoanAmortization data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLoanAmortizationHandler(
            ILoanAmortizationRepository LoanAmortizationRepository,
            IMapper mapper,
            ILogger<GetLoanAmortizationHandler> logger)
        {
            _LoanAmortizationRepository = LoanAmortizationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetLoanAmortizationQuery to retrieve a specific LoanAmortization.
        /// </summary>
        /// <param name="request">The GetLoanAmortizationQuery containing LoanAmortization ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanAmortizationDto>> Handle(GetLoanAmortizationQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the LoanAmortization entity with the specified ID from the repository
                var entity = await _LoanAmortizationRepository.AllIncluding(x=>x.Loan,x=>x.RefundDetails).FirstOrDefaultAsync(x=>x.Id==request.Id);
                if (entity != null)
                {
                    // Map the LoanAmortization entity to LoanAmortizationDto and return it with a success response
                    var LoanAmortizationDto = _mapper.Map<LoanAmortizationDto>(entity);
                    return ServiceResponse<LoanAmortizationDto>.ReturnResultWith200(LoanAmortizationDto);
                }
                else
                {
                    // If the LoanAmortization entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("LoanAmortization not found.");
                    return ServiceResponse<LoanAmortizationDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting LoanAmortization: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanAmortizationDto>.Return500(e);
            }
        }
    }

}
