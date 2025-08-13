using CBS.ProjectManagement.Data.Dto;
using CBS.ProjectManagement.Helper;
using MediatR;

namespace CBS.ProjectManagement.MediatR.Project.Commands
{
    /// <summary>
    /// Command to assign a user to a project.
    /// </summary>
    public class AssignUserToProjectCommand : IRequest<ServiceResponse<ProjectAssignmentDto>>
    {
        /// <summary>
        /// Gets or sets the ID of the project.
        /// </summary>
        public Guid ProjectId { get; set; }
        
        /// <summary>
        /// Gets or sets the ID of the user to assign.
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// Gets or sets the ID of the user making the assignment.
        /// </summary>
        public string AssignedByUserId { get; set; }
    }
}
