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
    /// Handles the retrieval of all BudgetItem based on the GetAllBudgetItemQuery.
    /// </summary>
    public class GetAllBudgetItemQueryHandler : IRequestHandler<GetAllBudgetItemsQuery, ServiceResponse<List<BudgetItemDto>>>
    {
        private readonly IBudgetItemRepository _BudgetItemRepository; // Repository for accessing BudgetItem data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllBudgetItemQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllBudgetItemQueryHandler.
        /// </summary>
        /// <param name="BudgetItemRepository">Repository for BudgetItem data access.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllBudgetItemQueryHandler(
            IBudgetItemRepository BudgetItemRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetAllBudgetItemQueryHandler> logger)
        {
            _BudgetItemRepository = BudgetItemRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllBudgetItemQuery to retrieve all BudgetItem.
        /// </summary>
        /// <param name="request">The GetAllBudgetItemQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<BudgetItemDto>>> Handle(GetAllBudgetItemsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve all BudgetItem entities from the repository
                var entities = await _BudgetItemRepository.All.ToListAsync(cancellationToken);

                // Step 2: Log the successful retrieval of BudgetItem
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "BudgetItem returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Step 3: Return the result with a successful response
                return ServiceResponse<List<BudgetItemDto>>.ReturnResultWith200(_mapper.Map<List<BudgetItemDto>>(entities));
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve BudgetItem: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<BudgetItemDto>>.Return500(e, "Failed to retrieve BudgetItem");
            }
        }
    }
}
