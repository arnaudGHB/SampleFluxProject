using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanCommentryMediaR.Queries;
using CBS.NLoan.Repository.LoanApplicationP.LoanCommentryP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanCommentryMediaR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetLoanCommentryHandler : IRequestHandler<GetLoanCommentryQuery, ServiceResponse<LoanCommentryDto>>
    {
        private readonly ILoanCommentryRepository _LoanCommentryRepository; // Repository for accessing LoanCommentry data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLoanCommentryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetLoanCommentryQueryHandler.
        /// </summary>
        /// <param name="LoanCommentryRepository">Repository for LoanCommentry data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLoanCommentryHandler(
            ILoanCommentryRepository LoanCommentryRepository,
            IMapper mapper,
            ILogger<GetLoanCommentryHandler> logger)
        {
            _LoanCommentryRepository = LoanCommentryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetLoanCommentryQuery to retrieve a specific LoanCommentry.
        /// </summary>
        /// <param name="request">The GetLoanCommentryQuery containing LoanCommentry ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanCommentryDto>> Handle(GetLoanCommentryQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the LoanCommentry entity with the specified ID from the repository
                var entity = await _LoanCommentryRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the LoanCommentry entity to LoanCommentryDto and return it with a success response
                    var LoanCommentryDto = _mapper.Map<LoanCommentryDto>(entity);
                    return ServiceResponse<LoanCommentryDto>.ReturnResultWith200(LoanCommentryDto);
                }
                else
                {
                    // If the LoanCommentry entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("LoanCommentry not found.");
                    return ServiceResponse<LoanCommentryDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting LoanCommentry: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanCommentryDto>.Return500(e);
            }
        }
    }

}
