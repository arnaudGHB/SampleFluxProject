using CBS.TransactionManagement.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Dto
{
    public class OpenningAnclClossingTillDto
    {
        public string UserIdInChargeOfThisTeller { get; set; }
        public string ProvisionedBy { get; set; }
        public string OpeningReference { get; set; }
        public bool IsCashReplenished { get; set; }
        public string TellerType { get; set; }
        public string IsPrimary { get; set; }
        public string ClossingReference { get; set; }
        public decimal ReplenishedAmount { get; set; }
        public DateTime OpenedDate { get; set; }
        public DateTime ClossedDate { get; set; } = new DateTime(1900, 1, 1);
        public decimal OpenOfDayAmount { get; set; } = 0;
        public decimal AmountReplenished { get; set; }
        public bool IsRequestedForCashReplenishment { get; set; }
        public decimal CashAtHand { get; set; } = 0;
        public decimal EndOfDayAmount { get; set; } = 0;
        public decimal AccountBalance { get; set; } = 0;
        public decimal LastOPerationAmount { get; set; } = 0;
        public string LastOperationType { get; set; }
        public decimal PreviouseBalance { get; set; }
        public string TellerComment { get; set; }
        public string BranchCode { get; set; }
        public string ClossedStatus { get; set; }
        public string PrimaryTellerComment { get; set; }
        public string PrimaryTellerConfirmationStatus { get; set; }
        public string TellerName { get; set; }
    }
}
