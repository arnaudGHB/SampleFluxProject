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
    /// Handles the retrieval of all BudgetAdjustment based on the GetAllBudgetAdjustmentQuery.
    /// </summary>
    public class GetAllBudgetAdjustmentQueryHandler : IRequestHandler<GetAllBudgetAdjustmentQuery, ServiceResponse<List<BudgetAdjustmentDto>>>
    {
        private readonly IBudgetAdjustmentRepository _assetRepository; // Repository for accessing Asset data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllBudgetAdjustmentQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllBudgetAdjustmentQueryHandler.
        /// </summary>
        /// <param name="assetRepository">Repository for Asset data access.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllBudgetAdjustmentQueryHandler(
            IBudgetAdjustmentRepository assetRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetAllBudgetAdjustmentQueryHandler> logger)
        {
            _assetRepository = assetRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllBudgetAdjustmentQuery to retrieve all BudgetAdjustment.
        /// </summary>
        /// <param name="request">The GetAllBudgetAdjustmentQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<BudgetAdjustmentDto>>> Handle(GetAllBudgetAdjustmentQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve all Asset entities from the repository
                var entities = await _assetRepository.All.ToListAsync(cancellationToken);

                // Step 2: Log the successful retrieval of BudgetAdjustment
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "BudgetAdjustment returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Step 3: Return the result with a successful response
                return ServiceResponse<List<BudgetAdjustmentDto>>.ReturnResultWith200(_mapper.Map<List<BudgetAdjustmentDto>>(entities));
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve BudgetAdjustment: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<BudgetAdjustmentDto>>.Return500(e, "Failed to retrieve BudgetAdjustment");
            }
        }
    }
}
