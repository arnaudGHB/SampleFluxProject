using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Helper;

namespace CBS.TransactionManagement.Dto
{
    public class AccountShortDto
    {
        public string Id { get; set; }
        public string AccountNumber { get; set; }
        public string status { get; set; }
        public string ProductId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string CustomerId { get; set; }
        public string AccountName { get; set; }
    }
}