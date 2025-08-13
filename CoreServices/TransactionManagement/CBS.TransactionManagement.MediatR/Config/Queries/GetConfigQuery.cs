using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    /// <summary>
    /// Represents a query to retrieve Config by name
    /// </summary>
    public class GetConfigQuery : IRequest<ServiceResponse<ConfigDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Config to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
