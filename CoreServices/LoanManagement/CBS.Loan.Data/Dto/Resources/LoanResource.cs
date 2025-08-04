using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Dto.Resources
{
    public class LoanResource : ResourceParameter
    {
        public LoanResource() : base("CustomerId")
        {
        }
        public bool IsByBranch { get; set; }

    }

}
