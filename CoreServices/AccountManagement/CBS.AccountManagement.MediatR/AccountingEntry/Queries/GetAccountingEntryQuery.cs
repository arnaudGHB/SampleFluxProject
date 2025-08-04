using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific AccountingEntry by its unique identifier.
    /// </summary>
    public class GetAccountingEntryQuery : IRequest<ServiceResponse<AccountingEntryDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AccountingEntry to be retrieved.
        /// </summary>
        public string? Id { get; set; }
    }
}