using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Branch by its unique identifier.
    /// </summary>
    public class GetBranchesByBankIDQuery : IRequest<ServiceResponse<List<BranchDto>>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Branch to be retrieved.
        /// </summary>
        public string BankID { get; set; }
    }
}
