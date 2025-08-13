using CBS.TransactionManagement.Data.Dto.TransferLimits;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new Transaction.
    /// </summary>
    public class TransferConfirmationCommand : IRequest<ServiceResponse<TransactionDto>>
    {
        public string TransferId { get; set; }
        public string Status { get; set; }
        public string? Note { get; set; }
    }

}
