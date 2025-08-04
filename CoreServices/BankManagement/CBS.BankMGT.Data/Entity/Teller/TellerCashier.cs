using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data
{
    public class TellerCashier : BaseEntity
    {
        public string Id { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string CashierID { get; set; }
        public bool IsFullDay { get; set; }
        public string Description { get; set; }
        public string TellerID { get; set; }
        public string BranchID { get; set; }
    }
}
