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
    /// Handles the command to update an Project based on UpdateProjectCommand.
    /// </summary>
    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ServiceResponse<ProjectDto>>
    {
        private readonly IProjectRepository _ProjectRepository; // Repository for accessing Project data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<UpdateProjectCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<BudgetManagementContext> _uow; // Unit of Work for managing transactions.
        private readonly UserInfoToken _userInfoToken; // User information token for audit logging.

        /// <summary>
        /// Constructor for initializing the UpdateProjectCommandHandler.
        /// </summary>
        /// <param name="ProjectRepository">Repository for Project data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User info token for audit logging.</param>
        /// <param name="uow">Unit of Work for managing transactions.</param>
        public UpdateProjectCommandHandler(
            IProjectRepository ProjectRepository,
            IMapper mapper,
            ILogger<UpdateProjectCommandHandler> logger,
            UserInfoToken userInfoToken,
            IUnitOfWork<BudgetManagementContext> uow)
        {
            _ProjectRepository = ProjectRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateProjectCommand to update an Project.
        /// </summary>
        /// <param name="request">The UpdateProjectCommand containing updated Project data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<ProjectDto>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the Project entity to be updated from the repository
                var existingProject = await _ProjectRepository.FindAsync(request.Id);

                // Step 2: Check if the Project entity exists
                if (existingProject != null)
                {
                    // Step 3: Update Project entity properties with values from the request
                 
                    // Step 4: Use the repository to update the existing Project entity
                    _ProjectRepository.Update(existingProject);

                    // Step 5: Save changes using Unit of Work
                    await _uow.SaveAsync();

                    // Step 6: Log success message
                    string msg = $"Project '{existingProject.ProjectName}' updated successfully.";
                    _logger.LogInformation(msg);

                    // Step 7: Audit log the update action
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, msg, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    // Step 8: Map the updated entity to a DTO for the response
                    var ProjectDto = _mapper.Map<ProjectDto>(existingProject);

                    // Step 9: Return the updated ProjectDto with a 200 status code
                    return ServiceResponse<ProjectDto>.ReturnResultWith200(ProjectDto);
                }
                else
                {
                    // Step 10: If the Project entity was not found, log an error and return a 404 Not Found response with an error message
                    string errorMessage = $"Project '{request.ProjectName}' was not found.";
                    _logger.LogError(errorMessage);

                    // Step 11: Audit log the failed update attempt
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<ProjectDto>.Return404(errorMessage);
                }
            }
            catch (Exception ex)
            {
                // Step 12: Log error and return a 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Project: {ex.Message}";
                _logger.LogError(errorMessage);

                // Step 13: Audit log the error
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<ProjectDto>.Return500(errorMessage);
            }
        }
    }
}
