using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific ChartOfAccount by its unique identifier.
    /// </summary>
    public class GetChartOfAccountQuery : IRequest<ServiceResponse<ChartOfAccountDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the ChartOfAccount to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}