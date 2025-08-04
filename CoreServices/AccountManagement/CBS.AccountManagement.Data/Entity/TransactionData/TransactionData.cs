namespace CBS.AccountManagement.Data
{
    public class TransactionData : BaseEntity
    {
       
        public string Id { get; set; }
       
        public string AccountNumber { get; set; }
       
        public string TransactionType { get; set; }

        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string ReferenceNumber { get; set; }
   
    }
 }