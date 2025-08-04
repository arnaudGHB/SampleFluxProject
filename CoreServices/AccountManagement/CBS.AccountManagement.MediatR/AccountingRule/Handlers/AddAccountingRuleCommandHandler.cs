using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Account.
    /// </summary>
    public class AddAccountingRuleCommandHandler : IRequestHandler<AddAccountingRuleCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingRuleRepository _AccountingRuleRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddAccountingRuleCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work.
        /// <summary>
        /// Constructor for initializing the AddAccountingRuleCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddAccountingRuleCommandHandler(
            IAccountingRuleRepository AccountingRuleRepository,
            IMapper mapper,
            ILogger<AddAccountingRuleCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            IMongoUnitOfWork? mongoUnitOfWork,
            UserInfoToken? userInfoToken)
        {
            _AccountingRuleRepository = AccountingRuleRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _mongoUnitOfWork = mongoUnitOfWork;
        }

        /// <summary>
        /// Handles the AddAccountingRuleCommand to add a new Account.
        /// </summary>
        /// <param name="request">The AddAccountingRuleCommand containing Account data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(AddAccountingRuleCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = "";
            List<Data.AccountingRule> rules= new List<Data.AccountingRule>();
            AccountingEventRule eventRule = new AccountingEventRule();
            try

            {
                var systemId = BaseUtilities.GenerateInsuranceUniqueNumber(16, "SYS_ID");
             
                eventRule = request.ToAccountingEventRule();
                // Get the MongoDB repository for TransacuytionTracker
                var _recordRepository = _mongoUnitOfWork.GetRepository<AccountingEventRule>();
                //var model = await _transactionTrackerRepository.FindAsync(request.TransactionReference);
                request.Id = systemId;
                var existingnRecord = await _recordRepository.GetByIdAsync(eventRule.Id);
                if (existingnRecord != null)
                {
                    errorMessage = $"record with ID {eventRule.Id} already exist.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage,
                          request, HttpStatusCodeEnum.NotFound, LogAction.AddAccountingRuleCommand, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                BaseUtilities.PrepareMonoDBDataForCreation(eventRule,_userInfoToken,TrackerState.Created);
               await _recordRepository.InsertAsync(eventRule);
                errorMessage = "Accounting event entries created successfully";
                await BaseUtilities.LogAndAuditAsync(errorMessage,
                           request, HttpStatusCodeEnum.OK, LogAction.AddAccountingRuleCommand, LogLevelInfo.Information);
                return ServiceResponse<bool>.ReturnResultWith200(true, errorMessage);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                  errorMessage = $"Error occurred reisgistering : {e.Message}";

                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage,
                          request, HttpStatusCodeEnum.InternalServerError, LogAction.AddAccountingRuleCommand, LogLevelInfo.Error);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}