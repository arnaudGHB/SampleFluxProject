using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Command
{
   
    public class MomocashCollectionLoanRepaymentCommand : IRequest<ServiceResponse<bool>>
    {
        public string TransactionReferenceId { get; set; }
        public string AccountNumber { get; set; }
        public bool IsOldSystemLoan { get; set; }
        public string LoanProductId { get; set; }
        public string BranchId { get; set; }
        public string Naration { get; set; }
        public string TellerSource { get; set; }
        public string MomoOperatorType { get; set; }
        public List<AmountCollection> AmountCollection { get; set; }
        public LoanRefundCollectionAlpha LoanRefundCollectionAlpha { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public MomocashCollectionLoanRepaymentCommand()
        {
            AmountCollection = new List<AmountCollection>();
            LoanRefundCollectionAlpha = new LoanRefundCollectionAlpha();
        }
    }

}







