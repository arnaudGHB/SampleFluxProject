using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.RescheduleLoanMediaR.Queries;
using CBS.NLoan.Repository.LoanApplicationP.RescheduleLoanP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.RescheduleLoanMediaR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetRescheduleLoanHandler : IRequestHandler<GetRescheduleLoanQuery, ServiceResponse<RescheduleLoanDto>>
    {
        private readonly IRescheduleLoanRepository _RescheduleLoanRepository; // Repository for accessing RescheduleLoan data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetRescheduleLoanHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetRescheduleLoanQueryHandler.
        /// </summary>
        /// <param name="RescheduleLoanRepository">Repository for RescheduleLoan data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetRescheduleLoanHandler(
            IRescheduleLoanRepository RescheduleLoanRepository,
            IMapper mapper,
            ILogger<GetRescheduleLoanHandler> logger)
        {
            _RescheduleLoanRepository = RescheduleLoanRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetRescheduleLoanQuery to retrieve a specific RescheduleLoan.
        /// </summary>
        /// <param name="request">The GetRescheduleLoanQuery containing RescheduleLoan ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<RescheduleLoanDto>> Handle(GetRescheduleLoanQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the RescheduleLoan entity with the specified ID from the repository
                var entity = await _RescheduleLoanRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the RescheduleLoan entity to RescheduleLoanDto and return it with a success response
                    var RescheduleLoanDto = _mapper.Map<RescheduleLoanDto>(entity);
                    return ServiceResponse<RescheduleLoanDto>.ReturnResultWith200(RescheduleLoanDto);
                }
                else
                {
                    // If the RescheduleLoan entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("RescheduleLoan not found.");
                    return ServiceResponse<RescheduleLoanDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting RescheduleLoan: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<RescheduleLoanDto>.Return500(e);
            }
        }
    }

}
