using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BudgetManagement.Helper;
using CBS.BudgetManagement.Repository;
using CBS.BudgetManagement.Common;
using CBS.BudgetManagement.Data;

using System.Threading;
using System.Threading.Tasks;
using CBS.BudgetManagement.MediatR.Commands;
using CBS.BudgetManagement.Domain;

namespace CBS.BudgetManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete an Project based on DeleteProjectCommand.
    /// </summary>
    public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, ServiceResponse<bool>>
    {
        private readonly IProjectRepository _ProjectRepository; // Repository for accessing Project data.
        private readonly ILogger<DeleteProjectCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<BudgetManagementContext> _uow; // Unit of Work for managing database operations.
        private readonly UserInfoToken _userInfoToken; // User information for audit logging.

        /// <summary>
        /// Constructor for initializing the DeleteProjectCommandHandler.
        /// </summary>
        /// <param name="ProjectRepository">Repository for Project data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="uow">Unit of Work for managing database operations.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        public DeleteProjectCommandHandler(
            IProjectRepository ProjectRepository,
            ILogger<DeleteProjectCommandHandler> logger,
            IUnitOfWork<BudgetManagementContext> uow,
            UserInfoToken userInfoToken)
        {
            _ProjectRepository = ProjectRepository;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the DeleteProjectCommand to delete an Project.
        /// </summary>
        /// <param name="request">The DeleteProjectCommand containing the ID of the Project to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Step 1: Check if the Project entity with the specified ID exists
                var existingProject = await _ProjectRepository.FindAsync(request.Id);
                if (existingProject == null)
                {
                    errorMessage = $"Project with ID {request.Id} not found.";
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

                // Step 2: Soft delete the Project
                existingProject.IsDeleted = true;
                _ProjectRepository.Update(existingProject);

                // Step 3: Save changes in the unit of work
                await _uow.SaveAsync();

                // Step 4: Return a successful response
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Project: {e.Message}";

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
