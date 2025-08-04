using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data
{
    public class EconomicActivity : BaseEntity
    {
        
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
