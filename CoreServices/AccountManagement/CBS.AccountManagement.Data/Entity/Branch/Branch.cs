using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data 
{
    public class Branch
    {
        public string BankId { get; set; }
        public string BranchId { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
