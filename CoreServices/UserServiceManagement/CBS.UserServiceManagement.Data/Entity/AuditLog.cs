using System;
using System.ComponentModel.DataAnnotations;

namespace CBS.UserServiceManagement.Data
{
    public class AuditLog
    {
        [Key]
        public Guid Id { get; set; }
        public string? UserId { get; set; }
        public string? UserEmail { get; set; }
        public string? EntityName { get; set; }
        public string? Action { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Changes { get; set; }
        public string? IPAddress { get; set; }
        public string? Url { get; set; }
    }
}
