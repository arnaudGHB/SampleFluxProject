using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Command
{
    /// <summary>
    /// Represents a query to retrieve a specific Transaction by its unique identifier.
    /// </summary>
    public class AutoPostingEventCommand : IRequest<ServiceResponse<bool>>
    {
        public string Source { get; set; }//Members_Account, Teller_Account, Virtual_Teller
        public string TransactionReferenceId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string MemberReference { get; set; }
        public List<AmountEventCollection> AmountEventCollections { get; set; }
        public AutoPostingEventCommand()
        {
            AmountEventCollections=new List<AmountEventCollection>();
        }
    }
    public class AddEventMemberAccountPostingCommand : IRequest<ServiceResponse<bool>>
    {
        public string Source { get; set; }//Members_Account, Teller_Account, Virtual_Teller
        public string TransactionReferenceId { get; set; }
        public string ProductId { get; set; }
        public DateTime TransactionDate { get; set; }
        public List<AmountEventCollection> AmountEventCollections { get; set; }
    }
}


