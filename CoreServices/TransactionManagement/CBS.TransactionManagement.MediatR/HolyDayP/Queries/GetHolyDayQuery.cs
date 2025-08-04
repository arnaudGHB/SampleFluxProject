using CBS.TransactionManagement.Data.Dto.HolyDayP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.HolyDayP.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetHolyDayQuery : IRequest<ServiceResponse<HolyDayDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the HolyDay to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
