using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Group by its unique identifier.
    /// </summary>
    public class GetGroupQuery : IRequest<ServiceResponse<DATA.Entity.Group>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Group to be retrieved.
        /// </summary>
        public string GroupId { get; set; }
    }
}
