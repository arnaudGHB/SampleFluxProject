using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific AccountFeature by its unique identifier.
    /// </summary>
    public class GetEntryTempDataQuery : IRequest<ServiceResponse<EntryTempDataDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AccountFeature to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}