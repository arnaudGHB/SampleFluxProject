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
    /// Handles the command to delete an Asset Type based on DeleteAssetTypeCommand.
    /// </summary>
    public class DeleteAssetTypeCommandHandler : IRequestHandler<DeleteAssetTypeCommand, ServiceResponse<bool>>
    {
        private readonly IAssetTypeRepository _assetTypeRepository; // Repository for accessing Asset Type data.
        private readonly ILogger<DeleteAssetTypeCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<FixedAssetsContext> _uow; // Unit of Work for managing database operations.
        private readonly UserInfoToken _userInfoToken; // User information for audit logging.

        /// <summary>
        /// Constructor for initializing the DeleteAssetTypeCommandHandler.
        /// </summary>
        /// <param name="assetTypeRepository">Repository for Asset Type data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="uow">Unit of Work for managing database operations.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        public DeleteAssetTypeCommandHandler(
            IAssetTypeRepository assetTypeRepository,
            ILogger<DeleteAssetTypeCommandHandler> logger,
            IUnitOfWork<FixedAssetsContext> uow,
            UserInfoToken userInfoToken)
        {
            _assetTypeRepository = assetTypeRepository;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the DeleteAssetTypeCommand to delete an Asset Type.
        /// </summary>
        /// <param name="request">The DeleteAssetTypeCommand containing the ID of the Asset Type to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteAssetTypeCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Step 1: Check if the Asset Type entity with the specified ID exists
                var existingAssetType = await _assetTypeRepository.FindAsync(request.Id);
                if (existingAssetType == null)
                {
                    errorMessage = $"Asset Type with ID {request.Id} not found.";
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

                // Step 2: Soft delete the Asset Type
                existingAssetType.IsDeleted = true;
                _assetTypeRepository.Update(existingAssetType);

                // Step 3: Save changes in the unit of work
                await _uow.SaveAsync();

                // Step 4: Return a successful response
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Asset Type: {e.Message}";

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
