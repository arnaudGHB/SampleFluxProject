using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BudgetManagement.MediatR.Queries;
using CBS.BudgetManagement.Repository;
using CBS.BudgetManagement.Helper;
using CBS.BudgetManagement.Data;
using System.Threading;
using System.Threading.Tasks;

namespace CBS.BudgetManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of BudgetPlan based on the GetBudgetPlanQuery.
    /// </summary>
    public class GetBudgetPlanQueryHandler : IRequestHandler<GetBudgetPlanQuery, ServiceResponse<BudgetPlanDto>>
    {
        private readonly IBudgetPlanRepository _BudgetPlanRepository; // Repository for accessing BudgetPlan data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetBudgetPlanQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetBudgetPlanQueryHandler.
        /// </summary>
        /// <param name="BudgetPlanRepository">Repository for BudgetPlan data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        public GetBudgetPlanQueryHandler(
            IBudgetPlanRepository BudgetPlanRepository,
            UserInfoToken userInfoToken,
            IMapper mapper, ILogger<GetBudgetPlanQueryHandler> logger)
        {
            _BudgetPlanRepository = BudgetPlanRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetBudgetPlanQuery to retrieve an BudgetPlan.
        /// </summary>
        /// <param name="request">The GetBudgetPlanQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BudgetPlanDto>> Handle(GetBudgetPlanQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the BudgetPlan entity from the repository
                var entity = await _BudgetPlanRepository.FindAsync(request.Id);

                // Step 2: Check if the entity exists
                if (entity == null)
                {
                    string notFoundMessage = $"BudgetPlan with ID '{request.Id}' not found.";
                    _logger.LogWarning(notFoundMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, notFoundMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<BudgetPlanDto>.Return404(notFoundMessage);
                }

                // Step 3: Log success and return the mapped entity as a DTO
                string successMessage = "BudgetPlan returned successfully.";
                _logger.LogInformation(successMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                var BudgetPlanDto = _mapper.Map<BudgetPlanDto>(entity);
                return ServiceResponse<BudgetPlanDto>.ReturnResultWith200(BudgetPlanDto);
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve BudgetPlan: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<BudgetPlanDto>.Return500(e, "Failed to retrieve BudgetPlan");
            }
        }
    }
}
