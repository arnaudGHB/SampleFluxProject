using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all BudgetCategory based on the GetAllBudgetCategoryNameQuery.
    /// </summary>
    public class GetAllBudgetCategoryQueryHandler : IRequestHandler<GetAllBudgetCategoryQuery, ServiceResponse<List<BudgetCategoryDto>>>
    {
        private readonly IBudgetCategoryRepository _BudgetCategoryRepository; // Repository for accessing BudgetCategory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllBudgetCategoryQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the GetAllBudgetCategoryQueryHandler.
        /// </summary>
        /// <param name="BudgetCategoryRepository">Repository for BudgetCategoryName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllBudgetCategoryQueryHandler(
            IBudgetCategoryRepository BudgetCategoryRepository,
            IMapper mapper, ILogger<GetAllBudgetCategoryQueryHandler> logger, UserInfoToken? userInfoToken)
        {
            // Assign provided dependencies to local variables.
            _BudgetCategoryRepository = BudgetCategoryRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetAllBudgetCategoryNameQuery to retrieve all BudgetCategoryName.
        /// </summary>
        /// <param name="request">The GetAllBudgetCategoryNameQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<BudgetCategoryDto>>> Handle(GetAllBudgetCategoryQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = string.Empty;
            try
            {
                // Retrieve all BudgetCategory entities from the repository
                var entities = await _BudgetCategoryRepository.All.Where(x=>x.IsDeleted.Equals(false)&&x.BranchId==_userInfoToken.BranchId).ToListAsync();
                 errorMessage = $"BudgetCategory : {entities.Count()} has been successfully.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllBudgetCategoryQuery",
request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<List<BudgetCategoryDto>>.ReturnResultWith200(_mapper.Map<List<BudgetCategoryDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all BudgetCategorys: {e.Message}");
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllBudgetCategoryQuery",
request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<BudgetCategoryDto>>.Return500(e, "Failed to get all BudgetCategoryName");
            }
        }
    }
}