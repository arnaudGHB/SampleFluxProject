using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Entity.LoanApplicationP
{
    public class DisburstedLoan
    {
        public string Id { get; set; }
        public string LoanId { get; set; }
        public DateTime DisbursmentDate { get; set; }
        public string DisbursedBy { get; set; }
        public string DisbursementStatus{ get; set; }
        public string Comment { get; set; }
        public virtual Loan Loan { get; set; }
    }
}
