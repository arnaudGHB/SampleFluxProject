using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.DailyCollectionManagement.Data.Dto
{
    public class AgentDto
    {
        public string Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string CNI { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
        public string BranchId { get; set; }
    }
}
