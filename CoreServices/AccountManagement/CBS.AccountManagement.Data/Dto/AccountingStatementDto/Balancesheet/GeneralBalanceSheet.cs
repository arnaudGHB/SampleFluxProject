using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data 
{
    public class BalanceSheetDto
    {
       
        public string? Name { get; set; }
        public string? Location { get; set; }
        public string? Address { get; set; }
        public DateTime Date { get; set; }
        public double TotalAsset { get; set; }
        public double TotalLiabilityEquity { get; set; }
        public List<BalanceSheetAccount> Accounts { get; set; } = new List<BalanceSheetAccount>();
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string ImmatriculationNumber { get; set; }
        public string Capital { get; set; }
        public string BranchTelephone { get; set; }
        public string HeadOfficeTelePhone { get; set; }
        public string WebSite { get; set; }
    }
    public class BalanceSheetEntry
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string AccountNumber { get; set; }

        public string Description { get; set; }

        public decimal Balance { get; set; }

        public string Currency { get; set; }

        public string Status { get; set; } // Active or Inactive

        public string Category { get; set; }// Assets or Liabilities



    }
}
