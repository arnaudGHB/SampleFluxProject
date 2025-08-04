using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data 
{
    public class TemplateAccountMapping
    {

        public string Id { get; set; }
        public string TemplateID { get; set; }
        public int LegNumber { get; set; }
        public string AccountID { get; set; }
        public string AmountFormula { get; set; }

        public virtual AccountingTemplate AccountingTemplate { get; set; }
        public virtual CashMovementTracker ChartOfAccount { get; set; }

    }

}
