using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a Account.
    /// </summary>
    public class UpdateBudgetItemDetailCommand : IRequest<ServiceResponse<BudgetItemDetailDto>>
    {
        public string BudgetCategoryId { get; set; }
        public string BudgetItem { get; set; }
        public string BudgetId { get; set; }
        public string CategoryBudgetLimit { get; set; }
        public string Id { get;   set; }
    }
}