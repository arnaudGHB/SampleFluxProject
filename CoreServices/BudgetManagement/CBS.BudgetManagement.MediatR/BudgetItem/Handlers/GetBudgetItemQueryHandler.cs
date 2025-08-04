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
    /// Handles the retrieval of BudgetItem based on the GetBudgetItemQuery.
    /// </summary>
    public class GetBudgetItemQueryHandler : IRequestHandler<GetBudgetItemQuery, ServiceResponse<BudgetItemDto>>
    {
        private readonly IBudgetItemRepository _BudgetItemRepository; // Repository for accessing BudgetItem data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetBudgetItemQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetBudgetItemQueryHandler.
        /// </summary>
        /// <param name="BudgetItemRepository">Repository for BudgetItem data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        public GetBudgetItemQueryHandler(
            IBudgetItemRepository BudgetItemRepository,
            UserInfoToken userInfoToken,
            IMapper mapper, ILogger<GetBudgetItemQueryHandler> logger)
        {
            _BudgetItemRepository = BudgetItemRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetBudgetItemQuery to retrieve an BudgetItem.
        /// </summary>
        /// <param name="request">The GetBudgetItemQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BudgetItemDto>> Handle(GetBudgetItemQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the BudgetItem entity from the repository
                var entity = await _BudgetItemRepository.FindAsync(request.Id);

                // Step 2: Check if the entity exists
                if (entity == null)
                {
                    string notFoundMessage = $"BudgetItem with ID '{request.Id}' not found.";
                    _logger.LogWarning(notFoundMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, notFoundMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<BudgetItemDto>.Return404(notFoundMessage);
                }

                // Step 3: Log success and return the mapped entity as a DTO
                string successMessage = "BudgetItem returned successfully.";
                _logger.LogInformation(successMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                var BudgetItemDto = _mapper.Map<BudgetItemDto>(entity);
                return ServiceResponse<BudgetItemDto>.ReturnResultWith200(BudgetItemDto);
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve BudgetItem: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<BudgetItemDto>.Return500(e, "Failed to retrieve BudgetItem");
            }
        }
    }
}
