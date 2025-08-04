using AutoMapper;
using CBS.AccountManagement.Common;
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
    /// Handles the retrieval of all Accounts based on the GetAllAccountQuery.
    /// </summary>
    public class GetAllAccountingRuleQueryHandler : IRequestHandler<GetAllAccountingRuleQuery, ServiceResponse<List<AccountingEventRule>>>
    {

        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllAccountingRuleQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work.
        private readonly UserInfoToken _userInfoToken; // MongoDB Unit of Work.
        /// <summary>
        /// Constructor for initializing the GetAllAccountQueryHandler.
        /// </summary>
        /// <param name="_AccountingRuleRepository">Repository for Accounts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllAccountingRuleQueryHandler(

            IMapper mapper, ILogger<GetAllAccountingRuleQueryHandler> logger, IMongoUnitOfWork? mongoUnitOfWork)
        {
            // Assign provided dependencies to local variables.
            _mongoUnitOfWork = mongoUnitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllAccountQuery to retrieve all Accounts.
        /// </summary>
        /// <param name="request">The GetAllAccountQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AccountingEventRule>>> Handle(GetAllAccountingRuleQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = string.Empty;
            try
            {
                // Retrieve all Accounts entities from the repository
                var _recordRepository = _mongoUnitOfWork.GetRepository<AccountingEventRule>();

                var existingnRecords = await _recordRepository.GetAllAsync();
                  existingnRecords= existingnRecords.Where(x=>x.IsDeleted==false).ToList();
                await BaseUtilities.LogAndAuditAsync(errorMessage,
                        request, HttpStatusCodeEnum.OK, LogAction.GetAllAccountingRuleQuery, LogLevelInfo.Information);
   
                return ServiceResponse<List<AccountingEventRule>>.ReturnResultWith200(existingnRecords.ToList());
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                errorMessage = $"Failed to get all Accounts: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage,
                      request, HttpStatusCodeEnum.InternalServerError, LogAction.GetAllAccountingRuleQuery, LogLevelInfo.Error);

                return ServiceResponse<List<AccountingEventRule>>.Return500(e, "Failed to get all Accounts");
            }
        }
    }
}