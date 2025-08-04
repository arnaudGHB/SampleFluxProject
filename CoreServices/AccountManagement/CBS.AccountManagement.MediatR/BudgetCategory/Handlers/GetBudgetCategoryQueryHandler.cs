using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific BudgetCategoryName based on its unique identifier.
    /// </summary>
    public class GetBudgetCategoryQueryHandler : IRequestHandler<GetBudgetCategoryQuery, ServiceResponse<BudgetCategoryDto>>
    {
        private readonly IBudgetCategoryRepository _BudgetCategoryRepository; // Repository for accessing BudgetCategoryName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetBudgetCategoryQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the GetBudgetCategoryNameQueryHandler.
        /// </summary>
        /// <param name="BudgetCategoryRepository">Repository for BudgetCategoryName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
 
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetBudgetCategoryQueryHandler(
            IBudgetCategoryRepository BudgetCategoryRepository,
            IMapper mapper,
            ILogger<GetBudgetCategoryQueryHandler> logger,
            UserInfoToken? userInfoToken)
        {
            _BudgetCategoryRepository = BudgetCategoryRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetBudgetCategoryNameQuery to retrieve a specific BudgetCategoryName.
        /// </summary>
        /// <param name="request">The GetBudgetCategoryNameQuery containing BudgetCategoryName ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BudgetCategoryDto>> Handle(GetBudgetCategoryQuery request, CancellationToken cancellationToken)
        {
            string message = null;
            try
            {
                // Retrieve the BudgetCategoryName entity with the specified ID from the repository
                var entity = await _BudgetCategoryRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    if (entity.IsDeleted)
                    {
                          message = $"BudgetCategory Id:{request.Id} has been deleted.";
                        // If the AccountCategory entity was not found, log the error and return a 404 Not Found response
                        await APICallHelper.AuditLogger(_userInfoToken.Email, "GetBudgetCategoryQuery",
request, message, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                        _logger.LogError(message);
                        return ServiceResponse<BudgetCategoryDto>.Return404(message);
                    }
                    else
                    {

                        // Map the BudgetCategoryName entity to BudgetCategoryNameDto and return it with a success response
                        var BudgetCategoryNameDto = _mapper.Map<BudgetCategoryDto>(entity);
                        await APICallHelper.AuditLogger(_userInfoToken.Email, "GetBudgetCategoryQuery",
request, message, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                        return ServiceResponse<BudgetCategoryDto>.ReturnResultWith200(BudgetCategoryNameDto);
                    }

                }
                else
                {
                      message = $"BudgetCategory Id:{request.Id} has been deleted.";
                    // If the BudgetCategoryName entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "GetBudgetCategoryQuery",
  request, message, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<BudgetCategoryDto>.Return404();
                }
            }
            catch (Exception e)
            {
                message = $"Error occurred while getting BudgetCategoryName: {e.Message}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetBudgetCategoryQuery",
request, message, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(message);
                return ServiceResponse<BudgetCategoryDto>.Return500(e);
            }
        }
    }
}