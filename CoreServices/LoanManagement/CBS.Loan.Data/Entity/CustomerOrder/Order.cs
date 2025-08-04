using CBS.NLoan.Data.Entity.LoanApplicationP;

namespace CBS.NLoan.Data.Entity.CustomerOrder
{
    public class Order : BaseEntity
    {
        public int Id { get; set; }
        public string CustomerID { get; set; }
        public string ProductName { get; set; }
        public virtual LoanApplication Loan { get; set; }
    }
}
