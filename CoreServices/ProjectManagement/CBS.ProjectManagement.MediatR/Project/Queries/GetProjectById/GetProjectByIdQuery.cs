using CBS.ProjectManagement.Data.Dto;
using CBS.ProjectManagement.Helper;
using MediatR;

namespace CBS.ProjectManagement.MediatR.Project.Queries
{
    /// <summary>
    /// Query to get a project by its ID.
    /// </summary>
    public class GetProjectByIdQuery : IRequest<ServiceResponse<ProjectDto>>
    {
        /// <summary>
        /// Gets or sets the ID of the project to retrieve.
        /// </summary>
        public Guid ProjectId { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether to include project assignments.
        /// </summary>
        public bool IncludeAssignments { get; set; } = false;
    }
}
