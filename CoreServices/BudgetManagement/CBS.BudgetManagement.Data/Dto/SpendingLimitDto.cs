using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BudgetManagement.Data 
{
    public class SpendingLimitDto
    {
        public string Id { get; set; }
        public string BranchId { get; set; }
        public string DepartmentId { get; set; }
        public string FiscalYearId { get; set; }
        public decimal LimitAmount { get; set; }
    }
}
