using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data 
{
    public class EntryTempDataDto
    {
        public string Id { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; } 
        public string BookingDirection { get; set; }
        public string? Status { get; set; } = "UnderReview";
        public decimal Amount { get; set; }
        public string AccountBalance { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public string ExternalBranchId { get; set; }
        public bool IsInterBranchTransaction { get; set; }
    }
}
