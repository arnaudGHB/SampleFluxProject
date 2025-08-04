using CBS.NLoan.Data.Entity.LoanApplicationP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Dto.LoanApplicationP
{
    public class LoanProductRepaymentCycleDto
    {
        public string Id { get; set; }
        public string RepaymentCycle { get; set; }
        public string LoanProductId { get; set; }
        public LoanProduct LoanProduct { get; set; }

    }
  
}
