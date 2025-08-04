namespace CBS.AccountManagement.Data
{
    public class AccountingClosure:BaseEntity
    {
        public string Id { get; set; }
        public string? UserId { get; set; }
        public DateTime DateOfClosure { get; set; }
        public int CountOfTransactions { get; set; }
        // Additional properties
        public string? ClosureType { get; set; } // Example: Regular, Year-End, etc.
        public decimal TotalClosureAmount { get; set; }
        public string? Notes { get; set; }
    }
}