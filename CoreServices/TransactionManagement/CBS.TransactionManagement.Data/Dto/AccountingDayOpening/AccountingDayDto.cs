using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.AccountingDayOpening
{
   
    public class AccountingDayDto
    {
        public string Id { get; set; }
        public string? BranchId { get; set; } // Null for centralized/global day
        public DateTime Date { get; set; }
        public bool IsClosed { get; set; }
        public string? ClosedBy { get; set; }
        public string? OpenedBy { get; set; }
        public DateTime? ClosedAt { get; set; }
        public DateTime? OpenedAt { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public bool IsCentralized { get; set; } // True if this is a centralized/global day
        public string? Note { get; set; } = "Opening of Accounting Day.";
        public DateTime? ReOpenedDate { get; set; } = DateTime.MinValue;

    }



}
