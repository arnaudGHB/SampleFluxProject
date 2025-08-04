using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data 
{
    public class ContraAccountMapping : BaseEntity
    {

        public string Id { get; set; }
        public string AccountingRuleId { get; set; }
        public string ContraAccountId { get; set; }
        public decimal ContraPercentage { get; set; }
        public virtual AccountingRuleX AccountingRule { get; set; }
        public virtual CashMovementTracker ContraAccount { get; set; }

    }
}
