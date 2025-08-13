using CBS.ProjectManagement.Common;
using CBS.ProjectManagement.Data.Entity;
using CBS.ProjectManagement.Domain.Context;
using Microsoft.EntityFrameworkCore;

namespace CBS.ProjectManagement.Repository
{
    /// <summary>
    /// Repository implementation for ProjectAssignment entity.
    /// </summary>
    public class ProjectAssignmentRepository : GenericRepository<ProjectAssignment, ProjectContext>, IProjectAssignmentRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectAssignmentRepository"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work instance.</param>
        public ProjectAssignmentRepository(IUnitOfWork<ProjectContext> unitOfWork) : base(unitOfWork)
        {
        }

        /// <inheritdoc />
        public async Task<ProjectAssignment> GetProjectAssignmentAsync(Guid projectId, Guid userId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(pa => pa.ProjectId == projectId && pa.UserId == userId && pa.IsActive);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ProjectAssignment>> GetAssignmentsByProjectAsync(Guid projectId)
        {
            return await _dbSet
                .Where(pa => pa.ProjectId == projectId && pa.IsActive)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ProjectAssignment>> GetAssignmentsByUserAsync(Guid userId)
        {
            return await _dbSet
                .Where(pa => pa.UserId == userId && pa.IsActive)
                .Include(pa => pa.Project)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<bool> IsUserAssignedToProjectAsync(Guid projectId, Guid userId)
        {
            return await _dbSet
                .AnyAsync(pa => pa.ProjectId == projectId && pa.UserId == userId && pa.IsActive);
        }

        /// <inheritdoc />
        public async Task<bool> DeactivateAssignmentAsync(Guid assignmentId)
        {
            var assignment = await _dbSet.FindAsync(assignmentId);
            if (assignment == null || !assignment.IsActive)
                return false;

            assignment.IsActive = false;
            _dbSet.Update(assignment);
            return true;
        }
    }
}
