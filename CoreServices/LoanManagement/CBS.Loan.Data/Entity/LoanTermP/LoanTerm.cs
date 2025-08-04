using CBS.NLoan.Data.Entity.LoanApplicationP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Entity.LoanTermP
{
    public class LoanTerm:BaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int MinInMonth { get; set; }
        public int MaxInMonth { get; set; }
        public virtual ICollection<LoanProduct> LoanProducts { get; set; }
    }
}
