using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class LoanProduct
    {
        public string id { get; set; }
        public string productCode { get; set; }
        public string productName { get; set; }
        public string loanInterestPeriod { get; set; }
    }


    public class SavingProduct
    {
        public string id { get; set; }
        public string productCode { get; set; }
        public string productName { get; set; }
        public string loanInterestPeriod { get; set; }
    }
}
