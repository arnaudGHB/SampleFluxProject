using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data 
{
    

    public class EventCatalog : BaseEntity
    {

        public int EventID { get; set; }
        public string EventName { get; set; }
        public string EventType { get; set; }

        public virtual ICollection<AccountingRuleX> AccountingRules { get; set; }

    }
}
