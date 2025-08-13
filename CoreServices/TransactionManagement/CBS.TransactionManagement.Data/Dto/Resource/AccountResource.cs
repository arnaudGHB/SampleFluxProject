using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.Resource
{
    public class AccountResource : ResourceParameter
    {
        public AccountResource() : base("CustomerId")
        {
        }

        public string? BranchId { get; set; }
        public bool IsByBranch { get; set; }

    }
   
}
