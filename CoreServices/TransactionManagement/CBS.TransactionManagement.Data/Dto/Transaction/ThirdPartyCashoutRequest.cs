using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.Transaction
{
    public class ThirdPartyCashoutRequest
    {
        public string AccountNumber { get; set; }
        public string ExternalReference { get; set; }
        public decimal Amount { get; set; }
        public string ApplicationCode { get; set; }
        public string Note { get; set; }
    }
  
}
