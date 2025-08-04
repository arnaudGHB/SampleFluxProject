using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data
{
    public class DailyTeller : BaseEntity
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ProvisionedBy { get; set; }
        public string TellerId { get; set; }
        public bool Status { get; set; }
        public bool IsPrimary { get; set; }
        public string BranchId { get; set; }
        public decimal MaximumWithdrawalAmount { get; set; }
        public decimal MaximumCeilin { get; set; }
        public virtual Teller Teller { get; set; }
        public virtual ICollection<PrimaryTellerProvisioningHistory> PrimaryTellerProvisioningHistories { get; set; }
        public virtual ICollection<SubTellerProvioningHistory> SubTellerProvioningHistories { get; set; }

    }
}
