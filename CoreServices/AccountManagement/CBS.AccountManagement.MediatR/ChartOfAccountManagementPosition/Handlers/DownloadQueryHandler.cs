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
    /// Handles the retrieval of all AccountClass based on the GetAllAccountCategoryQuery.
    /// </summary>
    public class DownloadQueryHandler : IRequestHandler<DownloadQuery, ServiceResponse<List<ChartOfAccountManagementPositionDto>>>
    {
        private readonly IChartOfAccountManagementPositionRepository _ChartOfAccountManagementPositionRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<DownloadQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the GetChartOfAccountManagementPositionQueryHandler.
        /// </summary>
        /// <param name="AccountClassRepository">Repository for AccountClass data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DownloadQueryHandler(
            IChartOfAccountManagementPositionRepository AccountClassRepository,
               IAccountCategoryRepository AccountCategoryRepository,
            IMapper mapper,
            ILogger<DownloadQueryHandler> logger, UserInfoToken userInfoToken)
        {
            _ChartOfAccountManagementPositionRepository = AccountClassRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }


        /// <summary>
        /// Handles the GetAllAccountClassQuery to retrieve all AccountClass.
        /// </summary>
        /// <param name="request">The GetAllAccountClassQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<ChartOfAccountManagementPositionDto>>> Handle(DownloadQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all AccountClasss entities from the repository
               
                    var entities = await _ChartOfAccountManagementPositionRepository.All.Where(x => x.IsDeleted.Equals(false)&&x.IsUniqueAccount==false).ToListAsync();
                    return ServiceResponse<List<ChartOfAccountManagementPositionDto>>.ReturnResultWith200(_mapper.Map<List<ChartOfAccountManagementPositionDto>>(entities));
                 

            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all ChartOfAccountManagementPositionDto: {e.Message}");
                return ServiceResponse<List<ChartOfAccountManagementPositionDto>>.Return500(e, "Failed to get all AccountClass");
            }
        }
    }
}