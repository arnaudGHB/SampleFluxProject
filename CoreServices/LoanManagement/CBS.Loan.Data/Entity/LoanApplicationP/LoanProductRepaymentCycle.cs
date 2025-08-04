using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Entity.LoanApplicationP
{
    public class LoanProductRepaymentCycle:BaseEntity
    {
        public string Id { get; set; }
        public string RepaymentCycle { get; set; }
        public string LoanProductId { get; set; }
        public virtual LoanProduct LoanProduct { get; set; }

    }
  
}
