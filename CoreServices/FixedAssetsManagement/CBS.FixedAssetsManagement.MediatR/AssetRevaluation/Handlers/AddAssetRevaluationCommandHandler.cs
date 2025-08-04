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
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new asset revaluation based on AddAssetRevaluationCommand.
    /// </summary>
    public class AddAssetRevaluationCommandHandler : IRequestHandler<AddAssetRevaluationCommand, ServiceResponse<AssetRevaluationDto>>
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IAssetRevaluationRepository _assetRevaluationRepository;
        private readonly ILogger<AddAssetRevaluationCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<FixedAssetsContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        public AddAssetRevaluationCommandHandler(
            IAssetRepository assetRepository,
            IAssetRevaluationRepository assetRevaluationRepository,
            ILogger<AddAssetRevaluationCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<FixedAssetsContext> uow)
        {
            _assetRepository = assetRepository;
            _assetRevaluationRepository = assetRevaluationRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
        }

        public async Task<ServiceResponse<AssetRevaluationDto>> Handle(AddAssetRevaluationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Find the asset by ID
                var asset = await _assetRepository.FindBy(a => a.Id == request.AssetId).FirstOrDefaultAsync();

                if (asset == null)
                {
                    string message = $"Asset with ID '{request.AssetId}' not found.";
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<AssetRevaluationDto>.Return404(message);
                }

                // Create the revaluation entity
                var assetRevaluation = new AssetRevaluation
                {
                    AssetId = asset.Id,
                    RevaluationDate = request.RevaluationDate,
                    OldValue = request.OldValue,
                    NewValue = request.NewValue,
                    Reason = request.Reason,
                    Id = BaseUtilities.GenerateInsuranceUniqueNumber(15, "ADIS")
                };

                // Add the revaluation record to the repository
                _assetRevaluationRepository.Add(assetRevaluation);

                // Update the asset's current value to the new value
                asset.CurrentValue = request.NewValue;
                _assetRepository.Update(asset);

                // Save changes
                await _uow.SaveAsync();

                // Log successful asset revaluation
                string revaluationMessage = $"Asset '{asset.AssetName}' revaluated successfully from {request.OldValue} to {request.NewValue}.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, revaluationMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Map the asset revaluation entity to a DTO for response
                var assetRevaluationDto = _mapper.Map<AssetRevaluationDto>(assetRevaluation);
                return ServiceResponse<AssetRevaluationDto>.ReturnResultWith200(assetRevaluationDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                string msg = $"Error occurred while revaluating asset: {e.Message}";
                _logger.LogError(msg);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, msg, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<AssetRevaluationDto>.Return500(msg);
            }
        }
    }
}
