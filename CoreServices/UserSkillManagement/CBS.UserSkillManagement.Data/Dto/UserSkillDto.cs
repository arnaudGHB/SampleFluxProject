using System;
using System.ComponentModel.DataAnnotations;
using CBS.UserSkillManagement.Data.Enum;

namespace CBS.UserSkillManagement.Data.Dto
{
    public class UserSkillDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public string SkillCategory { get; set; }
        public SkillLevel Level { get; set; }
        public string LevelName => Level.ToString();
        public int? YearsOfExperience { get; set; }
        public DateTime? LastUsed { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }

    public class AddUserSkillDto
    {
        [Required]
        public int SkillId { get; set; }
        
        [Required]
        public SkillLevel Level { get; set; }
        
        [Range(0, 50)]
        public int? YearsOfExperience { get; set; }
        
        public DateTime? LastUsed { get; set; }
        
        [MaxLength(500)]
        public string Notes { get; set; }
    }

    public class UpdateUserSkillDto
    {
        [Required]
        public int UserSkillId { get; set; }
        
        [Required]
        public SkillLevel Level { get; set; }
        
        [Range(0, 50)]
        public int? YearsOfExperience { get; set; }
        
        public DateTime? LastUsed { get; set; }
        
        [MaxLength(500)]
        public string Notes { get; set; }
    }
}
