using CBS.AccountManagement.Data;

using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new AddBudgetCategoryCommand.
    /// </summary>
    public class AddBudgetCategoryCommand : IRequest<ServiceResponse<BudgetCategoryDto>>
    {


        public string BudgetId { get; set; }
        public string? CategoryName { get; set; }
        public decimal BudgetLimit { get; set; }
        public string? TimePeriodId { get; set; } // e.g.yearly ,semesterly, Termly, Quarterly ,monthly, weekly

        public string Description { get; set; } // Description or purpose of the category
        public string UnitId { get; set; } // Owner of the category (e.g., branch, zone,head office)
        public string ResponsiblePerson { get; set; } // Person responsible for managing the category


    }
}