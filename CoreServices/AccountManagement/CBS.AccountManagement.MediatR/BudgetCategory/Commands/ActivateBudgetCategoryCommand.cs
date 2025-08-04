using CBS.AccountManagement.Data;

using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new BlockBudgetCategoryCommand.
    /// </summary>
    public class ActivateBudgetCategoryCommand : IRequest<ServiceResponse<BudgetCategoryDto>>
    {

 
        public string Id { get; set; }
       
    }
}