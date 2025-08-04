using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data 
{
    public class  TrialBalanceItem
    {
        public string? AccountNumber { get; set; }
        public string? AccountName { get; set; }
        public string?BeginningBalance { get; set; }
        public string? DebitBalance { get; set; }
        public string? CreditBalance { get; set; }
        public string? EndBalance { get; set; }

    }
}
