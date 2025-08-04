using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BudgetManagement.Data
{
    public class BudgetItem: BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string BudgetPlanID { get; set; }
        [Required]
        [MaxLength(255)]
        public string Description { get; set; }
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        public string BudgetCategoryId { get; set; }

        [ForeignKey("BudgetPlanID")]
        public BudgetPlan BudgetPlan { get; set; }
        public BudgetCategory BudgetCategory { get; set; }
        public ICollection<Expenditure> Expenditures { get; set; }
    }
}
