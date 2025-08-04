using CBS.AccountManagement.Data;

using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a customer.
    /// </summary>
    public class UpdateEntryTempDataCommand : IRequest<ServiceResponse<EntryTempDataDto>>
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }

        public string BookingDirection { get; set; }

        public decimal Amount { get; set; }
        public string AccountBalance { get; set; }
        public string Description { get; set; }

        public string Reference { get; set; }
    }
}