using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific GetBudgetQuery by its unique identifier.
    /// </summary>
    public class GetTrailBalanceUploudQuery : IRequest<ServiceResponse<TrailBalanceUploudDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AccountCategory to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}