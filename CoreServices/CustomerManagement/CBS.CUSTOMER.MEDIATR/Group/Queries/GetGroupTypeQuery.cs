using CBS.CUSTOMER.DATA.Entity.Groups;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Group by its unique identifier.
    /// </summary>
    public class GetGroupTypeQuery : IRequest<ServiceResponse<DATA.Entity.GroupType>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the GroupType to be retrieved.
        /// </summary>
        public string GroupTypeId { get; set; }
    }
}
