using CBS.UserSkillManagement.Data.Dto;
using CBS.UserSkillManagement.Data;
using CBS.UserSkillManagement.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CBS.UserSkillManagement.Repository
{
    public interface IUserSkillRepository : IGenericRepository<UserSkill>
    {
        Task<IEnumerable<UserSkill>> GetUserSkillsAsync(string userId);
        Task<UserSkill> GetUserSkillAsync(string userId, int skillId);
        Task<bool> UserHasSkillAsync(string userId, int skillId);
        Task<IEnumerable<UserSkill>> GetUsersBySkillAsync(int skillId, int minLevel = 0);
        Task<IEnumerable<UserSkill>> SearchUserSkillsAsync(string searchTerm, string userId = null);
    }
}
