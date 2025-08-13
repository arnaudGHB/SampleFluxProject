using System;

namespace CBS.ProjectManagement.Data.Dto
{
    /// <summary>
    /// Data Transfer Object for ProjectAssignment entity.
    /// </summary>
    public class ProjectAssignmentDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the project assignment.
        /// </summary>
        public Guid AssignmentId { get; set; }
        
        /// <summary>
        /// Gets or sets the ID of the project.
        /// </summary>
        public Guid ProjectId { get; set; }
        
        /// <summary>
        /// Gets or sets the ID of the user assigned to the project.
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// Gets or sets the date when the user was assigned to the project.
        /// </summary>
        public DateTime AssignmentDate { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the assignment is currently active.
        /// </summary>
        public bool IsActive { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the user assigned to the project.
        /// This is typically populated when retrieving data, not when creating/updating.
        /// </summary>
        public string UserName { get; set; }
        
        /// <summary>
        /// Gets or sets the email of the user assigned to the project.
        /// This is typically populated when retrieving data, not when creating/updating.
        /// </summary>
        public string UserEmail { get; set; }
    }
}
