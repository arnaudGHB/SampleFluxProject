using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class ChartOfAccountManagementPosition : BaseEntity
    {
        public string Id { get; set; }
        public string PositionNumber { get; set; }
        public string Description { get; set; }
        public string RootDescription { get; set; }
        public string ChartOfAccountId { get; set; }
        public string AccountNumber { get; set; }
        public string? Old_AccountNumber { get; set; }
        public string? New_AccountNumber { get; set; }
        public bool? IsUniqueAccount { get; set; }
        public virtual ChartOfAccount ChartOfAccount  { get; set; }
    }
}
