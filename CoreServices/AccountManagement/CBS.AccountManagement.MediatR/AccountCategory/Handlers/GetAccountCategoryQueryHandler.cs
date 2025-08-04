using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific AccountCategory based on its unique identifier.
    /// </summary>
    public class GetAccountCategoryQueryHandler : IRequestHandler<GetAccountCategoryQuery, ServiceResponse<AccountCartegoryDto>>
    {
        private readonly IAccountCategoryRepository _AccountCategoryRepository; // Repository for accessing AccountCategory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAccountCategoryQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAccountCategoryQueryHandler.
        /// </summary>
        /// <param name="AccountCategoryRepository">Repository for AccountCategory data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAccountCategoryQueryHandler(
            IAccountCategoryRepository AccountCategoryRepository,
            IMapper mapper,
            ILogger<GetAccountCategoryQueryHandler> logger)
        {
            _AccountCategoryRepository = AccountCategoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAccountCategoryQuery to retrieve a specific AccountCategory.
        /// </summary>
        /// <param name="request">The GetAccountCategoryQuery containing AccountCategory ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountCartegoryDto>> Handle(GetAccountCategoryQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the AccountCategory entity with the specified ID from the repository
                var entity = await _AccountCategoryRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    if (entity.IsDeleted) 
                    {
                        string message = "AccountCategory has been deleted.";
                        // If the AccountCategory entity was not found, log the error and return a 404 Not Found response
                        _logger.LogError(message);
                        return ServiceResponse<AccountCartegoryDto>.Return404(message);
                    }
                    else
                    {
                        // Map the AccountCategory entity to AccountCategoryDto and return it with a success response
                        var AccountCategoryDto = _mapper.Map<AccountCartegoryDto>(entity);
                        return ServiceResponse<AccountCartegoryDto>.ReturnResultWith200(AccountCategoryDto);
                    }
               
                }
                else
                {
                    // If the AccountCategory entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("AccountCategory not found.");
                    return ServiceResponse<AccountCartegoryDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting AccountCategory: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<AccountCartegoryDto>.Return500(e);
            }
        }
    }
}