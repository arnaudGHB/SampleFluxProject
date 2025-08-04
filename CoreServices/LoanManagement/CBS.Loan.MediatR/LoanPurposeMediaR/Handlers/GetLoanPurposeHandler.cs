using AutoMapper;
using CBS.NLoan.Data.Dto.LoanPurposeP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanPurposeMediaR.Queries;
using CBS.NLoan.Repository.LoanPurposeP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanPurposeMediaR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetLoanPurposeHandler : IRequestHandler<GetLoanPurposeQuery, ServiceResponse<LoanPurposeDto>>
    {
        private readonly ILoanPurposeRepository _LoanPurposeRepository; // Repository for accessing LoanPurpose data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLoanPurposeHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetLoanPurposeQueryHandler.
        /// </summary>
        /// <param name="LoanPurposeRepository">Repository for LoanPurpose data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLoanPurposeHandler(
            ILoanPurposeRepository LoanPurposeRepository,
            IMapper mapper,
            ILogger<GetLoanPurposeHandler> logger)
        {
            _LoanPurposeRepository = LoanPurposeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetLoanPurposeQuery to retrieve a specific LoanPurpose.
        /// </summary>
        /// <param name="request">The GetLoanPurposeQuery containing LoanPurpose ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanPurposeDto>> Handle(GetLoanPurposeQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the LoanPurpose entity with the specified ID from the repository
                var entity = await _LoanPurposeRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the LoanPurpose entity to LoanPurposeDto and return it with a success response
                    var LoanPurposeDto = _mapper.Map<LoanPurposeDto>(entity);
                    return ServiceResponse<LoanPurposeDto>.ReturnResultWith200(LoanPurposeDto);
                }
                else
                {
                    // If the LoanPurpose entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("LoanPurpose not found.");
                    return ServiceResponse<LoanPurposeDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting LoanPurpose: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanPurposeDto>.Return500(e);
            }
        }
    }

}
