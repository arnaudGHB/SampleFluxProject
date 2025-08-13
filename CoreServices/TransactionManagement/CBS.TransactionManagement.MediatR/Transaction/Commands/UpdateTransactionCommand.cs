using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to update a Transaction.
    /// </summary>
    public class UpdateTransactionCommand : IRequest<ServiceResponse<TransactionDto>>
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string AccountId { get; set; }
        public string TransactionRef { get; set; }
        public string TransactionType { get; set; }
        public string SourceDetails { get; set; }
    }

}
