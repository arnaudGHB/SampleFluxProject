using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
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
    public class GetAllTrailBalanceUploudHandler : IRequestHandler<GetAllTrailBalanceUploudQuery, ServiceResponse<List<TrailBalanceUploudDto>>>
    {
        private readonly ITrailBalanceUploudRepository _trailBalanceUploudRepository; // Repository for accessing Budget data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllBudgetQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the GetAllBudgetQueryHandler.
        /// </summary>
        /// <param name="BudgetRepository">Repository for BudgetName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllTrailBalanceUploudHandler(
            ITrailBalanceUploudRepository BudgetRepository,
            IMapper mapper, ILogger<GetAllBudgetQueryHandler> logger, UserInfoToken? userInfoToken)
        {
            // Assign provided dependencies to local variables.
            _trailBalanceUploudRepository = BudgetRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetAllBudgetNameQuery to retrieve all BudgetName.
        /// </summary>
        /// <param name="request">The GetAllBudgetNameQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<TrailBalanceUploudDto>>> Handle(GetAllTrailBalanceUploudQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = string.Empty;
            try
            {
                // Retrieve all Budget entities from the repository
                if (_userInfoToken.IsHeadOffice)
                {
                    var entities = await _trailBalanceUploudRepository.All.Where(c => c.IsDeleted == false ).ToListAsync();
                    errorMessage = $"Budget : {entities.Count()} has been successfully.";
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllBudgetQuery",
    request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                    return ServiceResponse<List<TrailBalanceUploudDto>>.ReturnResultWith200(_mapper.Map<List<TrailBalanceUploudDto>>(entities));
                }
                else
                {
                    var entities = await _trailBalanceUploudRepository.All.Where(c => c.IsDeleted == false && c.BranchId == _userInfoToken.BranchId).ToListAsync();
                    errorMessage = $"_trailBalance : {entities.Count()} has been successfully.";
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllBudgetQuery",
    request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                    return ServiceResponse<List<TrailBalanceUploudDto>>.ReturnResultWith200(_mapper.Map<List<TrailBalanceUploudDto>>(entities));
                }
              
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Budgets: {e.Message}");
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllBudgetQuery",
request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<TrailBalanceUploudDto>>.Return500(e, "Failed to get all BudgetName");
            }
        }
    }
}