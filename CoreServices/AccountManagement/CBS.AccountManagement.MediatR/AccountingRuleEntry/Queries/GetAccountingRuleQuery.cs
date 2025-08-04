using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific AccountCategory by its unique identifier.
    /// </summary>
    public class GetAccountingRuleEntryQuery : IRequest<ServiceResponse<AccountingRuleEntryDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AccountCategory to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }

    /// <summary>
    /// Represents a query to retrieve a specific AccountCategory by its unique identifier.
    /// </summary>
    public class GetAccountingRuleEntryByEventCodeQuery : IRequest<ServiceResponse<AccountingRuleEntryDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AccountCategory to be retrieved.
        /// </summary>
        public string EventCode { get; set; }
    }
}