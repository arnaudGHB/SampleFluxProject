using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific AccountFeature by its unique identifier.
    /// </summary>
    public class ConfirmPostedEntriesCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AccountFeature to be retrieved.
        /// </summary>
        public string Id { get; set; }
        public bool HasApproved { get; set; }
        public DateTime? TransactionDate { get;  set; }
        public bool? ValidationIsNotRequired { get; set; }
        public string BranchId { get;  set; }
    }
}