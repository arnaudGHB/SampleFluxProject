using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a UpdateBudgetCategoryCommand.
    /// </summary>
    public class UpdateBudgetCategoryCommand : IRequest<ServiceResponse<BudgetCategoryDto>>
    {


        public string Id { get; set; }
        public string BudgetId { get; set; }
        public string? CategoryName { get; set; }
        public decimal BudgetLimit { get; set; }
        public string? TimePeriod { get; set; }

    }
}