using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.Data
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? UserEmail { get; set; }
        public string? IPAddress { get; set; }
        public string? Url { get; set; }
        public required string EntityName { get; set; }
        public required string Action { get; set; }
        public DateTime Timestamp { get; set; }
        public required string Changes { get; set; }
    }
}
