using CBS.TransactionManagement.Data.Dto.TransferLimits;
using CBS.TransactionManagement.Data.Dto.VaultP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.VaultP.Queries
{
    /// <summary>
    /// Query to retrieve a vault by BranchId.
    /// </summary>
    public class GetVaultByBranchIdQuery : IRequest<ServiceResponse<VaultDto>>
    {
        /// <summary>
        /// Gets or sets the BranchId for the vault to retrieve.
        /// </summary>
        public string BranchId { get; set; }
    }
}
