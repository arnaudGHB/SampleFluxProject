using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CBS.UserSkillManagement.Common;
using CBS.UserSkillManagement.Data.Dto;
using CBS.UserSkillManagement.Data;
using CBS.UserSkillManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace CBS.UserSkillManagement.Repository
{
    public class UserSkillRepository : GenericRepository<UserSkill, UserSkillContext>, IUserSkillRepository
    {
        public UserSkillRepository(IUnitOfWork<UserSkillContext> uow) : base(uow)
        {
        }

        public async Task<IEnumerable<UserSkill>> GetUserSkillsAsync(string userId)
        {
            return await Find(us => us.UserId == userId)
                .Include(us => us.Skill)
                .OrderByDescending(us => us.Level)
                .ThenBy(us => us.Skill.Name)
                .ToListAsync();
        }

        public async Task<UserSkill> GetUserSkillAsync(string userId, int skillId)
        {
            return await Find(us => us.UserId == userId && us.SkillId == skillId)
                .Include(us => us.Skill)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UserHasSkillAsync(string userId, int skillId)
        {
            return await AnyAsync(us => us.UserId == userId && us.SkillId == skillId);
        }

        public async Task<IEnumerable<UserSkill>> GetUsersBySkillAsync(int skillId, int minLevel = 0)
        {
            return await Find(us => us.SkillId == skillId && (int)us.Level >= minLevel)
                .Include(us => us.Skill)
                .OrderByDescending(us => us.Level)
                .ThenBy(us => us.YearsOfExperience)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserSkill>> SearchUserSkillsAsync(string searchTerm, string userId = null)
        {
            var query = DbSet.Include(us => us.Skill).AsQueryable();

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(us => us.UserId == userId);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(us => 
                    us.Skill.Name.ToLower().Contains(term) ||
                    us.Skill.Description != null && us.Skill.Description.ToLower().Contains(term) ||
                    us.Notes != null && us.Notes.ToLower().Contains(term));
            }

            return await query
                .OrderByDescending(us => us.Level)
                .ThenBy(us => us.Skill.Name)
                .ToListAsync();
        }
    }
}
