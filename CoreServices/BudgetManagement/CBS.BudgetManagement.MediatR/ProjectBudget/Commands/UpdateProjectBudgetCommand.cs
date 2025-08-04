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

    public class UpdateProjectBudgetCommand : IRequest<ServiceResponse<ProjectBudgetDto>>
    {
        public string Id { get; set; }
        public string ProjectId { get; set; }
        public string FiscalYearId { get; set; }
        public decimal BudgetAmount { get; set; }
    }

   


}
