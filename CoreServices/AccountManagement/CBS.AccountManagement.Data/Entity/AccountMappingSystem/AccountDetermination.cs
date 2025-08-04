using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data 
{
    public class AccountDetermination : BaseEntity
    {

        public string Id { get; set; }
        public string AccountingRuleID { get; set; }
        public string DeterminationType { get; set; }
       // public string AccountType { get; set; }
        public string ChartOfAccountID { get; set; }
        public decimal AllocationPercentage { get; set; }
        public string AllocationFormula { get; set; }
        public virtual AccountingRuleX AccountingRule { get; set; }
        public virtual CashMovementTracker ChartOfAccount { get; set; }

    }

   
}
