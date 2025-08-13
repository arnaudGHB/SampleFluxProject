using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.CashCeilingMovement.Commands
{
    /// <summary>
    /// Represents a command to add a new CashReplenishmentPrimaryTeller.
    /// </summary>
    public class ValidationCashCeilingRequestCommand : IRequest<ServiceResponse<bool>>
    {
        public string Id { get; set; }
        public string ApprovedComment { get; set; }
        public string ApprovedStatus { get; set; }
        public CurrencyNotesRequest CurrencyNote { get; set; }
    }

}
