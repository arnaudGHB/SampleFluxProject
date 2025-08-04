using CBS.TransactionManagement.Data.Dto.TransferLimits;
using CBS.TransactionManagement.Data.Dto.VaultP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.VaultP.Queries
{
    /// <summary>
    /// Query to retrieve a vault by its unique Id.
    /// </summary>
    public class GetVaultByIdQuery : IRequest<ServiceResponse<VaultDto>>
    {
        /// <summary>
        /// Gets or sets the Id of the vault to retrieve.
        /// </summary>
        public string Id { get; set; }
    }

}
