using AutoMapper;
using CBS.ProjectManagement.Common;
using CBS.ProjectManagement.Data.Dto;
using CBS.ProjectManagement.Repository;
using MediatR;

namespace CBS.ProjectManagement.MediatR.Project.Queries.GetProjectById
{
    /// <summary>
    /// Handler for the GetProjectByIdQuery.
    /// </summary>
    public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ServiceResponse<ProjectDto>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetProjectByIdQueryHandler"/> class.
        /// </summary>
        /// <param name="projectRepository">The project repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        public GetProjectByIdQueryHandler(IProjectRepository projectRepository, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the get project by ID query.
        /// </summary>
        /// <param name="request">The get project by ID query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A service response containing the project DTO or an error message.</returns>
        public async Task<ServiceResponse<ProjectDto>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Get the project by ID, optionally including assignments
                var project = request.IncludeAssignments 
                    ? await _projectRepository.GetProjectWithAssignmentsAsync(request.ProjectId)
                    : await _projectRepository.GetByIdAsync(request.ProjectId);

                // Check if the project was found
                if (project == null)
                {
                    return ServiceResponse<ProjectDto>.ErrorResponse("Project not found.");
                }

                // Map the project to a DTO
                var projectDto = _mapper.Map<ProjectDto>(project);
                
                // Set the status as a string for the DTO
                projectDto.Status = project.Status.ToString();
                
                return ServiceResponse<ProjectDto>.SuccessResponse(projectDto, "Project retrieved successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception (in a real application, you would use ILogger)
                Console.WriteLine($"Error retrieving project: {ex.Message}");
                return ServiceResponse<ProjectDto>.ErrorResponse($"An error occurred while retrieving the project: {ex.Message}");
            }
        }
    }
}
