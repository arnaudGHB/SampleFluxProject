using CBS.NLoan.Data.Entity.LoanApplicationP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Dto.LoanApplicationP
{
    public class LoanProductRepaymentOrderDto
    {
        public string Id { get; set; }
        public int RepaymentOrder { get; set; }
        public string RepaymentReceive { get; set; }
        public int InterestOrder { get; set; }
        public int CapitalOrder { get; set; }
        public int FineOrder { get; set; }
        public decimal InterestRate { get; set; }
        public decimal CapitalRate { get; set; }
        public decimal FineRate { get; set; }
        /*public string LoanProductId { get; set; }
        public LoanProduct LoanProduct { get; set; }*/

    }
}
