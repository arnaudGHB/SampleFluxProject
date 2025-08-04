using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all AccountCategorys based on the GetAllAccountCategoryQuery.
    /// </summary>
    public class GetAllAccountCartegoryQueryHandler : IRequestHandler<GetAllAccountCategoryQuery, ServiceResponse<List<AccountCartegoryDto>>>
    {
        private readonly IAccountCategoryRepository _AccountCategoryRepository; // Repository for accessing AccountCategorys data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllAccountCartegoryQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllAccountCategoryQueryHandler.
        /// </summary>
        /// <param name="AccountCategoryRepository">Repository for AccountCategorys data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllAccountCartegoryQueryHandler(
            IAccountCategoryRepository AccountCategoryRepository,
            IMapper mapper, ILogger<GetAllAccountCartegoryQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _AccountCategoryRepository = AccountCategoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllAccountCategoryQuery to retrieve all AccountCategorys.
        /// </summary>
        /// <param name="request">The GetAllAccountCategoryQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AccountCartegoryDto>>> Handle(GetAllAccountCategoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all AccountCategorys entities from the repository
                var entities = await _AccountCategoryRepository.All.Where(x => x.IsDeleted.Equals(false)).ToListAsync();
                return ServiceResponse<List<AccountCartegoryDto>>.ReturnResultWith200(_mapper.Map<List<AccountCartegoryDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all AccountCategorys: {e.Message}");
                return ServiceResponse<List<AccountCartegoryDto>>.Return500(e, "Failed to get all AccountCategorys");
            }
        }
    }
}