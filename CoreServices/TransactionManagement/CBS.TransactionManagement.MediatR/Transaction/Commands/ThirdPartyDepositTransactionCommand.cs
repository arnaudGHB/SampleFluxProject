using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new Transaction.
    /// </summary>
    public class ThirdPartyDepositTransactionCommand : IRequest<ServiceResponse<TransactionDto>>
    {
        public decimal Amount { get; set; }
        public string AccountNumber { get; set; }
        public string DepositType { get; set; }
        public string Note { get; set; }
        public CurrencyNotesRequest CurrencyNotes { get; set; }
        public string? DepositorIDNumber { get; set; }
        public string? DepositorName { get; set; }
        public DateTime? DepositorIDIssueDate { get; set; }
        public DateTime? DepositorIDExpiryDate { get; set; }

    }

}
