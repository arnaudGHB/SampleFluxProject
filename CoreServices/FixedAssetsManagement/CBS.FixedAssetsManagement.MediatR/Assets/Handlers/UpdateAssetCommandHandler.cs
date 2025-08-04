using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.FixedAssetsManagement.Repository;
using CBS.FixedAssetsManagement.Data;
using CBS.FixedAssetsManagement.Common;
using CBS.FixedAssetsManagement.Domain;
using CBS.FixedAssetsManagement.Helper;
using CBS.FixedAssetsManagement.MediatR.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update an Asset based on UpdateAssetCommand.
    /// </summary>
    public class UpdateAssetCommandHandler : IRequestHandler<UpdateAssetCommand, ServiceResponse<AssetDto>>
    {
        private readonly IAssetRepository _assetRepository; // Repository for accessing Asset data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<UpdateAssetCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<FixedAssetsContext> _uow; // Unit of Work for managing transactions.
        private readonly UserInfoToken _userInfoToken; // User information token for audit logging.

        /// <summary>
        /// Constructor for initializing the UpdateAssetCommandHandler.
        /// </summary>
        /// <param name="assetRepository">Repository for Asset data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User info token for audit logging.</param>
        /// <param name="uow">Unit of Work for managing transactions.</param>
        public UpdateAssetCommandHandler(
            IAssetRepository assetRepository,
            IMapper mapper,
            ILogger<UpdateAssetCommandHandler> logger,
            UserInfoToken userInfoToken,
            IUnitOfWork<FixedAssetsContext> uow)
        {
            _assetRepository = assetRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateAssetCommand to update an Asset.
        /// </summary>
        /// <param name="request">The UpdateAssetCommand containing updated Asset data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AssetDto>> Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the Asset entity to be updated from the repository
                var existingAsset = await _assetRepository.FindAsync(request.Id);

                // Step 2: Check if the Asset entity exists
                if (existingAsset != null)
                {
                    // Step 3: Update Asset entity properties with values from the request
                    existingAsset.CurrentValue = request.CurrentValue;
                    existingAsset.Status = request.Status;

                    // Step 4: Use the repository to update the existing Asset entity
                    _assetRepository.Update(existingAsset);

                    // Step 5: Save changes using Unit of Work
                    await _uow.SaveAsync();

                    // Step 6: Log success message
                    string msg = $"Asset '{existingAsset.AssetName}' updated successfully.";
                    _logger.LogInformation(msg);

                    // Step 7: Audit log the update action
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, msg, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    // Step 8: Map the updated entity to a DTO for the response
                    var assetDto = _mapper.Map<AssetDto>(existingAsset);

                    // Step 9: Return the updated AssetDto with a 200 status code
                    return ServiceResponse<AssetDto>.ReturnResultWith200(assetDto);
                }
                else
                {
                    // Step 10: If the Asset entity was not found, log an error and return a 404 Not Found response with an error message
                    string errorMessage = $"Asset '{request.AssetName}' was not found.";
                    _logger.LogError(errorMessage);

                    // Step 11: Audit log the failed update attempt
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<AssetDto>.Return404(errorMessage);
                }
            }
            catch (Exception ex)
            {
                // Step 12: Log error and return a 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Asset: {ex.Message}";
                _logger.LogError(errorMessage);

                // Step 13: Audit log the error
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<AssetDto>.Return500(errorMessage);
            }
        }
    }
}
