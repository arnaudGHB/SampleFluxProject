using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.FixedAssetsManagement.MediatR.Queries;
using CBS.FixedAssetsManagement.Repository;
using CBS.FixedAssetsManagement.Helper;
using CBS.FixedAssetsManagement.Domain;
using CBS.FixedAssetsManagement.Data;
using System.Threading;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of MaintenanceLog based on the GetMaintenanceLogQuery.
    /// </summary>
    public class GetMaintenanceLogQueryHandler : IRequestHandler<GetMaintenanceLogQuery, ServiceResponse<MaintenanceLogDto>>
    {
        private readonly IMaintenanceLogRepository _maintenanceLogRepository; // Repository for accessing MaintenanceLog data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetMaintenanceLogQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetMaintenanceLogQueryHandler.
        /// </summary>
        /// <param name="maintenanceLogRepository">Repository for MaintenanceLog data access.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetMaintenanceLogQueryHandler(
            IMaintenanceLogRepository maintenanceLogRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetMaintenanceLogQueryHandler> logger)
        {
            _maintenanceLogRepository = maintenanceLogRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetMaintenanceLogQuery to retrieve a MaintenanceLog.
        /// </summary>
        /// <param name="request">The GetMaintenanceLogQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<MaintenanceLogDto>> Handle(GetMaintenanceLogQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the MaintenanceLog entity from the repository
                var entity = await _maintenanceLogRepository.FindAsync(request.Id);

                // Step 2: Check if the entity exists
                if (entity == null)
                {
                    string notFoundMessage = $"MaintenanceLog with ID '{request.Id}' not found.";
                    _logger.LogWarning(notFoundMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, notFoundMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<MaintenanceLogDto>.Return404(notFoundMessage);
                }

                // Step 3: Log success and return the mapped entity as a DTO
                string successMessage = "MaintenanceLog returned successfully.";
                _logger.LogInformation(successMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                var maintenanceLogDto = _mapper.Map<MaintenanceLogDto>(entity);
                return ServiceResponse<MaintenanceLogDto>.ReturnResultWith200(maintenanceLogDto);
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve MaintenanceLog: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<MaintenanceLogDto>.Return500(e, "Failed to retrieve MaintenanceLog");
            }
        }
    }
}
