using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BudgetManagement.Data
{

    public class ProjectBudget: BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string ProjectId { get; set; }
        public string FiscalYearId { get; set; }
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BudgetAmount { get; set; }

        [ForeignKey("ProjectID")]
        public Project Project { get; set; }
        [ForeignKey("FiscalYearID")]
        public FiscalYear FiscalYear { get; set; }
    }
}
