using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BudgetManagement.Data
{
    public class BudgetAdjustment: BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string BudgetPlanID { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestedDate { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        [Required]
        [MaxLength(255)]
        public string Reason { get; set; }
        [Required]
        [MaxLength(20)]
        public string Status { get; set; }
        public virtual BudgetPlan BudgetPlan { get; set; }

    }

}
