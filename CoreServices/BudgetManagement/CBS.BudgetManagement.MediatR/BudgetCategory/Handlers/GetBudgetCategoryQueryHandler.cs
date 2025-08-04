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
    /// Handles the retrieval of BudgetCategory based on the GetBudgetCategoryQuery.
    /// </summary>
    public class GetBudgetCategoryQueryHandler : IRequestHandler<GetBudgetCategoryQuery, ServiceResponse<BudgetCategoryDto>>
    {
        private readonly IBudgetCategoryRepository _BudgetCategoryRepository; // Repository for accessing BudgetCategory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetBudgetCategoryQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetBudgetCategoryQueryHandler.
        /// </summary>
        /// <param name="BudgetCategoryRepository">Repository for BudgetCategory data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        public GetBudgetCategoryQueryHandler(
            IBudgetCategoryRepository BudgetCategoryRepository,
            UserInfoToken userInfoToken,
            IMapper mapper, ILogger<GetBudgetCategoryQueryHandler> logger)
        {
            _BudgetCategoryRepository = BudgetCategoryRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetBudgetCategoryQuery to retrieve an BudgetCategory.
        /// </summary>
        /// <param name="request">The GetBudgetCategoryQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BudgetCategoryDto>> Handle(GetBudgetCategoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the BudgetCategory entity from the repository
                var entity = await _BudgetCategoryRepository.FindAsync(request.Id);

                // Step 2: Check if the entity exists
                if (entity == null)
                {
                    string notFoundMessage = $"BudgetCategory with ID '{request.Id}' not found.";
                    _logger.LogWarning(notFoundMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, notFoundMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<BudgetCategoryDto>.Return404(notFoundMessage);
                }

                // Step 3: Log success and return the mapped entity as a DTO
                string successMessage = "BudgetCategory returned successfully.";
                _logger.LogInformation(successMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                var BudgetCategoryDto = _mapper.Map<BudgetCategoryDto>(entity);
                return ServiceResponse<BudgetCategoryDto>.ReturnResultWith200(BudgetCategoryDto);
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve BudgetCategory: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<BudgetCategoryDto>.Return500(e, "Failed to retrieve BudgetCategory");
            }
        }
    }
}
