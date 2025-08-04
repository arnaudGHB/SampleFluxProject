using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.FixedAssetsManagement.Repository;
using CBS.FixedAssetsManagement.Data;
using CBS.FixedAssetsManagement.Common;
using CBS.FixedAssetsManagement.Domain;
using CBS.FixedAssetsManagement.Helper;
using System;
using System.Threading;
using System.Threading.Tasks;
using CBS.FixedAssetsManagement.MediatR.Commands;

namespace CBS.FixedAssetsManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a MaintenanceLog based on UpdateMaintenanceLogCommand.
    /// </summary>
    public class UpdateMaintenanceLogCommandHandler : IRequestHandler<UpdateMaintenanceLogCommand, ServiceResponse<MaintenanceLogDto>>
    {
        private readonly IMaintenanceLogRepository _maintenanceLogRepository; // Repository for accessing MaintenanceLog data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<UpdateMaintenanceLogCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<FixedAssetsContext> _uow; // Unit of Work for managing transactions.
        private readonly UserInfoToken _userInfoToken; // User information token for audit logging.

        /// <summary>
        /// Constructor for initializing the UpdateMaintenanceLogCommandHandler.
        /// </summary>
        /// <param name="maintenanceLogRepository">Repository for MaintenanceLog data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User info token for audit logging.</param>
        /// <param name="uow">Unit of Work for managing transactions.</param>
        public UpdateMaintenanceLogCommandHandler(
            IMaintenanceLogRepository maintenanceLogRepository,
            IMapper mapper,
            ILogger<UpdateMaintenanceLogCommandHandler> logger,
            UserInfoToken userInfoToken,
            IUnitOfWork<FixedAssetsContext> uow)
        {
            _maintenanceLogRepository = maintenanceLogRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateMaintenanceLogCommand to update a MaintenanceLog.
        /// </summary>
        /// <param name="request">The UpdateMaintenanceLogCommand containing updated maintenance log data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<MaintenanceLogDto>> Handle(UpdateMaintenanceLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the MaintenanceLog entity to be updated from the repository
                var existingLog = await _maintenanceLogRepository.FindAsync( request.Id) ;

                // Step 2: Check if the MaintenanceLog entity exists
                if (existingLog != null)
                {
                    // Step 3: Update MaintenanceLog entity properties with values from the request
                    existingLog.MaintenanceDate = request.MaintenanceDate;
                    existingLog.Description = request.Description;
                    existingLog.Cost = request.Cost;
                    existingLog.PerformedById = request.PerformedById;

                    // Step 4: Use the repository to update the existing MaintenanceLog entity
                    _maintenanceLogRepository.Update(existingLog);

                    // Step 5: Save changes using Unit of Work
                    await _uow.SaveAsync( );

                    // Step 6: Log success message
                    string msg = $"Maintenance log '{existingLog.Id}' updated successfully.";
                    _logger.LogInformation(msg);

                    // Step 7: Audit log the update action
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, msg, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    // Step 8: Map the updated entity to a DTO for the response
                    var logDto = _mapper.Map<MaintenanceLogDto>(existingLog);

                    // Step 9: Return the updated MaintenanceLogDto with a 200 status code
                    return ServiceResponse<MaintenanceLogDto>.ReturnResultWith200(logDto);
                }
                else
                {
                    // Step 10: If the MaintenanceLog entity was not found, log an error and return a 404 Not Found response with an error message
                    string errorMessage = $"Maintenance log with ID '{request.Id}' was not found.";
                    _logger.LogError(errorMessage);

                    // Step 11: Audit log the failed update attempt
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<MaintenanceLogDto>.Return404(errorMessage);
                }
            }
            catch (Exception ex)
            {
                // Step 12: Log error and return a 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Maintenance log: {ex.Message}";
                _logger.LogError(errorMessage);

                // Step 13: Audit log the error
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<MaintenanceLogDto>.Return500(errorMessage);
            }
        }
    }
}
