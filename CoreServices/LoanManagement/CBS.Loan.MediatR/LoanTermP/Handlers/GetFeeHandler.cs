using AutoMapper;
using CBS.NLoan.Data.Dto.LoanTermP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanTermP.Queries;
using CBS.NLoan.Repository.LoanTermP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanTermP.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetLoanTermHandler : IRequestHandler<GetLoanTermQuery, ServiceResponse<LoanTermDto>>
    {
        private readonly ILoanTermRepository _LoanTermRepository; // Repository for accessing LoanTerm data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLoanTermHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetLoanTermQueryHandler.
        /// </summary>
        /// <param name="LoanTermRepository">Repository for LoanTerm data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLoanTermHandler(
            ILoanTermRepository LoanTermRepository,
            IMapper mapper,
            ILogger<GetLoanTermHandler> logger)
        {
            _LoanTermRepository = LoanTermRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetLoanTermQuery to retrieve a specific LoanTerm.
        /// </summary>
        /// <param name="request">The GetLoanTermQuery containing LoanTerm ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanTermDto>> Handle(GetLoanTermQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the LoanTerm entity with the specified ID from the repository
                var entity = await _LoanTermRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the LoanTerm entity to LoanTermDto and return it with a success response
                    var LoanTermDto = _mapper.Map<LoanTermDto>(entity);
                    return ServiceResponse<LoanTermDto>.ReturnResultWith200(LoanTermDto);
                }
                else
                {
                    // If the LoanTerm entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("LoanTerm not found.");
                    return ServiceResponse<LoanTermDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting LoanTerm: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanTermDto>.Return500(e);
            }
        }
    }

}
