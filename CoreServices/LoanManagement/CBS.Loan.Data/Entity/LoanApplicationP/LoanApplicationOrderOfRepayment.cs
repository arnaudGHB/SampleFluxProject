using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Entity.LoanApplicationP
{
    public class LoanApplicationOrderOfRepayment
    {
        public string Id { get; set; }
        public string OrderOfRepayment { get; set; }
        public string OrderOfRepaymentName { get; set; }
        public string LoanApplicationId { get; set; }
        public virtual LoanApplication LoanApplication { get; set; }

    }
}
