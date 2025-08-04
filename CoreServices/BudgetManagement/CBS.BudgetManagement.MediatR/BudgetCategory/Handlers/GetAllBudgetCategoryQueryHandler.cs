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
    /// Handles the retrieval of all BudgetCategory based on the GetAllBudgetCategoryQuery.
    /// </summary>
    public class GetAllBudgetCategoryQueryHandler : IRequestHandler<GetAllBudgetCategoryrsQuery, ServiceResponse<List<BudgetCategoryDto>>>
    {
        private readonly IBudgetCategoryRepository _BudgetCategoryRepository; // Repository for accessing BudgetCategory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllBudgetCategoryQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllBudgetCategoryQueryHandler.
        /// </summary>
        /// <param name="BudgetCategoryRepository">Repository for BudgetCategory data access.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllBudgetCategoryQueryHandler(
            IBudgetCategoryRepository BudgetCategoryRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetAllBudgetCategoryQueryHandler> logger)
        {
            _BudgetCategoryRepository = BudgetCategoryRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllBudgetCategoryQuery to retrieve all BudgetCategory.
        /// </summary>
        /// <param name="request">The GetAllBudgetCategoryQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<BudgetCategoryDto>>> Handle(GetAllBudgetCategoryrsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve all BudgetCategory entities from the repository
                var entities = await _BudgetCategoryRepository.All.ToListAsync(cancellationToken);

                // Step 2: Log the successful retrieval of BudgetCategory
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "BudgetCategory returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Step 3: Return the result with a successful response
                return ServiceResponse<List<BudgetCategoryDto>>.ReturnResultWith200(_mapper.Map<List<BudgetCategoryDto>>(entities));
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve BudgetCategory: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<BudgetCategoryDto>>.Return500(e, "Failed to retrieve BudgetCategory");
            }
        }
    }
}
