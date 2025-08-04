using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class BranchEvenEntry 
    {
        public string Id { get; set; }
        public string SystemId { get; set; }
        public List<string> BranchId { get; set; }
    }
}
