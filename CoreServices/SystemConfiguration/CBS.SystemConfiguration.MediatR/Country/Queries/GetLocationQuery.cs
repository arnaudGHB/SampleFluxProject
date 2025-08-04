using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.Helper;

using MediatR;

namespace CBS.SystemConfiguration.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific AccountCategory by its unique identifier.
    /// </summary>
    public class GetLocationQuery : IRequest<ServiceResponse<LocationDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AccountCategory to be retrieved.
        /// </summary>
        public string Code { get; set; }
    }
}