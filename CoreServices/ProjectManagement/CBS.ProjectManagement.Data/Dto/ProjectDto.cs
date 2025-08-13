using CBS.ProjectManagement.Data.Enum;
using System;
using System.Collections.Generic;

namespace CBS.ProjectManagement.Data.Dto
{
    /// <summary>
    /// Data Transfer Object for Project entity.
    /// </summary>
    public class ProjectDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the project.
        /// </summary>
        public Guid ProjectId { get; set; }
        
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
        /// Gets or sets the status of the project as a string.
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// Gets or sets the collection of project assignments.
        /// </summary>
        public ICollection<ProjectAssignmentDto> ProjectAssignments { get; set; }
    }
}
