using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific AccountType by its unique identifier.
    /// </summary>
    public class GetAccountTypeQuery : IRequest<ServiceResponse<AccountTypeDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AccountType to be retrieved.
        /// </summary>
        public string Id { get; set; }

        public string IdType { get; set; }
    }
}