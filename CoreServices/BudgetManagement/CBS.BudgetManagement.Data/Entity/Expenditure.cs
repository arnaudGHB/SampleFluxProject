using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BudgetManagement.Data
{
    public class Expenditure: BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string DepartmentID { get; set; }
        public string BranchID { get; set; }
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        [Required]
        [MaxLength(255)]
        public string Description { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public string? BudgetItemID { get; set; }

        [ForeignKey("BudgetItemID")]
        public BudgetItem BudgetItem { get; set; }
    }

}
