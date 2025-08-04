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
    /// Handles the command to add a new asset type based on AddAssetTypeCommand.
    /// </summary>
    public class AddAssetTypeCommandHandler : IRequestHandler<AddAssetTypeCommand, ServiceResponse<AssetTypeDto>>
    {
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IDepreciationMethodRepository _depreciationMethodRepository;
        private readonly ILogger<AddAssetTypeCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<FixedAssetsContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        public AddAssetTypeCommandHandler(
            IAssetTypeRepository assetTypeRepository,
            IDepreciationMethodRepository depreciationMethodRepository,
            ILogger<AddAssetTypeCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<FixedAssetsContext> uow)
        {
            _assetTypeRepository = assetTypeRepository;
            _depreciationMethodRepository = depreciationMethodRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
        }

        public async Task<ServiceResponse<AssetTypeDto>> Handle(AddAssetTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if an asset type with the same name already exists
                var existingAssetType = await _assetTypeRepository.FindBy(x => x.TypeName == request.TypeName).FirstOrDefaultAsync();

                if (existingAssetType != null)
                {
                    string message = $"Asset type with name '{request.TypeName}' already exists.";
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<AssetTypeDto>.Return409(message);
                }

                // Check if the depreciation method exists
                var depreciationMethod = await _depreciationMethodRepository.FindAsync(request.DepreciationMethodId);
                if (depreciationMethod == null)
                {
                    string message = $"Depreciation method with ID '{request.DepreciationMethodId}' not found.";
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<AssetTypeDto>.Return404(message);
                }

                // Create the asset type entity
                var assetType = new AssetType
                {
                    TypeName = request.TypeName,
                    Description = request.Description,
                    DepreciationMethodId = request.DepreciationMethodId,
                    UsefulLifeYears = request.UsefulLifeYears
                };

                // Add the asset type to the repository
                _assetTypeRepository.Add(assetType);
                await _uow.SaveAsync();

                // Log successful creation
                string successMessage = $"Asset type '{request.TypeName}' created successfully.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Map the entity to DTO for response
                var assetTypeDto = _mapper.Map<AssetTypeDto>(assetType);
                return ServiceResponse<AssetTypeDto>.ReturnResultWith200(assetTypeDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response
                string errorMessage = $"Error occurred while adding asset type: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<AssetTypeDto>.Return500(errorMessage);
            }
        }
    }
}
