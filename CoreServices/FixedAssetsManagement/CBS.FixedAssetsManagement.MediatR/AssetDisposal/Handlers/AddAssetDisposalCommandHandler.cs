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
    /// Handles the command to add a new asset disposal based on AddAssetDisposalCommand.
    /// </summary>
    public class AddAssetDisposalCommandHandler : IRequestHandler<AddAssetDisposalCommand, ServiceResponse<AssetDisposalDto>>
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IAssetDisposalRepository _assetDisposalRepository;
        private readonly ILogger<AddAssetDisposalCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<FixedAssetsContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        public AddAssetDisposalCommandHandler(
            IAssetRepository assetRepository,
            IAssetDisposalRepository assetDisposalRepository,
            ILogger<AddAssetDisposalCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<FixedAssetsContext> uow)
        {
            _assetRepository = assetRepository;
            _assetDisposalRepository = assetDisposalRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
        }

        public async Task<ServiceResponse<AssetDisposalDto>> Handle(AddAssetDisposalCommand request, CancellationToken cancellationToken)
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
                    return ServiceResponse<AssetDisposalDto>.Return404(message);
                }

                // Create the disposal entity
                var assetDisposal = new AssetDisposal
                {
                    AssetId = asset.Id,
                    DisposalDate = request.DisposalDate,
                    DisposalMethod = request.DisposalMethod,
                    DisposalValue = request.DisposalValue,
                    Reason = request.Reason,
                    Id = BaseUtilities.GenerateInsuranceUniqueNumber(15,"ADIS")
                };

                // Add the disposal record to the repository
                _assetDisposalRepository.Add(assetDisposal);

                // Update the asset status
                asset.Status = "Disposed";
                _assetRepository.Update(asset);

                // Save changes
                await _uow.SaveAsync();

                // Log successful asset disposal
                string disposalMessage = $"Asset '{asset.AssetName}' disposed successfully.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, disposalMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Map the asset disposal entity to a DTO for response
                var assetDisposalDto = _mapper.Map<AssetDisposalDto>(assetDisposal);
                return ServiceResponse<AssetDisposalDto>.ReturnResultWith200(assetDisposalDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                string msg = $"Error occurred while disposing asset: {e.Message}";
                _logger.LogError(msg);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, msg, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<AssetDisposalDto>.Return500(msg);
            }
        }
    }
}
