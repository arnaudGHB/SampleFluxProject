using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.LoanRepayment
{
    public class LoanRepaymentOrderDto
    {
        public int InterestOrder { get; set; }
        public int CapitalOrder { get; set; }
        public int FineOrder { get; set; }
        public decimal InterestRate { get; set; }
        public decimal CapitalRate { get; set; }
        public decimal FineRate { get; set; }
        public string? LoanProductRepaymentOrderType { get; set; }
    }
}
