using System;
using System.ComponentModel.DataAnnotations;

namespace CBS.UserSkillManagement.Data.Dto
{
    public class SkillDto
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string Category { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }

    public class CreateSkillDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string Category { get; set; }
    }

    public class UpdateSkillDto
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string Category { get; set; }
        
        public bool IsActive { get; set; }
    }
}
