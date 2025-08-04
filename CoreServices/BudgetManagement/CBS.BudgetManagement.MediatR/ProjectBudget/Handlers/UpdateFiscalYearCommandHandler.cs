using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BudgetManagement.Repository;
using CBS.BudgetManagement.Data;
using CBS.BudgetManagement.Common;
 
using CBS.BudgetManagement.Helper;
using CBS.BudgetManagement.MediatR.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;
using CBS.BudgetManagement.Domain;

namespace CBS.BudgetManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update an ProjectBudget based on UpdateProjectBudgetCommand.
    /// </summary>
    public class UpdateProjectBudgetCommandHandler : IRequestHandler<UpdateProjectBudgetCommand, ServiceResponse<ProjectBudgetDto>>
    {
        private readonly IProjectBudgetRepository _ProjectBudgetRepository; // Repository for accessing ProjectBudget data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<UpdateProjectBudgetCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<BudgetManagementContext> _uow; // Unit of Work for managing transactions.
        private readonly UserInfoToken _userInfoToken; // User information token for audit logging.

        /// <summary>
        /// Constructor for initializing the UpdateProjectBudgetCommandHandler.
        /// </summary>
        /// <param name="ProjectBudgetRepository">Repository for ProjectBudget data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User info token for audit logging.</param>
        /// <param name="uow">Unit of Work for managing transactions.</param>
        public UpdateProjectBudgetCommandHandler(
            IProjectBudgetRepository ProjectBudgetRepository,
            IMapper mapper,
            ILogger<UpdateProjectBudgetCommandHandler> logger,
            UserInfoToken userInfoToken,
            IUnitOfWork<BudgetManagementContext> uow)
        {
            _ProjectBudgetRepository = ProjectBudgetRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateProjectBudgetCommand to update an ProjectBudget.
        /// </summary>
        /// <param name="request">The UpdateProjectBudgetCommand containing updated ProjectBudget data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<ProjectBudgetDto>> Handle(UpdateProjectBudgetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the ProjectBudget entity to be updated from the repository
                var existingProjectBudget = await _ProjectBudgetRepository.FindAsync(request.Id);

                // Step 2: Check if the ProjectBudget entity exists
                if (existingProjectBudget != null)
                {
                    // Step 3: Update ProjectBudget entity properties with values from the request
                 
                    // Step 4: Use the repository to update the existing ProjectBudget entity
                    _ProjectBudgetRepository.Update(existingProjectBudget);

                    // Step 5: Save changes using Unit of Work
                    await _uow.SaveAsync();

                    // Step 6: Log success message
                    string msg = $"ProjectBudget '{existingProjectBudget.Id}' updated successfully.";
                    _logger.LogInformation(msg);

                    // Step 7: Audit log the update action
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, msg, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    // Step 8: Map the updated entity to a DTO for the response
                    var ProjectBudgetDto = _mapper.Map<ProjectBudgetDto>(existingProjectBudget);

                    // Step 9: Return the updated ProjectBudgetDto with a 200 status code
                    return ServiceResponse<ProjectBudgetDto>.ReturnResultWith200(ProjectBudgetDto);
                }
                else
                {
                    // Step 10: If the ProjectBudget entity was not found, log an error and return a 404 Not Found response with an error message
                    string errorMessage = $"ProjectBudget '{request.Id}' was not found.";
                    _logger.LogError(errorMessage);

                    // Step 11: Audit log the failed update attempt
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<ProjectBudgetDto>.Return404(errorMessage);
                }
            }
            catch (Exception ex)
            {
                // Step 12: Log error and return a 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating ProjectBudget: {ex.Message}";
                _logger.LogError(errorMessage);

                // Step 13: Audit log the error
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<ProjectBudgetDto>.Return500(errorMessage);
            }
        }
    }
}
