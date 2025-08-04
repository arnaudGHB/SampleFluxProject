using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BudgetManagement.MediatR.Queries;
using CBS.BudgetManagement.Repository;
using CBS.BudgetManagement.Helper;
using CBS.BudgetManagement.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CBS.BudgetManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all BudgetPlan based on the GetAllBudgetPlanQuery.
    /// </summary>
    public class GetAllBudgetPlanQueryHandler : IRequestHandler<GetAllBudgetPlanQuery, ServiceResponse<List<BudgetPlanDto>>>
    {
        private readonly IBudgetPlanRepository _BudgetPlanRepository; // Repository for accessing BudgetPlan data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllBudgetPlanQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllBudgetPlanQueryHandler.
        /// </summary>
        /// <param name="BudgetPlanRepository">Repository for BudgetPlan data access.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllBudgetPlanQueryHandler(
            IBudgetPlanRepository BudgetPlanRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetAllBudgetPlanQueryHandler> logger)
        {
            _BudgetPlanRepository = BudgetPlanRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllBudgetPlanQuery to retrieve all BudgetPlan.
        /// </summary>
        /// <param name="request">The GetAllBudgetPlanQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<BudgetPlanDto>>> Handle(GetAllBudgetPlanQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve all BudgetPlan entities from the repository
                var entities = await _BudgetPlanRepository.All.ToListAsync(cancellationToken);

                // Step 2: Log the successful retrieval of BudgetPlan
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "BudgetPlan returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Step 3: Return the result with a successful response
                return ServiceResponse<List<BudgetPlanDto>>.ReturnResultWith200(_mapper.Map<List<BudgetPlanDto>>(entities));
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve BudgetPlan: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<BudgetPlanDto>>.Return500(e, "Failed to retrieve BudgetPlan");
            }
        }
    }
}
