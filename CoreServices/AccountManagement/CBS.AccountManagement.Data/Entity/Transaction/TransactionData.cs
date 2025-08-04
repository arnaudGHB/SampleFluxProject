namespace CBS.AccountManagement.Data
{
    public class Transaction : BaseEntity
    {
        public string Id { get; set; }
        public string? BudgetCategoryId { get; set; }
        public string? ChartOfAccountId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } // income or expense
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public string ReferenceNumber { get; set; } // Reference number or identifier for the transaction
        public string PaymentMethod { get; set; } // Payment method used for the transaction (e.g%%., cash, credit card, bank transfer)
        public string Location { get; set; } // Location or place where the transaction occurred
        public virtual BudgetCategory? BudgetCategory { get; set; }
        public virtual ChartOfAccount?  Account { get; set; }

    }
 }