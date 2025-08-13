using CBS.ProjectManagement.Common;
using CBS.ProjectManagement.Data.Entity;

namespace CBS.ProjectManagement.Repository
{
    /// <summary>
    /// Defines the interface for ProjectAssignment repository operations.
    /// </summary>
    public interface IProjectAssignmentRepository : IGenericRepository<ProjectAssignment>
    {
        /// <summary>
        /// Gets a project assignment by project ID and user ID.
        /// </summary>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The project assignment if found, otherwise null.</returns>
        Task<ProjectAssignment> GetProjectAssignmentAsync(Guid projectId, Guid userId);
        
        /// <summary>
        /// Gets all project assignments for a specific project.
        /// </summary>
        /// <param name="projectId">The ID of the project.</param>
        /// <returns>A list of project assignments for the specified project.</returns>
        Task<IEnumerable<ProjectAssignment>> GetAssignmentsByProjectAsync(Guid projectId);
        
        /// <summary>
        /// Gets all project assignments for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of project assignments for the specified user.</returns>
        Task<IEnumerable<ProjectAssignment>> GetAssignmentsByUserAsync(Guid userId);
        
        /// <summary>
        /// Checks if a user is already assigned to a project.
        /// </summary>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>True if the user is already assigned to the project, otherwise false.</returns>
        Task<bool> IsUserAssignedToProjectAsync(Guid projectId, Guid userId);
        
        /// <summary>
        /// Deactivates a project assignment.
        /// </summary>
        /// <param name="assignmentId">The ID of the assignment to deactivate.</param>
        /// <returns>True if the assignment was found and deactivated, otherwise false.</returns>
        Task<bool> DeactivateAssignmentAsync(Guid assignmentId);
    }
}
