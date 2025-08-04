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
    /// Handles the request to retrieve a specific BudgetName based on its unique identifier.
    /// </summary>
    public class GetBudgetQueryHandler : IRequestHandler<GetBudgetQuery, ServiceResponse<BudgetDto>>
    {
        private readonly IBudgetRepository _BudgetRepository; // Repository for accessing BudgetName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetBudgetQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the GetBudgetNameQueryHandler.
        /// </summary>
        /// <param name="BudgetRepository">Repository for BudgetName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
 
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetBudgetQueryHandler(
            IBudgetRepository BudgetRepository,
            IMapper mapper,
            ILogger<GetBudgetQueryHandler> logger,
            UserInfoToken? userInfoToken)
        {
            _BudgetRepository = BudgetRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetBudgetNameQuery to retrieve a specific BudgetName.
        /// </summary>
        /// <param name="request">The GetBudgetNameQuery containing BudgetName ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BudgetDto>> Handle(GetBudgetQuery request, CancellationToken cancellationToken)
        {
            string message = null;
            try
            {
                // Retrieve the BudgetName entity with the specified ID from the repository
                var entity = await _BudgetRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    if (entity.IsDeleted)
                    {
                          message = $"Budget Id:{request.Id} has been deleted.";
                        
                        await APICallHelper.AuditLogger(_userInfoToken.Email, "GetBudgetQuery",
request, message, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                        _logger.LogError(message);
                        return ServiceResponse<BudgetDto>.Return404(message);
                    }
                    else
                    {

                        // Map the BudgetName entity to BudgetNameDto and return it with a success response
                        var BudgetNameDto = _mapper.Map<BudgetDto>(entity);
                        await APICallHelper.AuditLogger(_userInfoToken.Email, "GetBudgetQuery",
request, message, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                        return ServiceResponse<BudgetDto>.ReturnResultWith200(BudgetNameDto);
                    }

                }
                else
                {
                      message = $"Budget Id:{request.Id} has been deleted.";
                    // If the BudgetName entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "GetBudgetQuery",
  request, message, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<BudgetDto>.Return404();
                }
            }
            catch (Exception e)
            {
                message = $"Error occurred while getting BudgetName: {e.Message}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetBudgetQuery",
request, message, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(message);
                return ServiceResponse<BudgetDto>.Return500(e);
            }
        }
    }
}