using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Command
{

    public class AddDisbursmementPostingCommand : IRequest<ServiceResponse<bool>>
    {
        public string TransactionReferenceId { get; set; }
        public string MemberReference { get; set; }
        public string AccountHolder { get; set; }
        public string SavingProductId { get; set; }
        public string LoanProductId { get; set; }
        public string SavingProductName { get; set; }
        public string Naration { get; set; }
       
        public string TellerSourceForLoanCommission { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public bool IsCommissionFromMember { get; set; }
        public List<DisbursementCollection> DisbursementCollections { get; set; }
        public AddDisbursmementPostingCommand()
        {
            DisbursementCollections = new List<DisbursementCollection>();
        }
    }

    public class DisbursementCollection
    {
        public string EventCode { get; set; }
        public decimal Amount { get; set; }
        public string Naration { get; set; }
    }

    
}
