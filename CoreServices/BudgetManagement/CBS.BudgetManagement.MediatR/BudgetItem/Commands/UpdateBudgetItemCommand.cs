using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.BudgetManagement.Helper;
using CBS.BudgetManagement.Data;
namespace CBS.BudgetManagement.MediatR.Commands
{
    // Asset Commands 


    public class UpdateBudgetItemCommand : IRequest<ServiceResponse<BudgetItemDto>>
    {

        public string Id { get; set; }
        public string BudgetPlanId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string BudgetCategoryId { get; set; }
        public string BudgetCategoryName { get; set; }
    }

   


}
