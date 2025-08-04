using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data
{
    public class TellerCashAllocation : BaseEntity
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string TellerCashierID { get; set; }
        public double Amount { get; set; }
        public string CurrencyID { get; set; }
        public string Note { get; set; }
        public string BranchID { get; set; }
    }
}
