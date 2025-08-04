using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using MediatR;
using Microsoft.Extensions.ObjectPool;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new Account.
    /// </summary>
    public class AddBudgetItemDetailCommand : IRequest<ServiceResponse<bool>>
    {

        public string BudgetCategoryId { get; set; }
        public string BudgetItem { get; set; }
        public string BudgetId { get; set; }
        public string CategoryBudgetLimit { get; set; }

 

    }
 
}