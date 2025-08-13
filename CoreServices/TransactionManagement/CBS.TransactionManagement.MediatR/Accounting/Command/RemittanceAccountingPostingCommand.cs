using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Command
{


    public class RemittanceAccountingPostingCommand : IRequest<ServiceResponse<bool>>
    {
        public string TransactionReferenceId { get; set; }
        public string MemberReference { get; set; }
        public string ProductId { get; set; }
        public string OperationType { get; set; }
        public string HeadOfficeBranchCode { get; set; }
        public string HeadOfficeBranchId { get; set; }
        public string Source { get; set; }
        public List<RemittanceAmountCollectionItem> AmountCollection { get; set; }
        public DateTime TransactionDate { get; set; }
        public RemittanceAccountingPostingCommand()
        {
            AmountCollection=new List<RemittanceAmountCollectionItem>();
            HeadOfficeBranchCode="000";
            HeadOfficeBranchId="1";
        }
    }

    public class RemittanceAmountCollectionItem
    {
        public string EventAttributeName { get; set; }
        public string LevelOfExecution { get; set; }
        public decimal Amount { get; set; }
        public string AmountType { get; set; }
        public string Naration { get; set; }
    }


}







