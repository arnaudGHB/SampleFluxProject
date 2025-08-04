using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Dto.ChartOfAccountDto;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using CBS.ChartOfAccount.MediatR.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.ChartOfAccount.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all ChartOfAccounts based on the GetAllChartOfAccountQuery.
    /// </summary>
    public class GetAllChartOfAccountAsTreeNodeQueryHandler : IRequestHandler<GetAllChartOfAccountAsTreeNodeQuery, ServiceResponse<List<TreeNodeDto>>>
    {
        private readonly IChartOfAccountRepository _AccountRepository; // Repository for accessing Accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllChartOfAccountAsTreeNodeQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllAccountQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Accounts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllChartOfAccountAsTreeNodeQueryHandler(
           IChartOfAccountRepository AccountRepository,
            IMapper mapper, ILogger<GetAllChartOfAccountAsTreeNodeQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _AccountRepository = AccountRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllChartOfAccountQuery to retrieve all Accounts.
        /// </summary>
        /// <param name="request">The GetAllChartOfAccountQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<TreeNodeDto>>> Handle(GetAllChartOfAccountAsTreeNodeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                List<TreeNodeDto> modelTreeNodeDto = new List<TreeNodeDto>();
                List<ChartOfAccountDto> models = new List<ChartOfAccountDto>();
                // Retrieve all Accounts entities from the repository
                var entities = await _AccountRepository.All.Where(p => p.IsDeleted.Equals(false)).ToListAsync();
                models = _mapper.Map(entities, models);
                var Treemodel = AccountManagement.Data.ChartOfAccountDto.ConvertToTreeNodeList(models.OrderBy(x => Convert.ToInt32(x.AccountNumber)).ToList());
                modelTreeNodeDto = _mapper.Map(Treemodel, modelTreeNodeDto);
                //modelTreeNodeDto.Sort((x, y) => x.id.CompareTo(y.id));
                return ServiceResponse<List<TreeNodeDto>>.ReturnResultWith200(new List<TreeNodeDto> { modelTreeNodeDto[0] });
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Accounts: {e.Message}");
                return ServiceResponse<List<TreeNodeDto>>.Return500(e, "Failed to get all Accounts");
            }
        }
    }
}