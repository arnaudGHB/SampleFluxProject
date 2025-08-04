using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Account.Queries;
using CBS.AccountManagement.MediatR.Queries;

using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.BudgetItemDetailManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific BudgetItemDetail based on its unique identifier.
    /// </summary>
    public class GetBudgetItemDetailByBudgetIdQueryHandler : IRequestHandler<GetBudgetItemByBudgetIdQuery, ServiceResponse<List<BudgetItemDetailDto>>>
    {
        private readonly IBudgetItemDetailRepository _BudgetItemDetailRepository; // Repository for accessing BudgetItemDetail data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetBudgetItemDetailByBudgetIdQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetBudgetItemDetailByReferenceQueryHandler.
        /// </summary>
        /// <param name="BudgetItemDetailRepository">Repository for BudgetItemDetail data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetBudgetItemDetailByBudgetIdQueryHandler(
            IBudgetItemDetailRepository BudgetItemDetailRepository,
            IMapper mapper,
            ILogger<GetBudgetItemDetailByBudgetIdQueryHandler> logger,UserInfoToken userInfoToken)
        {
            _BudgetItemDetailRepository = BudgetItemDetailRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetBudgetItemDetailQuery to retrieve a specific BudgetItemDetail.
        /// </summary>
        /// <param name="request">The GetBudgetItemDetailQuery containing BudgetItemDetail ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<BudgetItemDetailDto>>> Handle(GetBudgetItemByBudgetIdQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the BudgetItemDetail entity with the specified ID from the repository
                var BudgetItemDetail = _BudgetItemDetailRepository.FindBy(x=>x.BudgetId.Equals(request.BudgetId)&&x.BranchId==_userInfoToken.BranchId).ToList();
                if (BudgetItemDetail != null)
                {
                    errorMessage = $"Return BudgetItemDetailDto with a success response";
                    List<BudgetItemDetailDto> dataToMap = new List<BudgetItemDetailDto>();
                    var BudgetItemDetailDtos = _mapper.Map(BudgetItemDetail,dataToMap);
                        await APICallHelper.AuditLogger(_userInfoToken.Email, "GetBudgetItemDetailByBudgetItemDetailTypeQuery",
                            JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    return ServiceResponse<List<BudgetItemDetailDto>>.ReturnResultWith200(BudgetItemDetailDtos);
                }
                else
                {
                    // If the BudgetItemDetail entity was not found, log the error and return a 404 Not Found response
                    errorMessage = "BudgetItemDetail not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "GetBudgetItemDetailByBudgetItemDetailNumberQuery",
                        JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<List<BudgetItemDetailDto>>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting BudgetItemDetail: {BaseUtilities.GetInnerExceptionMessages(e)}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetBudgetItemDetailByBudgetItemDetailNumberQuery",
                    JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<List<BudgetItemDetailDto>>.Return500(e);
            }
        }
    }
}