using CBS.TransactionManagement.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Dto
{
    public class DailyTellerDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ProvisionedBy { get; set; }
        public string TellerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Status { get; set; }
        public bool IsPrimary { get; set; }
        public string BranchId { get; set; }
        public decimal MaximumWithdrawalAmount { get; set; }
        public decimal MaximumCeilin { get; set; }
        public TellerDto Teller { get; set; }
        public List<PrimaryTellerProvisioningHistoryDto> PrimaryTellerProvisioningHistories { get; set; }
        public List<SubTellerProvioningHistoryDto> SubTellerProvioningHistories { get; set; }

    }
}
