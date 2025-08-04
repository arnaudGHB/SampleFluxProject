using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.FixedAssetsManagement.MediatR.Queries;
using CBS.FixedAssetsManagement.Repository;
using CBS.FixedAssetsManagement.Helper;
using CBS.FixedAssetsManagement.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CBS.FixedAssetsManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all MaintenanceLogs based on the GetAllMaintenanceLogsQuery.
    /// </summary>
    public class GetAllMaintenanceLogsQueryHandler : IRequestHandler<GetAllMaintenanceLogsQuery, ServiceResponse<List<MaintenanceLogDto>>>
    {
        private readonly IMaintenanceLogRepository _maintenanceLogRepository; // Repository for accessing MaintenanceLog data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllMaintenanceLogsQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllMaintenanceLogsQueryHandler.
        /// </summary>
        /// <param name="maintenanceLogRepository">Repository for MaintenanceLog data access.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllMaintenanceLogsQueryHandler(
            IMaintenanceLogRepository maintenanceLogRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetAllMaintenanceLogsQueryHandler> logger)
        {
            _maintenanceLogRepository = maintenanceLogRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllMaintenanceLogsQuery to retrieve all MaintenanceLogs.
        /// </summary>
        /// <param name="request">The GetAllMaintenanceLogsQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<MaintenanceLogDto>>> Handle(GetAllMaintenanceLogsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve all MaintenanceLog entities from the repository
                var entities = await _maintenanceLogRepository.All.ToListAsync(cancellationToken);

                // Step 2: Log the successful retrieval of maintenance logs
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Maintenance logs returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Step 3: Return the result with a successful response
                return ServiceResponse<List<MaintenanceLogDto>>.ReturnResultWith200(_mapper.Map<List<MaintenanceLogDto>>(entities));
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve MaintenanceLogs: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<MaintenanceLogDto>>.Return500(e, "Failed to retrieve MaintenanceLogs");
            }
        }
    }
}
