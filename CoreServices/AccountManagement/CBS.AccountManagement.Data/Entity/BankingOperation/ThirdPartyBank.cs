using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class ThirdPartyBank : BaseEntity
    {
        public  string Id { get; set; }
        public string Name { get; set; }
 
        public string BankBranchCode { get; set; }
        public string BankCode { get; set; }
    }
}
