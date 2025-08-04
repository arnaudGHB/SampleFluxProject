using CBS.TransactionManagement.Data.Dto.ReversalRequestP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.Commands.ReversalRequestP
{
    /// <summary>
    /// Represents a command to add a new CashReplenishmentPrimaryTeller.
    /// </summary>
    public class ValidationReversalRequestCommand : IRequest<ServiceResponse<ReversalRequestDto>>
    {
        public string Status { get; set; } // "Pending", "Approved", "Rejected","Validated"
        public string ValidationComment { get; set; }
        public string Id { get; set; }
    }

}
