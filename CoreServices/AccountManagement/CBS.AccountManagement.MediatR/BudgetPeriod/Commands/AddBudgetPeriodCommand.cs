using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new customer.
    /// </summary>
    public class AddBudgetPeriodCommand : IRequest<ServiceResponse<List<BudgetPeriodDto>>>
    {

        public string Year { get;   set ; }  
    
        
    }
}