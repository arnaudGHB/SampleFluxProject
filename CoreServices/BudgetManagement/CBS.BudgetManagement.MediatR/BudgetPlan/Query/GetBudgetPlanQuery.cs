using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.BudgetManagement.Helper;
using CBS.BudgetManagement.Data;
namespace CBS.BudgetManagement.MediatR.Queries
{
    public class GetBudgetPlanQuery : IRequest<ServiceResponse<BudgetPlanDto>>
    {
        public string Id { get; set; }
    }

  

}
