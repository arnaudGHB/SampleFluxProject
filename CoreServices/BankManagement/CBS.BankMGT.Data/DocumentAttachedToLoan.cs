using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data
{
    public class DocumentAttachedToLoan
    {
        public string Id { get; set; }
        public string LoanApplicationID { get; set; }
        public string FilePath { get; set; }
    }
    public class DocumentAttachedToLoanCommand
    {
        public string LoanApplicationID { get; set; }
        //public IFormFileCollection AttachedFiles { get; set; }
    }
    public class DocumentAttachedToLoanDto
    {
        public string Id { get; set; }
        public string LoanApplicationID { get; set; }
        public string FilePath { get; set; }
        
    }
}
