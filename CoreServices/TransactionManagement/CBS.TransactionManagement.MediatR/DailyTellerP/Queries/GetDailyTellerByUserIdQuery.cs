using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.DailyTellerP.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific TempAccount by its unique identifier.
    /// </summary>
    public class GetDailyTellerByUserIdQuery : IRequest<ServiceResponse<DailyTellerDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the TempAccount to be retrieved.
        /// </summary>
        public string UserId { get; set; }
        public string TellerType { get; set; }
    }
}
