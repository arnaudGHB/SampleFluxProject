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
    /// Handles the retrieval of BudgetAdjustment based on the GetBudgetAdjustmentQuery.
    /// </summary>
    public class GetBudgetAdjustmentQueryHandler : IRequestHandler<GetBudgetAdjustmentQuery, ServiceResponse<BudgetAdjustmentDto>>
    {
        private readonly IBudgetAdjustmentRepository _BudgetAdjustmentRepository; // Repository for accessing BudgetAdjustment data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetBudgetAdjustmentQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetBudgetAdjustmentQueryHandler.
        /// </summary>
        /// <param name="BudgetAdjustmentRepository">Repository for BudgetAdjustment data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        public GetBudgetAdjustmentQueryHandler(
            IBudgetAdjustmentRepository BudgetAdjustmentRepository,
            UserInfoToken userInfoToken,
            IMapper mapper, ILogger<GetBudgetAdjustmentQueryHandler> logger)
        {
            _BudgetAdjustmentRepository = BudgetAdjustmentRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetBudgetAdjustmentQuery to retrieve an BudgetAdjustment.
        /// </summary>
        /// <param name="request">The GetBudgetAdjustmentQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BudgetAdjustmentDto>> Handle(GetBudgetAdjustmentQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the BudgetAdjustment entity from the repository
                var entity = await _BudgetAdjustmentRepository.FindAsync(request.Id);

                // Step 2: Check if the entity exists
                if (entity == null)
                {
                    string notFoundMessage = $"BudgetAdjustment with ID '{request.Id}' not found.";
                    _logger.LogWarning(notFoundMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, notFoundMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<BudgetAdjustmentDto>.Return404(notFoundMessage);
                }

                // Step 3: Log success and return the mapped entity as a DTO
                string successMessage = "BudgetAdjustment returned successfully.";
                _logger.LogInformation(successMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                var BudgetAdjustmentDto = _mapper.Map<BudgetAdjustmentDto>(entity);
                return ServiceResponse<BudgetAdjustmentDto>.ReturnResultWith200(BudgetAdjustmentDto);
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve BudgetAdjustment: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<BudgetAdjustmentDto>.Return500(e, "Failed to retrieve BudgetAdjustment");
            }
        }
    }
}
