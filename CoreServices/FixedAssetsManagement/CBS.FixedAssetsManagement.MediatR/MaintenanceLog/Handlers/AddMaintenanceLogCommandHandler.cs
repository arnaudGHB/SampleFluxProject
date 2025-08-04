using MediatR;
using Microsoft.Extensions.Logging;
using CBS.FixedAssetsManagement.Common;
using CBS.FixedAssetsManagement.Domain;
using CBS.FixedAssetsManagement.Repository;
using AutoMapper;
using CBS.FixedAssetsManagement.MediatR.Commands;
using CBS.FixedAssetsManagement.Helper;
using CBS.FixedAssetsManagement.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CBS.FixedAssetsManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new maintenance log based on AddMaintenanceLogCommand.
    /// </summary>
    public class AddMaintenanceLogCommandHandler : IRequestHandler<AddMaintenanceLogCommand, ServiceResponse<MaintenanceLogDto>>
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IMaintenanceLogRepository _maintenanceLogRepository;

        private readonly ILogger<AddMaintenanceLogCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<FixedAssetsContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        public AddMaintenanceLogCommandHandler(
            IAssetRepository assetRepository,
            IMaintenanceLogRepository maintenanceLogRepository,
      
            ILogger<AddMaintenanceLogCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<FixedAssetsContext> uow)
        {
            _assetRepository = assetRepository;
            _maintenanceLogRepository = maintenanceLogRepository;
 
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
        }

        public async Task<ServiceResponse<MaintenanceLogDto>> Handle(AddMaintenanceLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if the asset exists
                var asset = await _assetRepository.FindAsync(request.AssetId);
                if (asset == null)
                {
                    string message = $"Asset with ID '{request.AssetId}' not found.";
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<MaintenanceLogDto>.Return404(message);
                }

                // Create the maintenance log entry
                var maintenanceLog = new MaintenanceLog
                {
                    AssetId = asset.Id,
                    MaintenanceDate = request.MaintenanceDate,
                    Description = request.Description,
                    Cost = request.Cost,
                    PerformedById = request.PerformedById,
                    Id = BaseUtilities.GenerateInsuranceUniqueNumber(15, "MTN")
                };

                // Add the maintenance log to the repository
                _maintenanceLogRepository.Add(maintenanceLog);
                await _uow.SaveAsync();

                // Log the successful entry
                string successMessage = $"Maintenance log for asset '{asset.AssetName}' on '{request.MaintenanceDate}' added successfully.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Map the maintenance log entity to a DTO for response
                var maintenanceLogDto = _mapper.Map<MaintenanceLogDto>(maintenanceLog);
                return ServiceResponse<MaintenanceLogDto>.ReturnResultWith200(maintenanceLogDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response
                string errorMessage = $"Error occurred while adding maintenance log: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<MaintenanceLogDto>.Return500(errorMessage);
            }
        }
    }
}
