using CBS.TransactionManagement.Data.Dto.OtherCashInP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.otherCashIn.Commands
{
    /// <summary>
    /// Represents a command to update a DepositLimit.
    /// </summary>
    public class UpdateOtherTransactionCommand : IRequest<ServiceResponse<OtherTransactionDto>>
    {
        public string Id { get; set; }
        public string? EnventName { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string? TransactionType { get; set; }//Income Or Expenses
        public string? SourceType { get; set; }//Cash_Collection Or Member_Account
        public string? Naration { get; set; }
    }

}
