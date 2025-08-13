using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.HolyDayRecurringP.Commands
{

    /// <summary>
    /// Represents a command to delete a customer.
    /// </summary>
    public class DeleteHolyDayRecurringCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the customer to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
