using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.ClossingOfAccountP
{
    public class ClossingOfAccount: BaseEntity
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public DateTime NotificationDate { get; set; }
        public DateTime MatrurityDate { get; set; }
        public DateTime GracePeriodDate { get; set; }
        public string? Purpose { get; set; }
        public string? Comment { get; set; }
        public bool IsNotificationPaid { get; set; }
        public decimal NotificationCharge { get; set; }
        public string Status { get; set; }
        public string InitiatingBranchId { get; set; }
        public string MemberBranchId { get; set; }

    }
}
