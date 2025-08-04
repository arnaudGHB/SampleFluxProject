using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class UsersNotificationDto
    {
        public string Id { get; set; }
              public string UserId { get; set; }
        public string UserName { get; set; }
        public string BranchId { get; set; }
        public string Action { get; set; }
        public string ActionId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ActionUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsSeen { get; set; } = false;
    }
}
