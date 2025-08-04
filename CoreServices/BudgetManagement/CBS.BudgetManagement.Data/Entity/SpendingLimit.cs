using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BudgetManagement.Data
{
    public class SpendingLimit: BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string DepartmentId { get; set; }
        public string BranchId { get; set; }
        public string FiscalYearId { get; set; }
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal LimitAmount { get; set; }

        [ForeignKey("FiscalYearId")]
        public FiscalYear FiscalYear { get; set; }
    }
}
