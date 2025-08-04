using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BudgetManagement.Data
{
    public class BudgetCategory: BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string BudgetItem { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public ICollection<BudgetItem> BudgetItems { get; set; }
    }
}
