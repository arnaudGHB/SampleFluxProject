using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.Transaction
{
    public class TransferTTP
    {
        public decimal Amount { get; set; }
        public string SenderAccountNumber { get; set; }
        public string ReceiverAccountNumber { get; set; }
        public string? Note { get; set; }
        public string AccessCode { get; set; }
        public string SourceType { get; set; }
    }
    
}
