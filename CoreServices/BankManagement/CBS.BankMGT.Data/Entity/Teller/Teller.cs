using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data
{
    public class Teller:BaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double MaximumAmount { get; set; }
        public double MinimumAmount { get; set; }
        public double MaximumDepositAmount { get; set; }
        public double MinimumDepositAmount { get; set; }
        public double MaximumWithdrawalAmount { get; set; }
        public double MinimumWithdrawalAmount { get; set; }
        public string AccountID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string BranchID { get; set; }
    }
}
