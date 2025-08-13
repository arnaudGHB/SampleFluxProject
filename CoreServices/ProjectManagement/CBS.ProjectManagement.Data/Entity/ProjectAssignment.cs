using System;
using System.ComponentModel.DataAnnotations;
using CBS.ProjectManagement.Common;

namespace CBS.ProjectManagement.Data.Entity
{
    /// <summary>
    /// Represents an assignment of a user to a project.
    /// </summary>
    public class ProjectAssignment : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the project assignment.
        /// </summary>
        [Key]
        public Guid AssignmentId { get; set; } = Guid.NewGuid();
        
        /// <summary>
        /// Gets or sets the ID of the project.
        /// </summary>
        [Required]
        public Guid ProjectId { get; set; }
        
        /// <summary>
        /// Gets or sets the ID of the user assigned to the project.
        /// </summary>
        [Required]
        public Guid UserId { get; set; }
        
        /// <summary>
        /// Gets or sets the date when the user was assigned to the project.
        /// </summary>
        [Required]
        public DateTime AssignmentDate { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Gets or sets a value indicating whether the assignment is currently active.
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Gets or sets the navigation property for the project.
        /// </summary>
        public virtual Project Project { get; set; }
    }
}
