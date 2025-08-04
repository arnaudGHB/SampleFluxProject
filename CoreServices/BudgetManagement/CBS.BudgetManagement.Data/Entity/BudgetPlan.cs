using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BudgetManagement.Data
{
    public class BudgetPlan: BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string DepartmentId { get; set; }
        public string BranchId { get; set; }
        public string FiscalYearId { get; set; }
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }
        [Required]
        [MaxLength(20)]
        public string Status { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public ICollection<BudgetItem> BudgetItems { get; set; }
        public ICollection<BudgetAdjustment> BudgetAdjustments { get; set; }
    }
}
