using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.FixedAssetsManagement.Common;
using CBS.FixedAssetsManagement.Domain;
using CBS.FixedAssetsManagement.Repository;
using AutoMapper;
using CBS.FixedAssetsManagement.MediatR.Commands;
using CBS.FixedAssetsManagement.Helper;
using CBS.FixedAssetsManagement.Data;

namespace CBS.FixedAssetsManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Asset based on AddAssetCommand.
    /// </summary>
    public class AddAssetCommandHandler : IRequestHandler<AddAssetCommand, ServiceResponse<AssetDto>>
    {
        private readonly IAssetRepository _assetRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILogger<AddAssetCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<FixedAssetsContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        public AddAssetCommandHandler(
            IAssetRepository assetRepository,
            ILocationRepository locationRepository,
            IDepartmentRepository departmentRepository,
            ILogger<AddAssetCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<FixedAssetsContext> uow)
        {
            _assetRepository = assetRepository;
            _locationRepository = locationRepository;
            _departmentRepository = departmentRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
        }

        public async Task<ServiceResponse<AssetDto>> Handle(AddAssetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if an asset with the same serial number already exists
                var existingAsset = await _assetRepository.FindBy(x => x.SerialNumber == request.SerialNumber).FirstOrDefaultAsync();
                string message = $"Asset '{request.AssetName}' created successfully.";

                if (existingAsset != null)
                {
                    message = $"Asset with serial number '{request.SerialNumber}' already exists.";
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<AssetDto>.Return409(message);
                }

                // Verify if the Location exists
                var location = await _locationRepository.FindBy(x => x.Id == request.LocationId).FirstOrDefaultAsync();
                if (location == null)
                {
                    message = $"Location with ID '{request.LocationId}' does not exist.";
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<AssetDto>.Return404(message);
                }

                // Verify if the Department exists
                var department = await _departmentRepository.FindBy(x => x.Id == request.DepartmentId).FirstOrDefaultAsync();
                if (department == null)
                {
                    message = $"Department with ID '{request.DepartmentId}' does not exist.";
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<AssetDto>.Return404(message);
                }

                // Map the request to an Asset entity
                var asset = _mapper.Map<Asset>(request);
                asset.Id = BaseUtilities.GenerateInsuranceUniqueNumber(25, "AST");
                asset.CurrentValue = request.PurchaseCost; // Initially, current value is the same as purchase cost
                asset.Status = "Active"; // Set initial status

                // Add the new asset to the repository
                _assetRepository.Add(asset);
                await _uow.SaveAsync();

                // Log successful creation of the asset
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Map the asset entity back to a DTO for response
                var assetDto = _mapper.Map<AssetDto>(asset);
                return ServiceResponse<AssetDto>.ReturnResultWith200(assetDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                string msg = $"Error occurred while adding asset: {e.Message}";
                _logger.LogError(msg);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, msg, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<AssetDto>.Return500(msg);
            }
        }
    }
}
