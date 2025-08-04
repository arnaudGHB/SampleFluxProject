using CBS.AccountManagement.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data 
{
    public class BudgetCategoryDto
    {
        public string Id { get; set; }

        public string? CategoryName { get; set; }


        public string Description { get; set; } // Description or purpose of the category



        public string ChartOfAccountId { get; set; } // Owner of the category (e.g., branch, zone,head office)



        public ICollection<Transaction> Transactions { get; set; }

        public virtual CashMovementTracker Account { get; set; }

    }
}
