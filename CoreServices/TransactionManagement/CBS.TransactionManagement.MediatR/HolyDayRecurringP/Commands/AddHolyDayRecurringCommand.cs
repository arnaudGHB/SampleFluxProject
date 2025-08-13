using CBS.TransactionManagement.Data.Dto.HolyDayRecurringP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.RecurringRecurringP.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddHolyDayRecurringCommand : IRequest<ServiceResponse<HolyDayRecurringDto>>
    {
        public string? BranchId { get; set; } // Branch-specific or global
        public string Name { get; set; }
        public bool IsGlobal { get; set; } // If true, the holiday configuration applies globally to all branches.
        public string? HolidayType { get; set; }// Type of the holiday (e.g., weekend, public holiday).
        public string? RecurrencePattern { get; set; }// Recurrence pattern (e.g., weekly, yearly).
        public string? RecurringDay { get; set; }// The day the recurring holiday happens (e.g., Monday).
        public int Month { get; set; }
        public int DayOfMonth { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }

    }

}
