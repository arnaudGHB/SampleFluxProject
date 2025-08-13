using CBS.UserSkillManagement.Data.Dto;
using CBS.UserSkillManagement.Data;
using CBS.UserSkillManagement.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CBS.UserSkillManagement.Repository
{
    public interface ISkillRepository : IGenericRepository<Skill>
    {
        Task<IEnumerable<Skill>> GetSkillsByCategoryAsync(string category);
        Task<IEnumerable<Skill>> SearchSkillsAsync(string searchTerm);
        Task<bool> SkillExistsAsync(string name, string category);
        Task<Skill> GetSkillWithUserSkillsAsync(int skillId);
    }
}
