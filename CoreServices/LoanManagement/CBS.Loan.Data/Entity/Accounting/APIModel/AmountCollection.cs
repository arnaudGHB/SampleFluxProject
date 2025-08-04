using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Data
{
   
    public class AmountCollection
    {
        public string EventAttributeName { get; set; }
        public decimal Amount { get; set; }
        public bool IsPrincipal { get; set; }
    }
}
