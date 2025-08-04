using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class CashReplenishmentCallBackModel 
    { 
        public decimal Amount { get;   set; }
        public string replenishmentReferenceNumber { get;   set; }
        public string branchId { get;   set; }
    }

 
}
