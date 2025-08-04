using CBS.NLoan.Data.Data;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.Command
{
    /// <summary>
    /// Represents a query to retrieve a specific Transaction by its unique identifier.
    /// </summary>
    public class AddAccountingPostingCommand : IRequest<ServiceResponse<bool>>
    {
        public string TransactionReferenceId { get; set; }
        public string AccountNumber { get; set; }
        public string? MemberReference { get; set; }
        public string LoanProductId { get; set; }
        public string LoanProductName { get; set; }
        public string Naration { get; set; }
        public string BranchId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}



