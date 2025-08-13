using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBS.UserSkillManagement.Data.Enum;

namespace CBS.UserSkillManagement.Data
{
    [Table("UserSkills")]
    public class UserSkill : BaseEntity
    {
        [Required]
        [MaxLength(450)]
        public string UserId { get; set; }

        [Required]
        public int SkillId { get; set; }

        [Required]
        public SkillLevel Level { get; set; }

        public int? YearsOfExperience { get; set; }

        public DateTime? LastUsed { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }

        // Navigation properties
        [ForeignKey("SkillId")]
        public virtual Skill Skill { get; set; }
    }
}
