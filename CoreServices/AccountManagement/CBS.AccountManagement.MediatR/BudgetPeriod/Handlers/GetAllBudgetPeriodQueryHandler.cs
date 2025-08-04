using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all BudgetPeriods based on the GetAllBudgetPeriodQuery.
    /// </summary>
    public class GetAllBudgetPeriodQueryHandler : IRequestHandler<GetAllBudgetPeriodQuery, ServiceResponse<List<BudgetPeriodDto>>>
    {
        private readonly IBudgetPeriodRepository _BudgetPeriodRepository; // Repository for accessing BudgetPeriods data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllBudgetPeriodQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllBudgetPeriodQueryHandler.
        /// </summary>
        /// <param name="BudgetPeriodRepository">Repository for BudgetPeriods data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllBudgetPeriodQueryHandler(
            IBudgetPeriodRepository BudgetPeriodRepository,
            IMapper mapper, ILogger<GetAllBudgetPeriodQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _BudgetPeriodRepository = BudgetPeriodRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllBudgetPeriodQuery to retrieve all BudgetPeriods.
        /// </summary>
        /// <param name="request">The GetAllBudgetPeriodQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<BudgetPeriodDto>>> Handle(GetAllBudgetPeriodQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all BudgetPeriods entities from the repository
                var entities = await _BudgetPeriodRepository.All.Where(x => x.IsDeleted.Equals(false)).ToListAsync();
                return ServiceResponse<List<BudgetPeriodDto>>.ReturnResultWith200(_mapper.Map<List<BudgetPeriodDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all BudgetPeriods: {e.Message}");
                return ServiceResponse<List<BudgetPeriodDto>>.Return500(e, "Failed to get all BudgetPeriods");
            }
        }
    }
}