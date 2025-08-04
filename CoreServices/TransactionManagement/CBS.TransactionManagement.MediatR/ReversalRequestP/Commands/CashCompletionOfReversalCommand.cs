using CBS.TransactionManagement.Data.Dto.ReversalRequestP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.Commands.ReversalRequestP
{
    /// <summary>
    /// Represents a command to add a new CashReplenishment.
    /// </summary>
    public class CashCompletionOfReversalCommand : IRequest<ServiceResponse<bool>>
    {
        public string Id { get; set; }
    }

}
