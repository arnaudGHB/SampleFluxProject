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
    /// Handles the command to add a new asset transfer based on AddAssetTransferCommand.
    /// </summary>
    public class AddAssetTransferCommandHandler : IRequestHandler<AddAssetTransferCommand, ServiceResponse<AssetTransferDto>>
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IAssetTransferRepository _assetTransferRepository;
        private readonly ILogger<AddAssetTransferCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<FixedAssetsContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        public AddAssetTransferCommandHandler(
            IAssetRepository assetRepository,
            IDepartmentRepository departmentRepository,
            IAssetTransferRepository assetTransferRepository,
            ILogger<AddAssetTransferCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<FixedAssetsContext> uow)
        {
            _assetRepository = assetRepository;
            _departmentRepository = departmentRepository;
            _assetTransferRepository = assetTransferRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
        }

        public async Task<ServiceResponse<AssetTransferDto>> Handle(AddAssetTransferCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Verify if the asset exists
                var asset = await _assetRepository.FindAsync(request.AssetId);
                if (asset == null)
                {
                    string message = $"Asset with ID '{request.AssetId}' not found.";
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<AssetTransferDto>.Return404(message);
                }

                // Verify if the from and to departments exist
                var fromDepartment = await _departmentRepository.FindBy(x => x.Id == request.FromDepartmentId).FirstOrDefaultAsync();
                if (fromDepartment == null)
                {
                    string message = $"From department with ID '{request.FromDepartmentId}' not found.";
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<AssetTransferDto>.Return404(message);
                }

                var toDepartment = await _departmentRepository.FindBy(x => x.Id == request.ToDepartmentId).FirstOrDefaultAsync();
                if (toDepartment == null)
                {
                    string message = $"To department with ID '{request.ToDepartmentId}' not found.";
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<AssetTransferDto>.Return404(message);
                }

                // Create the asset transfer entity
                var assetTransfer = new AssetTransfer
                {
                    AssetId = asset.Id,
                    FromDepartmentId = request.FromDepartmentId,
                    ToDepartmentId = request.ToDepartmentId,
                    TransferDate = request.TransferDate,
                    Reason = request.Reason,
                    Id = BaseUtilities.GenerateInsuranceUniqueNumber(15, "AST")
                };

                // Add the asset transfer to the repository
                _assetTransferRepository.Add(assetTransfer);

                // Update the asset to reflect the new department
                asset.DepartmentId = request.ToDepartmentId;
                _assetRepository.Update(asset);

                // Save changes
                await _uow.SaveAsync();

                // Log the successful transfer
                string successMessage = $"Asset '{asset.AssetName}' transferred from department '{fromDepartment.DepartmentName}' to '{toDepartment.DepartmentName}' successfully.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Map the asset transfer entity to DTO for response
                var assetTransferDto = _mapper.Map<AssetTransferDto>(assetTransfer);
                return ServiceResponse<AssetTransferDto>.ReturnResultWith200(assetTransferDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response
                string errorMessage = $"Error occurred while transferring asset: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<AssetTransferDto>.Return500(errorMessage);
            }
        }
    }
}
