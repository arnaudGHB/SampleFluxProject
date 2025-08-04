using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.RescheduleLoanMediaR.Queries;
using CBS.NLoan.Repository.LoanApplicationP.RescheduleLoanP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.RescheduleLoanMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllRescheduleLoanHandler : IRequestHandler<GetAllRescheduleLoanQuery, ServiceResponse<List<RescheduleLoanDto>>>
    {
        private readonly IRescheduleLoanRepository _RescheduleLoanRepository; // Repository for accessing RescheduleLoans data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllRescheduleLoanHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllRescheduleLoanQueryHandler.
        /// </summary>
        /// <param name="RescheduleLoanRepository">Repository for RescheduleLoans data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllRescheduleLoanHandler(
            IRescheduleLoanRepository RescheduleLoanRepository,
            IMapper mapper, ILogger<GetAllRescheduleLoanHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _RescheduleLoanRepository = RescheduleLoanRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllRescheduleLoanQuery to retrieve all RescheduleLoans.
        /// </summary>
        /// <param name="request">The GetAllRescheduleLoanQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<RescheduleLoanDto>>> Handle(GetAllRescheduleLoanQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all RescheduleLoans entities from the repository
                var entities = await _RescheduleLoanRepository.All.ToListAsync();
                return ServiceResponse<List<RescheduleLoanDto>>.ReturnResultWith200(_mapper.Map<List<RescheduleLoanDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all RescheduleLoans: {e.Message}");
                return ServiceResponse<List<RescheduleLoanDto>>.Return500(e, "Failed to get all RescheduleLoans");
            }
        }
    }
}
