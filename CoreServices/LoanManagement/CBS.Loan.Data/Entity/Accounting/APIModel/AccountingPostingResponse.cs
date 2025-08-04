using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Data
{
    public class AccountingPostingResponse
    {
        public string id { get; set; }
        public DateTime entryDate { get; set; }
        public DateTime valueDate { get; set; }
        public string entryType { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; }
        public string description { get; set; }
        public string referenceID { get; set; }
        public string status { get; set; }
        public string reviewedBy { get; set; }
        public string source { get; set; }
        public string eventCode { get; set; }
        public string bankId { get; set; }
        public string branchId { get; set; }
        public string operationType { get; set; }
        public string accountId { get; set; }
        public string accountNumber { get; set; }
    }

}
