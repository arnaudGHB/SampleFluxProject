using CBS.TransactionManagement.Data.Dto.HolyDayRecurringP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.HolyDayRecurringP.Commands
{
    /// <summary>
    /// Represents a command to update a Loan.
    /// </summary>
    public class UpdateHolyDayRecurringCommand : IRequest<ServiceResponse<HolyDayRecurringDto>>
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

    }

}
