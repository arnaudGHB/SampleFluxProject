using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.TellerP.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Teller by its unique identifier.
    /// </summary>
    public class GetTellerAccountByUserIDQuery : IRequest<ServiceResponse<DailyTellerDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Teller to be retrieved.
        /// </summary>
        public string UserID { get; set; }
    }
}
