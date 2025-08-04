using CBS.AccountManagement.Data;

using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new AddBudgetCommand.
    /// </summary>
    public class AddBudgetCommand : IRequest<ServiceResponse<BudgetDto>>
    {
      
        public string? Year { get; set; }
        public decimal TotalBudgetAmount { get; set; } // Total budget amount for the entire period
        public string Description { get; set; } // Description or purpose of the budget
        public string BudgetPeriodId { get; set; }
        public string UnitId { get; set; }
    }
}