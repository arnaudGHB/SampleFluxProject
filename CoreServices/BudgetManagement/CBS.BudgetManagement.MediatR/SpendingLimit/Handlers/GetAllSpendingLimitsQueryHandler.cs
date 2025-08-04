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
    /// Handles the retrieval of all SpendingLimits based on the GetAllSpendingLimitsQuery.
    /// </summary>
    public class GetAllSpendingLimitsQueryHandler : IRequestHandler<GetAllSpendingLimitQuery, ServiceResponse<List<SpendingLimitDto>>>
    {
        private readonly ISpendingLimitRepository _SpendingLimitRepository; // Repository for accessing SpendingLimit data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllSpendingLimitsQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllSpendingLimitsQueryHandler.
        /// </summary>
        /// <param name="SpendingLimitRepository">Repository for SpendingLimit data access.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllSpendingLimitsQueryHandler(
            ISpendingLimitRepository SpendingLimitRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetAllSpendingLimitsQueryHandler> logger)
        {
            _SpendingLimitRepository = SpendingLimitRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllSpendingLimitsQuery to retrieve all SpendingLimits.
        /// </summary>
        /// <param name="request">The GetAllSpendingLimitsQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<SpendingLimitDto>>> Handle(GetAllSpendingLimitQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve all SpendingLimit entities from the repository
                var entities = await _SpendingLimitRepository.All.ToListAsync(cancellationToken);

                // Step 2: Log the successful retrieval of SpendingLimits
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "SpendingLimits returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Step 3: Return the result with a successful response
                return ServiceResponse<List<SpendingLimitDto>>.ReturnResultWith200(_mapper.Map<List<SpendingLimitDto>>(entities));
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve SpendingLimits: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<SpendingLimitDto>>.Return500(e, "Failed to retrieve SpendingLimits");
            }
        }
    }
}
