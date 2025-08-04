using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BudgetManagement.Data 
{
    public class ExpenditureDto
    {
        public string Id { get; set; }
        public string DepartmentId { get; set; }
        public string BranchId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string BudgetItemId { get; set; }
    }
}
