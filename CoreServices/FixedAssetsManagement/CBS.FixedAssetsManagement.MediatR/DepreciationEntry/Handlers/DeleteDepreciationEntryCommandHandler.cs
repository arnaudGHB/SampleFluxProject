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
    /// Handles the command to delete a Depreciation Entry based on DeleteDepreciationEntryCommand.
    /// </summary>
    public class DeleteDepreciationEntryCommandHandler : IRequestHandler<DeleteDepreciationEntryCommand, ServiceResponse<bool>>
    {
        private readonly IDepreciationEntryRepository _depreciationEntryRepository; // Repository for accessing Depreciation Entry data.
        private readonly ILogger<DeleteDepreciationEntryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<FixedAssetsContext> _uow; // Unit of Work for managing database operations.
        private readonly UserInfoToken _userInfoToken; // User information for audit logging.

        /// <summary>
        /// Constructor for initializing the DeleteDepreciationEntryCommandHandler.
        /// </summary>
        /// <param name="depreciationEntryRepository">Repository for Depreciation Entry data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="uow">Unit of Work for managing database operations.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        public DeleteDepreciationEntryCommandHandler(
            IDepreciationEntryRepository depreciationEntryRepository,
            ILogger<DeleteDepreciationEntryCommandHandler> logger,
            IUnitOfWork<FixedAssetsContext> uow,
            UserInfoToken userInfoToken)
        {
            _depreciationEntryRepository = depreciationEntryRepository;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the DeleteDepreciationEntryCommand to delete a Depreciation Entry.
        /// </summary>
        /// <param name="request">The DeleteDepreciationEntryCommand containing the ID of the Depreciation Entry to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteDepreciationEntryCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Step 1: Check if the Depreciation Entry entity with the specified ID exists
                var existingEntry = await _depreciationEntryRepository.FindAsync(request.Id);
                if (existingEntry == null)
                {
                    errorMessage = $"Depreciation Entry with ID {request.Id} not found.";
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

                // Step 2: Soft delete the Depreciation Entry
                existingEntry.IsDeleted = true;
                _depreciationEntryRepository.Update(existingEntry);

                // Step 3: Save changes in the unit of work
                await _uow.SaveAsync();

                // Step 4: Return a successful response
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Depreciation Entry: {e.Message}";

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
