using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific AccountCategory by its unique identifier.
    /// </summary>090491483100383
    public class GetRootAccountNumberByProductIdQuery : IRequest<ServiceResponse<AccountMap>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AccountCategory to be retrieved.
        /// </summary>090491483100383@Principal_Saving_Account
        public string ProductId { get; set; }

        public string GetProductKey () { return ProductId+ "@Principal_Saving_Account"; }
    }
}