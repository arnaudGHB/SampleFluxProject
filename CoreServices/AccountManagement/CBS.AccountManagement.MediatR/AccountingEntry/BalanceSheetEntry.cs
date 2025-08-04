namespace CBS.AccountManagement.MediatR
{
    public class BalanceSheetEntry
    {
        public int Id { get; set; }

        public string Name { get; set; } 

        public string AccountNumber { get; set; } 

        public string Description { get; set; }

        public decimal Balance { get; set; }

        public string Currency { get; set; }

        public string Status { get; set; } // Active or Inactive

        public string Category { get; set; }// Assets or Liabilities



    }
}