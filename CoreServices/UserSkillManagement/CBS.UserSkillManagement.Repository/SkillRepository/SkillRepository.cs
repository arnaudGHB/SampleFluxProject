using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CBS.UserSkillManagement.Common;
using CBS.UserSkillManagement.Data;
using CBS.UserSkillManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace CBS.UserSkillManagement.Repository
{
    public class SkillRepository : GenericRepository<Skill, UserSkillContext>, ISkillRepository
    {
        public SkillRepository(IUnitOfWork<UserSkillContext> uow) : base(uow)
        {
        }

        public async Task<IEnumerable<Skill>> GetSkillsByCategoryAsync(string category)
        {
            return await Find(s => s.Category == category && s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Skill>> SearchSkillsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            var term = searchTerm.ToLower();
            return await Find(s => 
                    s.IsActive && 
                    (s.Name.ToLower().Contains(term) || 
                     s.Description != null && s.Description.ToLower().Contains(term) ||
                     s.Category != null && s.Category.ToLower().Contains(term)))
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<bool> SkillExistsAsync(string name, string category)
        {
            return await AnyAsync(s => 
                s.Name.ToLower() == name.ToLower() && 
                s.Category.ToLower() == category.ToLower());
        }

        public async Task<Skill> GetSkillWithUserSkillsAsync(int skillId)
        {
            return await DbSet
                .Include(s => s.UserSkills)
                .FirstOrDefaultAsync(s => s.Id == skillId);
        }
    }
}
