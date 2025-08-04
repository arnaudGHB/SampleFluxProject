using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.HolyDayRecurringP
{
    public class HolyDayRecurringDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? BranchId { get; set; } // Branch-specific or global
        public bool IsGlobal { get; set; } // If true, the holiday configuration applies globally to all branches.
        public string? HolidayType { get; set; }// Type of the holiday (e.g., weekend, public holiday).
        public string? RecurrencePattern { get; set; }// Recurrence pattern (e.g., weekly, yearly).
        public string? RecurringDay { get; set; }// The day the recurring holiday happens (e.g., Monday).
        public int Month { get; set; }
        public int DayOfMonth { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }
        public BranchDto Branch { get; set; }
    }



}
