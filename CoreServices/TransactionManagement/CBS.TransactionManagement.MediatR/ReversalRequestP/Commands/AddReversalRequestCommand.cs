using CBS.TransactionManagement.Data.Dto.ReversalRequestP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.Commands.ReversalRequestP
{
    /// <summary>
    /// Represents a command to add a new CashReplenishment.
    /// </summary>
    public class AddReversalRequestCommand : IRequest<ServiceResponse<ReversalRequestDto>>
    {
        public string TransactionId { get; set; }
        public string Reason { get; set; }
        public string IncidentNote { get; set; }
    }

}
