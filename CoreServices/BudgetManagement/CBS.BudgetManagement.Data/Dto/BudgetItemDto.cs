using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BudgetManagement.Data 
{
    public class BudgetItemDto
    {
        public string Id { get; set; }
        public string BudgetPlanId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string BudgetCategoryId { get; set; }
        public string BudgetCategoryName { get; set; }
    }
}
