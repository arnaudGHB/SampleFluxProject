using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Account based on DeleteAccountCommand.
    /// </summary>
    public class DeleteAccountingRuleCommandHandler : IRequestHandler<DeleteAccountingRuleCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingRuleRepository _AccountingRuleRepository; // Repository for accessing Account data.
        private readonly ILogger<DeleteAccountingRuleCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work.
        private readonly UserInfoToken _userInfoToken; // MongoDB Unit of Work.
        /// <summary>
        /// Constructor for initializing the DeleteAccountCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteAccountingRuleCommandHandler(
            IAccountingRuleRepository AccountingRuleRepository, IMapper mapper,
            ILogger<DeleteAccountingRuleCommandHandler> logger
, IUnitOfWork<POSContext> uow, IMongoUnitOfWork? mongoUnitOfWork, UserInfoToken? userInfoToken)
        {
            _mongoUnitOfWork = mongoUnitOfWork;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the DeleteAccountCommand to delete a Account.
        /// </summary>
        /// <param name="request">The DeleteAccountCommand containing Account ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteAccountingRuleCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Account entity with the specified ID exists
                var _recordRepository = _mongoUnitOfWork.GetRepository<AccountingEventRule>();

                var existingnRecord = await _recordRepository.GetByIdAsync(request.Id);
                if (existingnRecord == null)
                {
                    errorMessage = $"record with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage,
                          request, HttpStatusCodeEnum.NotFound, LogAction.DeleteAccountingRuleCommand, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                else
                {
                    if (existingnRecord.IsDeleted)
                    {
                        errorMessage = $"record with ID {request.Id} not found. element already deleted";
                        _logger.LogError(errorMessage);
                        await BaseUtilities.LogAndAuditAsync(errorMessage,
                              request, HttpStatusCodeEnum.NotFound, LogAction.DeleteAccountingRuleCommand, LogLevelInfo.Warning);
                        return ServiceResponse<bool>.Return409(errorMessage);
                    }
                    else
                    {
            
                        //BaseUtilities.PrepareMonoDBDataForCreation(existingnRecord, _userInfoToken, TrackerState.Deleted);
                        await _recordRepository.DeleteAsync(existingnRecord.Id);
                        errorMessage = "Account event entries updated successfully";
                        await BaseUtilities.LogAndAuditAsync(errorMessage,
                                   request, HttpStatusCodeEnum.OK, LogAction.AddAccountingRuleCommand, LogLevelInfo.Information);
                        return ServiceResponse<bool>.ReturnResultWith200(true);
                    }
                  
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Account: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}