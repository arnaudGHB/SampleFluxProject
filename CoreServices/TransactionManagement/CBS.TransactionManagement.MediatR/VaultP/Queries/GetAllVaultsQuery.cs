using CBS.TransactionManagement.Data.Dto.TransferLimits;
using CBS.TransactionManagement.Data.Dto.VaultP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.VaultP.Queries
{
    /// <summary>
    /// Query to retrieve all vaults.
    /// </summary>
    public class GetAllVaultsQuery : IRequest<ServiceResponse<List<VaultDto>>>
    {
    }
}
