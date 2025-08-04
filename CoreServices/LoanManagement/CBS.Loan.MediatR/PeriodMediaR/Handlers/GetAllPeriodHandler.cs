using AutoMapper;
using CBS.NLoan.Data.Dto.PeriodP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.PeriodMediaR.Queries;
using CBS.NLoan.Repository.PeriodP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.PeriodMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllPeriodHandler : IRequestHandler<GetAllPeriodQuery, ServiceResponse<List<PeriodDto>>>
    {
        private readonly IPeriodRepository _PeriodRepository; // Repository for accessing Periods data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllPeriodHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllPeriodQueryHandler.
        /// </summary>
        /// <param name="PeriodRepository">Repository for Periods data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllPeriodHandler(
            IPeriodRepository PeriodRepository,
            IMapper mapper, ILogger<GetAllPeriodHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _PeriodRepository = PeriodRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllPeriodQuery to retrieve all Periods.
        /// </summary>
        /// <param name="request">The GetAllPeriodQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<PeriodDto>>> Handle(GetAllPeriodQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Periods entities from the repository
                var entities = await _PeriodRepository.All.ToListAsync();
                return ServiceResponse<List<PeriodDto>>.ReturnResultWith200(_mapper.Map<List<PeriodDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Periods: {e.Message}");
                return ServiceResponse<List<PeriodDto>>.Return500(e, "Failed to get all Periods");
            }
        }
    }
}
