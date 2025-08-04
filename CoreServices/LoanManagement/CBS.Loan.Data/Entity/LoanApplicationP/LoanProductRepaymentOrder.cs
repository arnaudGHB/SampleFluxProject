using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Entity.LoanApplicationP
{
    public class LoanProductRepaymentOrder:BaseEntity
    {
        public string Id { get; set; }
        public int InterestOrder { get; set; }
        public int CapitalOrder { get; set; }
        public int FineOrder { get; set; }
        public decimal InterestRate { get; set; }
        public decimal CapitalRate { get; set; }
        public decimal FineRate { get; set; }
        public string? LoanProductRepaymentOrderType { get; set; }//SalaryRefundOrder Or NormalRefund
       // public string LoanDeliquencyPeriod { get; set; }
        public string RepaymentTypeName { get; set; }
    /*    public string LoanProductId { get; set; }
        public virtual LoanProduct LoanProduct { get; set; }*/

    }
}
