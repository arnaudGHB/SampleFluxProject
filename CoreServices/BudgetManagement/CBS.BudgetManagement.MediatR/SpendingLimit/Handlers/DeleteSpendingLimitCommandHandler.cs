using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BudgetManagement.Helper;
using CBS.BudgetManagement.Repository;
using CBS.BudgetManagement.Common;
using CBS.BudgetManagement.Data;
 
using System.Threading;
using System.Threading.Tasks;
using CBS.BudgetManagement.MediatR.Commands;
using CBS.BudgetManagement.Domain;

namespace CBS.BudgetManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete an SpendingLimit based on DeleteSpendingLimitCommand.
    /// </summary>
    public class DeleteSpendingLimitCommandHandler : IRequestHandler<DeleteSpendingLimitCommand, ServiceResponse<bool>>
    {
        private readonly ISpendingLimitRepository _SpendingLimitRepository; // Repository for accessing SpendingLimit data.
        private readonly ILogger<DeleteSpendingLimitCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<BudgetManagementContext> _uow; // Unit of Work for managing database operations.
        private readonly UserInfoToken _userInfoToken; // User information for audit logging.

        /// <summary>
        /// Constructor for initializing the DeleteSpendingLimitCommandHandler.
        /// </summary>
        /// <param name="SpendingLimitRepository">Repository for SpendingLimit data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="uow">Unit of Work for managing database operations.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        public DeleteSpendingLimitCommandHandler(
            ISpendingLimitRepository SpendingLimitRepository,
            ILogger<DeleteSpendingLimitCommandHandler> logger,
            IUnitOfWork<BudgetManagementContext> uow,
            UserInfoToken userInfoToken)
        {
            _SpendingLimitRepository = SpendingLimitRepository;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the DeleteSpendingLimitCommand to delete an SpendingLimit.
        /// </summary>
        /// <param name="request">The DeleteSpendingLimitCommand containing the ID of the SpendingLimit to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteSpendingLimitCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Step 1: Check if the SpendingLimit entity with the specified ID exists
                var existingSpendingLimit = await _SpendingLimitRepository.FindAsync(request.Id);
                if (existingSpendingLimit == null)
                {
                    errorMessage = $"SpendingLimit with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(
                        _userInfoToken.Email,
                        LogAction.Delete.ToString(),
                        request,
                        errorMessage,
                        LogLevelInfo.Error.ToString(),
                        404,
                        _userInfoToken.Token);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                // Step 2: Soft delete the SpendingLimit
                existingSpendingLimit.IsDeleted = true;
                _SpendingLimitRepository.Update(existingSpendingLimit);

                // Step 3: Save changes in the unit of work
                await _uow.SaveAsync();

                // Step 4: Return a successful response
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting SpendingLimit: {e.Message}";

                // Step 5: Log error and return a 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(
                    _userInfoToken.Email,
                    LogAction.Delete.ToString(),
                    request,
                    errorMessage,
                    LogLevelInfo.Error.ToString(),
                    500,
                    _userInfoToken.Token);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}
