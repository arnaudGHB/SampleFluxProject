using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class TrialBalanceFile : BaseEntity
    {
        public string Id { get; set; }
        public string Owner { get; set; }
        public string FilePath { get; set; }
        public string Size { get; set; }

  
    }
}
