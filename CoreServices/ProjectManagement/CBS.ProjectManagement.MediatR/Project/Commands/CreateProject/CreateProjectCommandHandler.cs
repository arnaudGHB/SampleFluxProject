using AutoMapper;
using CBS.ProjectManagement.Common;
using CBS.ProjectManagement.Data.Dto;
using CBS.ProjectManagement.Data.Entity;
using CBS.ProjectManagement.Data.Enum;
using CBS.ProjectManagement.Repository;
using MediatR;

namespace CBS.ProjectManagement.MediatR.Project.Commands.CreateProject
{
    /// <summary>
    /// Handler for the CreateProjectCommand.
    /// </summary>
    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ServiceResponse<ProjectDto>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<ProjectContext> _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateProjectCommandHandler"/> class.
        /// </summary>
        /// <param name="projectRepository">The project repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public CreateProjectCommandHandler(
            IProjectRepository projectRepository,
            IMapper mapper,
            IUnitOfWork<ProjectContext> unitOfWork)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Handles the create project command.
        /// </summary>
        /// <param name="request">The create project command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A service response containing the created project DTO or an error message.</returns>
        public async Task<ServiceResponse<ProjectDto>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            // Check if a project with the same name already exists
            if (await _projectRepository.ProjectExistsAsync(request.Name))
            {
                return ServiceResponse<ProjectDto>.ErrorResponse("A project with this name already exists.");
            }

            // Map the command to a Project entity
            var project = _mapper.Map<Project>(request);
            
            // Set additional properties
            project.Status = ProjectStatus.Active;
            project.CreatedBy = request.CreatedByUserId;
            project.UpdatedBy = request.CreatedByUserId;

            try
            {
                // Add the project to the repository
                await _projectRepository.AddAsync(project);
                
                // Save changes to the database
                var result = await _unitOfWork.SaveChangesAsync();
                
                if (result > 0)
                {
                    // Map the created project back to a DTO
                    var projectDto = _mapper.Map<ProjectDto>(project);
                    return ServiceResponse<ProjectDto>.SuccessResponse(projectDto, "Project created successfully.");
                }
                
                return ServiceResponse<ProjectDto>.ErrorResponse("Failed to create project.");
            }
            catch (Exception ex)
            {
                // Log the exception (in a real application, you would use ILogger)
                Console.WriteLine($"Error creating project: {ex.Message}");
                return ServiceResponse<ProjectDto>.ErrorResponse($"An error occurred while creating the project: {ex.Message}");
            }
        }
    }
}
