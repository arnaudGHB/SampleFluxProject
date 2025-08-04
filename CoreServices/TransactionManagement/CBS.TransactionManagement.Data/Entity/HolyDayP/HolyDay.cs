using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.HolyDayP
{
    public class HolyDay:BaseEntity
    {
        public string Id { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string? BranchId { get; set; }
        public bool IsCentralisedConfiguration { get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsNormalOperationDay { get; set; }
    }
}
