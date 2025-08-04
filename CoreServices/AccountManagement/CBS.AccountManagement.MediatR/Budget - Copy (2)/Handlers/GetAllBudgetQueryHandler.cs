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
    /// Handles the retrieval of all Budget based on the GetAllBudgetNameQuery.
    /// </summary>
    public class GetAllBudgetQueryHandler : IRequestHandler<GetAllBudgetQuery, ServiceResponse<List<BudgetDto>>>
    {
        private readonly IBudgetRepository _BudgetRepository; // Repository for accessing Budget data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllBudgetQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the GetAllBudgetQueryHandler.
        /// </summary>
        /// <param name="BudgetRepository">Repository for BudgetName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllBudgetQueryHandler(
            IBudgetRepository BudgetRepository,
            IMapper mapper, ILogger<GetAllBudgetQueryHandler> logger, UserInfoToken? userInfoToken)
        {
            // Assign provided dependencies to local variables.
            _BudgetRepository = BudgetRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetAllBudgetNameQuery to retrieve all BudgetName.
        /// </summary>
        /// <param name="request">The GetAllBudgetNameQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<BudgetDto>>> Handle(GetAllBudgetQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = string.Empty;
            try
            {
                // Retrieve all Budget entities from the repository
                var entities = await _BudgetRepository.All.Where(c=>c.IsDeleted == false && c.BranchId == _userInfoToken.BranchId).ToListAsync();
                 errorMessage = $"Budget : {entities.Count()} has been successfully.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllBudgetQuery",
request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<List<BudgetDto>>.ReturnResultWith200(_mapper.Map<List<BudgetDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Budgets: {e.Message}");
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllBudgetQuery",
request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<BudgetDto>>.Return500(e, "Failed to get all BudgetName");
            }
        }
    }
}