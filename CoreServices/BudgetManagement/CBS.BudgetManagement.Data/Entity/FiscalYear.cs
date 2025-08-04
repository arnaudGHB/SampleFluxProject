using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BudgetManagement.Data
{
    public class FiscalYear: BaseEntity
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }

        public ICollection<BudgetPlan> BudgetPlans { get; set; }
        public ICollection<SpendingLimit> SpendingLimits { get; set; }
        public ICollection<ProjectBudget> ProjectBudgets { get; set; }
    }
}
