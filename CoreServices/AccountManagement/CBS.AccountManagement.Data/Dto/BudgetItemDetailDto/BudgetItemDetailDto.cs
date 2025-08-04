
namespace CBS.AccountManagement.Data
{
    public class BudgetItemDetailDto
    {
        public string Id { get; set; }
        public string BudgetCategoryId { get; set; }
        public string BudgetItem { get; set; }
        public string BudgetId { get; set; }
        public string CategoryBudgetLimit { get; set; }
        public decimal CategoryActualSpending { get; set; } // Actual spending recorded for this category
        public decimal CategoryRemainingBudget { get; set; } // Remaining budget amount for this category
                                                             // public string ResponsiblePerson { get; set; } // Person responsible for managing the category

        public DateTime LastExpenseDate { get; set; }
        public decimal LastExpenseAmount { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
    }
}