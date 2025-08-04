using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.Communication.Data.Dto
{
    public  class NotificationDto
    {
        public string? Id { get; set; } 
        public string? NotificationType { get; set; } 
        public string? Body { get; set; }

        public string? Title { get; set; }
        public string Status { get; set; }
        public string? MemberReference { get; set; }
        public string? CustomerName { get; set; }
        public string BankID { get; set; }
        public string BranchID { get; set; }
        public string BranchName{ get; set; }
        public DateTime Timestamp { get; set; }
    }
}
