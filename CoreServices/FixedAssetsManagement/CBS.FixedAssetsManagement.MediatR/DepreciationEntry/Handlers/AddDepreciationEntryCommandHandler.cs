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
    /// Handles the command to add a new depreciation entry based on AddDepreciationEntryCommand.
    /// </summary>
    public class AddDepreciationEntryCommandHandler : IRequestHandler<AddDepreciationEntryCommand, ServiceResponse<DepreciationEntryDto>>
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IDepreciationEntryRepository _depreciationEntryRepository;
        private readonly ILogger<AddDepreciationEntryCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<FixedAssetsContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        public AddDepreciationEntryCommandHandler(
            IAssetRepository assetRepository,
            IDepreciationEntryRepository depreciationEntryRepository,
            ILogger<AddDepreciationEntryCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<FixedAssetsContext> uow)
        {
            _assetRepository = assetRepository;
            _depreciationEntryRepository = depreciationEntryRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
        }

        public async Task<ServiceResponse<DepreciationEntryDto>> Handle(AddDepreciationEntryCommand request, CancellationToken cancellationToken)
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
                    return ServiceResponse<DepreciationEntryDto>.Return404(message);
                }

                // Check if the book value after depreciation is valid
                if (request.BookValueAfter < 0)
                {
                    string message = "Book value after depreciation cannot be negative.";
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 400, _userInfoToken.Token);
                    return ServiceResponse<DepreciationEntryDto>.Return400(message);
                }

                // Create the depreciation entry
                var depreciationEntry = new DepreciationEntry
                {
                    AssetId = asset.Id,
                    DepreciationDate = request.DepreciationDate,
                    DepreciationAmount = request.DepreciationAmount,
                    BookValueAfter = request.BookValueAfter,
                    Id= BaseUtilities.GenerateInsuranceUniqueNumber(15,"DEP")
                };

                // Add the depreciation entry to the repository
                _depreciationEntryRepository.Add(depreciationEntry);

                // Update the asset's current book value
                asset.CurrentValue = request.BookValueAfter;
                _assetRepository.Update(asset);

                // Save changes
                await _uow.SaveAsync();

                // Log the successful entry
                string successMessage = $"Depreciation entry for asset '{asset.AssetName}' on '{request.DepreciationDate}' recorded successfully.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Map the depreciation entry to a DTO for response
                var depreciationEntryDto = _mapper.Map<DepreciationEntryDto>(depreciationEntry);
                return ServiceResponse<DepreciationEntryDto>.ReturnResultWith200(depreciationEntryDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response
                string errorMessage = $"Error occurred while recording depreciation entry: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<DepreciationEntryDto>.Return500(errorMessage);
            }
        }
    }
}
