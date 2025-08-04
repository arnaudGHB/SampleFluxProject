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


    public class UpdateBudgetCategoryCommand : IRequest<ServiceResponse<BudgetCategoryDto>>
    {

        public string Id { get; set; }
        public string Name { get; set; }
    }

   


}
