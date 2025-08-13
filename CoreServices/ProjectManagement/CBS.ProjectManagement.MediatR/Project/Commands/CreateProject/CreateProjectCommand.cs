using CBS.ProjectManagement.Data.Dto;
using CBS.ProjectManagement.Helper;
using MediatR;

namespace CBS.ProjectManagement.MediatR.Project.Commands
{
    /// <summary>
    /// Command to create a new project.
    /// </summary>
    public class CreateProjectCommand : IRequest<ServiceResponse<ProjectDto>>
    {
        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the description of the project.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Gets or sets the start date of the project.
        /// </summary>
        public DateTime StartDate { get; set; }
        
        /// <summary>
        /// Gets or sets the end date of the project.
        /// </summary>
        public DateTime EndDate { get; set; }
        
        /// <summary>
        /// Gets or sets the ID of the user creating the project.
        /// </summary>
        public string CreatedByUserId { get; set; }
    }
}
