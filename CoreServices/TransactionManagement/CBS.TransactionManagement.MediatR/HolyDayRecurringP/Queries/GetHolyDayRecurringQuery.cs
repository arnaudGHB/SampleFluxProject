using CBS.TransactionManagement.Data.Dto.HolyDayRecurringP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.HolyDayRecurringP.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetHolyDayRecurringQuery : IRequest<ServiceResponse<HolyDayRecurringDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the HolyDayRecurring to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
