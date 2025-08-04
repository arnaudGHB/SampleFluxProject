using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Command
{
    /// <summary>
    /// Represents a query to retrieve a specific Transaction by its unique identifier.
    /// </summary>
    public class AddAccountingPostingCommand : IRequest<ServiceResponse<bool>>
    {
        public string TransactionReferenceId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountHolder { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string OperationType { get; set; }
        public string MemberReference { get; set; }
        public string Naration { get; set; }
        public string ExternalBranchCode { get; set; }
        public string ExternalBranchId { get; set; }
        public bool IsInterBranchTransaction { get; set; }
        public string Source { get; set; }
        public List<AmountCollection> AmountCollection { get; set; }
        public List<AmountEventCollection> AmountEventCollections { get; set; }
        public DateTime TransactionDate { get; set; }
        public AddAccountingPostingCommand()
        {
            Source = TellerSources.Physical_Teller.ToString();
        }
    }

   


}







