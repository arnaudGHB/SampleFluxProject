using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.TellerP.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific PrimaryTellerProvisioningHistory by its unique identifier.
    /// </summary>
    public class GetPrimaryTellerProvisioningHistoryByBranchIDQuery : IRequest<ServiceResponse<List<PrimaryTellerProvisioningHistoryDto>>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the PrimaryTellerProvisioningHistory to be retrieved.
        /// </summary>
        public string BranchId { get; set; }
    }
}
