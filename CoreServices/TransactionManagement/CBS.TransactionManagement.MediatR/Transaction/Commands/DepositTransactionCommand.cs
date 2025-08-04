using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new Transaction.
    /// </summary>
    public class DepositTransactionCommand : IRequest<ServiceResponse<TransactionDto>>
    {
        public decimal Amount { get; set; }
        public string AccountNumber { get; set; }
        public string DepositType { get; set; }
        public string? Note { get; set; }
        public CurrencyNotesRequest CurrencyNotes { get; set; }
        public string BranchId { get; set; }
        public string BankId { get; set; }
        public bool IsDepositDoneByAccountOwner { get; set; }
        public string? DepositerTelephone { get; set; }
        public string? DepositorIDNumberPlaceOfIssue { get; set; }
        public string? DepositerNote { get; set; }

        public string? DepositorIDNumber { get; set; }
        public string? DepositorName { get; set; }
        public string? DepositorIDIssueDate { get; set; }
        public string? DepositorIDExpiryDate { get; set; }

    }

}
