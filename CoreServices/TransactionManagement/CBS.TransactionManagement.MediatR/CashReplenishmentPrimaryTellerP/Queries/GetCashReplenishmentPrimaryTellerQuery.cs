using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific CashReplenishment by its unique identifier.
    /// </summary>
    public class GetCashReplenishmentPrimaryTellerQuery : IRequest<ServiceResponse<CashReplenishmentPrimaryTellerDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the CashReplenishment to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
