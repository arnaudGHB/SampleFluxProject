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
    /// Handles the command to delete an Asset Revaluation based on DeleteAssetRevaluationCommand.
    /// </summary>
    public class DeleteAssetRevaluationCommandHandler : IRequestHandler<DeleteAssetRevaluationCommand, ServiceResponse<bool>>
    {
        private readonly IAssetRevaluationRepository _assetRevaluationRepository; // Repository for accessing Asset Revaluation data.
        private readonly ILogger<DeleteAssetRevaluationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<FixedAssetsContext> _uow; // Unit of Work for managing database operations.
        private readonly UserInfoToken _userInfoToken; // User information for audit logging.

        /// <summary>
        /// Constructor for initializing the DeleteAssetRevaluationCommandHandler.
        /// </summary>
        /// <param name="assetRevaluationRepository">Repository for Asset Revaluation data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="uow">Unit of Work for managing database operations.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        public DeleteAssetRevaluationCommandHandler(
            IAssetRevaluationRepository assetRevaluationRepository,
            ILogger<DeleteAssetRevaluationCommandHandler> logger,
            IUnitOfWork<FixedAssetsContext> uow,
            UserInfoToken userInfoToken)
        {
            _assetRevaluationRepository = assetRevaluationRepository;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the DeleteAssetRevaluationCommand to delete an Asset Revaluation.
        /// </summary>
        /// <param name="request">The DeleteAssetRevaluationCommand containing the ID of the Asset Revaluation to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteAssetRevaluationCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Step 1: Check if the Asset Revaluation entity with the specified ID exists
                var existingRevaluation = await _assetRevaluationRepository.FindAsync(request.Id);
                if (existingRevaluation == null)
                {
                    errorMessage = $"Asset Revaluation with ID {request.Id} not found.";
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

                // Step 2: Soft delete the Asset Revaluation
                existingRevaluation.IsDeleted = true;
                _assetRevaluationRepository.Update(existingRevaluation);

                // Step 3: Save changes in the unit of work
                await _uow.SaveAsync();

                // Step 4: Return a successful response
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Asset Revaluation: {e.Message}";

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
