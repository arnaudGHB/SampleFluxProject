using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{

    /// <summary>
    /// Represents a command to delete a AuditTrail.
    /// </summary>
    public class DeleteAuditTrailCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AuditTrail to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
