using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.OtherCashInP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.TransactionManagement.otherCashIn.Commands
{
    /// <summary>
    /// Represents a command to add a new DepositLimit.
    /// </summary>
    public class AddOtherTransactionCommand : IRequest<ServiceResponse<OtherTransactionDto>>
    {
        public string? EnventName { get; set; }
        public decimal Amount { get; set; }
        public string EventCode { get; set; }
        public string Name { get; set; }
        public string Naration { get; set; }
        public string? Direction { get; set; }
        public string? TransactionType { get; set; }//Income Or Expenses
        public string? SourceType { get; set; }//Cash_Collection Or Member_Account
        [Required(ErrorMessage = "Member's reference number is required")]
        public string CustomerId { get; set; }
        public string? AccountNumber { get; set; }
        public string? CNI { get; set; }
        public string? TelephoneNumber { get; set; }
        public CurrencyNotesRequest CurrencyNotesRequest { get; set; }
    }

}
