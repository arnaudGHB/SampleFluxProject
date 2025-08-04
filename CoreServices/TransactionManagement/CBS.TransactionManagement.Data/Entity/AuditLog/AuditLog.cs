using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? UserFullName { get; set; }
        public string? BranchId { get; set; }
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }
        public string? IPAddress { get; set; }
        public DateTime Timestamp { get; set; }
        public string Url { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string LogLevel { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }  // If you want to log exceptions
    }
    public class FrontEndAuditTB
    {
        [Key]
        public string UsersAuditID { get; set; }
        public string? UserName { get; set; }
        public string? UserRole { get; set; } // New property
        public string? SessionID { get; set; }
        public string? IPAddress { get; set; }
        public string? PageAccessed { get; set; }
        public DateTime? LoggedInAt { get; set; }
        public DateTime? LoggedOutAt { get; set; }
        public string? LoginStatus { get; set; }
        public string? ControllerName { get; set; }
        public string? ActionName { get; set; }
        public string? RequestMethod { get; set; } // New property
        public string? UserAgent { get; set; } // New property
        public string? Referer { get; set; } // New property
        public string? SerializedRequestData { get; set; }
        public string? BranchName { get; set; }
        public string? AjaxPostData { get; set; }
    }
}
