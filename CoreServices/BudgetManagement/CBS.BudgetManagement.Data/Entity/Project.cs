using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BudgetManagement.Data
{
    public class Project: BaseEntity
    {
        [Key]
        public string  Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string ProjectName { get; set; }
        [MaxLength(255)]
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public ICollection<ProjectBudget> ProjectBudgets { get; set; }
    }
}
