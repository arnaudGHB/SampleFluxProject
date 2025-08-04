using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific AccountCategory by its unique identifier.
    /// </summary>
    public class GetAccountByEventCodeQuery : IRequest<ServiceResponse<List<InfoAccount>>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AccountCategory to be retrieved.
        /// </summary>
        public string EventCode { get; set; }
        public string ToBranchCode { get;   set; }
        public string ToBranchId { get;   set; }
    }
}