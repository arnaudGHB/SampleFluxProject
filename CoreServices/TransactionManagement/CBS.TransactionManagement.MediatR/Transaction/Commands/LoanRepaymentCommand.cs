using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new Transaction.
    /// </summary>
    public class LoanRepaymentCommand : IRequest<ServiceResponse<TransactionDto>>
    {
        public decimal Amount { get; set; }
        public decimal Principal { get; set; }
        public decimal Interest { get; set; }
        public decimal Tax { get; set; }
        public decimal Penalty { get; set; }
        public string LoanId { get; set; }
        public string AccountNumber { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentChannel { get; set; }
        public string DepositType { get; set; }
        public string? Note { get; set; }
        public CurrencyNotesRequest CurrencyNotes { get; set; }
        public bool IsDepositDoneByAccountOwner { get; set; }
        public string? DepositerTelephone { get; set; }
        public string? DepositorIDNumberPlaceOfIssue { get; set; }
        public string? DepositerNote { get; set; }
        public string? DepositorIDNumber { get; set; }
        public string? DepositorName { get; set; }
        public string? DepositorIDIssueDate { get; set; }
        public string? DepositorIDExpiryDate { get; set; }
        public bool IsLomSum { get; set; }

    }

}
