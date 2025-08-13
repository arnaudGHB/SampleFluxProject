using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBS.UserSkillManagement.Data
{
    [Table("Skills")]
    public class Skill : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string Category { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation property
        public virtual ICollection<UserSkill> UserSkills { get; set; }
    }
}
