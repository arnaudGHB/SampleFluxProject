using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BudgetManagement.Data 
{
    public class BudgetPlanDto
    {
        public string Id { get; set; }
        public string DepartmentId { get; set; }
        public string BranchId { get; set; }
        public string FiscalYearId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public List<BudgetItemDto> BudgetItems { get; set; }
    }
}
