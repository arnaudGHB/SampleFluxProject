using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Resources
{
    public class CustomerResource : ResourceParameter
    {
        public CustomerResource() : base("CustomerId")
        {
        }

        public string BranchId { get; set; }
        public bool IsByBranch { get; set; }

    }
    public class GroupResource : GroupResourceParameter
    {
        public string BranchId { get; set; }
        public bool IsByBranch { get; set; }

    }
}
