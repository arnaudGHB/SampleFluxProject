using CBS.TransactionManagement.Data.Dto.TransferLimits;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new Transaction.
    /// </summary>
    public class TransferRequestCommand : IRequest<ServiceResponse<TransferDto>>
    {
        public decimal Amount { get; set; }
        public string SenderAccountNumber { get; set; }
        public string ReceiverAccountNumber { get; set; }
        public string? Note { get; set; }
    }

}
