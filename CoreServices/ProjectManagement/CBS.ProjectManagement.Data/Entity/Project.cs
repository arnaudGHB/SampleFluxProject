using System;
using System.ComponentModel.DataAnnotations;
using CBS.ProjectManagement.Data.Enum;
using CBS.ProjectManagement.Common;

namespace CBS.ProjectManagement.Data.Entity
{
    /// <summary>
    /// Represents a project in the system.
    /// </summary>
    public class Project : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the project.
        /// </summary>
        [Key]
        public Guid ProjectId { get; set; } = Guid.NewGuid();
        
        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the description of the project.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Gets or sets the start date of the project.
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }
        
        /// <summary>
        /// Gets or sets the end date of the project.
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }
        
        /// <summary>
        /// Gets or sets the status of the project.
        /// </summary>
        public ProjectStatus Status { get; set; } = ProjectStatus.Active;
        
        /// <summary>
        /// Gets or sets the collection of project assignments.
        /// </summary>
        public virtual ICollection<ProjectAssignment> ProjectAssignments { get; set; }
    }
}
