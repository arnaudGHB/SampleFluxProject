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
    /// Handles the command to delete a Depreciation Method based on DeleteDepreciationMethodCommand.
    /// </summary>
    public class DeleteDepreciationMethodCommandHandler : IRequestHandler<DeleteDepreciationMethodCommand, ServiceResponse<bool>>
    {
        private readonly IDepreciationMethodRepository _depreciationMethodRepository; // Repository for accessing Depreciation Method data.
        private readonly ILogger<DeleteDepreciationMethodCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<FixedAssetsContext> _uow; // Unit of Work for managing database operations.
        private readonly UserInfoToken _userInfoToken; // User information for audit logging.

        /// <summary>
        /// Constructor for initializing the DeleteDepreciationMethodCommandHandler.
        /// </summary>
        /// <param name="depreciationMethodRepository">Repository for Depreciation Method data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="uow">Unit of Work for managing database operations.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        public DeleteDepreciationMethodCommandHandler(
            IDepreciationMethodRepository depreciationMethodRepository,
            ILogger<DeleteDepreciationMethodCommandHandler> logger,
            IUnitOfWork<FixedAssetsContext> uow,
            UserInfoToken userInfoToken)
        {
            _depreciationMethodRepository = depreciationMethodRepository;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the DeleteDepreciationMethodCommand to delete a Depreciation Method.
        /// </summary>
        /// <param name="request">The DeleteDepreciationMethodCommand containing the ID of the Depreciation Method to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteDepreciationMethodCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Step 1: Check if the Depreciation Method entity with the specified ID exists
                var existingMethod = await _depreciationMethodRepository.FindAsync(request.Id);
                if (existingMethod == null)
                {
                    errorMessage = $"Depreciation Method with ID {request.Id} not found.";
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

                // Step 2: Soft delete the Depreciation Method
                existingMethod.IsDeleted = true;
                _depreciationMethodRepository.Update(existingMethod);

                // Step 3: Save changes in the unit of work
                await _uow.SaveAsync();

                // Step 4: Return a successful response
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Depreciation Method: {e.Message}";

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
