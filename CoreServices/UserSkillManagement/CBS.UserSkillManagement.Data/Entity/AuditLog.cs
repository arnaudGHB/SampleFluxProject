using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBS.UserSkillManagement.Data
{
    [Table("AuditLogs")]
    public class AuditLog : BaseEntity
    {
        public string? UserEmail;
        public string? EntityName;
        public DateTime Timestamp;
        public string Changes;
        public string? IPAddress;
        public string Url;
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string TableName { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Action { get; set; } // "Create", "Update", "Delete"
        
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string AffectedColumns { get; set; }
        public string PrimaryKey { get; set; }
        
        [MaxLength(200)]
        public string IpAddress { get; set; }

       
    }
}
