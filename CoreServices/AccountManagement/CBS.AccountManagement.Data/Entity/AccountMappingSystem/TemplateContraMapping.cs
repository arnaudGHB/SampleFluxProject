using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data 
{
    public class TemplateContraMapping
    {

        public int ContraMappingID { get; set; }
        public int TemplateID { get; set; }
        public int ContraAccountID { get; set; }
        public decimal ContraPercentage { get; set; }

        public virtual AccountingTemplate AccountingTemplate { get; set; }
        public virtual CashMovementTracker ContraAccount { get; set; }

    }
}
