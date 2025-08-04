using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.FixedAssetsManagement.Helper;
using CBS.FixedAssetsManagement.Repository;
using CBS.FixedAssetsManagement.Common;
using CBS.FixedAssetsManagement.Data;
using CBS.FixedAssetsManagement.Domain;
using System.Threading;
using System.Threading.Tasks;
using CBS.FixedAssetsManagement.MediatR.Commands;

namespace CBS.FixedAssetsManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Maintenance Log based on DeleteMaintenanceLogCommand.
    /// </summary>
    public class DeleteMaintenanceLogCommandHandler : IRequestHandler<DeleteMaintenanceLogCommand, ServiceResponse<bool>>
    {
        private readonly IMaintenanceLogRepository _maintenanceLogRepository; // Repository for accessing Maintenance Log data.
        private readonly ILogger<DeleteMaintenanceLogCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<FixedAssetsContext> _uow; // Unit of Work for managing database operations.
        private readonly UserInfoToken _userInfoToken; // User information for audit logging.

        /// <summary>
        /// Constructor for initializing the DeleteMaintenanceLogCommandHandler.
        /// </summary>
        /// <param name="maintenanceLogRepository">Repository for Maintenance Log data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="uow">Unit of Work for managing database operations.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        public DeleteMaintenanceLogCommandHandler(
            IMaintenanceLogRepository maintenanceLogRepository,
            ILogger<DeleteMaintenanceLogCommandHandler> logger,
            IUnitOfWork<FixedAssetsContext> uow,
            UserInfoToken userInfoToken)
        {
            _maintenanceLogRepository = maintenanceLogRepository;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the DeleteMaintenanceLogCommand to delete a Maintenance Log.
        /// </summary>
        /// <param name="request">The DeleteMaintenanceLogCommand containing the ID of the Maintenance Log to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteMaintenanceLogCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Step 1: Check if the Maintenance Log entity with the specified ID exists
                var existingLog = await _maintenanceLogRepository.FindAsync(request.Id);
                if (existingLog == null)
                {
                    errorMessage = $"Maintenance Log with ID {request.Id} not found.";
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

                // Step 2: Soft delete the Maintenance Log
                existingLog.IsDeleted = true;
                _maintenanceLogRepository.Update(existingLog);

                // Step 3: Save changes in the unit of work
                await _uow.SaveAsync();

                // Step 4: Return a successful response
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Maintenance Log: {e.Message}";

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
