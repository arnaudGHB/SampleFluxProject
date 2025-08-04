using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Command
{

   

    public class AddDisbursmementRefinancingCommand : IRequest<ServiceResponse<bool>>
    {
        public string TransactionReferenceId { get; set; }
        public string MemberReference { get; set; }
        public string AccountHolder { get; set; }
        public string SavingProductId { get; set; }
        public string LoanProductId { get; set; }
        public string SavingProductName { get; set; }
        public string Naration { get; set; }
        public decimal AmountToDisbursed { get; set; }
        public decimal AmountToPayBackToCash { get; set; }
        public string DisbursementType { get; set; }
        public DateTime TransactionDate { get; set; }
        public bool IsCommissionFromMember { get; set; }
        public List<DisbursementCollection> DisbursementCollections { get; set; }
        public AddDisbursmementRefinancingCommand()
        {
            DisbursementCollections=new List<DisbursementCollection>();
        }
    }
}
