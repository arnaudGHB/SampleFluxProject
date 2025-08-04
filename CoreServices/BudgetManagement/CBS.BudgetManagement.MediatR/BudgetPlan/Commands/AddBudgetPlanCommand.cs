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
    public class AddBudgetPlanCommand : IRequest<ServiceResponse<BudgetPlanDto>>
    {

 
        public string DepartmentId { get; set; }
        public string BranchId { get; set; }
        public string FiscalYearId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }



}
