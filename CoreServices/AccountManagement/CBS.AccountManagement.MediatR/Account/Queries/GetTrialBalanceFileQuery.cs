using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific TrialBalanceFile by its unique identifier.
    /// </summary>
    public class GetTrialBalanceFileQuery : IRequest<ServiceResponse<TrialBalanceFileDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the TrialBalanceFile to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}