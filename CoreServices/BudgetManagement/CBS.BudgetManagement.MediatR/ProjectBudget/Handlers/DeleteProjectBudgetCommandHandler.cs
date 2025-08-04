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
    /// Handles the command to delete an ProjectBudget based on DeleteProjectBudgetCommand.
    /// </summary>
    public class DeleteProjectBudgetCommandHandler : IRequestHandler<DeleteProjectBudgetCommand, ServiceResponse<bool>>
    {
        private readonly IProjectBudgetRepository _ProjectBudgetRepository; // Repository for accessing ProjectBudget data.
        private readonly ILogger<DeleteProjectBudgetCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<BudgetManagementContext> _uow; // Unit of Work for managing database operations.
        private readonly UserInfoToken _userInfoToken; // User information for audit logging.

        /// <summary>
        /// Constructor for initializing the DeleteProjectBudgetCommandHandler.
        /// </summary>
        /// <param name="ProjectBudgetRepository">Repository for ProjectBudget data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="uow">Unit of Work for managing database operations.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        public DeleteProjectBudgetCommandHandler(
            IProjectBudgetRepository ProjectBudgetRepository,
            ILogger<DeleteProjectBudgetCommandHandler> logger,
            IUnitOfWork<BudgetManagementContext> uow,
            UserInfoToken userInfoToken)
        {
            _ProjectBudgetRepository = ProjectBudgetRepository;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the DeleteProjectBudgetCommand to delete an ProjectBudget.
        /// </summary>
        /// <param name="request">The DeleteProjectBudgetCommand containing the ID of the ProjectBudget to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteProjectBudgetCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Step 1: Check if the ProjectBudget entity with the specified ID exists
                var existingProjectBudget = await _ProjectBudgetRepository.FindAsync(request.Id);
                if (existingProjectBudget == null)
                {
                    errorMessage = $"ProjectBudget with ID {request.Id} not found.";
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

                // Step 2: Soft delete the ProjectBudget
                existingProjectBudget.IsDeleted = true;
                _ProjectBudgetRepository.Update(existingProjectBudget);

                // Step 3: Save changes in the unit of work
                await _uow.SaveAsync();

                // Step 4: Return a successful response
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting ProjectBudget: {e.Message}";

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
