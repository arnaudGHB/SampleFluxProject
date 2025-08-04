using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a Account based on UpdateAccountCommand.
    /// </summary>
    public class UpdateAccountingEventRuleCommandHandler : IRequestHandler<UpdateAccountingEventRuleCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingRuleRepository _AccountingRuleRepository; // Repository for accessing Account data.
        private readonly ILogger<UpdateAccountingEventRuleCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work.
        private readonly UserInfoToken _userInfoToken; // MongoDB Unit of Work.
        /// <summary>
        /// Constructor for initializing the UpdateAccountCommandHandler.
        /// </summary>
        /// <param name="AccountingRuleRepository">Repository for Account data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateAccountingEventRuleCommandHandler(
            IAccountingRuleRepository AccountingRuleRepository,
            ILogger<UpdateAccountingEventRuleCommandHandler> logger,
            IMapper mapper,
            IMongoUnitOfWork? mongoUnitOfWork,
            UserInfoToken? userInfoToken,
            IUnitOfWork<POSContext> uow = null)
        {
            _AccountingRuleRepository = AccountingRuleRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _mongoUnitOfWork = mongoUnitOfWork;
        }

        /// <summary>
        /// Handles the UpdateAccountingRule Command to update a AccountingRule.
        /// </summary>
        /// <param name="request">The UpdateAccountingEventRuleCommand containing updated AccountingRule data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(UpdateAccountingEventRuleCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = "";
            try
            {
                // Retrieve the Account entity to be updated from the repository
                var _recordRepository = _mongoUnitOfWork.GetRepository<AccountingEventRule>();
       
                var eventRule = await _recordRepository.GetByIdAsync(request.Id);
                if (eventRule == null)
                {
                    errorMessage = $"record with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage,
                          request, HttpStatusCodeEnum.NotFound, LogAction.AddAccountingRuleCommand, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                BaseUtilities.PrepareMonoDBDataForCreation(eventRule, _userInfoToken, TrackerState.Modified);

                 eventRule  = request.ToAccountingEventRule();
                await _recordRepository.UpdateAsync(eventRule.Id, eventRule);
                errorMessage = "Account event entries updated successfully";
                await BaseUtilities.LogAndAuditAsync(errorMessage,
                           request, HttpStatusCodeEnum.OK, LogAction.AddAccountingRuleCommand, LogLevelInfo.Information);

                return ServiceResponse<bool>.ReturnResultWith200(true);

            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                 errorMessage = $"Error occurred while updating Account: {e.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage,
                   request, HttpStatusCodeEnum.InternalServerError, LogAction.UpdateAccountingEventRuleCommand, LogLevelInfo.Information);
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}