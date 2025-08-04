using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.TellerP.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific PrimaryTellerProvisioningHistory by its unique identifier.
    /// </summary>
    public class GetPrimaryTellerProvisioningHistoryQuery : IRequest<ServiceResponse<PrimaryTellerProvisioningHistoryDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the PrimaryTellerProvisioningHistory to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
