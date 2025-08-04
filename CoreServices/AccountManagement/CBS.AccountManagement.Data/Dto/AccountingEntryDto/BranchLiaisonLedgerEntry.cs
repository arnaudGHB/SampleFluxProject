using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data 
{    public class BranchLiaisonLedgerEntry
    {
        public string BranchId { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public string AccountId { get; set; }

        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public decimal TotalDrAmount { get; set; }
        public decimal TotalCrAmount { get; set; }
        public decimal Balance { get; set; }
    }
}
