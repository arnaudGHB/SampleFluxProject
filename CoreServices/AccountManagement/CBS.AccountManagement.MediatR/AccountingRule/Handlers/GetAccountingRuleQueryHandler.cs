using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Account based on its unique identifier.
    /// </summary>
    public class GetAccountingRuleQueryHandler : IRequestHandler<GetAccountingRuleQuery, ServiceResponse<AccountingEventRule>>
    {
        private readonly IAccountingRuleRepository _AccountingRuleRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAccountingRuleQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work.
        private readonly UserInfoToken _userInfoToken; // MongoDB Unit of Work.
        /// <summary>
        /// Constructor for initializing the GetAccountingRuleQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAccountingRuleQueryHandler(
            IAccountingRuleRepository AccountingRuleRepository,
            IMapper mapper,
            ILogger<GetAccountingRuleQueryHandler> logger,
            IMongoUnitOfWork? mongoUnitOfWork)
        {
            _mongoUnitOfWork = mongoUnitOfWork;

 
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAccountingRuleQuery to retrieve a specific Account.
        /// </summary>
        /// <param name="request">The GetAccountingRuleQuery containing Account ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountingEventRule>> Handle(GetAccountingRuleQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                var _recordRepository = _mongoUnitOfWork.GetRepository<AccountingEventRule>();

                var existingnRecord = await _recordRepository.GetByIdAsync(request.Id);
                if (existingnRecord == null)
                {
                    errorMessage = $"record with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage,
                          request, HttpStatusCodeEnum.NotFound, LogAction.GetAccountingRuleQuery, LogLevelInfo.Warning);
                    return ServiceResponse<AccountingEventRule>.Return409(errorMessage);
                }
                else
                {
                    errorMessage = $"Record: retrieve successfully.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage,
                          request, HttpStatusCodeEnum.OK, LogAction.GetAccountingRuleQuery, LogLevelInfo.Information);
                    return ServiceResponse<AccountingEventRule>.ReturnResultWith200(existingnRecord);
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Account: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<AccountingEventRule>.Return500(e);
            }
        }
    }
}