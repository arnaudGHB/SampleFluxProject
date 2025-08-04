using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data
{
    public class BankZoneBranchDto
    {
        public string BankingZoneId { get; set; } // Identifier for the associated Banking Zone
        public string CorrespondingBankBranchId { get; set; } // Identifier for the corresponding bank branch
        public string BranchId { get; set; } // Identifier for the specific branch
    }

    public class BankZoneBranchOBJ
    {
        public string BankingZoneId { get; set; } // Identifier for the associated Banking Zone
        public string CorrespondingBankBranchId { get; set; } // Identifier for the corresponding bank branch
        public string BranchId { get; set; } // Identifier for the specific branch
        public string CorrespondingBankBranch { get; set; } // Identifier for the corresponding bank branch
        public string BranchName { get; set; } // Identifier for the specific branch

        public string BankingZoneName { get; set; } // Identifier for the specific branch

        public string Code { get; set; }
    }
}
