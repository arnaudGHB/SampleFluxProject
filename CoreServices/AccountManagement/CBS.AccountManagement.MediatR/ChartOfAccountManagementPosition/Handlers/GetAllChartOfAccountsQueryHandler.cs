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
    public class GetAllChartOfAccountsQueryHandler : IRequestHandler<GetAllChartOfAccountMPsQuery, ServiceResponse<List<ChartOfAccountStateDto>>>
    {
        private readonly IChartOfAccountManagementPositionRepository _ChartOfAccountManagementPositionRepository;
        private readonly IChartOfAccountRepository _chartOfAccountRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllChartOfAccountsQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IAccountRepository _accountRepository;
        /// <summary>
        /// Constructor for initializing the GetChartOfAccountManagementPositionQueryHandler.
        /// </summary>
        /// <param name="AccountClassRepository">Repository for AccountClass data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllChartOfAccountsQueryHandler(
            IChartOfAccountManagementPositionRepository AccountClassRepository,
               IChartOfAccountRepository chartOfAccountRepository,
            IMapper mapper,
            ILogger<GetAllChartOfAccountsQueryHandler> logger,
            IAccountRepository? accountRepository)
        {
            _ChartOfAccountManagementPositionRepository = AccountClassRepository;
            _chartOfAccountRepository= chartOfAccountRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
            _logger = logger;
        }


        /// <summary>
        /// Handles the GetAllAccountClassQuery to retrieve all AccountClass.
        /// </summary>
        /// <param name="request">The GetAllAccountClassQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<ChartOfAccountStateDto>>> Handle(GetAllChartOfAccountMPsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all AccountClasss entities from the repository
                var ChartofAccountManagementPositions = await _ChartOfAccountManagementPositionRepository.All.Where(x => x.IsDeleted.Equals(false) && x.IsUniqueAccount.Equals(false)).ToListAsync();
                string code = "[BranchCodeX]";
                var dataChart = await _chartOfAccountRepository.All.Where(x => x.IsDeleted.Equals(false)).ToListAsync();

                var listOfItems = (from item in ChartofAccountManagementPositions
                                   join element in dataChart on item.ChartOfAccountId equals element.Id
                                   select new ChartOfAccountStateDto
                                   {
                                       Id = item.Id,
                                       GeneralRepresentation = item.Description+"-"+ element.AccountNumber.PadRight(6, '0') + code + item.PositionNumber.PadRight(3, '0')

                                   }).ToList();
                return ServiceResponse<List<ChartOfAccountStateDto>>.ReturnResultWith200(listOfItems);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all ChartOfAccountManagementPositionDto: {e.Message}");
                return ServiceResponse<List<ChartOfAccountStateDto>>.Return500(e, "Failed to get all AccountClass");
            }
        }
    }
}