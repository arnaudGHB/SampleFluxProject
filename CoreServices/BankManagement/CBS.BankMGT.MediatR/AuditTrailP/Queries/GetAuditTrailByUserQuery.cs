using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific AuditTrail by its unique identifier.
    /// </summary>
    public class GetAuditTrailByUserQuery : IRequest<ServiceResponse<List<AuditTrailDto>>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AuditTrail to be retrieved.
        /// </summary>
        public string UserName { get; set; }
    }
}
