using CBS.TransactionManagement.Data.Dto.ReversalRequestP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Queries.ReversalRequestP
{
    /// <summary>
    /// Represents a query to retrieve a specific CashReplenishment by its unique identifier.
    /// </summary>
    public class GetReversalRequestQuery : IRequest<ServiceResponse<ReversalRequestDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the CashReplenishment to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
