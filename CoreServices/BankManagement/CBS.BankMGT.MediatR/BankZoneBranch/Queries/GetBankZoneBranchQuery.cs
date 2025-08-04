using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific BankZoneBranch by its unique identifier.
    /// </summary>
    public class GetBankZoneBranchQuery : IRequest<ServiceResponse<BankZoneBranchDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the BankZoneBranch to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
