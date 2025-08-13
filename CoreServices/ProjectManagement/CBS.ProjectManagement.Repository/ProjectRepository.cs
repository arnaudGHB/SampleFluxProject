using CBS.ProjectManagement.Common;
using CBS.ProjectManagement.Data.Entity;
using CBS.ProjectManagement.Domain.Context;
using Microsoft.EntityFrameworkCore;

namespace CBS.ProjectManagement.Repository
{
    /// <summary>
    /// Repository implementation for Project entity.
    /// </summary>
    public class ProjectRepository : GenericRepository<Project, ProjectContext>, IProjectRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectRepository"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work instance.</param>
        public ProjectRepository(IUnitOfWork<ProjectContext> unitOfWork) : base(unitOfWork)
        {
        }

        /// <inheritdoc />
        public async Task<Project> GetProjectWithAssignmentsAsync(Guid projectId)
        {
            return await _dbSet
                .Include(p => p.ProjectAssignments)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);
        }

        /// <inheritdoc />
        public async Task<bool> ProjectExistsAsync(string projectName)
        {
            return await _dbSet
                .AnyAsync(p => p.Name.ToLower() == projectName.ToLower());
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Project>> GetActiveProjectsAsync()
        {
            return await _dbSet
                .Where(p => p.Status == ProjectStatus.Active)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }
    }
}
