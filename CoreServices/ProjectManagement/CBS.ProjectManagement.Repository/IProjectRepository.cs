using CBS.ProjectManagement.Common;
using CBS.ProjectManagement.Data.Entity;

namespace CBS.ProjectManagement.Repository
{
    /// <summary>
    /// Defines the interface for Project repository operations.
    /// </summary>
    public interface IProjectRepository : IGenericRepository<Project>
    {
        /// <summary>
        /// Gets a project by its unique identifier including its assignments.
        /// </summary>
        /// <param name="projectId">The ID of the project to retrieve.</param>
        /// <returns>The project with the specified ID, or null if not found.</returns>
        Task<Project> GetProjectWithAssignmentsAsync(Guid projectId);
        
        /// <summary>
        /// Checks if a project with the specified name already exists.
        /// </summary>
        /// <param name="projectName">The name of the project to check.</param>
        /// <returns>True if a project with the specified name exists, otherwise false.</returns>
        Task<bool> ProjectExistsAsync(string projectName);
        
        /// <summary>
        /// Gets all active projects.
        /// </summary>
        /// <returns>A list of active projects.</returns>
        Task<IEnumerable<Project>> GetActiveProjectsAsync();
    }
}
