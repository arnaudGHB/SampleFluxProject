using CBS.AccountManagement.Data.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class AccountingRuleDto
    {
         
            public string Id { get; set; }
            public string RuleName { get; set; }
            public string Description { get; set; }
            public string SystemDescription { get; set; }
            public string System_Id { get; set; }
            public string BookingDirection { get; set; }
            public string MFI_ChartOfAccountId { get; set; }
            public string AccountNumber { get; set; }
            public double Amount { get; set; }
            public string AccountName { get; set; }
        
    }
}
