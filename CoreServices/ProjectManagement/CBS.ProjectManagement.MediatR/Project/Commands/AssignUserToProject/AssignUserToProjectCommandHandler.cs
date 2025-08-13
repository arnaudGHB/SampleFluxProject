using AutoMapper;
using CBS.ProjectManagement.Common;
using CBS.ProjectManagement.Data.Dto;
using CBS.ProjectManagement.Data.Entity;
using CBS.ProjectManagement.Repository;
using MediatR;

namespace CBS.ProjectManagement.MediatR.Project.Commands.AssignUserToProject
{
    /// <summary>
    /// Handler for the AssignUserToProjectCommand.
    /// </summary>
    public class AssignUserToProjectCommandHandler : IRequestHandler<AssignUserToProjectCommand, ServiceResponse<ProjectAssignmentDto>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectAssignmentRepository _projectAssignmentRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<ProjectContext> _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssignUserToProjectCommandHandler"/> class.
        /// </summary>
        /// <param name="projectRepository">The project repository.</param>
        /// <param name="projectAssignmentRepository">The project assignment repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public AssignUserToProjectCommandHandler(
            IProjectRepository projectRepository,
            IProjectAssignmentRepository projectAssignmentRepository,
            IMapper mapper,
            IUnitOfWork<ProjectContext> unitOfWork)
        {
            _projectRepository = projectRepository;
            _projectAssignmentRepository = projectAssignmentRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Handles the assign user to project command.
        /// </summary>
        /// <param name="request">The assign user to project command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A service response containing the project assignment DTO or an error message.</returns>
        public async Task<ServiceResponse<ProjectAssignmentDto>> Handle(AssignUserToProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if the project exists
                var project = await _projectRepository.GetByIdAsync(request.ProjectId);
                if (project == null)
                {
                    return ServiceResponse<ProjectAssignmentDto>.ErrorResponse("Project not found.");
                }

                // Check if the user is already assigned to the project
                if (await _projectAssignmentRepository.IsUserAssignedToProjectAsync(request.ProjectId, request.UserId))
                {
                    return ServiceResponse<ProjectAssignmentDto>.ErrorResponse("User is already assigned to this project.");
                }

                // Create a new project assignment
                var assignment = new ProjectAssignment
                {
                    ProjectId = request.ProjectId,
                    UserId = request.UserId,
                    IsActive = true,
                    CreatedBy = request.AssignedByUserId,
                    UpdatedBy = request.AssignedByUserId
                };

                // Add the assignment to the repository
                await _projectAssignmentRepository.AddAsync(assignment);
                
                // Save changes to the database
                var result = await _unitOfWork.SaveChangesAsync();
                
                if (result > 0)
                {
                    // Map the created assignment to a DTO
                    var assignmentDto = _mapper.Map<ProjectAssignmentDto>(assignment);
                    return ServiceResponse<ProjectAssignmentDto>.SuccessResponse(assignmentDto, "User assigned to project successfully.");
                }
                
                return ServiceResponse<ProjectAssignmentDto>.ErrorResponse("Failed to assign user to project.");
            }
            catch (Exception ex)
            {
                // Log the exception (in a real application, you would use ILogger)
                Console.WriteLine($"Error assigning user to project: {ex.Message}");
                return ServiceResponse<ProjectAssignmentDto>.ErrorResponse($"An error occurred while assigning user to project: {ex.Message}");
            }
        }
    }
}
