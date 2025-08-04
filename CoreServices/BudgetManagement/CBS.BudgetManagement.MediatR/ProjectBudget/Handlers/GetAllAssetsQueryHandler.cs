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
    /// Handles the retrieval of all ProjectBudgets based on the GetAllProjectBudgetsQuery.
    /// </summary>
    public class GetAllProjectBudgetsQueryHandler : IRequestHandler<GetAllProjectBudgetsQuery, ServiceResponse<List<ProjectBudgetDto>>>
    {
        private readonly IProjectBudgetRepository _ProjectBudgetRepository; // Repository for accessing ProjectBudget data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllProjectBudgetsQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllProjectBudgetsQueryHandler.
        /// </summary>
        /// <param name="ProjectBudgetRepository">Repository for ProjectBudget data access.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllProjectBudgetsQueryHandler(
            IProjectBudgetRepository ProjectBudgetRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetAllProjectBudgetsQueryHandler> logger)
        {
            _ProjectBudgetRepository = ProjectBudgetRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllProjectBudgetsQuery to retrieve all ProjectBudgets.
        /// </summary>
        /// <param name="request">The GetAllProjectBudgetsQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<ProjectBudgetDto>>> Handle(GetAllProjectBudgetsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve all ProjectBudget entities from the repository
                var entities = await _ProjectBudgetRepository.All.ToListAsync(cancellationToken);

                // Step 2: Log the successful retrieval of ProjectBudgets
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "ProjectBudgets returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Step 3: Return the result with a successful response
                return ServiceResponse<List<ProjectBudgetDto>>.ReturnResultWith200(_mapper.Map<List<ProjectBudgetDto>>(entities));
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve ProjectBudgets: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<ProjectBudgetDto>>.Return500(e, "Failed to retrieve ProjectBudgets");
            }
        }
    }
}
