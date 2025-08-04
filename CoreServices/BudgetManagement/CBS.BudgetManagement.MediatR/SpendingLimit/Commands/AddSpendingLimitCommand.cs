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
    public class AddSpendingLimitCommand : IRequest<ServiceResponse<SpendingLimitDto>>
    {
 
        public string BranchId { get; set; }
        public string DepartmentId { get; set; }
        public string FiscalYearId { get; set; }
        public decimal LimitAmount { get; set; }
    }



}
