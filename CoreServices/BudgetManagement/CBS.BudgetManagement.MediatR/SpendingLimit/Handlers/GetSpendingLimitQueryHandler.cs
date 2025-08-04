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
    /// Handles the retrieval of SpendingLimit based on the GetSpendingLimitQuery.
    /// </summary>
    public class GetSpendingLimitQueryHandler : IRequestHandler<GetSpendingLimitQuery, ServiceResponse<SpendingLimitDto>>
    {
        private readonly ISpendingLimitRepository _SpendingLimitRepository; // Repository for accessing SpendingLimit data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetSpendingLimitQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetSpendingLimitQueryHandler.
        /// </summary>
        /// <param name="SpendingLimitRepository">Repository for SpendingLimit data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        public GetSpendingLimitQueryHandler(
            ISpendingLimitRepository SpendingLimitRepository,
            UserInfoToken userInfoToken,
            IMapper mapper, ILogger<GetSpendingLimitQueryHandler> logger)
        {
            _SpendingLimitRepository = SpendingLimitRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetSpendingLimitQuery to retrieve an SpendingLimit.
        /// </summary>
        /// <param name="request">The GetSpendingLimitQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<SpendingLimitDto>> Handle(GetSpendingLimitQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the SpendingLimit entity from the repository
                var entity = await _SpendingLimitRepository.FindAsync(request.Id);

                // Step 2: Check if the entity exists
                if (entity == null)
                {
                    string notFoundMessage = $"SpendingLimit with ID '{request.Id}' not found.";
                    _logger.LogWarning(notFoundMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, notFoundMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<SpendingLimitDto>.Return404(notFoundMessage);
                }

                // Step 3: Log success and return the mapped entity as a DTO
                string successMessage = "SpendingLimit returned successfully.";
                _logger.LogInformation(successMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                var SpendingLimitDto = _mapper.Map<SpendingLimitDto>(entity);
                return ServiceResponse<SpendingLimitDto>.ReturnResultWith200(SpendingLimitDto);
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve SpendingLimit: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<SpendingLimitDto>.Return500(e, "Failed to retrieve SpendingLimit");
            }
        }
    }
}
