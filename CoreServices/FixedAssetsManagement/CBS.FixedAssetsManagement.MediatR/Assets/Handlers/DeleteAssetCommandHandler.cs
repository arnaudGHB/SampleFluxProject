using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.FixedAssetsManagement.Helper;
using CBS.FixedAssetsManagement.Repository;
using CBS.FixedAssetsManagement.Common;
using CBS.FixedAssetsManagement.Data;
using CBS.FixedAssetsManagement.Domain;
using System.Threading;
using System.Threading.Tasks;
using CBS.FixedAssetsManagement.MediatR.Commands;

namespace CBS.FixedAssetsManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete an Asset based on DeleteAssetCommand.
    /// </summary>
    public class DeleteAssetCommandHandler : IRequestHandler<DeleteAssetCommand, ServiceResponse<bool>>
    {
        private readonly IAssetRepository _assetRepository; // Repository for accessing Asset data.
        private readonly ILogger<DeleteAssetCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<FixedAssetsContext> _uow; // Unit of Work for managing database operations.
        private readonly UserInfoToken _userInfoToken; // User information for audit logging.

        /// <summary>
        /// Constructor for initializing the DeleteAssetCommandHandler.
        /// </summary>
        /// <param name="assetRepository">Repository for Asset data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="uow">Unit of Work for managing database operations.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        public DeleteAssetCommandHandler(
            IAssetRepository assetRepository,
            ILogger<DeleteAssetCommandHandler> logger,
            IUnitOfWork<FixedAssetsContext> uow,
            UserInfoToken userInfoToken)
        {
            _assetRepository = assetRepository;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the DeleteAssetCommand to delete an Asset.
        /// </summary>
        /// <param name="request">The DeleteAssetCommand containing the ID of the Asset to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteAssetCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Step 1: Check if the Asset entity with the specified ID exists
                var existingAsset = await _assetRepository.FindAsync(request.Id);
                if (existingAsset == null)
                {
                    errorMessage = $"Asset with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(
                        _userInfoToken.Email,
                        LogAction.Delete.ToString(),
                        request,
                        errorMessage,
                        LogLevelInfo.Error.ToString(),
                        404,
                        _userInfoToken.Token);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                // Step 2: Soft delete the Asset
                existingAsset.IsDeleted = true;
                _assetRepository.Update(existingAsset);

                // Step 3: Save changes in the unit of work
                await _uow.SaveAsync();

                // Step 4: Return a successful response
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Asset: {e.Message}";

                // Step 5: Log error and return a 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(
                    _userInfoToken.Email,
                    LogAction.Delete.ToString(),
                    request,
                    errorMessage,
                    LogLevelInfo.Error.ToString(),
                    500,
                    _userInfoToken.Token);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}
