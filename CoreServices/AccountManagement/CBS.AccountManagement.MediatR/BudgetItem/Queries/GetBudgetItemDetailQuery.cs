using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific BudgetItemDetailCategory by its unique identifier.
    /// </summary>
    public class GetBudgetItemDetailQuery : IRequest<ServiceResponse<BudgetItemDetailDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the BudgetItemDetailCategory to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}