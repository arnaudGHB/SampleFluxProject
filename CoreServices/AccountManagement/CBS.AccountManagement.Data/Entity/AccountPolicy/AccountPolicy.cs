using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class AccountPolicy : BaseEntity
    {
        public string AccountId { get; set; }
        public decimal MaximumAlert { get; set; }
        public decimal MinimumAlert { get; set; }
        public string Name { get; set; }
        public string MinMessage { get; set; }
        public string PolicyOwner { get; set; }
        public string MaxMessage { get; set; }

        public string Id { get; set; }
        public virtual ChartOfAccountManagementPosition Account { get; set; }
    }
}
